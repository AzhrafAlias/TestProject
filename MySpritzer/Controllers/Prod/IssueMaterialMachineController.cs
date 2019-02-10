using MySpritzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.Prod
{
    public class IssueMaterialMachineController : Controller
    {
        // GET: IssueMaterialMachine
        public ActionResult Index(string Company)
        {
            if (Session["User"] == null)
            {
                Session["From"] = "IssueMaterialMachine";
                return RedirectToAction("Login", "Home");
            }


            IssueMaterialMachineModel entity = new IssueMaterialMachineModel();
            entity.Company = Session["Company"].ToString();
            //HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialValidatePart", entity).Result;

            // return View(lstEnts);
            return View(entity);
        }

        public ActionResult ValidatePart(IssueMaterialMachineModel obj)
        {
            string msg = string.Empty;
            IssueMaterialMachineModel objReturn = new IssueMaterialMachineModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialMachineValidatePart", obj).Result;
            objReturn = response.Content.ReadAsAsync<IssueMaterialMachineModel>().Result;
           
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

        public ActionResult ValidateLot(IssueMaterialMachineModel obj)
        {
            string msg = string.Empty;
            IssueMaterialMachineModel objReturn = new IssueMaterialMachineModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialMachineValidateLot", obj).Result;
            objReturn = response.Content.ReadAsAsync<IssueMaterialMachineModel>().Result;
            
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

        public ActionResult Save(IssueMaterialMachineModel obj)
        {
            string msg = string.Empty;
            IssueMaterialMachineModel objReturn = new IssueMaterialMachineModel();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("IssueMaterialMachineConfirm", obj).Result;
            objReturn = response.Content.ReadAsAsync<IssueMaterialMachineModel>().Result;
            
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