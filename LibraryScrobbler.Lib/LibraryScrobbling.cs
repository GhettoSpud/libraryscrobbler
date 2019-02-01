using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;

namespace GraphicScrobbler.Lib
{
    public static class LibraryScrobbling
    {
        public static HttpResponseMessage ScrobbleSong(JObject songJson)
        {
            var client = new HttpClient();
            var uri = new Uri("http://ws.audioscrobbler.com/2.0/");
            var content = getScrobbleHttpResponse(songJson);

            return client.PostAsync(uri, content).Result;
        }

        public static FormUrlEncodedContent getScrobbleHttpResponse(JObject songJson)
        {
            var httpValues = new Dictionary<string, string>
            {
                //{ "artist", songJson[ },
            };

            var content = new FormUrlEncodedContent(httpValues);

            return content;
        }
    }
}
