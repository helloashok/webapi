using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;

using System.Data.SqlClient;
using Facebook;
namespace WebApplication9.Controllers
{
    public class HomeController : Controller
    {
        // String Accesstoken;
        // String Accounts;


        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult image()
        {
            return View();
        }
        
        ///////////////////////////////////////////
        ///
        
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        [AllowAnonymous]
        public ActionResult Facebook()
         {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "415547075642876",
                client_secret = "ab7b6d89df67c7ee5b5df7de14b0f241",
                //redirect_uri = RedirectUri.AbsoluteUri,
                redirect_uri=  "http://localhost:50186/api/Image/GetToken",
               // RedirectUri.AbsoluteUri,       
                response_type = "code",
                scope = "email,manage_pages,publish_pages" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(String code)
        {
            var fb = new FacebookClient(); 
           dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "415547075642876",
                client_secret = "ab7b6d89df67c7ee5b5df7de14b0f241",
               redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            


            var client = new FacebookClient(accessToken);

             dynamic token = client.Get("336107027195328?fields=access_token");
            var pageaccesstoken = token;


            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(pageaccesstoken);
          

       var abc=    JObject.Parse(jsonString)["access_token"];
            String pagetoken = abc.ToString();
            ViewBag.token = pagetoken;

              var clientpost = new FacebookClient(pagetoken);
       




            byte[] imageArray = System.IO.File.ReadAllBytes(@"C:\Users\rajan\source\repos\WebApplication9\WebApplication9\App_Data\image\370144f6-38c7-482c-93b9-c5b172515757.png");

            dynamic parameters = new System.Dynamic.ExpandoObject();
            parameters.source = new FacebookMediaObject
            {
                ContentType="image",
                FileName="abc"
            }.SetValue(imageArray);




            dynamic poosting = clientpost.Post("/336107027195328/photos", parameters);










            return View("getfacebooktoken");

        }
    }
}
