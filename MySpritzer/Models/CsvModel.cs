using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class CsvModel
    {
        public int PONum { get; set; }
        public int DONum { get; set; }
        public string PartNum { get; set; }
        public int LotNum { get; set; }
        public decimal QtyPerContainer { get; set; }
        public string Uom { get; set; }
        public int NumberOfTags { get; set; }
    }
}