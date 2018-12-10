using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using System.Web.Http;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json.Linq;

using System.Data.SqlClient;
using Facebook;
using System.Web;

namespace WebApplication9.Controllers
{
    public class ImageController : ApiController
    {
        public string path;


        public Task<HttpResponseMessage> Post()
        {
            List<string> savedFilePath = new List<string>();
            // Check if the request contains multipart/form-data
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            String username = HttpContext.Current.Request.Form["username"];
            String email = HttpContext.Current.Request.Form["email"];
            var applicationPath = AppDomain.CurrentDomain.BaseDirectory;

               System.IO.Directory.CreateDirectory(string.Concat(applicationPath,@"App_Data/image/",username));
            
            
           


            string rootPath = HttpContext.Current.Server.MapPath(string.Concat("~/App_Data/image/", username));
           
            var provider = new MultipartFileStreamProvider(rootPath);


            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    foreach (MultipartFileData dataitem in provider.FileData)
                    {

                        try
                        {


                            string name = dataitem.Headers.ContentDisposition.FileName.Replace("\"", "");


                            string newFileName =string.Concat(username,"_" ,Guid.NewGuid()) + Path.GetExtension(name);


                            File.Move(dataitem.LocalFileName, Path.Combine(rootPath, newFileName));


                            path = Path.Combine(rootPath, newFileName);

                            //  int id = (int)Convert.ToInt64(Guid.NewGuid());





                        }
                        catch(Exception ex)
                        {
                            var x = ex;
                        }



                    }
                    return Request.CreateResponse(HttpStatusCode.Created, savedFilePath);
                });


            return task;
        }










































        /*  public  async Task<HttpResponseMessage> Post()
          {

              return await GetAccessCode(); 
          }*/
        public async Task<HttpResponseMessage> GetAccessCode()
        {
            try
            {  var fb = new FacebookClient();
                var loginUrl = fb.GetLoginUrl(new
                {
                    client_id = "415547075642876",
                    client_secret = "ab7b6d89df67c7ee5b5df7de14b0f241",
                    //redirect_uri = RedirectUri.AbsoluteUri,
                    redirect_uri = "http://localhost:50186/api/Image/GetToken",
                    // RedirectUri.AbsoluteUri,       
                    response_type = "code",
                    scope = "email,manage_pages,publish_pages" // Add other permissions as needed
                });
                
            }
            catch (Exception ex)
            {
                var a = ex;
            }


            return  null;
         
            
        }
        public IHttpActionResult GetCode(String code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "415547075642876",
                client_secret = "ab7b6d89df67c7ee5b5df7de14b0f241",
                redirect_uri = "http://localhost:50186/api/Image/GetCode",
                code = code
            });
            String token = result.access_token;


            GetToken(token);


            return null;

        }
        public IHttpActionResult GetToken(String token)
        {
            
            var client = new FacebookClient(token);

            dynamic pagetoken=client.Get("336107027195328?fields=access_token");
            var pageaccesstoken = pagetoken;
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(pageaccesstoken);


            var abc = JObject.Parse(jsonString)["access_token"];
            String patoken = abc.ToString();
           
            var clientpost = new FacebookClient(patoken);

            byte[] imageArray = System.IO.File.ReadAllBytes(@"C:\Users\rajan\source\repos\WebApplication9\WebApplication9\App_Data\image\00f3a6c4-c5e4-4918-b32b-39f06779e875.jpg");

            dynamic parameters = new System.Dynamic.ExpandoObject();
            parameters.source = new FacebookMediaObject
            {
                ContentType = "image",
                FileName = "abc"
            }.SetValue(imageArray);

            dynamic poosting = clientpost.Post("/336107027195328/photos", parameters);

            return null;

        }
        







    }
  

}

