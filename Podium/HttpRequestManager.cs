using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Podium
{
    class HttpRequestManager
    {
        static string blizzID = "blizzID";
        static string blizzSecret = "blizzSecret";

        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> BlizzOAuthConnectAsync()
        {
            List<KeyValuePair<string, string>> headerPairs = new List<KeyValuePair<string, string>>();
            headerPairs.Add(new KeyValuePair<string, string>("client_id", blizzID));
            headerPairs.Add(new KeyValuePair<string, string>("client_secret", blizzSecret));
            headerPairs.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

            var result = await PostHttpRequest("https://kr.battle.net/oauth/token", headerPairs);

            OAuthToken token = JsonConvert.DeserializeObject<OAuthToken>(result);

            return token.AccessToken;
        }

        public static async Task<string> PostHttpRequest(string uri, List<KeyValuePair<string, string>> headerPairs = null)
        {
            var content = new FormUrlEncodedContent(headerPairs);

            var response = await client.PostAsync(uri, content);

            string result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public static async Task<string> GetHttpRequest(string uri, List<KeyValuePair<string, string>> headerPairs = null, string authToken = null)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            string result = "";
            if (authToken != null)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            try
            {
                result = await client.GetAsync(uri).Result.Content.ReadAsStringAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result;
        }
    }
}
