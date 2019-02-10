using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class IssueMaterialModel
    {
        public string Company { get; set; }

        public string Part { get; set; }
        public string Desc { get; set; }
        [DisplayName("Lot")]
        public string LotNum { get; set; }
        [DisplayName("Whse")]
        public string FromWH { get; set; }
        [DisplayName("Bin")]
        public string FromBin { get; set; }
        public string ToWH { get; set; }
        public string ToBin { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal Qty { get; set; }

        public string UOM { get; set; }
        public bool SupplierPart { get; set; }
        public string ActualPart { get; set; }
        public decimal BalQty { get; set; }
        public string TranStatus { get; set; }
    }
}