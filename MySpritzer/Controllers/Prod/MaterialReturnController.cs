using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.Prod
{
    public class MaterialReturnController : Controller
    {
        // GET: MaterialReturn
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                Session["From"] = "MaterialReturn";
                return RedirectToAction("Login", "Home");
            }

            MaterialReturnModel entity = new MaterialReturnModel();
            entity.Company = Session["Company"].ToString();
            entity.CreatedBy = Session["User"].ToString();
            entity.ReasonList = GetReason(Session["Company"].ToString());
            return View(entity);
        }

        public ActionResult ValidatePart(MaterialReturnModel obj)
        {
            MaterialReturnWEBAPI objSend = new MaterialReturnWEBAPI();
            objSend = ConvertToModelAPI(obj);
            string msg = string.Empty;
            MaterialReturnWEBAPI objReturn = new MaterialReturnWEBAPI();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("RetnMaterialValidatePart", objSend).Result;
            objReturn = response.Content.ReadAsAsync<MaterialReturnWEBAPI>().Result;
            MaterialReturnModel objMVC = new MaterialReturnModel();
            objMVC = ConvertToModelMVC(objReturn);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = objMVC }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidateLot(MaterialReturnModel obj)
        {
            MaterialReturnWEBAPI objSend = new MaterialReturnWEBAPI();
            objSend = ConvertToModelAPI(obj);
            string msg = string.Empty;
            MaterialReturnWEBAPI objReturn = new MaterialReturnWEBAPI();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("RetnMaterialValidateLot", objSend).Result;
            objReturn = response.Content.ReadAsAsync<MaterialReturnWEBAPI>().Result;
            MaterialReturnModel objMVC = new MaterialReturnModel();
            objMVC = ConvertToModelMVC(objReturn);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = objMVC }, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult ValidateWHBin(string Company, string Warehouse, string BinNum)
        {
            string msg;
            bool success = false ;

            string qry = $"RetnMaterialValidateWH?Company={Company}&Warehouse={Warehouse}&BinNum={BinNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
            if (response.IsSuccessStatusCode)
            {
                msg = "";
                success = true;
            }
            else
            {
                string msgJson = response.Content.ReadAsStringAsync().Result;
                success = false;
                try
                {
                    JObject jObject = JObject.Parse(msgJson);
                    string msgJ = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    if (msgDtl == null)
                    {
                        msg = msgJ;
                    }
                    else
                    {
                        msg = msgDtl;
                    }
                }
                catch (Exception ex)
                {
                    msg = msgJson;
                }
            }
            return Json(new { Success = success, Message = msg }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Close()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Save(MaterialReturnModel obj)
        {
            MaterialReturnWEBAPI objSend = new MaterialReturnWEBAPI();
            objSend = ConvertToModelAPI(obj);
            string msg = string.Empty;
            MaterialReturnWEBAPI objReturn = new MaterialReturnWEBAPI();
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("RetnMaterialConfirm", objSend).Result;
            objReturn = response.Content.ReadAsAsync<MaterialReturnWEBAPI>().Result;
            MaterialReturnModel objMVC = new MaterialReturnModel();
            objMVC = ConvertToModelMVC(objReturn);
            if (objReturn.TranStatus != "Success")
            {
                msg = objReturn.TranStatus;
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = objMVC }, JsonRequestBehavior.AllowGet);
            }
        }

        public IEnumerable<SelectListItem> GetReason(string Company)
        {

            List<DMRReason> dMRs = new List<DMRReason>();
            string Qry = $"ReturnReason?Company={Company}";
            List<SelectListItem> ReturnReason = new List<SelectListItem>();
            try
            {

            }
            catch (Exception ex)
            {

            }
            
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            if (response.IsSuccessStatusCode)
            {
                dMRs = response.Content.ReadAsAsync<List<DMRReason>>().Result;
                foreach (DMRReason o in dMRs)
                {
                    SelectListItem selListItem = new SelectListItem() { Value = o.ReasonCode, Text = o.Description };
                    ReturnReason.Add(selListItem);
                }
            }
            
            return ReturnReason;
        }

        private MaterialReturnWEBAPI ConvertToModelAPI(MaterialReturnModel materialReturn)
        {
            MaterialReturnWEBAPI materialReturnWEBAPI = new MaterialReturnWEBAPI();
            materialReturnWEBAPI.ActPartNum = materialReturn.ActPartNum;
            materialReturnWEBAPI.BalQty = materialReturn.BalQty;
            materialReturnWEBAPI.Company = materialReturn.Company;
            materialReturnWEBAPI.CreatedBy = materialReturn.CreatedBy;
            materialReturnWEBAPI.Desc = materialReturn.Desc;
            materialReturnWEBAPI.FromBin = materialReturn.FromBin;
            materialReturnWEBAPI.FromWH = materialReturn.FromWH;
            materialReturnWEBAPI.ToBin = materialReturn.ToBin;
            materialReturnWEBAPI.ToWH = materialReturn.ToWH;
            materialReturnWEBAPI.Lot = materialReturn.Lot;
            materialReturnWEBAPI.PartNum = materialReturn.PartNum;
            materialReturnWEBAPI.Qty = materialReturn.Qty;
            materialReturnWEBAPI.Reason = materialReturn.Reason;
            materialReturnWEBAPI.RetReason = materialReturn.RetReason;
            materialReturnWEBAPI.SupplierPart = materialReturn.SupplierPart;
            materialReturnWEBAPI.TranStatus = materialReturn.TranStatus;
            materialReturnWEBAPI.UOM = materialReturn.UOM;
            return materialReturnWEBAPI;
        }

        private MaterialReturnModel ConvertToModelMVC(MaterialReturnWEBAPI materialReturn)
        {
            MaterialReturnModel materialReturnWEBAPI = new MaterialReturnModel();
            materialReturnWEBAPI.ActPartNum = materialReturn.ActPartNum;
            materialReturnWEBAPI.BalQty = materialReturn.BalQty;
            materialReturnWEBAPI.Company = materialReturn.Company;
            materialReturnWEBAPI.CreatedBy = materialReturn.CreatedBy;
            materialReturnWEBAPI.Desc = materialReturn.Desc;
            materialReturnWEBAPI.FromBin = materialReturn.FromBin;
            materialReturnWEBAPI.FromWH = materialReturn.FromWH;
            materialReturnWEBAPI.Lot = materialReturn.Lot;
            materialReturnWEBAPI.PartNum = materialReturn.PartNum;
            materialReturnWEBAPI.Qty = materialReturn.Qty;
            materialReturnWEBAPI.Reason = materialReturn.Reason;
            materialReturnWEBAPI.RetReason = materialReturn.RetReason;
            materialReturnWEBAPI.SupplierPart = materialReturn.SupplierPart;
            materialReturnWEBAPI.TranStatus = materialReturn.TranStatus;
            materialReturnWEBAPI.UOM = materialReturn.UOM;
            return materialReturnWEBAPI;
        }
    }
}