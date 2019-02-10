using MySpritzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.Prod
{
    public class PMCTransferController : Controller
    {
        // GET: PMCTransfer
        public ActionResult Index(string Company)
        {
            if (Session["User"] == null)
            {
                Session["From"] = "PMCTransfer";
                return RedirectToAction("Login", "Home");
            }


            // string Qry = $"IssueMaterialConfirm?PONum={PONum}&Company={Company}";
            //HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            PMCTransferModel entity = new PMCTransferModel();
            entity.Company = Session["Company"].ToString();
            //HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialValidatePart", entity).Result;

            // return View(lstEnts);
            return View(entity);
        }

        public ActionResult ValidatePart(PMCTransferModel obj)
        {
            string msg = string.Empty;
            PMCTransferModel objReturn = new PMCTransferModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("PMCTransferValidatePart", obj).Result;
            objReturn = response.Content.ReadAsAsync<PMCTransferModel>().Result;
            //return View(objReturn);
            //string msg = response.Content.ReadAsStringAsync().Result;

            // return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = objReturn }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidateLot(PMCTransferModel obj)
        {
            string msg = string.Empty;
            PMCTransferModel objReturn = new PMCTransferModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("PMCTransferValidateLot", obj).Result;
            objReturn = response.Content.ReadAsAsync<PMCTransferModel>().Result;
            //return View(objReturn);
            //string msg = response.Content.ReadAsStringAsync().Result;

            // return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = objReturn }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Close()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Save(PMCTransferModel obj)
        {
            string msg = string.Empty;
            PMCTransferModel objReturn = new PMCTransferModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("PMCTransferConfirm", obj).Result;
            objReturn = response.Content.ReadAsAsync<PMCTransferModel>().Result;
            //return View(objReturn);
            //string msg = response.Content.ReadAsStringAsync().Result;

            // return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                return Json(new { success = false, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, Message = "Sucessfully Updated!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}