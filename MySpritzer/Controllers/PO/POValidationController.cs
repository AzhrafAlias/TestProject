using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers
{
    public class POValidationController : Controller
    {
        // GET: POValidation
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult TuValid(string lotNum)
        {
            bool isValue = false;
            if (lotNum == "ABC")
            {
                isValue = true;
            }

            return Json(new { Value = isValue }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TuValidation(string Company, string DONum,int SuppNum,string TuId, string lotNum)
        {
            if (!string.IsNullOrEmpty(lotNum))
            {
                string Qry = $"TuValidation?Company={Company}&DONum={DONum}&TuId={TuId}&LotNum={lotNum}&SuppNum{SuppNum}";
            }
            bool isFailed = false;
            if (TuId=="XYZ")
            {
                isFailed = true;
            }
            return View(isFailed);
        }

        public ActionResult LotNumValidation(string Company, string PartNum, string lotNum)
        {
            bool isFailed = false;
            if (lotNum == "LOT")
            {
                isFailed = true;
            }
            return View(isFailed);
        }
    }
}