using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class MaterialReturnWEBAPI
    {
        public string Company { get; set; }
        public string PartNum { get; set; }
        public string ActPartNum { get; set; }
        public bool SupplierPart { get; set; }

        public string FromWH { get; set; }

        public string FromBin { get; set; }
        public string ToWH { get; set; }

        public string ToBin { get; set; }
        public string UOM { get; set; }
        public string Desc { get; set; }

        public string Lot { get; set; }
        public decimal BalQty { get; set; }
        public decimal Qty { get; set; }
        public string Reason { get; set; }

        public string RetReason { get; set; }

        public string TranStatus { get; set; }
        public string CreatedBy { get; set; }
    }
}