using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class FinishGoodModel
    {
        public string Company { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string Key4 { get; set; } //
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public int PalletQty { get; set; }
        public int PickedPalletQty { get; set; } //
        public int Qty { get; set; }
        public int PickedQty { get; set; } //
        public string PalletType { get; set; }
        public string Comment { get; set; }

        public string TruckNo { get; set; }
        public string LoadingBay { get; set; }
    }
}