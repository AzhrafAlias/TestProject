using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using MySpritzer.Models;

namespace MySpritzer.Controllers
{
    public class InterCoRcvController : Controller
    {
        // GET: InterCoRcv
        public ActionResult Index(string Company)
        {
            if (Session["User"] == null)
            {
                Session["From"] = "InterCoRcv";
                return RedirectToAction("Login", "Home");
            }
            //Session["Company"] = "CSTP";
            //Session["User"] = "a";

            InterCoRcvModel entity = new InterCoRcvModel();
            entity.Company = Session["Company"].ToString();

            return View(entity);

        }

        public ActionResult GetPODO(string Company, int PO, string DO)
        {

            InterCoRcvModel obj = new InterCoRcvModel();
            string Qry = $"ICRecGetPO?Company={Company}&PONum={PO}&DONum={DO}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            obj = response.Content.ReadAsAsync<InterCoRcvModel>().Result;
            //return View(obj);
            if (obj.TranStatus != "Success")
            {
                string msg = obj.TranStatus;
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = obj }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Confirm(InterCoRcvModel obj)
        {
            string msg = string.Empty;
            InterCoRcvModel objReturn = new InterCoRcvModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("ICRecConfirm", obj).Result;
            objReturn = response.Content.ReadAsAsync<InterCoRcvModel>().Result;
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

        public ActionResult Reject(InterCoRcvModel obj)
        {
            string msg = string.Empty;
            InterCoRcvModel objReturn = new InterCoRcvModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("ICRecReject", obj).Result;
            objReturn = response.Content.ReadAsAsync<InterCoRcvModel>().Result;
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
    }
}