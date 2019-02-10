using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
        public ActionResult Login()
        {
            GetCompany();

            ViewBag.Title = "LogIn Page";

            return View();
        }

        public ActionResult LoginUser(string UserId, string UserPassword, string Company)
        {

            ViewBag.Title = "LogIn Page";

            EpiUserModel objUserReq = new EpiUserModel();
            objUserReq.Company = Company;
            objUserReq.UserId = UserId;
            objUserReq.Password = UserPassword;

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("EpiLogin", objUserReq).Result;
            if (response.IsSuccessStatusCode)
            {
                objUserReq = response.Content.ReadAsAsync<EpiUserModel>().Result;
                if (!objUserReq.IsEpiUser)
                {
                    ViewBag.Err = "Login Failed!";
                    GetCompany();
                    return View("Login");
                }
                else
                {
                    Session["User"] = UserId;
                    Session["Company"] = Company;

                    try
                    {
                        List<Menu> menuModel = new List<Menu>();
                        string Qry = $"MenuListByUsers?company={Company}&userid={UserId}";
                        HttpResponseMessage response1 = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                        if (response1.IsSuccessStatusCode)
                        {
                            menuModel = response1.Content.ReadAsAsync<List<Menu>>().Result;
                            Session["Menu"] = menuModel;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errMs = ex.ToString();
                    }


                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                //return new HttpStatusCodeResult(500, "Custom Error Message 2");
                string msgJson = response.Content.ReadAsStringAsync().Result;
                JObject jObject = JObject.Parse(msgJson);
                string msg = (string)jObject.SelectToken("Message");
                string msgDtl = (string)jObject.SelectToken("MessageDetail");
                if (msgDtl != null)
                {
                    ViewBag.Err = msgDtl;
                }
                else
                {
                    ViewBag.Err = msg;
                }
                GetCompany();
                return View("Login");
            }



        }

        public ActionResult Logout()
        {

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index");

        }

        public void GetCompany()
        {
            try
            {
                List<CompanyModel> objCompLst = new List<CompanyModel>();
                string Qry1 = $"Company";
                HttpResponseMessage response1 = GlobalVariables.WebApiClient.GetAsync(Qry1).Result;
                if (response1.IsSuccessStatusCode)
                {
                    objCompLst = response1.Content.ReadAsAsync<List<CompanyModel>>().Result;
                    ViewBag.Companies = objCompLst;
                }
                else
                {
                    //objCompLst = response1.Content.ReadAsAsync<List<CompanyModel>>().Result;
                    //ViewBag.Companies = objCompLst;
                    //ViewBag.Err = "Please check your connection.";
                }


            }
            catch (Exception ex)
            {

            }
        }

    }
}
