using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class FGTopUpInvTransListModel
    {
        public string Company { get; set; }
        public string LotNo { get; set; }
        public string PartNo { get; set; }
        public decimal Qty { get; set; }
        public string IUM { get; set; }
        public string FromWH { get; set; }
        public string FromBin { get; set; }
        public string ToWH { get; set; }
        public string ToBin { get; set; }
    }
}