using MySpritzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Controllers
{
    public class FGReduceController : Controller
    {
        // GET: FGReduce
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

            return View();
        }

        // GET: FGReduce
        [HttpPost]
        public ActionResult CheckLotNum(string Company, string LotNum)
        {
            Session["FGReduce"] = null;

            string msg = "";
            string status = "";
            decimal onhandQty = 0;
            string PartDescription = "";

            FGReduceModel fgReduceModel = new FGReduceModel();
            try
            {
                string qry = $"FGReduce?Company={Company}&LotNum={LotNum}";
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync(qry).Result;
                if (response.IsSuccessStatusCode)
                {
                    fgReduceModel = response.Content.ReadAsAsync<FGReduceModel>().Result;
                    Session["FGReduce"] = fgReduceModel;
                    onhandQty = fgReduceModel.OnhandQty;
                    PartDescription = fgReduceModel.PartDescription;
                    msg = "Lot Num Exist";
                    status = "Success";
                }
                else
                {
                    onhandQty = 0;
                    msg = response.Content.ReadAsStringAsync().Result;
                    status = "Fail";
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                status = "Fail";
            }

            return Json(new { Message = msg, Status = status, OnhandQty = onhandQty, PartDescription = PartDescription }, JsonRequestBehavior.AllowGet);
        }

        // GET: FGReduce
        [HttpPost]
        public ActionResult UpdateLotNum(FGReduceModel obj)
        {
            FGReduceModel fgReduceModel = (FGReduceModel)Session["FGReduce"];
            fgReduceModel.ModifiedQty = obj.ModifiedQty;

            string msg = "";
            string status = "";

            try
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("FGReduce", fgReduceModel).Result;
                if (response.IsSuccessStatusCode)
                {
                    string msgJson = "";
                    msgJson = response.Content.ReadAsStringAsync().Result;
                    Session["FGReduce"] = null;
                    status = "Success";

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
                else
                {
                    msg = response.Content.ReadAsStringAsync().Result;
                    status = "Fail";
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                status = "Fail";
            }

            return Json(new { Message = msg, Status = status }, JsonRequestBehavior.AllowGet);
        }


        // GET: FGReduce/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FGReduce/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FGReduce/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FGReduce/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FGReduce/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FGReduce/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FGReduce/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
