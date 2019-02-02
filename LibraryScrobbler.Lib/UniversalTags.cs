using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryScrobbler.Lib
{
    public enum UniversalTags
    {
        TrackNumber,
        Genre,
        Title,
        Artist,
        AlbumArtist,
        Album,
        Date,
        Comment,
        Artwork,
        EncodedBy,
        ComposedBy,
    }

    public struct UniversalTag
    {
        public UniversalTags Name { get; internal set; }
        public string ID3Name { get; internal set; }
        public string VorbisName { get; internal set; }
    }

    public static class UniversalTagging
    {
        private static readonly UniversalTag TrackNumber = new UniversalTag
        {
            Name = UniversalTags.TrackNumber,
            ID3Name = "TRCK",
            VorbisName = "TrackNumber",
        };

        private static readonly UniversalTag Genre = new UniversalTag
        {
            Name = UniversalTags.Genre,
            ID3Name = "TCON",
            VorbisName = "Genre",
        };

        private static readonly UniversalTag Title = new UniversalTag
        {
            Name = UniversalTags.Title,
            ID3Name = "TIT2",
            VorbisName = "Title",
        };

        private static readonly UniversalTag Artist = new UniversalTag
        {
            Name = UniversalTags.Artist,
            ID3Name = "TPE1",
            VorbisName = "Artist",
        };

        private static readonly UniversalTag AlbumArtist = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

        private static readonly UniversalTag Album = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

        private static readonly UniversalTag Date = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

        private static readonly UniversalTag Comment = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

        private static readonly UniversalTag Artwork = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

        private static readonly UniversalTag EncodedBy = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

        private static readonly UniversalTag ComposedBy = new UniversalTag
        {
            Name = UniversalTags.AlbumArtist,
            ID3Name = "TPE2",
            VorbisName = "AlbumArtist",
        };

    }
}
