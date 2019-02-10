using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.WI
{
    public class FGTopUpController : Controller
    {
        // GET: FGTopUp
        public ActionResult TopUp()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            FGTopUpModel obj = new FGTopUpModel();
            Session["FGTopUpModel"] = null;
            return View();
        }
        public ActionResult GetData(string Company = null, string LotNum = null)
        {
            try
            {
                List<FGTopUpModel> objs = new List<FGTopUpModel>();

                FGTopUpModel obj = null;

                if (!string.IsNullOrEmpty(LotNum) && !string.IsNullOrEmpty(Company))
                {
                    if (Session["FGTopUpModel"] != null)
                    {
                        objs = (List<FGTopUpModel>)Session["FGTopUpModel"];
                    }
                    if (objs.Where(w => w.Company == Company && w.LotNo == LotNum).Count() > 0)
                    {
                        throw new Exception("Lot Exist Already!");
                        //return Json(new { success = false, responseText = "Lot Exist Already!" }, JsonRequestBehavior.AllowGet);
                    }

                    string Qry = $"FGTopUp?Company={Company}&LotNum={LotNum}";
                    HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        obj = response.Content.ReadAsAsync<FGTopUpModel>().Result;
                        if (!string.IsNullOrEmpty(obj.LotNo))
                        {
                            //objs.Add(obj);
                            Session["FGTopUpModel"] = null;
                            Session["FGTopUpModel"] = objs;

                        }
                        else
                        {
                            string msg = response.Content.ReadAsStringAsync().Result;
                            throw new Exception(msg);
                            //throw new Exception("API Resonse is null!");
                            //return Json(new { success = false, responseText = "API Resonse is null!" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        string msgJson = response.Content.ReadAsStringAsync().Result;
                        JObject jObject = JObject.Parse(msgJson);
                        string msg = (string)jObject.SelectToken("Message");
                        string msgDtl = (string)jObject.SelectToken("MessageDetail");
                        string exMsg = (string)jObject.SelectToken("ExceptionMessage");
                        string exType = (string)jObject.SelectToken("ExceptionType");
                        msg += $"\n{msgDtl} \n{exMsg}";
                        //ViewBag.Status = msg;
                        //return Json(new { success = false, responseText = msg }, JsonRequestBehavior.AllowGet);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    //return Json(new { success = false, responseText = "Company Or LotNum is Null!" }, JsonRequestBehavior.AllowGet);
                    throw new Exception("Company Or LotNum is Null!");
                }

                return Json(new { success = true, responseText = obj }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult FGTopUpGrid(string Company, string LotNum, decimal Qty)
        {
            try
            {
                List<FGTopUpModel> objs = new List<FGTopUpModel>();
                FGTopUpModel obj = new FGTopUpModel();
                ViewBag.Status = "";
                if (Session["FGTopUpModel"] != null)
                {
                    objs = (List<FGTopUpModel>)Session["FGTopUpModel"];
                }
                else
                {
                    throw new Exception("No Scanned LotData!");

                    //return Json(new { success = false, responseText = "No Scanned LotData!" }, JsonRequestBehavior.AllowGet);
                }

                int cnt = objs.Where(w => w.Company == Company && w.LotNo == LotNum).Count();
                //if (cnt == 1)
                //{
                //    objs.Where(w => w.Company == Company && w.LotNo == LotNum).ToList().ForEach(u => u.Qty = Qty);
                //    Session["FGTopUpModel"] = null;
                //    Session["FGTopUpModel"] = objs;

                //    //var result= PartialView( objs);


                //}
                //else
                //{
                //    if (cnt < 1)
                //    {
                //        throw new Exception("LotNo not found in Scanned List!");
                //        //return Json(new { success = false, responseText = "LotNo not found in Scanned List!" }, JsonRequestBehavior.AllowGet);
                //        //ViewBag.Status = "LotNo not found in Scanned List!";

                //    }
                //    else
                //    {
                //        throw new Exception("Duplicate LotNo found in Scanned List!");
                //        //return Json(new { success = false, responseText = "Duplicate LotNo found in Scanned List!" }, JsonRequestBehavior.AllowGet);
                //        //ViewBag.Status = "LotNo not found in Scanned List!";
                //    }
                //}

                if (cnt >= 1)
                {
                    throw new Exception("Duplicate LotNo found in Scanned List!");
                    

                    //var result= PartialView( objs);


                }
                else
                {
                    obj.Company = Company;
                    obj.LotNo = LotNum;
                    obj.Qty = Qty;

                    objs.Add(obj);
                    Session["FGTopUpModel"] = null;
                    Session["FGTopUpModel"] = objs;
                }
                return Json(new { success = true, responseText = RenderRazorViewToString("~/Views/FGTopUp/RemoveLot.cshtml", objs) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult TopUp(FGTopUpModel obj)
        {
            try
            {
                if (Session["User"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                List<FGTopUpModel> objs = null;
                if (Session["FGTopUpModel"] != null)
                {
                    objs = (List<FGTopUpModel>)Session["FGTopUpModel"];
                }
                else
                {
                    throw new Exception("No Scanned LotData!");
                    //return Json(new { success = false, responseText = "No Scanned LotData!" }, JsonRequestBehavior.AllowGet);
                }

                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("FGTopUp", objs).Result;
                if (response.IsSuccessStatusCode)
                {
                    Session["FGTopUpModel"] = null;
                }
                else
                {
                    string msgJson = response.Content.ReadAsStringAsync().Result;
                    JObject jObject = JObject.Parse(msgJson);
                    string msg = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    if (msgDtl == null)
                    {
                        throw new Exception(msg);
                    }
                    else
                    {
                        throw new Exception(msgDtl);
                    }
                }
                //ViewBag.Status = "Inserted Sucessfully!";
                return Json(new { success = true, responseText = "Inserted Sucessfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
           // return View(obj);
        }
        // GET: FGTopUp/Details/5
        public ActionResult RemoveLot(string Company, string LotNum)
        {
            try
            {
                List<FGTopUpModel> objs = new List<FGTopUpModel>();
                FGTopUpModel obj = new FGTopUpModel();
                if (Session["FGTopUpModel"] != null)
                {
                    objs = (List<FGTopUpModel>)Session["FGTopUpModel"];
                    obj = objs.Where(w => w.Company == Company && w.LotNo == LotNum).FirstOrDefault();
                    objs.Remove(obj);

                    Session["FGTopUpModel"] = null;
                    Session["FGTopUpModel"] = objs;


                    //return PartialView("~/Views/FGTopUp/RemoveLot.cshtml", objs);

                }
                else
                {
                    throw new Exception("Selected Item not in Memory!");
                    //ViewBag.Status = "Selected Item not in Memory!";
                    //return Json(new { success = false, responseText = "Selected Item not in Memory!" }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { success = true, responseText = RenderRazorViewToString("~/Views/FGTopUp/RemoveLot.cshtml", objs) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult EmptyData()
        {
            Session["FGTopUpModel"] = null;
            List<FGTopUpModel> objs = new List<FGTopUpModel>();
            return Json(new { success = true, responseText = RenderRazorViewToString("~/Views/FGTopUp/RemoveLot.cshtml", objs) }, JsonRequestBehavior.AllowGet);
        }
    }
}