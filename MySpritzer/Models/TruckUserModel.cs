using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class TruckUserModel
    {
        public string Company { get; set; }
        public string Key1_DPNum { get; set; }
        public string Key2 { get; set; }
        public string UserID { get; set; }
        public string UserInCharge { get; set; }
        public bool Valid { get; set; }
    }
}