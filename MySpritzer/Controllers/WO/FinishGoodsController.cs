using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers.WO
{
    public class FinishGoodsController : Controller
    {
        // GET: FinishGoods
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

        public ActionResult FinishGoods(FGShipToModel obj) {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Company"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string PilotID = (string)Session["User"];
            string Company = (string)Session["Company"];

            List<FinishGoodModel> fgs = new List<FinishGoodModel>();
            try
            {
                //gonna add 1 more parameter (shipto)
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
            }
            catch (Exception ex)
            {
                ViewBag.Status = ex.Message.ToString();
            }

            return View();
        }
    }
}