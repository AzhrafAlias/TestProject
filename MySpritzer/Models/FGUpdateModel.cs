using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class FGUpdateModel
    {
        public string Company { get; set; }
        public string RunningNo_Key1 { get; set; }
        public string DPNum_Key2 { get; set; }
        public string SONum_Key3 { get; set; }
        public string PartNum { get; set; }
        public string LotNum { get; set; }
        public int Qty { get; set; }
        public string UserID { get; set; }
        public string UD110Key2 { get; set; }

        public string Status { get; set; }
    }
}