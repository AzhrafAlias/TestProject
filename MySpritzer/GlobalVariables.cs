using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Web;

namespace MySpritzer
{
    public static class GlobalVariables
    {

        public static HttpClient WebApiClient = new HttpClient();

        static GlobalVariables()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(
   delegate { return true; }
);

            //WebApiClient.BaseAddress = new Uri("http://10.0.10.29/SpritzerAPI/api/");

            WebApiClient.BaseAddress = new Uri("http://localhost:65458/api/");
            //WebApiClient.BaseAddress = new Uri("http://localhost/WebAPISample/api/");

            //WebApiClient.BaseAddress = new Uri("http://192.188.1.97/SpritzerAPI/api/");
            //WebApiClient.BaseAddress = new Uri("http://192.188.1.182/WebApi/api/");

            //WebApiClient.BaseAddress = new Uri("http://192.188.1.168/WebAPISample/api/"); //Mathan



            //WebApiClient.BaseAddress = new Uri("http://10.0.10.29/SpritzerTestAPI/api/");


            WebApiClient.DefaultRequestHeaders.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


    }
}