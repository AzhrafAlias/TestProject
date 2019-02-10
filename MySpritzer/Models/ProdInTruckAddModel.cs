using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class ProdInTruckAddModel
    {
        public string Company { get; set; }
        public string DPNo { get; set; }
        public decimal QtyPallet { get; set; }

        public string PartNo { get; set; }
        public string PartDesc { get; set; }
        public string Ud108Key1 { get; set; }
    }
}