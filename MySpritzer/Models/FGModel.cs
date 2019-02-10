using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MySpritzer.Models
{
    public class FGModel
    {
        public string Company { get; set; }
        public string LotNum { get; set; }
        public decimal OnhandQty { get; set; }
        [DisplayName("UOM")]
        public string DimCode { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public string FromWH { get; set; }
        public string FromBin { get; set; }
        public string ToWH { get; set; }
        public string ToBin { get; set; }
    }
}