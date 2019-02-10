using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class FGTopUpModel
    {
        public string Company { get; set; }
        [DisplayName("LotNum")]
        public string LotNo { get; set; }
        //public string PartNo;
        public decimal Qty { get; set; }
        public List<FGTopUpInvTransListModel> fGTopUpInvTransLists { get; set; }
        public string PartDescription { get; set; }
    }
}