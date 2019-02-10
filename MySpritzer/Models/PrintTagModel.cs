using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PrintTagModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public int PONum { get; set; }
        public int POLine { get; set; }
        public decimal QtyPerContainer { get; set; }
        public string Uom { get; set; }
        public decimal TransactionQty { get; set; }
        public decimal TagQty { get; set; }
        public int NumberOfTags { get; set; }

        public string DONum { get; set; }
        public string PartNum { get; set; }
        public string LotNum { get; set; }
    }
}