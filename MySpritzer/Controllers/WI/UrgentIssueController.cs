using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.WI
{
    public class UrgentIssueController : Controller
    {
        // GET: UrgentIssue
        public ActionResult UI()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            UrgentIssueModel obj = new UrgentIssueModel();
            ViewBag.Status = "";
            return View(obj);
        }

        [HttpPost]
        public ActionResult UI(UrgentIssueModel obj)
        {
            if (obj != null)
            {
                if (obj.UserQty <= obj.Qty)
                {
                    obj.Qty = obj.UserQty;

                    HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("UrgentIssue", obj).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        obj = new UrgentIssueModel();
                        ViewBag.Status = "Update Successfully!";
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
                        ViewBag.Status = msg;
                    }
                }
                else
                {
                    ViewBag.Status = "UserQty not enough!";
                }

            }
            return View(obj);

        }

        public ActionResult GetData(string Company, string JobNum, string PartNum, string LotNum)
        {
            UrgentIssueModel entity = new UrgentIssueModel();
            if (!string.IsNullOrEmpty(Company) && !string.IsNullOrEmpty(JobNum) && !string.IsNullOrEmpty(PartNum))
            {
                string Qry = string.Empty;
                if (string.IsNullOrEmpty(LotNum))
                {
                    Qry = $"UrgentIssue?Company={Company}&JobNum={JobNum}&PartNum={PartNum}";
                }
                else
                {
                    Qry = $"UrgentIssue?Company={Company}&JobNum={JobNum}&PartNum={PartNum}&LotNum={LotNum}";
                }

                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(Qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    entity = response.Content.ReadAsAsync<UrgentIssueModel>().Result;
                    entity.UserQty = entity.Qty;
                }
                else
                {
                    //var httpErr = response.Content.ReadAsAsync<errHttpAmin.HttpError>();
                    string msgJson = response.Content.ReadAsStringAsync().Result;
                    JObject jObject = JObject.Parse(msgJson);
                    string msg = (string)jObject.SelectToken("Message");
                    string msgDtl = (string)jObject.SelectToken("MessageDetail");
                    string exMsg = (string)jObject.SelectToken("ExceptionMessage");
                    string exType = (string)jObject.SelectToken("ExceptionType");
                    msg += $"\n{msgDtl} \n{exMsg}";
                    //ViewBag.Error = msg;//httpErr.Result["Message"].ToString();
                    return Json(new { success = false, responseText = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, responseText = "Company Or JobNum Or PartNum Or LotNum is Empty!" }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { success = true, responseText = entity }, JsonRequestBehavior.AllowGet);

        }
    }
}