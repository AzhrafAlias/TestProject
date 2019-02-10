using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class ProductInTruckModel
    {
        public string Company { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string ChildKey1 { get; set; }
        public string ChildKey2 { get; set; }
        public string ChildKey3 { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public string LotNum { get; set; }
        public decimal QtyCtn { get; set; }
        public decimal QtyPallet { get; set; }
        public string Comment { get; set; }
        public bool CustomerPallet { get; set; }
        public string PalletType { get; set; }
        public int FS_NoIncludePallet_c { get; set; }

        //public decimal TotalPartQty { get; set; } //For display only. When Add On (Total Qty for that Part)
    }
}