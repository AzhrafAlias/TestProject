using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;

namespace MySpritzer.Controllers
{
    public class LoosePickingController : Controller
    {
        // Get a list of SONum by Company
        // GET: LoosePicking.
        public ActionResult Index(string Company)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Clear unused session
            Session["PalletSelectionModels"] = null;
            Session["LoosePickingDetailModel"] = null;
            Session["PalletSelectionModel"] = null;
            Session["PalletNum"] = null;
            Session["RetrieveModel"] = null;
            Session["SONum"] = null;
            Session["ShipByDate"] = null;

            //For back button purpose. From Details page to Index page without passing Parameters. Set the parameter from Session itself
            if (string.IsNullOrEmpty(Company) && Session["Company"] != null)
            {
                Company = (string)Session["Company"];
            }

            List<LoosePickingHeaderModel> lphs = new List<LoosePickingHeaderModel>();

            try
            {
                string Qry = $"LoosePicking?Company={Company}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;

                if (response.IsSuccessStatusCode)
                {
                    lphs = response.Content.ReadAsAsync<List<LoosePickingHeaderModel>>().Result;
                    ViewBag.Status = "";
                }
                else
                {
                    string msgJson = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        JObject jObject = JObject.Parse(msgJson);
                        string msg = (string)jObject.SelectToken("Message");
                        string msgDtl = (string)jObject.SelectToken("MessageDetail");
                        if (msgDtl == null)
                        {
                            ViewBag.Status = msg;
                        }
                        else
                        {
                            ViewBag.Status = msgDtl;
                        }
                    }
                    catch(Exception ex)
                    {
                        ViewBag.Status = msgJson;
                    }

                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = ex.Message.ToString();
            }

            return View(lphs);
        }

        // Get a list of item (PartNum) in a SONum
        // GET: LoosePicking/Details/5
        public ActionResult Details(string Company=null, string SONum=null, DateTime? ShipByDate=null)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Clear unused session
            Session["PalletSelectionModels"] = null;

            
            // Only set a new session value if record not same
            // Set into session if param not empty. This is for back button purpose
            if (!string.IsNullOrEmpty(Company) && !string.IsNullOrEmpty(SONum) && ShipByDate != null)
            {
                Session["Company"] = Company; 
                Session["SONum"] = SONum;
                Session["ShipByDate"] = ShipByDate;
            }
            else
            {
                Company = (string)Session["Company"];
                SONum = (string)Session["SONum"];
                ShipByDate = (DateTime)Session["ShipByDate"];
            }

            string dt = Convert.ToDateTime(ShipByDate).ToString("yyyy-MM-dd"); //ShipByDate.ToString("yyyy-MM-dd");

            List<LoosePickingDetailModel> lpds = new List<LoosePickingDetailModel>();

            try
            {
                string Qry = $"LoosePicking?Company={Company}&SONum={SONum}&ShipByDate={dt}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    lpds = response.Content.ReadAsAsync<List<LoosePickingDetailModel>>().Result;
                    //If no list in this page, just automatically redirect to SO Num list page
                    if (lpds == null)
                    {
                        return RedirectToAction("Index", "LoosePicking");
                    }
                    if (lpds.Count <= 0)
                    {
                        return RedirectToAction("Index", "LoosePicking");
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }

            return View(lpds);
        }

        // Keep only 1 loose picking detail in this page. Whole object is set from button click from JS (Details Page)
        public ActionResult PalletSelection(LoosePickingDetailModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Clear Previous Session
            Session["PalletNum"] = null;
            Session["PalletSelectionModels"] = null;

            //for back button. Call this action without parameters / object
            if (string.IsNullOrEmpty(obj.PartNum) && Session["LoosePickingDetailModel"] != null) {
                obj = (LoosePickingDetailModel)Session["LoosePickingDetailModel"];
            }

            //This qty is for JS Checking. If qty <= 0, then this item cannot be scan anymore
            ViewBag.LoosePickingDetailQty = obj.Qty;

            List<PalletModel> objLstPModels = new List<PalletModel> {
                new PalletModel{PId=0,PQty=0},
                new PalletModel{PId=1,PQty=0},
                new PalletModel{PId=2,PQty=0},
                new PalletModel{PId=3,PQty=0},
                new PalletModel{PId=4,PQty=0},
                new PalletModel{PId=5,PQty=0},
                new PalletModel{PId=6,PQty=0},
                new PalletModel{PId=7,PQty=0}
            };

            List<PalletSelectionModel> objLstPSelectedModels = new List<PalletSelectionModel>();
            //LPPalletSelection?Company={Company}&UD108Key1={UD108Key1}&SysRowId={SysRowId}
            string Qry = $"LPPalletSelection?Company={obj.Company}&SONum={obj.SONum}&ShipByDate={obj.ShipByDate.ToString("yyyy-MM-dd")}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                objLstPSelectedModels = response.Content.ReadAsAsync<List<PalletSelectionModel>>().Result;
                Session["PalletSelectionModels"] = objLstPSelectedModels;
            }
            if(objLstPSelectedModels!=null && objLstPSelectedModels.Any())
            {
                foreach(var pm in objLstPModels)
                {
                    decimal pqty=objLstPSelectedModels.Where(w => w.PalletNum == pm.PId).Sum(s => s.PalletQty);
                    pm.PQty = pqty;

                    objLstPSelectedModels.Where(w => w.PalletNum == pm.PId).Any(s => s.VerifyStatus == false ? pm.isVerified = false: pm.isVerified = true);
                }

                //List<PalletSelectionModel> verifyPsms = psms.Where(w => w.PalletNum == id).ToList();
                //if (verifyPsms.Any(b => b.VerifyStatus == false))
                //{
                //    ViewBag.VerifyStatus = "F";
                //}
                //else
                //{
                //    ViewBag.VerifyStatus = "T";
                //}

            }

            // Keep this loose picking detail object and bring it to Product Scanning Page
            Session["LoosePickingDetailModel"] = obj;


            return View(objLstPModels);
        }

        //************PRODUCT SCANNING - PICKING PROCESS************//

        //Product Scanning Page for Picking Process. id = PalletNum
        public ActionResult ProductScanning(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Testing data
            //List<LotNumSuggestion> lotNums = new List<LotNumSuggestion>();
            //LotNumSuggestion ln = new LotNumSuggestion();
            //ln.LotNum = "Lot12345";
            //ln.OnHandQty = 100;
            //lotNums.Add(ln);

            //LotNumSuggestion ln2 = new LotNumSuggestion();
            //ln2.LotNum = "LotZXC";
            //ln2.OnHandQty = 200;
            //lotNums.Add(ln2);

            //if (lotNums != null)
            //{
            //    //Display in the modal
            //    TempData["LotNums"] = lotNums;
            //}


            int palletNum = id;
            Session["PalletNum"] = palletNum;

            //Data from loose picking detail bring to pallet selection model for update purpose
            LoosePickingDetailModel lpd = (LoosePickingDetailModel)Session["LoosePickingDetailModel"];

            //Set some data from LoosePickingDetailModel Object to PalletSelectionModel for update through WEB Api. Because we need to post by PalletSelectionModel Object through WEB Api /LPPalletSelection(POST)
            PalletSelectionModel ps = new PalletSelectionModel();
            ps.Id = lpd.Id;
            ps.UD108Key1 = lpd.UD108Key1;
            ps.SysRowId = lpd.SysRowId;
            ps.Company = lpd.Company;
            ps.Plant = lpd.Plant;
            ps.SONum = lpd.SONum;
            ps.ShipByDate = lpd.ShipByDate;
            ps.PartNum = lpd.PartNum;
            ps.PartDesc = lpd.PartDesc;
            ps.ActQty = lpd.ActQty;
            ps.PrevQty = lpd.PrevLPQty;
            ps.PalletNum = id; //
            //ps.LotNum = ""; // Set after they scan the lot num and its valid. On Action: CheckProductScanningLotNum
            ps.PalletQty = lpd.Qty;//
            ps.UomCode = lpd.UomCode;
            //ps.FromWareHouse
            //ps.FromBinNum
            //ps.ToWareHouse
            //ps.ToBinNum
            //ps.VerifyStatus
            ps.Remark = lpd.Remark;
            ps.LoosePickStatus = lpd.LoosePickStatus;
            ps.UserId = lpd.UserId;
            ps.DPNum = lpd.DPNum;
            //ps.RowMod

            //Keep PalletSelectionModel Object into Session
            Session["PalletSelectionModel"] = ps;

            //Get Suggested Lot Num from Web API
            List<LotNumSuggestion> lotNums = new List<LotNumSuggestion>();
            lotNums = GetSuggestionLotNum(ps.Company, ps.PartNum);
            if (lotNums != null)
            {
                //Display in the modal
                TempData["LotNums"] = lotNums;
            }

            return View(ps);
        }

        // Checking Lot Num in Picking Process. (One of the function in ProductScanning Page)
        public ActionResult CheckProductScanningLotNum(string Company, string PartNum, string LotNum) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string msg = "-";
            decimal _Qty = 0;
            //LPLotNum
            //LPRetrieve

            string Qry = $"LPLotNum?Company={Company}&PartNum={PartNum}&LotNum={LotNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            if (response.IsSuccessStatusCode)
            {
                PalletSelectionModel p = new PalletSelectionModel();
                p = response.Content.ReadAsAsync<PalletSelectionModel>().Result;
                PalletSelectionModel pallet = (PalletSelectionModel)Session["PalletSelectionModel"];
                pallet.FromWareHouse = p.FromWareHouse; // "L05";
                pallet.FromBinNum = p.FromBinNum; // "RS_BTL";
                pallet.ToWareHouse = p.ToWareHouse; // "L09";
                pallet.ToBinNum = p.ToBinNum; // "L09";
                //pallet.Qty = rmApi.Qty;
                pallet.LotNum = p.LotNum;

                _Qty = p.PalletQty;

                //pallet.Id = rm.Id;

                // Set some missing data into the Current Session
                Session["PalletSelectionModel"] = pallet;
                msg = "Valid";

                //Session["RetrieveApiModel"] = rm;
                //return RedirectToAction("Retrieve");
                return Json(new { Message = msg, PalletQty = _Qty, Warning = p.LotCheck }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string msgJson = response.Content.ReadAsStringAsync().Result;

                try
                {
                    JObject jObject = JObject.Parse(msgJson);
                    string msg_ = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    if (msgDtl == null)
                    {
                        msg = msg_;
                    }
                    else
                    {
                        msg = msgDtl;
                    }
                }
                catch(Exception ex)
                {
                    msg = msgJson;
                }


            }

            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);

        }

        // Update / Submit Scanning Page form of ProductScanning Page for Picking Process
        [HttpPost]
        //public ActionResult UpdateProductScanning(PalletSelectionModel obj)
        public ActionResult UpdateProductScanning(PalletSelectionModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            PalletSelectionModel sessionPallet = (PalletSelectionModel)Session["PalletSelectionModel"];

            obj.FromWareHouse = sessionPallet.FromWareHouse;//"L05";
            obj.FromBinNum = sessionPallet.FromBinNum; // "RS_BTL";
            obj.ToWareHouse = sessionPallet.ToWareHouse; //"L09";
            obj.ToBinNum = sessionPallet.ToBinNum;//"L09";
            //pallet.Qty = rmApi.Qty;
            obj.ShipByDate = (DateTime)Session["ShipByDate"];

            string errMsg = "-";
            string msg = "-";
            string status = "";
            
            //Check Qty. If finish, change to LoosePickStatus to Full
            LoosePickingDetailModel lpd = (LoosePickingDetailModel)Session["LoosePickingDetailModel"];
            decimal newLpdQty = 0;
            decimal qty = lpd.Qty;
            decimal palletQty = obj.PalletQty;
            newLpdQty = qty - palletQty;
            if (newLpdQty <= 0)
            {
                obj.LoosePickStatus = "Full";
            }

            //JavaScriptSerializer js = new JavaScriptSerializer();
            //var json = js.Serialize(obj);


            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("LPPalletSelection", obj).Result;
            if (response.IsSuccessStatusCode)
            {
                //Set New Qty in LPDM Session
                //lpd.Qty = newLpdQty;
                //Session["LoosePickingDetailModel"] = lpd;
                //Session["LoosePickingDetailModelXQty"] = newLpdQty;
                lpd.Qty = newLpdQty;
                Session["LoosePickingDetailModel"] = lpd;
                msg = response.Content.ReadAsStringAsync().Result;
                status = "Success";

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
            else {
                msg = response.Content.ReadAsStringAsync().Result;
                status = "Fail";

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
            

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        //************PRODUCT SCANNING - PICKING PROCESS END************//

        // Show list of added PartNum in a pallet by Pallet Id. id =  pallet id. This is during Picking Process.
        public ActionResult Verification(int id) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            Session["PalletNum"] = id;
            ViewBag.VerifyStatus = "F";

            List<PalletSelectionModel> psms = (List<PalletSelectionModel>)Session["PalletSelectionModels"];
            //List<PalletSelectionModel> verifyPsms = psms.Where(w=>w.PalletNum == id)

            List<PalletSelectionModel> verifyPsms = psms.Where(w=>w.PalletNum == id).ToList();
            if (verifyPsms.Any(b => b.VerifyStatus == false))
            {
                ViewBag.VerifyStatus = "F";
            }
            else {
                ViewBag.VerifyStatus = "T";
            }

            //LoosePickingDetailModel lpd = (LoosePickingDetailModel)Session["LoosePickingDetailModel"];
            //ViewBag.BackButtonObject = lpd;

            return View(verifyPsms);
        }

        //Verified by Pallet Id
        public ActionResult Verified()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int id = (int)Session["PalletNum"];

            List<PalletSelectionModel> psms = (List<PalletSelectionModel>)Session["PalletSelectionModels"];

            List<PalletSelectionModel> verifiedPsms = psms.Where(w => w.PalletNum == id).ToList();

            string errMsg = "-";
            string msg = "-";
            string status = "";
            //bool isTestSuccess = false;

            try
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("LoosePicking", verifiedPsms).Result;
                if (response.IsSuccessStatusCode)
                {
                    status = "Success";
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
                else
                {
                    status = "Fail";
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
            }
            catch (Exception ex)
            {
                status = "Fail";
                errMsg = ex.Message.ToString();
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        //delete by Id
        public ActionResult DeleteFromVerification(int id) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int palletNum = (int)Session["PalletNum"];
            string msg = "";

            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("LoosePicking?id=" + id).Result;
            //TempData["RespondMessage"] = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode) {
                msg = response.Content.ReadAsStringAsync().Result;

                //Remove from PalletSelectionModel List
                List<PalletSelectionModel> psms = (List<PalletSelectionModel>)Session["PalletSelectionModels"];
                List<PalletSelectionModel> verifiedPsms = psms.Where(w => w.PalletNum == palletNum).ToList();
                PalletSelectionModel ps = verifiedPsms.Where(w => w.Id == id).SingleOrDefault();
                decimal redeemQty = ps.PalletQty;
                string uniqueKey = ps.UD108Key1;
                verifiedPsms.Remove(ps);
                Session["PalletSelectionModels"] = verifiedPsms;

                //Add back the delete Qty to the PartNum , find by UD108Key1
                LoosePickingDetailModel lpd = (LoosePickingDetailModel)Session["LoosePickingDetailModel"];
                if (uniqueKey == lpd.UD108Key1) {
                    lpd.Qty = lpd.Qty + redeemQty;
                    Session["LoosePickingDetailModel"] = lpd;
                }
                msg = "Success";
            }

            //TempData["ResponseMessage"] = msg;
            //return View("Verification", verifiedPsms);

            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Retrieve(LoosePickingDetailModel obj) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (obj == null && Session["LoosePickingDetailModel"] != null)
            {
                obj = (LoosePickingDetailModel)Session["LoosePickingDetailModel"];
            }

            RetrieveModel rm = new RetrieveModel();
            rm.Id = obj.Id;
            rm.UD108Key1 = obj.UD108Key1;
            rm.Company = obj.Company;
            rm.Plant = obj.Plant;
            rm.SONum = obj.SONum;
            rm.PartNum = obj.PartNum;
            rm.PartDesc = obj.PartDesc;
            //rm.LotNum
            rm.Qty = obj.Qty;
            rm.UOMCode = obj.UomCode;
            //rm.FromWareHouse
            //rm.FromBinNum
            //rm.ToWareHouse
            //rm.ToBinNum
            rm.UserId = obj.UserId;

            Session["RetrieveModel"] = rm;

            return View(rm);
        }

        public ActionResult UpdateRetrieve(string LotNum) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "-";
            bool success = false;

            RetrieveModel rm = (RetrieveModel)Session["RetrieveModel"];
            rm.LotNum = LotNum;

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("LPRetrieve", rm).Result;
            if (response.IsSuccessStatusCode)
            {
                msg = response.Content.ReadAsStringAsync().Result;
                success = true;

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
            else
            {
                msg = response.Content.ReadAsStringAsync().Result;
                success = false;

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


            return Json(new { Message = errMsg, Success = success }, JsonRequestBehavior.AllowGet);
        }

        //Check Lot Num when Retrieve
        public ActionResult CheckLotNum(string Company, string LotNum, string PartNum) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "-";
            string Qry = $"LPRetrieve?Company={Company}&LotNum={LotNum}&PartNum={PartNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            
            if (response.IsSuccessStatusCode)
            {
                RetrieveModel rmApi = new RetrieveModel();
                rmApi = response.Content.ReadAsAsync<RetrieveModel>().Result;
                RetrieveModel rm = (RetrieveModel)Session["RetrieveModel"];
                rm.FromWareHouse = rmApi.FromWareHouse;//"L05";
                rm.FromBinNum = rmApi.FromBinNum; // "RS_BTL";
                rm.ToWareHouse = rmApi.ToWareHouse; //"L09";
                rm.ToBinNum = rmApi.ToBinNum;//"L09";
                rm.Qty = rmApi.Qty;
                rm.LotNum = rmApi.LotNum;
                rm.Id = rm.Id;
                Session["RetrieveModel"] = rm;
                msg = "Valid";
                //Session["RetrieveApiModel"] = rm;
                //return RedirectToAction("Retrieve");
                return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
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

        //Get the suggestion lot num in Product Scanning Page
        public List<LotNumSuggestion> GetSuggestionLotNum(string Company, string PartNum) {

            List<LotNumSuggestion> lotNums = new List<LotNumSuggestion>();

            string errMsg = "";
            string msg = "";

            try
            {
                string qry = $"LPLotSuggestions?Company={Company}&PartNum={PartNum}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    msg = "";
                    lotNums = response.Content.ReadAsAsync<List<LotNumSuggestion>>().Result;
                }
                else
                {
                    msg = response.Content.ReadAsStringAsync().Result;
                    lotNums = null;

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
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                lotNums = null;
            }

            return lotNums;
        }


    }
}
