using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class ProdInTruckConModel
    {
        public string Company { get; set; }
        public string DPNo { get; set; }
        public string UD110Key2 { get; set; }
        public bool CustomerPallet { get; set; }
        public List<ProdInTruckConP> PalletList { get; set; }
    }

    public class ProdInTruckConP
    {
        public int Index { get; set; }
        public string PalletType { get; set; }
        public decimal PalletQty { get; set; }
    }
}