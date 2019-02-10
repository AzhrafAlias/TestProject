using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySpritzer.Models
{
    public class MaterialReturnModel
    {
        public string Company { get; set; }
        [DisplayName("Part")]
        public string PartNum { get; set; }
        public string ActPartNum { get; set; }
        public bool SupplierPart { get; set; }
        [DisplayName("Whse")]
        public string FromWH { get; set; }
        [DisplayName("Bin")]
        public string FromBin { get; set; }
        public string ToWH { get; set; }//
        public string ToBin { get; set; }//
        public string UOM { get; set; }
        public string Desc { get; set; }
        [DisplayName("Lot")]
        public string Lot { get; set; }
        public decimal BalQty { get; set; }
        public decimal Qty { get; set; }
        public string Reason { get; set; }
        public IEnumerable<SelectListItem> ReasonList { get; set; }
        public string RetReason { get; set; }

        public string TranStatus { get; set; }
        public string CreatedBy { get; set; }
    }
}