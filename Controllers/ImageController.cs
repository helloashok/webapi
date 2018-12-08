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
          string rootPath = HttpContext.Current.Server.MapPath("~/App_Data/image/");
            // this is added line
            var provider = new MultipartFileStreamProvider(rootPath);
            //add a try catch block while storing image in a folder 
            //filename = filename_TimeStamp. DateTime.Now.Timespan
            //exception return null or some error message
            //if no exception save to db  if no excetion then post to fb
            
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
                    {  /*  if(!Directory.Info("~/App-Data/image/")
                                   {
                                   Directory.
                               }
                               String dirpath = HttpContext.Current.Server.MapPath("~/App_Data/image/" + nothing);*/



                            string name = dataitem.Headers.ContentDisposition.FileName.Replace("\"", "");
                        

                            string newFileName = Guid.NewGuid()+ Path.GetExtension(name);
                            

                            File.Move(dataitem.LocalFileName, Path.Combine(rootPath,newFileName));
                          

                           path = Path.Combine(rootPath, newFileName);
                           
                          //  int id = (int)Convert.ToInt64(Guid.NewGuid());
                            

                            
                            string connetionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\rajan\\Documents\\Database1.mdf;Integrated Security=True;Connect Timeout=3";
                            try
                            {
                                SqlConnection connection;



                                connection = new SqlConnection(connetionString);

                                connection.Open();

                                System.Diagnostics.Debug.WriteLine("connection started");


                                String query = "Select Count(*) from userinformation where username='"+username+"'";

                                SqlCommand cmd = new SqlCommand(query, connection);
                                
                                Int32 count = (Int32)cmd.ExecuteScalar();
                                connection.Close();

                                System.Diagnostics.Debug.WriteLine(username, "the same value is printed here");

                                if (count == 0)
                                {

                                    try
                                    {
                                        connection.Open();
                                        System.Diagnostics.Debug.WriteLine("username doesnot  exist");
                                        String queryforinsert = "Insert into userinformation(username,filepath) Values (@username,@filepath)";
                                        SqlCommand cmdforinsert = new SqlCommand(queryforinsert, connection);
                                        System.Diagnostics.Debug.WriteLine("starting the insertion");

                                        cmdforinsert.Parameters.AddWithValue("@username", username);
                                        cmdforinsert.Parameters.AddWithValue("@filepath", path);
                                        cmdforinsert.ExecuteNonQuery();
                                    }
                                    catch (SqlException E)
                                    {
                                        System.Diagnostics.Debug.WriteLine(E);
                                    }

                                }


                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("this username  exist in database");

                                    connection.Open();
                                    try
                                    {
                                        String querysecond = "UPDATE userinformation SET filepath = '" + path + "' WHERE username= '" + username + "' ";
                                        SqlCommand cmdsecond = new SqlCommand(querysecond, connection);


                                        System.Diagnostics.Debug.WriteLine("the query is executed");


                                        cmdsecond.ExecuteNonQuery();
                                    }
                                    catch (SqlException e)
                                    {
                                        System.Diagnostics.Debug.WriteLine(e);
                                    }
                                }
                            }
                            catch(SqlException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e);
                            }
                            }
                        catch (Exception ex)
                        {
                            string message = ex.Message;
                        }
                    }






                //    GetToken();
                    return Request.CreateResponse(HttpStatusCode.Created, savedFilePath);
                    
                });
          
            return task;
        } 
        public IHttpActionResult GetAccessCode()
        {
            {
                var fb = new FacebookClient();
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

                return null;

            }
            
        }
        public Uri GetToken(String code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "415547075642876",
                client_secret = "ab7b6d89df67c7ee5b5df7de14b0f241",
                redirect_uri = "http://localhost:50186/api/Image/GetToken",
                code = code
            });
            return null;

        }








        }
  

}

