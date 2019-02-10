using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.WI
{
    public class FGController : Controller
    {
        // GET: FG
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            FGModel obj = new FGModel();
            if (Session["Company"] != null)
            {
                obj.Company = Session["Company"].ToString();
            }
            return View(obj);
        }

        public ActionResult GetFGByLot(string Company, string LotNum)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            FGModel entity = new FGModel();

            if (!string.IsNullOrEmpty(Company) && !string.IsNullOrEmpty(LotNum))
            {
                string Qry = $"ReceiveFGProduct?Company={Company}&LotNum={LotNum}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    entity = response.Content.ReadAsAsync<FGModel>().Result;
                }
                else
                {
                    //var httpErr = response.Content.ReadAsAsync<errHttpAmin.HttpError>();
                    string msgJson = response.Content.ReadAsStringAsync().Result;
                    JObject jObject = JObject.Parse(msgJson);
                    string msg = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    //ViewBag.Error = msg;//httpErr.Result["Message"].ToString();
                    return Json(new { success = false, responseText = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, responseText = "Company Or LotNum is Empty!" }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { success = true, responseText = entity }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index(FGModel objFGModel)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (objFGModel != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(objFGModel.Company))
                    {
                        objFGModel.Company = Session["Company"].ToString();
                    }
                    HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("ReceiveFGProduct", objFGModel).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        objFGModel = null;
                        objFGModel = new FGModel();
                        ViewBag.Status = "Update Successfully!";
                        ModelState.Clear();
                        return View(objFGModel);
                    }
                    else
                    {
                        //string msgJson = "Test error message";
                        string msgJson = response.Content.ReadAsStringAsync().Result;
                        JObject jObject = JObject.Parse(msgJson);
                        string msg = (string)jObject.SelectToken("Message");
                        string msgDtl = (string)jObject.SelectToken("MessageDetail");
                        ViewBag.Error = msg;
                        //ViewBag.Error = msgJson;

                        objFGModel = null;
                        objFGModel = new FGModel();
                        ModelState.Clear();
                        return View(objFGModel);
                    }
                }
                catch (Exception ex) {
                    ViewBag.Error = ex.ToString();
                }
 
            }
            else
            {
                ViewBag.Error = "Object is null.";
            }
            return View(objFGModel);
        }
    }
}