using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace GraphicScrobbler.Lib
{
    public static class LibraryParsing
    {
        private static JsonSerializer _serializer = JsonSerializer.CreateDefault();

        public static void ParseMetadata(
            DirectoryInfo directory,
            DirectoryInfo outputDirectory,
            bool shouldOverwrite)
        {
            var files = directory.EnumerateFiles();
            var validFiles = files.Where(n => supportedExtensions.Contains(n.Extension));

            var fileTagDictionaries = new Dictionary<string, Dictionary<string, string>>();

            foreach (var file in validFiles)
            {
                var path = file.FullName;
                TagLib.File tagFile = null;
                try
                {
                    tagFile = TagLib.File.Create(path);
                }
                catch (Exception)
                {
                    // TODO: Add Logging
                    continue;
                }

                if (tagFile.GetTag(TagLib.TagTypes.Xiph) is TagLib.Ogg.XiphComment xiph)
                {
                    var tagDictionary = xiph.ToDictionary(x => x, x => xiph.GetFirstField(x));
                    fileTagDictionaries.Add(file.Name, tagDictionary);
                    continue;
                }

                if (tagFile.GetTag(TagLib.TagTypes.Id3v2) is TagLib.Id3v2.Tag id3File)
                {
                    var tagDictionary = GetID3v2TagDictionary(id3File);
                    fileTagDictionaries.Add(file.Name, tagDictionary);
                    continue;
                }
            }

            if (fileTagDictionaries.Any())
            {
                var json = JsonConvert.SerializeObject(fileTagDictionaries);
                var tagOutputFileName = $"{directory.Name}.json";
                var tagOutputPath = $"{outputDirectory.FullName}\\{tagOutputFileName}";

                if (shouldOverwrite || !File.Exists(tagOutputPath))
                {
                    Directory.CreateDirectory(outputDirectory.FullName);

                    var writer = new StreamWriter(tagOutputPath);
                    writer.AutoFlush = true;

                    writer.Write(json);
                }
            }
        }

        private static Dictionary<string, string> GetID3v2TagDictionary(TagLib.Id3v2.Tag id3File)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var frame in id3File)
            {
                string frameId = frame.FrameId.ToString();
                if (IgnoredId3Tags.Contains(frameId)) // Comments are often duplicated, just ignore them
                    continue;

                string key = GetID3v2TagTitle(frame);
                string value = GetID3v2TagValue(frame);

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        private static string GetID3v2TagTitle(TagLib.Id3v2.Frame frame)
        {
            // TODO: Handle NXXX
            if (frame.FrameId == "TXXX") // Non-native (user-defined) tag.
            {
                var regex = new Regex("\\[.*\\]");
                var firstMatch = regex.Match(frame.ToString()).Value;
                if (firstMatch.Length >= 2)
                    return firstMatch.Substring(1, firstMatch.Length - 2); // Trim the single pair of brackets surrounding title
            }

            bool success = ID3v2KeyNameMap.TryGetValue(frame.FrameId.ToString(), out UniversalTags name);
            return success
                ? name.ToString()
                : frame.FrameId.ToString();
        }

        private static string GetID3v2TagValue(TagLib.Id3v2.Frame frame)
        {
            // TODO: Handle NXXX
            if (frame.FrameId == "TXXX") // Non-native (user-defined) tag.
            {
                var regex = new Regex("\\] .*");
                var firstMatch = regex.Match(frame.ToString()).Value;
                var fieldValue = firstMatch.Substring(2); // Trim the leading bracket & space before value
                return fieldValue;
            }

            return frame.ToString();
        }

        public static readonly Dictionary<string, UniversalTags> ID3v2KeyNameMap = new Dictionary<string, UniversalTags>
        {
            { "TRCK", UniversalTags.TrackNumber },
            { "TCON", UniversalTags.Genre },
            { "TIT2", UniversalTags.Title },
            { "TPE1", UniversalTags.Artist },
            { "TPE2", UniversalTags.AlbumArtist },
            { "TALB", UniversalTags.Album },
            { "TDRC", UniversalTags.Date },
            { "COMM", UniversalTags.Comment },
            { "APIC", UniversalTags.Artwork },
            { "TENC", UniversalTags.EncodedBy },
            { "TCOM", UniversalTags.ComposedBy },
        };

        public static readonly Dictionary<UniversalTags, UniversalTag> TagMap = new Dictionary<UniversalTags, UniversalTag>
        {
            {
                UniversalTags.TrackNumber, new UniversalTag
                {
                    Id = UniversalTags.TrackNumber,
                    //Id3Key = 
                }
            },
            {
                UniversalTags.TrackNumber, new UniversalTag
                {
                    Id = UniversalTags.TrackNumber
                }
            },


            //{ "TRCK", UniversalTags.TrackNumber },
            //{ "TCON", UniversalTags.Genre },
            //{ "TIT2", UniversalTags.Title },
            //{ "TPE1", UniversalTags.Artist },
            //{ "TPE2", UniversalTags.AlbumArtist },
            //{ "TALB", UniversalTags.Album },
            //{ "TDRC", UniversalTags.Date },
            //{ "COMM", UniversalTags.Comment },
            //{ "APIC", UniversalTags.Artwork },
            //{ "TENC", UniversalTags.EncodedBy },
            //{ "TCOM", UniversalTags.ComposedBy },
        };

        public static readonly HashSet<string> IgnoredId3Tags = new HashSet<string>
        {
            "COMM",
            "PRIV",
        };

        public static readonly HashSet<string> supportedExtensions = new HashSet<string>
        {
            ".mp3",
            ".flac",
            ".wav",
        };
    }
}
