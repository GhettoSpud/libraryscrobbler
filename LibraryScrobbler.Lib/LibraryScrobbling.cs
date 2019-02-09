using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Data;

namespace LibraryScrobbler.Lib
{
    public static class LibraryScrobbling
    {
        public static FormUrlEncodedContent getScrobbleHttpResponse(JObject songJson)
        {
            var httpValues = new Dictionary<string, string>
            {
                //{ "artist", songJson[ },
            };

            var content = new FormUrlEncodedContent(httpValues);

            return content;
        }

        public static void ScrobbleAlbum(DataTable table)
        {
            throw new NotImplementedException();
        }

        public static void ScrobbleTrack(DataTable table)
        {
            var client = new HttpClient();
            var uri = new Uri("http://ws.audioscrobbler.com/2.0/");
            //var content = getScrobbleHttpResponse(songJson);

            //client.PostAsync(uri, null);
        }
    }
}
