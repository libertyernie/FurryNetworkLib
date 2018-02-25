using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FurryNetworkLib {
    public class FurryNetworkClient {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        private FurryNetworkClient() { }

        private async Task<HttpWebRequest> CreateRequest(string method, string urlPath, object jsonBody = null) {
            var req = WebRequest.CreateHttp("https://beta.furrynetwork.com/api/" + urlPath);
            req.Method = method;
            req.UserAgent = "FurryNetworkLib/0.1 (https://www.github.com/libertyernie/FurryNetworkLib)";
            if (AccessToken != null) {
                req.Headers["Authorization"] = $"Bearer {AccessToken}";
            }
            if (jsonBody != null) {
                req.ContentType = "application/json";
                using (var sw = new StreamWriter(await req.GetRequestStreamAsync())) {
                    await sw.WriteAsync(JsonConvert.SerializeObject(jsonBody));
                }
            }
            return req;
        }

        private async Task<WebResponse> ExecuteRequest(string method, string urlPath, object jsonBody = null) {
            try {
                var req = await CreateRequest(method, urlPath, jsonBody);
                return await req.GetResponseAsync();
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized) {
                await GetNewAccessToken();

                var req = await CreateRequest(method, urlPath, jsonBody);
                return await req.GetResponseAsync();
            }
        }

        public static async Task<FurryNetworkClient> LoginAsync(string username, string password) {
            var req = WebRequest.CreateHttp("https://beta.furrynetwork.com/api/oauth/token");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "application/json";
            req.UserAgent = "FurryNetworkLib/0.1 (https://www.github.com/libertyernie/FurryNetworkLib)";
            using (var sw = new StreamWriter(await req.GetRequestStreamAsync())) {
                await sw.WriteAsync($"username={WebUtility.UrlEncode(username)}&");
                await sw.WriteAsync($"password={WebUtility.UrlEncode(password)}&");
                await sw.WriteAsync($"grant_type=password&client_id=123&client_secret=");
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

        public async Task GetNewAccessToken() {
            var req = WebRequest.CreateHttp("https://beta.furrynetwork.com/api/oauth/token");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "application/json";
            req.UserAgent = "FurryNetworkLib/0.1 (https://www.github.com/libertyernie/FurryNetworkLib)";
            using (var sw = new StreamWriter(await req.GetRequestStreamAsync())) {
                await sw.WriteAsync($"client_id=123&");
                await sw.WriteAsync($"grant_type=refresh_token&");
                await sw.WriteAsync($"refresh_token={RefreshToken}");
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

                RefreshToken = obj.refresh_token ?? RefreshToken;
                AccessToken = obj.access_token;
            }
        }

        public async Task<User> GetUserAsync() {
            using (var resp = await ExecuteRequest("GET", "user"))
            using (var sr = new StreamReader(resp.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<User>(json);
            }
        }

        public async Task<SearchResults> SearchByTypeAsync(string type, string sort = null, int? from = 0) {
            string qs = "";
            if (!string.IsNullOrEmpty(sort)) {
                qs += $"&sort={WebUtility.UrlEncode(sort)}";
            }
            if (from != null) {
                qs += $"&from={from}";
            }
            using (var resp = await ExecuteRequest("GET", $"search/{WebUtility.UrlEncode(type)}?{qs}"))
            using (var sr = new StreamReader(resp.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<SearchResults>(json);
            }
        }

        public async Task<SearchResults> SearchByCharacterAsync(string character, IEnumerable<string> types = null, string sort = null, int? from = 0) {
            string qs = $"character={character}";
            if (types != null && types.Any()) {
                qs += $"&types[]={string.Join(",", types.Select(s => WebUtility.UrlEncode(s)))}";
            }
            if (!string.IsNullOrEmpty(sort)) {
                qs += $"&sort={WebUtility.UrlEncode(sort)}";
            }
            if (from != null) {
                qs += $"&from={from}";
            }
            using (var resp = await ExecuteRequest("GET", $"search?{qs}"))
            using (var sr = new StreamReader(resp.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<SearchResults>(json);
            }
        }

        public async Task LogoutAsync() {
            using (var resp = await ExecuteRequest("POST", "oauth/logout", jsonBody: new {
                refresh_token = RefreshToken
            })) { }

            AccessToken = null;
            RefreshToken = null;
        }
    }
}
