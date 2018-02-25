using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FurryNetworkLib {
    public class FurryNetworkClient {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        private FurryNetworkClient() { }

        private HttpWebRequest CreateRequest(string method, string urlPath) {
            var req = WebRequest.CreateHttp("https://beta.furrynetwork.com/api/" + urlPath);
            req.Method = method;
            req.UserAgent = "FurryNetworkLib/0.1 (https://www.github.com/libertyernie/FurryNetworkLib)";
            if (AccessToken != null) {
                req.Headers["Authorization"] = $"Bearer {AccessToken}";
            }
            return req;
        }

        public static async Task<FurryNetworkClient> LoginAsync(string username, string password) {
            var req = WebRequest.CreateHttp("https://beta.furrynetwork.com/api/oauth/token");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "application/json";
            req.UserAgent = "FurryNetworkLib/0.1 (https://www.github.com/libertyernie/FurryNetworkLib)";
            using (var sw = new StreamWriter(await req.GetRequestStreamAsync())) {
                string str = "";
                str+=($"username={WebUtility.UrlEncode(username)}&");
                str += ($"password={WebUtility.UrlEncode(password)}&");
                str += ($"grant_type=password&client_id=123&client_secret=");
                await sw.WriteAsync(str);
                await sw.FlushAsync();
            }
            using (var resp = await req.GetResponseAsync())
            using (var sr = new StreamReader(resp.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                var obj = JsonConvert.DeserializeAnonymousType(json, new {
                    access_token = "",
                    expires_in = 0,
                    token_type = "",
                    refresh_token = "",
                    user_id = 0
                });
                if (obj.token_type != "bearer") {
                    throw new Exception("Token returned was not a bearer token");
                }
                return new FurryNetworkClient {
                    AccessToken = obj.access_token,
                    RefreshToken = obj.refresh_token
                };
            }
        }

        public async Task<User> GetUserAsync() {
            var req = CreateRequest("GET", "user");
            req.Accept = "application/json";
            using (var resp = await req.GetResponseAsync())
            using (var sr = new StreamReader(resp.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<User>(json);
            }
        }

        public async Task LogoutAsync() {
            var req = CreateRequest("POST", "oauth/logout");
            req.ContentType = "application/json";
            using (var sw = new StreamWriter(await req.GetRequestStreamAsync())) {
                await sw.WriteLineAsync(JsonConvert.SerializeObject(new {
                    refresh_token = RefreshToken
                }));
            }

            using (var resp = await req.GetResponseAsync()) { }

            AccessToken = null;
            RefreshToken = null;
        }
    }
}
