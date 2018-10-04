using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MVC_App.Controllers
{
    public class HomeController : Controller
    {
        private const string BASE_URL = "https://instance";
        private static readonly string AUTH_URL = BASE_URL + "/Relativity/Identity/connect/authorize";
        private static readonly string TOKEN_URL = BASE_URL + "/Relativity/Identity/connect/token";
        private static readonly string VALIDATE_URL = BASE_URL + "/Relativity/Identity//connect/accesstokenvalidation";
        private const string CLIENT_ID = "your-client-id";
        private const string CLIENT_SECRET = "your-client-secret";
        private const string SCOPE = "UserInfoAccess";
        private const string REDIRECT_URI = "http://localhost:49203/home/authorize";
        private const string GRANT_TYPE = "code";

        private static string _accessToken = String.Empty;

        private static string _refreshToken = String.Empty;

        private static string _tokenType = String.Empty;

        private string GetRedirectUri()
        {
            var uriBuilder = new UriBuilder(AUTH_URL);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["client_id"] = CLIENT_ID;
            //query["client_secret"] = CLIENT_SECRET;
            query["scope"] = SCOPE;
            query["redirect_uri"] = REDIRECT_URI;
            query["response_type"] = GRANT_TYPE;
            query["state"] = "abc";

            uriBuilder.Query = query.ToString();
            string finalUrl = uriBuilder.ToString();
            return finalUrl;
        }


        /// <summary>
        /// Extracts the query string param from the query string
        /// e.g. GetParam("https://thing?foo=bar", "foo") => "bar"
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetParam(string url, string param)
        {
            var uriBuilder = new UriBuilder(url);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            string result = query[param];
            return result;
        }


        /// <summary>
        /// Performs an HTTP post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        private string Post(string url, string payload)
        {
            HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
            StringContent content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            if (response != null && response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return String.Empty;
        }


        /// <summary>
        /// Performs an HTTP GET
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string Get(string url)
        {
            HttpClient httpClient = new HttpClient();           
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response != null && response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return String.Empty;
        }

        private bool ValidateToken(string token, out string username)
        {
            username = "";

            var uriBuilder = new UriBuilder(VALIDATE_URL);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;

            uriBuilder.Query = query.ToString();
            string finalUrl = uriBuilder.ToString();
            string result = this.Get(finalUrl);

            // get the username
            try
            {
                JObject parsed = JObject.Parse(result);
                username = parsed["rel_un"].ToString();
            }
            catch (JsonReaderException jre)
            {
                // display/log error
                return false;
            }

            return true;
        }


        public ActionResult Index()
        {
            if (String.IsNullOrEmpty(_accessToken))
            {
                string finalUrl = GetRedirectUri();
                return Redirect(finalUrl);
            }

            return View();
        }


        public ActionResult Authorize()
        {
            
            string url = Request.Url.AbsoluteUri;

            // extract the value of the code
            const string param = "code";
            string code = this.GetParam(url, param);

            var body = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", CLIENT_ID },
                { "client_secret", CLIENT_SECRET },
                { "code", code },
                { "redirect_uri", REDIRECT_URI }
            };

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, TOKEN_URL)
            {
                Content = new FormUrlEncodedContent(body)
            };

            HttpResponseMessage result = client.SendAsync(request).Result;

            if (result != null && result.IsSuccessStatusCode)
            {
                string output = result.Content.ReadAsStringAsync().Result;

                // parse result
                JObject parsedJson = JObject.Parse(output);
                _tokenType = parsedJson["token_type"].ToString();
                _accessToken = parsedJson["access_token"].ToString();

                string username;
                ValidateToken(_accessToken, out username);

                // redirect to home page if successful
                return Redirect(Url.Content("~/"));
            }

            return NotAuthorized();
        }


        public ActionResult NotAuthorized()
        {
            ViewBag.Message = "Access denied";
            return View();
        }
    }
}