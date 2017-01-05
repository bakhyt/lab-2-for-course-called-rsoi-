using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using MvcApplication1.Models;
namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        
        public void AuthMethod()
        {
            ViewBag.Result = "Нет результата";
            //string result = "";
            string apiUrl = ConfigurationManager.AppSettings["apiUrl"];

            string code = Request.Params["code"];
            string grant_type = "authorization_code";
            if (Session["expiresDate"] != null)
            {
                DateTime expiresDate = Convert.ToDateTime(Session["expiresDate"]);
                if (expiresDate < DateTime.Now)
                {
                    // нужно обновить refresh_token
                    //grant_type=refresh_token&client_id=31337&client_secret=deadbeef&refresh_token=8xLOxBtZp8
                    grant_type = "refresh_token";
                }
            }

            if (grant_type == "refresh_token" || (!string.IsNullOrEmpty(code) && Session["authPars"] == null))
            {
                // получаем токен авторизации
                string authUrl = string.Format("{0}/oauth/token", apiUrl);
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("grant_type", grant_type);
                param.Add("client_id", "464119");
                param.Add("client_secret", "deadbeef");

                if (grant_type == "authorization_code")
                    param.Add("code", code);
                else
                {
                    AuthParams authPar = (AuthParams)Session["authPars"];
                    param.Add("refresh_token", authPar.refresh_token);
                }
                // param.Add("redirect_uri", code);

                //grant_type=authorization_code&client_id=464119&client_secret=deadbeef&code=DoRieb0y&
                //redirect_uri=http%3A%2F%2Fexample.com%2Fcb%2F123

                string authcode = GetResult(authUrl, "POST", param);
                AuthParams authPars = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthParams>(authcode);
                Session["authPars"] = authPars;
                Session["expiresDate"] = DateTime.Now.AddSeconds(authPars.expires_in);
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Тест лабораторная работа 1";

            AuthMethod();
            string apiUrl = ConfigurationManager.AppSettings["apiUrl"];

            List<MyRequest> responses = new List<MyRequest>();
            string serviceUrl;
            serviceUrl = string.Format("{0}/goods?page=1&size=5", apiUrl);
            string result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/goods/1", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });


            serviceUrl = string.Format("{0}/deliveries?page=1&size=5", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/customers?page=1&size=5", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders?page=1&size=5", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders", apiUrl);
            result = GetResult(serviceUrl, "POST");
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });
            int id = -1;
            Int32.TryParse(result, out id);

            serviceUrl = string.Format("{0}/orders?orderId={1}&goodId=1&customerId=1&deliveryId=1", apiUrl, id);
            result = GetResult(serviceUrl, "POST");
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders/{1}", apiUrl, id);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders?orderId={1}", apiUrl, Convert.ToInt32(id));
            result = GetResult(serviceUrl, "DELETE");
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            ViewBag.Responses = responses;


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Тест лабораторная работа 2";

            AuthMethod();
            string apiUrl = ConfigurationManager.AppSettings["apiUrl"];

            List<MyRequest> responses = new List<MyRequest>();
            string serviceUrl;
            serviceUrl = string.Format("{0}/goods2?page=1&size=5", apiUrl);
            string result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/goods2/1", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/deliveries2?page=1&size=5", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/customers2?page=1&size=5", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });


            serviceUrl = string.Format("{0}/orders2?page=1&size=5", apiUrl);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders2", apiUrl);
            result = GetResult(serviceUrl, "POST");
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });
            int id = -1;
            Int32.TryParse(result, out id);

            serviceUrl = string.Format("{0}/orders2?orderId={1}&goodId=1&customerId=2&deliveryId=1", apiUrl, id);
            result = GetResult(serviceUrl, "POST");
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders2/{1}", apiUrl, id);
            result = GetResult(serviceUrl);
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            serviceUrl = string.Format("{0}/orders2?orderId={1}", apiUrl, Convert.ToInt32(id));
            result = GetResult(serviceUrl, "DELETE");
            responses.Add(new MyRequest { Url = serviceUrl, Response = result });

            ViewBag.Responses = responses;
            return View();
        }

        private string GetResult(string serviceUrl, string method = "GET", Dictionary<string, string> param = null)
        {
            string result;
            WebRequest request = WebRequest.Create(serviceUrl);
            object obj = Session["authPars"];
            if (obj != null)
            {
                AuthParams pars = (AuthParams)Session["authPars"];
                if (pars != null)
                {
                    request.Headers["Authorization"] = JsonConvert.SerializeObject(pars);
                }
            }
            request.Method = method;

            if (method == "POST" && param != null && param.Count > 0)
            {
                string postString = "";
                foreach (KeyValuePair<string, string> item in param)
                {
                    postString = string.IsNullOrEmpty(postString) ? string.Format("{0}={1}", item.Key, item.Value) : postString + string.Format("&{0}={1}", item.Key, item.Value);
                }
                const string contentType = "application/x-www-form-urlencoded";
                byte[] data = Encoding.ASCII.GetBytes(postString);

                request.ContentType = contentType;
                request.ContentLength = data.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                    //if (method != "GET")
                    result = responseString;
                    //else
                    //    result = string.Format("Запрос: {0} \r\n Ответ: {1} ", serviceUrl, responseString);
                }
            }
            catch (Exception)
            {
                // result = string.Format("Запрос: {0} \r\n Ответ: Недостаточно прав для выполнения операции ", serviceUrl);
                result = "Недостаточно прав для выполнения операции ";
            }

            return result;
        }
    }
}
