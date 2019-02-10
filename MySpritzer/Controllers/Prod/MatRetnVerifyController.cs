using MySpritzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.Prod
{
    public class MatRetnVerifyController : Controller
    {
        // GET: MatRetnVerify
        public ActionResult Index(string Company)
        {

            if (Session["User"] == null)
            {
                Session["From"] = "MatRetnVerify";
                return RedirectToAction("Login", "Home");
            }

            string Comp = string.Empty;

            if (Company == null)
            {
                Comp = Session["Company"].ToString();
            }
            else
            {
                Comp = Company;
            }
            List<MatRetnVerifyModel> objList = new List<MatRetnVerifyModel>();
            try
            {
                string Qry = $"MatRetnVerifyGetList?Company={Comp}&SessionUser={Session["User"].ToString()}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    objList = response.Content.ReadAsAsync<List<MatRetnVerifyModel>>().Result;
                }
            }
            catch (Exception ex)
            {

            }

            //Dummy data for view testing purpose

            //MatRetnVerifyModel mrv = new MatRetnVerifyModel();
            //mrv.Company = "Test Comp";
            //mrv.PartNum = "Test PartNum";
            //mrv.ActPartNum = "Test PartNum";
            //mrv.SupplierPart = true;
            //mrv.FromWH = "Test wh";
            //mrv.FromBin = "Test wh";
            //mrv.UOM = "Test";
            //mrv.Desc = "Test";
            //mrv.Lot = "Test";
            //mrv.BalQty = 10;
            //mrv.Qty = 10;
            //mrv.Reason = "Test";
            //mrv.RetReason = "Test";
            //mrv.TranStatus = "Test";
            //mrv.CreatedBy = "Test";
            //mrv.KeyId = 1;
            //objList.Add(mrv);

            TempData["MTV"] = objList;
            TempData.Keep();
            return View(objList);
        }

        public ActionResult Detail(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }



            //MatRetnVerifyModel test = new MatRetnVerifyModel();
            List<MatRetnVerifyModel> objList = null;
            if (TempData["MTV"] != null)
            {
                objList = (List<MatRetnVerifyModel>)TempData["MTV"];
            }
            MatRetnVerifyModel obj = objList.Where(w => w.KeyId == id).FirstOrDefault();
            TempData.Keep();
            return View(obj);
        }

        public ActionResult Confirm(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string User = (string)Session["User"];

            List<MatRetnVerifyModel> objList = null;
            if (TempData["MTV"] != null)
            {
                objList = (List<MatRetnVerifyModel>)TempData["MTV"];
            }
            MatRetnVerifyModel obj = objList.Where(w => w.KeyId == id).FirstOrDefault();
            obj.SessionUser = User;

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("MatRetnConfirm", obj).Result;
            string objReturn = string.Empty;


            objReturn = response.Content.ReadAsStringAsync().Result;

            return Json(new { Message = objReturn }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Reject(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<MatRetnVerifyModel> objList = null;
            if (TempData["MTV"] != null)
            {
                objList = (List<MatRetnVerifyModel>)TempData["MTV"];
            }
            MatRetnVerifyModel obj = objList.Where(w => w.KeyId == id).FirstOrDefault();
            obj.SessionUser = (string)Session["User"];
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("MatRetnReject", obj).Result;
            string objReturn = string.Empty;


            objReturn = response.Content.ReadAsStringAsync().Result;

            return Json(new { Message = objReturn }, JsonRequestBehavior.AllowGet);
        }
    }
}