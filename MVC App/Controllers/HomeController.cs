using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_App.Controllers
{
    public class HomeController : Controller
    {

        private string GetRedirectUri()
        {
            const string AUTH_URL = "https://p-dv-vm-two7bin.kcura.corp/Relativity/Identity/connect/authorize";
            const string CLIENT_ID = "2b28a80c61ea80a08a5e7a7e24";
            const string CLIENT_SECRET = "52a2e31b9c461e6a8ec874444fda5ce6a68b72ef";
            const string SCOPE = "UserInfoAccess";
            const string REDIRECT_URI = "http://localhost:49203";
            const string GRANT_TYPE = "code";

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


        public ActionResult Index()
        {
            string finalUrl = GetRedirectUri();

            //return View();
            return Redirect(finalUrl);
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