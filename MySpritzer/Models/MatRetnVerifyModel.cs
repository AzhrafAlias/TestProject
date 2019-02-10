using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class MatRetnVerifyModel
    {
        public string Company { get; set; }
        [DisplayName("Part")]
        public string PartNum { get; set; }
        public string ActPartNum { get; set; }
        public bool SupplierPart { get; set; }
        [DisplayName("WH")]
        public string FromWH { get; set; }
        [DisplayName("Bin")]
        public string FromBin { get; set; }
        [DisplayName("ToWH")]
        public string ToWH { get; set; }
        [DisplayName("ToBin")]
        public string ToBin { get; set; }
        public string UOM { get; set; }
        public string Desc { get; set; }
        public string Lot { get; set; }
        public decimal BalQty { get; set; }
        public decimal Qty { get; set; }
        public string Reason { get; set; }
        [DisplayName("ReturnType")]
        public string RetReason { get; set; }
        public string TranStatus { get; set; }
        public string CreatedBy { get; set; }

        public int KeyId { get; set; }
        public string SessionUser { get; set; }
    }
}