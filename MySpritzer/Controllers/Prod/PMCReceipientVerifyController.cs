using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using errHttpAmin = System.Web.Http;

namespace MySpritzer.Controllers.Prod
{
    public class PMCReceipientVerifyController : Controller
    {
        // GET: PMCReceipientVerify
        public ActionResult Index(string Company, string UserId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<PMCReceipientVerificationModel> entitys = new List<PMCReceipientVerificationModel>();
            if (!string.IsNullOrEmpty(Company))
            {
                string Qry = $"ProdPMCReceipientVerify?Company={Company}&UserId={UserId}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    entitys = response.Content.ReadAsAsync<List<PMCReceipientVerificationModel>>().Result;
                    TempData["PMCReceipientVerificationModels"] = entitys;
                    TempData.Keep();
                }
                else
                {
                    var httpErr = response.Content.ReadAsAsync<errHttpAmin.HttpError>();
                    ViewBag.Error = httpErr.Result["Message"].ToString();
                }
            }

            return View(entitys);
        }

        // GET: PMCReceipientVerify/Details/5
        public ActionResult PMCReceipientVerify(int FKId = 0)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            PMCReceipientVerificationModel objEntity = new PMCReceipientVerificationModel();
            if (FKId > 0)
            {
                List<PMCReceipientVerificationModel> entitys = (List<PMCReceipientVerificationModel>)TempData["PMCReceipientVerificationModels"];
                TempData.Keep();
                objEntity = entitys.Where(w => w.PickRawMtrlFKId == FKId).FirstOrDefault();

                ViewBag.ReasonTypes = new SelectList(objEntity.ReasonTypes, "ReasonCode", "ReasonCode");
            }
            else
            {
                ViewBag.Status = "Id is null!";
            }
            return View(objEntity);
        }

        // GET: PMCReceipientVerify/Create
        [HttpPost]
        public ActionResult PMCReceipientVerify(PMCReceipientVerificationModel obj)
        {

            if (TempData["PMCReceipientVerificationModels"] != null && obj != null)
            {
                List<PMCReceipientVerificationModel> tempLstData = (List<PMCReceipientVerificationModel>)TempData["PMCReceipientVerificationModels"];
                PMCReceipientVerificationModel tempData = tempLstData.Where(w => w.PickRawMtrlFKId == obj.PickRawMtrlFKId).FirstOrDefault();
                tempData.RejQty = obj.RejQty;
                tempData.ReasonCode = obj.ReasonCode;

                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("ProdPMCReceipientVerify", tempData).Result;
                if (response.IsSuccessStatusCode)
                {
                    if (obj.RejQty <= 0)
                    {
                        //No need status if success
                        //ViewBag.Status = "Inserted Sucessfully!";
                    }
                    else
                    {
                        return Json(new { success = true, responseText = "Inserted Sucessfully!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string msgJson = response.Content.ReadAsStringAsync().Result;
                    JObject jObject = JObject.Parse(msgJson);
                    string msg = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    if (obj.RejQty <= 0)
                    {
                        ViewBag.Status = msg + "\n" + msgDtl;
                    }
                    else
                    {
                        return Json(new { sucess = false, responseText = msg + "\n" + msgDtl }, JsonRequestBehavior.AllowGet);
                    }
                    //throw new Exception(msgDtl);
                }


            }
            return View(obj);

        }
    }
}