using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using errHttpAmin = System.Web.Http;

namespace MySpritzer.Controllers.WI
{
    public class PickRawMtrlController : Controller
    {
        // GET: PickRawMtrl
        public ActionResult Index(string Company, string UserId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            Session["PickRawMtrlModels"] = null;
            Session["ScanLotPRMLst"] = null;

            List<PickRawMtrlModel> entitys = new List<PickRawMtrlModel>();
            if (!string.IsNullOrEmpty(Company))
            {
                string Qry = $"PickRawMtrl?Company={Company}&UserId={UserId}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    entitys = response.Content.ReadAsAsync<List<PickRawMtrlModel>>().Result;
                    entitys.ForEach(s => s.UserId = UserId);
                    Session["PickRawMtrlModels"] = entitys;
                }
                else
                {
                    var httpErr = response.Content.ReadAsAsync<errHttpAmin.HttpError>();
                    ViewBag.Error = httpErr.Result["Message"].ToString();
                }
            }

            //test data
            //PickRawMtrlModel p = new PickRawMtrlModel();
            //p.RequestDate = DateTime.Now;
            //p.PartNum = "Test Part Num";
            //p.Qty = 3;
            //p.UOM = "Test UOM";

            //p.Id = 1;
            //p.PalletTypeInfo = new List<PalletTypeModel>();
            //entitys.Add(p);

            return View(entitys);
        }

        // GET: PickRawMtrl/Details/5
        public ActionResult Pick(Guid id, bool isFirstGet = true)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<PickRawMtrlModel> entitys = (List<PickRawMtrlModel>)Session["PickRawMtrlModels"];
            PickRawMtrlModel objEntity = entitys.Where(w => w.SysRowId == id).FirstOrDefault();
            //if (isFirstGet)
            //{
                objEntity.Qty = objEntity.ActQty;
                //Session["ScanLotPRMLst"] = null;
                //Testing purpose
                //objEntity.ConvFactor = 5;
            //}
            //else
            //{
                if (Session["ScanLotPRMLst"] != null)
                {
                    List<PickRawMtrlModel> objScanLotPRMList = (List<PickRawMtrlModel>)Session["ScanLotPRMLst"];
                    decimal scanqty = objScanLotPRMList.Sum(s => s.Qty);

                    objEntity.Qty = objEntity.ActQty - scanqty;

                    //remainConvFactor from prev
                    foreach (var c in objScanLotPRMList)
                    {
                        objEntity.ConvFactor = c.ConvFactor;
                    }

                ViewBag.ScanListCount = objScanLotPRMList.Count;
                }
            //}

            

            //drop down data
            ViewBag.PalletTypeInfo = new SelectList(objEntity.PalletTypeInfo, "PartNum", "PartNum");
            //ViewBag.PalletTypeInfo = objEntity.PalletTypeInfo.Select(s => new SelectList(new[] { new { value = s.PartNum, text = s.PartNum } }));
            return View(objEntity);
        }


        [HttpPost]
        public ActionResult Pick(PickRawMtrlModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<PickRawMtrlModel> objScanLotPRMList = new List<PickRawMtrlModel>();
            if (Session["ScanLotPRMLst"] != null)
            {
                objScanLotPRMList = (List<PickRawMtrlModel>)Session["ScanLotPRMLst"];
            }
            else
            {
                objScanLotPRMList = new List<PickRawMtrlModel>();
            }

            if (obj.isFinish == false)
            {
                objScanLotPRMList.Add(obj);
            }
            
            Session["ScanLotPRMLst"] = objScanLotPRMList;
            //objScanLotPRMList.ForEach(s => s.UserId = obj.UserId);
            decimal qty = objScanLotPRMList.Sum(s => s.Qty);
            decimal actQty = obj.ActQty;
            //List<PickRawMtrlModel> objPRMEntitys = (List<PickRawMtrlModel>)TempData["PickRawMtrlModels"];
            //PickRawMtrlModel objEntity = objPRMEntitys.Where(w => w.SysRowId == obj.SysRowId).FirstOrDefault();
            //objEntity.Qty = 0;
            if (qty < actQty)
            {
                //objEntity.Qty = actQty - qty;
                //return RedirectToAction("Pick", new { id = obj.SysRowId, isFirstGet = false });
                return Json(new {isFinish = false, Success = false, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //bool isSuccessStatusCode = false;
                string msg = "";

                try
                {
                    //Session["ScanLotPRMLst"] = null;
                    obj.isFinish = true;
                    foreach (PickRawMtrlModel item in objScanLotPRMList)
                    {
                        HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("PickRawMtrl", item).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //msg = response.Content.ReadAsStringAsync().Result;
                            msg = "";
                        }
                        else
                        {
                            //isSuccessStatusCode = false;

                            string msgJson = response.Content.ReadAsStringAsync().Result;

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

                            return Json(new { isFinish = true, Success = false, Message = msg }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //isSuccessStatusCode = true;

                    //ViewBag.Status = "Inserted Sucessfully!";
                    string Company = "";
                    string UserId = "";

                    if (!string.IsNullOrEmpty(Session["Company"] as string))
                    {
                        Company = Session["Company"] as string;

                    }
                    if (!string.IsNullOrEmpty(Session["User"] as string))
                    {

                        UserId = Session["User"].ToString();
                    }
                    return Json(new { isFinish = true, Success = true, Message = msg }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction($"Index", new { Company = Company, UserId = UserId });
                }
                catch (Exception ex)
                {
                    //ViewBag.Status = ex.Message.ToString();
                    msg = ex.Message.ToString();
                    return Json(new { isFinish = true, Success = false, Message = msg }, JsonRequestBehavior.AllowGet);
                }
                

            }

            //drop down data
            //List<PickRawMtrlModel> entitys = (List<PickRawMtrlModel>)TempData["PickRawMtrlModels"];
            //PickRawMtrlModel objEntity = entitys.Where(w => w.SysRowId == obj.SysRowId).FirstOrDefault();
            //if (objEntity != null)
            //{
            //    ViewBag.PalletTypeInfo = new SelectList(objEntity.PalletTypeInfo, "PartNum", "PartNum");
            //}

            //return View(obj);
            
        }

        [HttpGet]
        public ActionResult GetWarehseLst(string Company, string PartNum, string LotNum)
        {
            List<WarehseModel> WarehseModels = new List<WarehseModel>();
            if (!string.IsNullOrEmpty(Company))
            {
                string Qry = $"PRMWarehouseSugg?Company={Company}&PartNum={PartNum}&LotNum={LotNum}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    WarehseModels = response.Content.ReadAsAsync<List<WarehseModel>>().Result;

                }
                else
                {
                    return new HttpStatusCodeResult(500, "Custom Error Message 2");
                }
            }
            return PartialView("~/Views/PickRawMtrl/GetWarehseLst.cshtml", WarehseModels);

        }

        //TuConvPickRawMtrl
        public ActionResult GetTuConv(string Company, string PartNum, string LotNum, string TuId)
        {

            string msg = "";
            bool success = false;

            decimal _TuConv = 0;
            decimal _Qty = 0;
            string _UOMCode = "";

            //testing purpose
            //success = true;
            //_TuConv = 10;
            //_Qty = 18000;
            //_UOMCode = "BAG";

            PickRawMtrlTuIdModel pickRawMtrlTuIdModel = new PickRawMtrlTuIdModel();

            try
            {
                string qry = $"TuConvPickRawMtrl?Company={Company}&LotNum={LotNum}&PartNum={PartNum}&TuId={TuId}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    pickRawMtrlTuIdModel = response.Content.ReadAsAsync<PickRawMtrlTuIdModel>().Result;
                    _TuConv = pickRawMtrlTuIdModel.TuConv;
                    _Qty = pickRawMtrlTuIdModel.Qty;
                    _UOMCode = pickRawMtrlTuIdModel.UOMCode;

                    msg = "";
                    success = true;
                }
                else
                {
                    msg = response.Content.ReadAsStringAsync().Result;
                    success = false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                success = false;
            }

            return Json(new { Message = msg, Success = success, TuConv = _TuConv, Qty = _Qty, UOMCode = _UOMCode }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ValidateLot(string Company, string PartNum, string LotNum)
        {
            string msg = "";
            bool success = false;

            string qry = $"PickRawValidateLot?&Company={Company}&PartNum={PartNum}&LotNum={LotNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
            if (response.IsSuccessStatusCode)
            {
                msg = "";
                success = true;
            }
            else
            {
                string msgJson = response.Content.ReadAsStringAsync().Result;
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

                success = false;
            }

            return Json(new { Success = success, Message = msg }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ValidateWH(string Company, string PartNum, string FromWarehse)
        {
            string msg = "";
            bool success = false;

            string ValidateType = "WH";
            //string qry = $"PickRawMaterialValidateWHBinOHQ?ValidateType={ValidateType}&Company={Company}&PartNum=&LotNum=&WarehouseCode={FromWarehse}&BinNum={FromBinNum}&OnHandQty=";
            string qry = $"PickRawMaterialValidateWHBinOHQ?ValidateType={ValidateType}&Company={Company}&PartNum={PartNum}&LotNum=&WarehouseCode={FromWarehse}&BinNum=&OnHandQty=0";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
            if (response.IsSuccessStatusCode)
            {
                msg = "";
                success = true;
            }
            else
            {
                string msgJson = response.Content.ReadAsStringAsync().Result;
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

                success = false;
            }

            return Json(new { Success = success, Message = msg }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ValidateWHBin(string Company, string FromWarehse, string FromBinNum, string PartNum)
        {
            string msg = "";
            bool success = false;
            //public HttpResponseMessage Get(string ValidateType, string Company, string PartNum, string LotNum, string WarehouseCode, string BinNum, decimal OnHandQty)
            string ValidateType = "WHBin";
            string qry = $"PickRawMaterialValidateWHBinOHQ?ValidateType={ValidateType}&Company={Company}&PartNum={PartNum}&LotNum=&WarehouseCode={FromWarehse}&BinNum={FromBinNum}&OnHandQty=0";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
            if (response.IsSuccessStatusCode)
            {
                msg = "";
                success = true;
            }
            else
            {
                string msgJson = response.Content.ReadAsStringAsync().Result;
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

                success = false;
            }

            return Json(new { Success = success, Message = msg }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetOnHandQty(string Company, string PartNum, string LotNo, string FromWarehse, string FromBinNum, decimal Qty)
        {
            if (Qty > 0)
            {
                string msg = "";
                bool success = false;
                //public HttpResponseMessage Get(string ValidateType, string Company, string PartNum, string LotNum, string WarehouseCode, string BinNum, decimal OnHandQty)
                string ValidateType = "OHQ";
                string qry = $"PickRawMaterialValidateWHBinOHQ?ValidateType={ValidateType}&Company={Company}&PartNum={PartNum}&LotNum={LotNo}&WarehouseCode={FromWarehse}&BinNum={FromBinNum}&OnHandQty={Qty}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    msg = "";
                    success = true;
                }
                else
                {
                    string msgJson = response.Content.ReadAsStringAsync().Result;
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

                    success = false;
                }

                return Json(new { Success = success, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = "Qty is less than 0." }, JsonRequestBehavior.AllowGet);
            }

            
        }
    }
}

//string qry = $"PickRawValidateWHBinOHQ?ValidateType={ValidateType}&Company={Company}&PartNum={PartNum}&LotNum={LotNum}&WarehouseCode={FromWarehse}&BinNum={""}&OnHandQty={0}";