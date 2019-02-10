using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    //Finish Goods Pick Process Page - Object response get from WEB API (sample: FPLotNum?Company=CSTP&PartNum=2-NMW-SPR-023024-C001&LotNum=101043) when Checking Lot Num
    public class FGLotNumResponseModel
    {
        public string Company { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public string LotNum { get; set; }
        public decimal OnhandQty { get; set; }
    }
}