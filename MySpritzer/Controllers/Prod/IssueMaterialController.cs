using MySpritzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.Prod
{
    public class IssueMaterialController : Controller
    {
        // GET: IssueMaterialModel
        public ActionResult Index(string Company)
        {
            if (Session["User"] == null)
            {
                Session["From"] = "IssueMaterial";
                return RedirectToAction("Login", "Home");
            }


            // string Qry = $"IssueMaterialConfirm?PONum={PONum}&Company={Company}";
            //HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            IssueMaterialModel entity = new IssueMaterialModel();
            entity.Company = Session["Company"].ToString();
            //HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialValidatePart", entity).Result;

            // return View(lstEnts);
            return View(entity);
        }

        public ActionResult ValidatePart(IssueMaterialModel obj)
        {
            string msg = string.Empty;
            IssueMaterialModel objReturn = new IssueMaterialModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialValidatePart", obj).Result;
            objReturn = response.Content.ReadAsAsync<IssueMaterialModel>().Result;
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

        public ActionResult ValidateLot(IssueMaterialModel obj)
        {
            string msg = string.Empty;
            IssueMaterialModel objReturn = new IssueMaterialModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialValidateLot", obj).Result;
            objReturn = response.Content.ReadAsAsync<IssueMaterialModel>().Result;
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

        public ActionResult Save(IssueMaterialModel obj)
        {
            string msg = string.Empty;
            IssueMaterialModel objReturn = new IssueMaterialModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialConfirm", obj).Result;
            objReturn = response.Content.ReadAsAsync<IssueMaterialModel>().Result;
            //return View(objReturn);
            //string msg = response.Content.ReadAsStringAsync().Result;

            // return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                //return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
                return Json(new { success = false, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //return Json(new { Data = objReturn }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, Message = "Sucessfully Updated!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}