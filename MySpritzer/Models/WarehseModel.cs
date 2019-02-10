using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class WarehseModel
    {
        public string Company { get; set; }
        public string PartNum { get; set; }
        public string LotNum { get; set; }
        public string Warehse { get; set; }
        public string BinNum { get; set; }
        public string SuppId { get; set; }
        public decimal Qty { get; set; }
        public DateTime ExpriyDate { get; set; }
    }
}