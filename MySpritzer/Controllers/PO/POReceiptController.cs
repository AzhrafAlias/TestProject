using MySpritzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using errHttpAmin = System.Web.Http;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MySpritzer.Controllers
{
    public class POReceiptController : Controller
    {
        //int poDetailId;
        // GET: POReceipt

        public ActionResult Index(string PONum, string Company)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            Company = (string)Session["Company"];

            POHeaderModel entity = new POHeaderModel();
            if (!string.IsNullOrEmpty(Company)&& !string.IsNullOrEmpty(PONum))
            {
                string Qry = $"POReceipt?PONum={PONum}&Company={Company}"; //Ashraf
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {

                    entity = response.Content.ReadAsAsync<POHeaderModel>().Result;
                    if (Request.QueryString["DONum"] != null)
                        entity.DONum = Request.QueryString["DONum"].ToString();
                    Session["POHeaderModel"] = entity;
                }
                else
                {
                    var httpErr = response.Content.ReadAsAsync<errHttpAmin.HttpError>();
                    ViewBag.Error = httpErr.Result["Message"].ToString();
                }

            }

            return View(entity);
        }

        // GET: POReceipt/Details/5
        public ActionResult Details(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            POHeaderModel entity = (POHeaderModel)Session["POHeaderModel"];
            //Session["POHeaderModel"] = entity;
            if (Request.QueryString["DONum"] != null) {
                entity.DONum = Request.QueryString["DONum"].ToString();
            }

            Session["PONum"] = entity.PONum;
            Session["DONum"] = entity.DONum;
            Session["Company"] = entity.Company;

            //TempData.Keep("POHeaderModel");

            PODetailModel objPODtl = entity?.PODetails?.Where(w => w.POLine == id).SingleOrDefault();
            objPODtl.POInfo = $"{objPODtl.PONum}/{objPODtl.POLine}";
            objPODtl.SuppNum = entity.SuppNum;
            objPODtl.DONum = entity.DONum;

            entity.PODetails = new List<PODetailModel>();
            entity.PODetails.Add(objPODtl);

            return View(objPODtl);
        }

        public ActionResult TuValidation(string Company, string DONum, string TuId, string LotNum, string PartNum, string SuppNum)
        {
            string msg = "";
            bool isValid = false;
            
            string qry = $"TuValidation?Company={Company}&DONum={DONum}&TuId={TuId}&LotNum={LotNum}&PartNum={PartNum}&SuppNum={SuppNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
            if (response.IsSuccessStatusCode)
            {
                isValid = response.Content.ReadAsAsync<bool>().Result;
            }
            else
            {
                msg = response.Content.ReadAsStringAsync().Result;
            }
            return Json(new { Message = msg, Valid = isValid }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LotAttribute(PODetailModel obj)
        {
            if (Session["User"] == null || obj == null)
            {
                return RedirectToAction("Login", "Home");
            }
            

            Session["LotNum"] = obj.LotNum;

            //if (TempData["ModifiedPODetailModel"] == obj) {

            //}

            POHeaderModel entity = (POHeaderModel)Session["POHeaderModel"];
            //This is for LotAttribute only
            PODetailModel objPODtl = new PODetailModel();
            objPODtl = entity.PODetails.Where(w => w.POLine == obj.POLine).SingleOrDefault();

            Session["LotNum"] = objPODtl.LotNum;
            Session["PartNum"] = objPODtl.PartNum;

            //entity.PODetails = new List<PODetailModel>();
            //entity.PODetails.Add(obj );
            entity.PODetails.Where(w => w.POLine == obj.POLine).ToList().ForEach(s => s.TuId = obj.TuId);
            entity.PODetails.Where(w => w.POLine == obj.POLine).ToList().ForEach(s => s.LotNum = obj.LotNum);
            entity.PODetails.Where(w => w.POLine == obj.POLine).ToList().ForEach(s => s.Qty = obj.Qty);
            entity.PODetails.Where(w => w.POLine == obj.POLine).ToList().ForEach(s => s.TuConv = obj.TuConv);

            Session["ModifiedPOHeaderModel"] = entity;
            //TempData.Keep("ModifiedPOHeaderModel");

            //Lot Validation (is it exist in PartLot table or not)
            string Qry = $"LotNumValidation?Company={obj.Company}&PartNum={obj.PartNum}&LotNum={obj.LotNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            bool isLotExist = response.Content.ReadAsAsync<bool>().Result;

            //AMIN - Check Lot Attribute M or T or else
            LotAttributeModel tempLotAtt = obj.LotAttribute;
            bool isNullAtt = false;
            if ((tempLotAtt.AttBatch == null && tempLotAtt.AttMfgBatch == null && tempLotAtt.AttMfgLot == null && tempLotAtt.AttHeat == null && tempLotAtt.AttFirmware == null && tempLotAtt.AttBeforeDt == null && tempLotAtt.AttMfgDt == null && tempLotAtt.AttCureDt == null && tempLotAtt.AttExpDt == null)
                || (tempLotAtt.AttBatch == "N" && tempLotAtt.AttMfgBatch == "N" && tempLotAtt.AttMfgLot == "N" && tempLotAtt.AttHeat == "N" && tempLotAtt.AttFirmware == "N" && tempLotAtt.AttBeforeDt == "N" && tempLotAtt.AttMfgDt == "N" && tempLotAtt.AttCureDt == "N" && tempLotAtt.AttExpDt == "N"))
            {
                isNullAtt = true;
            }
            else
            {
                isNullAtt = false;
            }

            //test data
            //objPODtl.TrackLot = true;
            //isLotExist = false;
            //isNullAtt = false;

            //tempLotAtt.AttBeforeDt = "M";
            //tempLotAtt.AttMfgDt = "M";
            //tempLotAtt.AttCureDt = "M";
            //tempLotAtt.AttExpDt = "M";


            //if (objPODtl!=null && objPODtl.TrackLot && !isLotExist)
            if (objPODtl != null && objPODtl.TrackLot && !isLotExist && !isNullAtt)
            {
                //LotNum Validation fails then open LotAttribute Page
                LotAttributeModel objLotAttribute = obj.LotAttribute == null ? objPODtl.LotAttribute : obj.LotAttribute;
                objLotAttribute.LotNum = obj.LotNum;
                objLotAttribute.DONum = obj.DONum;
                return View(objLotAttribute);
            }
            else
            {
                
                //LotNum Validation sucess then open PrintTag Page
                PrintTagModel objPrintTag = new PrintTagModel();
                //if Carton label tick true then prompt the printtag page
                if (!entity.CartonLabel)
                {
                   return RedirectToAction("ManualUpdate", objPrintTag);
                }
                objPrintTag.Id = obj.Id;
                objPrintTag.Company = obj.Company;
                objPrintTag.PONum = obj.PONum;
                objPrintTag.POLine = obj.POLine;
                objPrintTag.DONum = obj.DONum;
                objPrintTag.PartNum = obj.PartNum;
                objPrintTag.LotNum = obj.LotNum;

                ViewBag.Uom = entity.PODetails.Select(s => s.UomCode).FirstOrDefault<string>();
                ViewBag.UserQty = entity.PODetails.Select(s => s.Qty).FirstOrDefault<decimal>();

                return RedirectToAction("PrintTag", objPrintTag);
                //return View("PrintTag", objPrintTag);
            }

        }


        [HttpPost]
        public ActionResult Update(PrintTagModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "-";
            string msg = "-";

            POHeaderModel entity = (POHeaderModel)Session["ModifiedPOHeaderModel"];
            TempData["PONum"] = entity.PONum;
            TempData["DONum"] = entity.DONum;
            TempData["Company"] = entity.Company;

            entity.PODetails.ForEach(s => s.PrintTag = obj);

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("POReceipt", entity).Result;
            //TempData["RespondMessage"] = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                errMsg = "";
                //errMsg = "TEST";
                //return RedirectToAction("Index", new { Company = entity.Company, PONum = entity.PONum, DONum = entity.DONum});
                //var url = '@Url.Action("Index", "POReceipt")' + "?Company=" + '@Model.Company' + "&PONum=" + '@Model.PONum' + "&DONum=" + '@Model.DONum';
            }
            else
            {
                msg = response.Content.ReadAsStringAsync().Result;
                try
                {
                    JObject jObject = JObject.Parse(msg);
                    string msgJ = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    if (msgDtl == null)
                    {
                        errMsg = msgJ;
                    }
                    else
                    {
                        errMsg = msgDtl;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = msg;
                }
            }

            return Json(new { Message = errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManualUpdate(PrintTagModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "-";

            POHeaderModel entity = (POHeaderModel)Session["ModifiedPOHeaderModel"];
            TempData["PONum"] = entity.PONum;
            TempData["DONum"] = entity.DONum;
            TempData["Company"] = entity.Company;

            entity.PODetails.ForEach(s => s.PrintTag = obj);

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("POReceipt", entity).Result;
            //TempData["RespondMessage"] = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                errMsg = "";
            }
            else
            {
                msg = response.Content.ReadAsStringAsync().Result;
                try
                {
                    JObject jObject = JObject.Parse(msg);
                    string msgJ = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    if (msgDtl == null)
                    {
                        errMsg = msgJ;
                    }
                    else
                    {
                        errMsg = msgDtl;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = msg;
                }
            }

            ViewBag.Status = errMsg;
            ViewBag.PONum = entity.PONum;
            ViewBag.DONum = entity.DONum;
            ViewBag.Company = entity.Company;
            return View();

           // return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
        }
        // GET: POReceipt/Delete/5
        public ActionResult Delete(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }



        public ActionResult PrintTag(LotAttributeModel lotAttributeModel)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            POHeaderModel entity = (POHeaderModel)Session["ModifiedPOHeaderModel"];

            entity.PODetails.ForEach(s => s.LotAttribute = lotAttributeModel);

            Session["ModifiedPOHeaderModel"] = entity;
            //TempData.Keep("ModifiedPOHeaderModel");
            ViewBag.Uom = entity.PODetails.Select(s => s.UomCode).FirstOrDefault<string>();
            ViewBag.UserQty = entity.PODetails.Select(s => s.Qty).FirstOrDefault<decimal>();

            PrintTagModel objPrintTag = new PrintTagModel();
            if (!entity.CartonLabel)
            {
                return RedirectToAction("ManualUpdate", objPrintTag);
            }
            
            objPrintTag.Id = lotAttributeModel.Id;
            objPrintTag.Company = lotAttributeModel.Company;
            objPrintTag.PONum = lotAttributeModel.PONum;
            objPrintTag.POLine = lotAttributeModel.POLine;
            objPrintTag.DONum = lotAttributeModel.DONum;
            objPrintTag.PartNum = lotAttributeModel.PartNum;
            objPrintTag.LotNum = lotAttributeModel.LotNum;

            return View(objPrintTag);
        }

        //public ActionResult CheckPONum(string PONum, string Company) {
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    string msg = "";

        //    if (!string.IsNullOrEmpty(Company) && !string.IsNullOrEmpty(PONum))
        //    {
        //        string Qry = $"POReceipt?PONum={PONum}&Company={Company}";
        //        HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var httpErr = response.Content.ReadAsAsync<errHttpAmin.HttpError>();
        //            msg = httpErr.Result["Message"].ToString();
        //        }

        //    }

            
        //    return Json(new { Message = msg}, JsonRequestBehavior.AllowGet);
        //}

    }
}