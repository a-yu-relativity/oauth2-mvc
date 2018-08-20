using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_App.Controllers
{
    public class HomeController : Controller
    {
        private const string AUTH_URL = "https://p-dv-vm-two7bin.kcura.corp/Relativity/Identity/connect/authorize";
        private const string TOKEN_URL = "https://p-dv-vm-two7bin.kcura.corp/Relativity/Identity/connect/token";
        private const string CLIENT_ID = "2b28a80c61ea80a08a5e7a7e24";
        private const string CLIENT_SECRET = "52a2e31b9c461e6a8ec874444fda5ce6a68b72ef";
        private const string SCOPE = "UserInfoAccess";
        private const string REDIRECT_URI = "http://localhost:49203";
        private const string GRANT_TYPE = "code";


        private string _bearerToken = String.Empty;

        private string _refreshToken = String.Empty;

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


        public ActionResult Index()
        {
            if (String.IsNullOrEmpty(_bearerToken))
            {
                string finalUrl = GetRedirectUri();
                return Redirect(finalUrl);
            }

            return View();
        }


        public ActionResult Redirect()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}