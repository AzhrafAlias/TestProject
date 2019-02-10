using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class FGReduceModel
    {
        public string Company { get; set; }
        public string Plant { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public string LotNum { get; set; }
        public string FromWarehouse { get; set; }
        public string FromBinNum { get; set; }
        public string ToWarehouse { get; set; }
        public string ToBinNum { get; set; }
        public string Uom { get; set; } //DimCode
        public decimal OnhandQty { get; set; }
        public decimal ModifiedQty { get; set; }
        public string PartDescription { get; set; }
    }
}