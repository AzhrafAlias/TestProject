using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySpritzer.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MySpritzer.Controllers
{
    public class FullPickingController : Controller
    {
        //http://localhost:65458/api/PilotToTruckInfo?Company=CSTP&GetType=GetPilotID
        //http://localhost:65458/api/PilotToTruckInfo?Company=CSTP&GetType=GetLoadingBay
        //http://localhost:65458/api/PilotToTruckInfo?Company=CSTP&GetType=GetUD110&Key1=35&Key2=

        // GET: FullPicking
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";

            string Company = (string)Session["Company"];
            string UserID = (string)Session["User"];


            //Clear previous unused sessions
            Session["QAApprovalModel"] = null;
            Session["PilotToTruckModel"] = null;
            Session["ProdInTruckConPs"] = null;

            Session["TruckArriveModel"] = null;
            Session["QAApprovalModel"] = null;
            Session["FinishGoodModel"] = null;
            Session["FGLotNumResponseModel"] = null;
            Session["Criterias"] = null;

            ////Dummy Data

            //List<TruckArriveModel> trucks = new List<TruckArriveModel>();
            //TruckArriveModel truck = new TruckArriveModel();
            //truck.Company = "CSTP";
            //truck.TruckNo = "atduuasdtuasy";
            //truck.Transporter = "";
            //truck.TimeSlot = "";
            //truck.TruckType = "DOMESTIC";
            //truck.SealNo = "12345";
            //truck.TruckStatus = "READY";
            ////truck.TruckInspected = true;
            //truck.Key1 = "1"; //DPNum
            //truck.Key2 = "2";
            //truck.Key3 = "";
            //truck.Key4 = "";
            //truck.Key5 = "";
            //trucks.Add(truck);

            ////Criteria dummy data

            //List<InspectionCriteriaModel> criterias = new List<InspectionCriteriaModel>();
            //InspectionCriteriaModel criteria = new InspectionCriteriaModel();
            //criteria.Id = 1;
            //criteria.Company = "CSTP";
            //criteria.Criteria = "Not Smelly";
            //criteria.Mandatory = true;
            //criterias.Add(criteria);
            //ViewBag.Criterias = criterias;

            List<TruckArriveModel> trucks = new List<TruckArriveModel>();
            List<InspectionCriteriaModel> criterias = new List<InspectionCriteriaModel>();

            try
            {

                string Qry = $"TruckArrive?Company={Company}&UserID={UserID}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    trucks = response.Content.ReadAsAsync<List<TruckArriveModel>>().Result;
                    Session["TruckArriveModel"] = trucks;
                }

                //Get Criteria for Inspection
                string QryCriteria = $"InspectionCriteria?Company={Company}";
                HttpResponseMessage responseCriteria = GlobalVariables.WebApiClient.GetAsync(QryCriteria).Result;
                if (responseCriteria.IsSuccessStatusCode)
                {
                    criterias = responseCriteria.Content.ReadAsAsync<List<InspectionCriteriaModel>>().Result;
                    ViewBag.Criterias = criterias;
                    InspectionCriteriaModel c = new InspectionCriteriaModel();
                    c.Criteria = "Canvas";
                    criterias.Add(c);

                    Session["Criterias"] = criterias;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
            }

            //********Check if has any error from previous action that returning to the Index Action

            if (TempData["Error"] != null)
            {
                errMsg = (string)TempData["Error"];
                ViewBag.Error = errMsg;
            }
            //********Check if has any error from previous action that returning to the Index Action//

            return View(trucks);
        }


        public ActionResult PilotAssignment(PilotToTruckModel pilotToTruck) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //*****************Checking valid sv to view this
            string userid = (string)Session["User"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(pilotToTruck.UD25Company, pilotToTruck.UD25Key1, pilotToTruck.UD25Key2, userid);

            if (UVM.Valid == false) {
                TempData["Error"] = UVM.Msg;
                return RedirectToAction("Index", "FullPicking");
            }
            //*****************Checking valid sv to view this


            //http://localhost:65458/api/PilotToTruckInfo?Company=CSTP&Key1=100&Key2=100
            //Pilot and Loading Bay Model
            PilotAndLoadingBayModel pnlb = new PilotAndLoadingBayModel();
            Session["PilotToTruckModel"] = pilotToTruck;

            try
            {
                string QryPilotLoadingBayModel = $"PilotToTruckInfo?Company={pilotToTruck.UD25Company}&Key1={pilotToTruck.UD25Key1}&Key2={pilotToTruck.UD25Key2}";

                HttpResponseMessage responsePilotLoadingBayModel = GlobalVariables.WebApiClient.GetAsync(QryPilotLoadingBayModel).Result;
                if (responsePilotLoadingBayModel.IsSuccessStatusCode)
                {
                    pnlb = responsePilotLoadingBayModel.Content.ReadAsAsync<PilotAndLoadingBayModel>().Result;
                    ViewBag.Pilots = pnlb.pilotIDList1s;
                    ViewBag.LoadingBays = pnlb.loadingBayList1s;
                    ViewBag.TruckNo = pilotToTruck.TruckNo;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
                return RedirectToAction("Index", "FullPicking");
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdatePilotAssignment(PilotToTruckModel pilotToTruck) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "";
            string status = "";

            PilotToTruckModel currPilotToTruck = (PilotToTruckModel)Session["PilotToTruckModel"];
            currPilotToTruck.UD25FS_PilotID_c = pilotToTruck.UD25FS_PilotID_c;
            currPilotToTruck.UD25FS_LoadingBay_c = pilotToTruck.UD25FS_LoadingBay_c;
            currPilotToTruck.UD25FS_SupervisorID_c = (string)Session["User"];
            currPilotToTruck.UD25FS_PilotAssigDateTime_c = DateTime.Now;
            if (currPilotToTruck.UD25Key2 == null) {
                currPilotToTruck.UD25Key2 = "";
            }

            try
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("AssignPilotToTruck", currPilotToTruck).Result;
                if (response.IsSuccessStatusCode)
                {
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
                    status = "Fail";
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        //Update Seal Number for Truck Arrive
        [HttpPost]
        public ActionResult UpdateTruckArrive(string DPNum, string SealNo, string Key2)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "";
            string status = "";

            //*****************Check or update valid sv to view this
            string userid = (string)Session["User"];
            string Company = (string)Session["Company"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(Company, DPNum, Key2, userid);

            if (UVM.Valid == false)
            {
                msg = UVM.Msg;
                status = "Fail";
                return Json(new { Message = msg, Status = status }, JsonRequestBehavior.AllowGet);
                //TempData["Error"] = UVM.Msg;
                //return RedirectToAction("Index", "FullPicking");
            }
            //*****************Check or update valid sv to view this

            List<TruckArriveModel> trucks = new List<TruckArriveModel>();
            trucks = (List<TruckArriveModel>)Session["TruckArriveModel"];
            TruckArriveModel truck = new TruckArriveModel();
            truck = trucks.Where(w => w.Key1 == DPNum).SingleOrDefault();
            truck.SealNo = SealNo;
            //if(string.IsNullOrEmpty(truck.TruckType))
            //    truck.TruckType = "EXPORT";

            try
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("TruckArrive", truck).Result;
                if (response.IsSuccessStatusCode)
                {
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

                    catch (Exception ex) {
                        errMsg = msg;
                    }
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                status = "Fail";
                errMsg = ex.Message.ToString();
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitInspectionForm(InspectionFormModel m)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "";
            string status = "";

            //Simply convert to empty if Key2 value is null
            if (m.Key2 == null)
            {
                m.Key2 = "";
            }

            //*****************Check or update valid sv to view this
            string Company = (string)Session["Company"];
            string UserID = (string)Session["User"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(Company, m.DPNum, m.Key2, UserID);

            if (UVM.Valid == false)
            {
                msg = UVM.Msg;
                status = "Fail";
                return Json(new { Message = msg, Status = status }, JsonRequestBehavior.AllowGet);
                //TempData["Error"] = UVM.Msg;
                //return RedirectToAction("Index", "FullPicking");
            }
            //*****************Check or update valid sv to view this

            //All Criteria
            List<InspectionCriteriaModel> icms = (List<InspectionCriteriaModel>)Session["Criterias"];

            List<InspectionCriteriaModel> falseItem = new List<InspectionCriteriaModel>();
            if (m.CriteriaType != null)
            {
                falseItem = icms.Where(x => !m.CriteriaType.Any(y => y == x.Criteria)).ToList();
            }
            else
            {
                falseItem = icms;
            }


            UpdateInspectionModel updateModel = new UpdateInspectionModel();
            updateModel.Company = m.Company;
            updateModel.Key1 = m.DPNum;
            updateModel.Key2 = m.Key2;//m.Key2;
            updateModel.Items = new List<Item>();
            //Add True item
            if (m.CriteriaType != null)
            {
                for (int i = 0; i < m.CriteriaType.Count; i++)
                {
                    //Check with all api checkboxes

                    Item item = new Item();
                    item.CriteriaType = m.CriteriaType[i];
                    item.Result = true;
                    updateModel.Items.Add(item);
                }
            }
            //Add False Item
            for (int i = 0; i < falseItem.Count; i++)
            {
                //Check with all api checkboxes
                Item item = new Item();
                item.CriteriaType = falseItem[i].Criteria;
                item.Result = false;
                updateModel.Items.Add(item);
            }



            try
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("UpdateInspectionResult", updateModel).Result;
                if (response.IsSuccessStatusCode)
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

                    status = "Success";
                }
                else
                {
                    if ((int)response.StatusCode == 300)
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


                        status = "300";

                    }
                    else
                    {
                        msg = response.Content.ReadAsStringAsync().Result;
                        try {
                            JObject jObject = JObject.Parse(msg);
                            string msgJ = (string)jObject.SelectToken("Message");
                            string msgDtl = (string)jObject.SelectToken("MessageDetail");
                            if (msgDtl == null)
                            {
                                errMsg= msgJ;
                            }
                            else
                            {
                                errMsg= msgDtl;
                            }
                        }
                        catch(Exception ex)
                        {
                            errMsg = msg;
                        }

                        status = "Fail";

                    }

                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        //https://www.jqueryscript.net/tags.php?/Signature%20Pad/
        //https://www.youtube.com/watch?v=mqjhff5daVA
        public ActionResult QaApproval(string Company, string DPNum, string Key2) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }



            //*****************Checking valid sv to view this
            string UserID = (string)Session["User"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(Company, DPNum, Key2, UserID);

            if (UVM.Valid == false)
            {
                TempData["Error"] = UVM.Msg;
                return RedirectToAction("Index", "FullPicking");
            }
            //*****************Checking valid sv to view this




            QAApprovalModel qa = new QAApprovalModel();

            //To show previous data if they came back here from QAApprovalDriver View
            if (Session["QAApprovalModel"] != null)
            {
                QAApprovalModel sessionQa = (QAApprovalModel)Session["QAApprovalModel"];
                qa.UD110Company = sessionQa.UD110Company;
                qa.UD110Key1 = sessionQa.UD110Key1;
                qa.UD110Key2 = sessionQa.UD110Key2;
                qa.UD110FS_QAInspStatus = sessionQa.UD110FS_QAInspStatus;
                qa.UD110FS_TruckStaRemarks = sessionQa.UD110FS_TruckStaRemarks;
                qa.UD110FS_TruckQASignanture = sessionQa.UD110FS_TruckQASignanture;
                qa.UD110FS_TruckLorryDriverSignature = sessionQa.UD110FS_TruckLorryDriverSignature;
            }
            else
            {
                qa.UD110Company = Company;
                qa.UD110Key1 = DPNum;
                qa.UD110Key2 = Key2;

            }

            return View(qa);
        }

        public ActionResult QaApprovalDriver() {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }


            QAApprovalModel q = (QAApprovalModel)Session["QAApprovalModel"];
            return View(q);//
        }

        [HttpPost]
        public ActionResult SubmitQaApproval(QAApprovalModel q) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //******************************************
            string Company = (string)Session["Company"];
            string UserID = (string)Session["User"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(Company, q.UD110Key1, q.UD110Key2, UserID);

            if (UVM.Valid == false)
            {
                TempData["Error"] = UVM.Msg;
                return RedirectToAction("Index", "FullPicking");
            }
            //******************************************

            Session["QAApprovalModel"] = q;

            string errMsg = "";
            string msg = "";
            string status = ""; //response status
            string inspectStatus = "";
            //QAApprovalModel qSubmit = q;

            if (q.UD110FS_QAInspStatus)
            {
                inspectStatus = "Pass";
                //Submit UD110FS_TruckQASignanture
                try
                {
                    HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("UpdateQAApproval", q).Result;
                    if (response.IsSuccessStatusCode)
                    {
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
                        catch(Exception ex)
                        {
                            errMsg = msg;
                        }

                    }
                    else
                    {
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
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message.ToString();
                    status = "Fail";
                }
            }

            else
            {
                status = "Success";
                inspectStatus = "Fail";
            }

            return Json(new { Message = errMsg, Status = status, InspectStatus = inspectStatus }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitQaApprovalDriver(QAApprovalModel q)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }


            string errMsg = "";
            string msg = "";
            string status = "";
            //QAApprovalModel qSubmit = q;

            if (!q.UD110FS_QAInspStatus)
            {
                try
                {
                    HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("UpdateQAApproval", q).Result;
                    if (response.IsSuccessStatusCode)
                    {
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

                        //Session["QAApprovalModel"] = null;
                    }
                    else
                    {
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
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message.ToString();
                    status = "Fail";
                }
            }
            else {
                status = "Success";
                errMsg = "This QA is PASS. No need Driver Signature.";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductInTruckShipTo() {
            List<FGShipToModel> shiptos = new List<FGShipToModel>();

            //testing purpose
            FGShipToModel fgShipto = new FGShipToModel();
            fgShipto.Company = "CSSA";
            fgShipto.DPNum = "1";
            fgShipto.ShipTo = "A";
            fgShipto.SONum = "2";
            fgShipto.Status = "P";
            shiptos.Add(fgShipto);

            return View(shiptos);
        }

        // SV Review Product In Truck. From TruckArriveModel
        public ActionResult ProductInTruck(string Company, string DPNum, string SealNo, string Key2, string TruckNo)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Key2 == null)
            {
                Key2 = "";
            }

            //******************************************
            string UserID = (string)Session["User"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(Company, DPNum, Key2, UserID);

            if (UVM.Valid == false)
            {
                TempData["Error"] = UVM.Msg;
                return RedirectToAction("Index", "FullPicking");
            }
            //******************************************

            //List<ProdInTruckConP> prodInTruckConPs = new List<ProdInTruckConP>();
            //Session["ProdInTruckConPs"] = prodInTruckConPs;
            Session["ProdInTruckConPs"] = null;
            Session["ProdInTruckConPs"] = new List<ProdInTruckConP>(); // Declared in session because want to be used in ProductInTruckCon and ProdInTruckAddPalletType Actions

            //Get Pallet Type
            string qryPalletPart = $"PalletPart?Company={Company}&DPNum={DPNum}";//Need to add DPNum param
            HttpResponseMessage responsePalletPart = GlobalVariables.WebApiClient.GetAsync(qryPalletPart).Result;
            if (responsePalletPart.IsSuccessStatusCode)
            {
                List<PalletPart> palletParts = responsePalletPart.Content.ReadAsAsync<List<PalletPart>>().Result;
                //testing purpose
                //PalletPart ptPart = new PalletPart();
                //ptPart.Company = "CSSA";
                //ptPart.PalletStkCode = "test true 1";
                //ptPart.DefaultPallet = true;
                //palletParts.Add(ptPart);

                //PalletPart ptPart2 = new PalletPart();
                //ptPart2.Company = "CSSA";
                //ptPart2.PalletStkCode = "test true 2";
                //ptPart2.DefaultPallet = true;
                //palletParts.Add(ptPart2);

                //PalletPart ptPart3 = new PalletPart();
                //ptPart3.Company = "CSSA";
                //ptPart3.PalletStkCode = "test false 3";
                //ptPart3.DefaultPallet = false;
                //palletParts.Add(ptPart3);

                ViewBag.PalletParts = palletParts;
            }

            try
            {
                //ProdInTruck?Company=CSTP&DPNo=1
                string qry = $"ProdInTruck?Company={Company}&DPNo={DPNum}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    Decimal totalQty = 0;
                    int FS_NoIncludePallet_c = 0;
                    bool isEnableCustPallet = false;
                    //string Key2 = "";
                    List<ProductInTruckModel> prodInTrucks = response.Content.ReadAsAsync<List<ProductInTruckModel>>().Result;
                    
                    foreach (ProductInTruckModel prod in prodInTrucks) {
                        //totalQty = totalQty + prod.QtyPallet;
                        //totalQty = totalQty + prod.QtyCtn;

                        //Key2 = prod.Key2;
                        totalQty = prod.QtyPallet; //Display on Pallet Type Confirmation
                        FS_NoIncludePallet_c = prod.FS_NoIncludePallet_c; //Dont display Pallet Type Confirmation if FS_NoIncludePallet_c value pallet is 1
                        isEnableCustPallet = prod.CustomerPallet;
                    }
                    ViewBag.TotalQty = totalQty;
                    ViewBag.SealNo = SealNo;
                    ViewBag.DPNo = DPNum;
                    ViewBag.Company = Company;
                    ViewBag.Key2 = Key2;
                    ViewBag.CustomerPallet = isEnableCustPallet; // to enable and disable pallet selection dropdown in pallet type con
                    ViewBag.TruckNo = TruckNo;
                    ViewBag.FS_NoIncludePallet_c = FS_NoIncludePallet_c;

                    //testing purpose
                    //ProductInTruckModel p = new ProductInTruckModel();
                    //p.ChildKey1 = "1";
                    //p.ChildKey2 = "2";
                    //p.ChildKey3 = "3";
                    //p.Comment = "test comment";
                    //p.Company = "CSSA";
                    //p.CustomerPallet = true;
                    //p.Description = "test desc";
                    //p.Key1 = "K1";
                    //p.Key2 = "K2";
                    //p.Key3 = "K3";
                    //p.LotNum = "test lot";
                    //p.PalletType = "F";
                    //p.PartNum = "4-NMW-DES-050024-W001x";
                    //p.QtyCtn = 4;
                    //p.QtyPallet = 11;
                    //prodInTrucks.Add(p);

                    return View(prodInTrucks);
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }

            return View();
        }


        [HttpPost]
        public ActionResult ProdInTruckCon (ProdInTruckConModel prodCon) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //******************************************
            string UserID = (string)Session["User"];
            string Company = (string)Session["Company"];

            UserValidationModel UVM = new UserValidationModel();
            UVM = UserValidation(Company, prodCon.DPNo, prodCon.UD110Key2, UserID);

            if (UVM.Valid == false)
            {
                TempData["Error"] = UVM.Msg;
                return RedirectToAction("Index", "FullPicking");
            }
            //******************************************

            List<ProdInTruckConP> prodInTruckConPs = (List<ProdInTruckConP>)Session["ProdInTruckConPs"];

            ProdInTruckConModel prodConUpdate = prodCon;
            //Simply replace null to empty. (because js read empty string as null). REAL CASE UD110Key2 WILL NOT BE NULL
            if (prodCon.UD110Key2 == null) {
                prodCon.UD110Key2 = "";
            }
            prodConUpdate.PalletList = prodInTruckConPs;

            if (prodConUpdate.PalletList == null || prodConUpdate.PalletList.Count == 0)
            {
                prodConUpdate.CustomerPallet = false;
            }
            else
            {
                prodConUpdate.CustomerPallet = true;
            }

            string errMsg = "";
            string msg = "";
            string status = "";

            try
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("ProdInTruckCon", prodConUpdate).Result;
                if (response.IsSuccessStatusCode)
                {
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
                else
                {
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

            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }


        //Add
        [HttpPost]
        public ActionResult ProdInTruckAddPalletType(int Index, string PalletType, decimal PalletQty, decimal TotalQty) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<ProdInTruckConP> prodInTruckConPs = (List<ProdInTruckConP>) Session["ProdInTruckConPs"];


            StringBuilder previousAssigned = new StringBuilder();

            //Max is can only assign 2 pallet type 
            if (prodInTruckConPs.Count <= 2) {
                ProdInTruckConP pallet = new ProdInTruckConP();
                pallet.Index = Index;
                pallet.PalletType = PalletType;
                pallet.PalletQty = PalletQty;
                prodInTruckConPs.Add(pallet);

                Session["ProdInTruckConPs"] = prodInTruckConPs;

                ViewBag.ProdInTruckConPs = Session["ProdInTruckConPs"];
                foreach (var p in prodInTruckConPs) {
                    previousAssigned.Append(" Pallet Type: " + p.PalletType + " Qty:" + p.PalletQty.ToString());
                }
            }


            Decimal currTotalQty = TotalQty;
            currTotalQty = currTotalQty - PalletQty;
            ViewBag.TotalQty = currTotalQty;

            return Json(new { Message = PalletType + " PalletQty: " + PalletQty.ToString() + " currTotalQty: " + currTotalQty.ToString(), Status = "", PreviousAssigned = previousAssigned.ToString() }, JsonRequestBehavior.AllowGet);
        }

        //Delete One Item
        // api/RemoveLotFromTruck?Company={Company}&Key1={Key1}&Key2={Key2}&Key3={Key3}&ChildKey1={ChildKey1}&ChildKey2={ChildKey2}&ChildKey3={ChildKey3}&Reason={Reason}
        public ActionResult ProdInTruckDelete(string Company, string Key1, string Key2, string Key3, string ChildKey1, string ChildKey2, string ChildKey3, string Reason)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "";
            string status = "";
            try
            {
                string qry = $"RemoveLotFromTruck?Company={Company}&Key1={Key1}&Key2={Key2}&Key3={Key3}&ChildKey1={ChildKey1}&ChildKey2={ChildKey2}&ChildKey3={ChildKey3}&Reason={Reason}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
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
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }
        //api/RemoveAllLotFromTruck?Company={Company}&Key2={Key2}&Reason={Reason}
        [HttpPost]
        public ActionResult ProdInTruckDeleteAll(string Company, string Key2, string Reason)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "";
            string status = "";
            try
            {
                string qry = $"RemoveAllLotFromTruck?Company={Company}&Key2={Key2}&Reason={Reason}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
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
                else
                {
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
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProdInTruckAdd(string Company, string DPNo, decimal QtyPallet, string PartNo, string PartDesc, string Ud108Key1)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string errMsg = "";
            string msg = "";
            string status = "";
            try
            {
                ProdInTruckAddModel prodInTruckAddModel = new ProdInTruckAddModel();
                prodInTruckAddModel.Company = Company;
                prodInTruckAddModel.DPNo = DPNo;
                prodInTruckAddModel.QtyPallet = QtyPallet;
                prodInTruckAddModel.PartNo = PartNo;
                //prodInTruckAddModel.PartNo = "AddOn";
                prodInTruckAddModel.PartDesc = PartDesc + " (Add On)";
                prodInTruckAddModel.Ud108Key1 = Ud108Key1;

                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("ProdInTruckAdd", prodInTruckAddModel).Result;
                if (response.IsSuccessStatusCode)
                {
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
                else
                {
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

            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }

            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProdInTruckPalletTypeCheck(int Index) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Session["ProdInTruckConPs"] 
            string palletType = "";
            decimal palletQty = 0;

            List<ProdInTruckConP> prodInTruckConPs = (List<ProdInTruckConP>)Session["ProdInTruckConPs"];
            if (prodInTruckConPs.Count > 0) {
                ProdInTruckConP prodInTruckConP = prodInTruckConPs.Where(w => w.Index == Index).SingleOrDefault();
                palletType = prodInTruckConP.PalletType;
                palletQty = prodInTruckConP.PalletQty;
            }
            return Json(new { Message = "" , Status = "", PalletType = palletType, PalletQty = palletQty }, JsonRequestBehavior.AllowGet);
        }


        //This View is for Pilot
        public ActionResult FinishGoodsShipTo()
        {
            List<FGShipToModel> shiptos = new List<FGShipToModel>();

            //testing purpose
            //FGShipToModel fgShipto = new FGShipToModel();
            //fgShipto.Company = "CSSA";
            //fgShipto.DPNum = "1";
            //fgShipto.ShipTo = "A";
            //fgShipto.SONum = "2";
            //fgShipto.Status = "P";
            //shiptos.Add(fgShipto);

            return View(shiptos);
        }

        //public ActionResult FinishGoods(string LoadingBay, string TruckNo) {
        public ActionResult FinishGoods(FGShipToModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //if (Session["UserType"] == null || (string)Session["UserType"] != "Pilot")
            //{
            //    return RedirectToAction("Login", "Home");
            //}

            string PilotID = (string)Session["User"];
            string Company = (string)Session["Company"];

            List<FinishGoodModel> fgs = new List<FinishGoodModel>();

            //if (string.IsNullOrEmpty(LoadingBay)) {
            //    //Req LoadingBay from WebApi
            //    //Company, type, pilotID, LoadingBay

            //    string qryLB = $"PilotToTruckInfo?Company={Company}&type=LoadingBay&pilotID={PilotID}";

            //    List<LoadingBayList1> lbays = new List<LoadingBayList1>();

            //    HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qryLB).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        lbays = response.Content.ReadAsAsync<List<LoadingBayList1>>().Result;
            //        ViewBag.LoadingBays = lbays;
            //    }
            //}
            //else
            //{
            //    //Req TruckNo from WebApi
            //    string qryTruck = $"PilotToTruckInfo?Company={Company}&type=Truck&pilotID={PilotID}&LoadingBay={LoadingBay}";

            //    List<TruckList> trucks = new List<TruckList>();

            //    HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qryTruck).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        trucks = response.Content.ReadAsAsync<List<TruckList>>().Result;
            //        ViewBag.TruckNo = trucks;
            //    }
            //}

            //Req item list by loading bay and truck no
            //if (!string.IsNullOrEmpty(LoadingBay) && !string.IsNullOrEmpty(TruckNo)) {

                try
                {
                    string qry = $"FGPickingInfo?Company={Company}&PilotID={PilotID}";
                    HttpResponseMessage responseMessage = GlobalVariables.WebApiClient.GetAsync(qry).Result;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        fgs = responseMessage.Content.ReadAsAsync<List<FinishGoodModel>>().Result;
                        return View(fgs);
                    }
                    else
                    {
                        string msgJson = responseMessage.Content.ReadAsStringAsync().Result;
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
            //}

            return View();
        }

        public ActionResult FinishGoodsPickProcess(FinishGoodModel fg) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //for back button. Call this action without parameters / object
            if (string.IsNullOrEmpty(fg.PartNum) && Session["FinishGoodModel"] != null)
            {
                fg = (FinishGoodModel)Session["FinishGoodModel"];
            }

            //Check for Picked Pallet
            if (fg.PickedPalletQty >= fg.PalletQty)
            {
                ViewBag.Status = "All pallets are already being picked!";
            }

            Session["FinishGoodModel"] = fg;

            return View(fg);
        }

        public ActionResult FinishGoodsPickProcessLoose(FinishGoodModel fg)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //for back button. Call this action without parameters / object
            if (string.IsNullOrEmpty(fg.PartNum) && Session["FinishGoodModel"] != null)
            {
                fg = (FinishGoodModel)Session["FinishGoodModel"];
            }

            //Check for Picked Pallet
            if (fg.PickedPalletQty >= fg.PalletQty)
            {
                ViewBag.Status = "All pallets are already being picked!";
            }

            Session["FinishGoodModel"] = fg;

            return View(fg);
        }

        public ActionResult CheckFinishGoodsLotNum(string Company, string PartNum, string LotNum, int Qty)
        {
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
            //LPLotNum
            //LPRetrieve
            string Qry = $"FPLotNum?Company={Company}&PartNum={PartNum}&LotNum={LotNum}";
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
            if (response.IsSuccessStatusCode)
            {
                FGLotNumResponseModel fgLotNum = new FGLotNumResponseModel();
                fgLotNum = response.Content.ReadAsAsync<FGLotNumResponseModel>().Result;

                Session["FGLotNumResponseModel"] = fgLotNum;

                if (fgLotNum.OnhandQty >= Qty)
                {
                    errMsg = "Valid";
                }
                else
                {
                    errMsg = $"Onhand Qty: {fgLotNum.OnhandQty} is less than Qty: {Qty}";
                }
                
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
                catch(Exception ex)
                {
                    errMsg = msg;
                }

            }

            return Json(new { Message = errMsg }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        //public ActionResult UpdateProductScanning(PalletSelectionModel obj)
        public ActionResult UpdateFinishGoods(FinishGoodModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            FGLotNumResponseModel fgResponse = (FGLotNumResponseModel)Session["FGLotNumResponseModel"];

            List<FGUpdateModel> fgUpdateModels = new List<FGUpdateModel>();
            FGUpdateModel fgUpdateModel = new FGUpdateModel();
            fgUpdateModel.Company = obj.Company;
            fgUpdateModel.RunningNo_Key1 = obj.Key1;
            fgUpdateModel.DPNum_Key2 = obj.Key2;
            fgUpdateModel.SONum_Key3 = obj.Key3;
            fgUpdateModel.PartNum = obj.PartNum;
            fgUpdateModel.UD110Key2 = obj.Key2;
            //fgUpdateModel.PalletLblId = finishGoodModel.PalletLblId;
            //fgUpdateModel.PalletLblNum = finishGoodModel.PalletLblNum;
            fgUpdateModel.LotNum = fgResponse.LotNum; //
            fgUpdateModel.Qty = obj.Qty;
            fgUpdateModel.Status = "PICKED";
            fgUpdateModel.UserID = (string)Session["User"];
            fgUpdateModels.Add(fgUpdateModel);

            string msg = "";
            string status = "";
            string errMsg = string.Empty;

            try
            {
                //bool testbool = false;
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("FGPickingInfo", fgUpdateModels).Result;
                if (response.IsSuccessStatusCode)
                //if (testbool == true)
                {
                    FinishGoodModel finishGoodModel = (FinishGoodModel)Session["FinishGoodModel"];
                    finishGoodModel.PickedPalletQty = finishGoodModel.PickedPalletQty + 1;
                    Session["FinishGoodModel"] = finishGoodModel;
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
                else
                {
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
                    catch(Exception ex)
                    {
                        errMsg = msg;
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                status = "Fail";
            }
            //var json = new JavaScriptSerializer().Serialize(obj);
            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateFinishGoodsLoose(FinishGoodModel obj)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            FGUpdateLooseModel fgUpdateModel = new FGUpdateLooseModel();
            fgUpdateModel.Company = obj.Company;
            fgUpdateModel.RunningNo_Key1 = obj.Key1;
            fgUpdateModel.DPNum_Key2 = obj.Key2;
            fgUpdateModel.SONum_Key3 = obj.Key3;
            fgUpdateModel.PalletLblId = obj.PartNum;
            fgUpdateModel.PalletLblNum = obj.PartDesc;
            fgUpdateModel.Status = "PICKED";

            string errMsg = "";
            string msg = "";
            string status = "";

            try
            {
                //bool testbool = false;
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("LoosePickTruck", fgUpdateModel).Result;
                if (response.IsSuccessStatusCode)
                //if (testbool == true)
                {
                    FinishGoodModel finishGoodModel = (FinishGoodModel)Session["FinishGoodModel"];
                    finishGoodModel.PickedPalletQty = finishGoodModel.PickedPalletQty + 1;
                    Session["FinishGoodModel"] = finishGoodModel;
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
                else
                {
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
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
            }
            //var json = new JavaScriptSerializer().Serialize(obj);
            return Json(new { Message = errMsg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SupervisorLogin(string Username, string Password) {
            bool status = false;
            string msg = "";

            EpiUserModel objUserReq = new EpiUserModel();
            objUserReq.Company = (string)Session["Company"];
            objUserReq.UserId = Username;
            objUserReq.Password = Password;

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("EpiLogin", objUserReq).Result;
            if (response.IsSuccessStatusCode)
            {
                objUserReq = response.Content.ReadAsAsync<EpiUserModel>().Result;
                if (!objUserReq.IsEpiUser)
                {
                    status = false;
                    msg = "Login Failed!";
                }
                else
                {
                    status = true;
                    msg = "Login Success!";
                }
            }
            else
            {
                status = false;
                msg = "Login Failed!";
            }

            return Json(new { Status = status, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        public UserValidationModel UserValidationUpdate(string Company, string Key1_DPNum, string Key2, string UserID) {
            UserValidationModel userValidation = new UserValidationModel();

            string errMsg = "";
            string msg = "";
            bool status = false;

            //***************************Update to DB which SV is updating this truck seal number
            TruckUserModel tuModel = new TruckUserModel();
            tuModel.Company = Company;
            tuModel.Key1_DPNum = Key1_DPNum;
            //Check if null, simply replace to empty
            if (Key2 == null)
            {
                Key2 = "";
            }
            tuModel.Key2 = Key2;
            tuModel.UserID = UserID;

            try
            {
                HttpResponseMessage responseUpdateTruckUser = GlobalVariables.WebApiClient.PostAsJsonAsync("TruckUser", tuModel).Result;
                if (responseUpdateTruckUser.IsSuccessStatusCode)
                {
                    msg = responseUpdateTruckUser.Content.ReadAsStringAsync().Result;
                    status = true;

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
                    catch(Exception ex)
                    {
                        errMsg = msg;
                    }


                }
                else
                {
                    msg = responseUpdateTruckUser.Content.ReadAsStringAsync().Result;
                    status = false;

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
                status = false;
            }
            //***************************
            userValidation.Msg = errMsg;
            userValidation.Valid = status;
            return userValidation;
        }


        public UserValidationModel UserValidation(string Company, string Key1_DPNum, string Key2, string UserID)
        {
            UserValidationModel userValidation = new UserValidationModel();

            //*********Checking if its a valid SV to View this Action
            string msg = "";
            bool isValid = false;

            try
            {
                string qryTruckUser = $"TruckUser?company={Company}&key1={Key1_DPNum}&key2={Key2}&userid={UserID}";

                HttpResponseMessage responseTruckUser = GlobalVariables.WebApiClient.GetAsync(qryTruckUser).Result;
                if (responseTruckUser.IsSuccessStatusCode)
                {
                    TruckUserModel tuModel = new TruckUserModel();
                    tuModel = responseTruckUser.Content.ReadAsAsync<TruckUserModel>().Result;
                    if (tuModel.Valid == true && tuModel.UserInCharge.ToUpper() == UserID.ToUpper())
                    {
                        //Can continue
                        isValid = true;
                    }
                    else if (string.IsNullOrEmpty(tuModel.UserInCharge))
                    {
                        //Automatically Updated the User through Web API POST Method
                        UserValidationModel UVMUpdate = new UserValidationModel();
                        UVMUpdate = UserValidationUpdate(Company, Key1_DPNum, Key2, UserID);
                        if (UVMUpdate.Valid == false)
                        {
                            userValidation.Msg = UVMUpdate.Msg;
                            userValidation.Valid = UVMUpdate.Valid;
                            return userValidation;
                        } else
                        {
                            isValid = true;
                        }
                    }
                    else
                    {
                        isValid = false;
                        //Cannot continue. Redirect to index page with an error msg
                        //msg = responseTruckUser.Content.ReadAsStringAsync().Result;
                        //if (string.IsNullOrEmpty(msg))
                        //{
                            msg = "The person in charge for this truck is " + tuModel.UserInCharge;
                        //}
                        //TempData["Error"] = errMsg;
                        //return RedirectToAction("Index", "FullPicking");
                    }
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                msg = ex.Message.ToString();
                //TempData["Error"] = errMsg;
                //return RedirectToAction("Index", "FullPicking");
            }
            //*********Checking if its a valid SV to View this Action

            userValidation.Msg = msg;
            userValidation.Valid = isValid;
            return userValidation;
        }


    }
}
