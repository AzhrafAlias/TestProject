using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer
{
    public class LotNumSuggestion
    {
        public string LotNum { get; set; }
        public decimal OnHandQty { get; set; }
        public string FromWareHouse { get; set; }
        public string FromBinNum { get; set; }
        public string ToWareHouse { get; set; }
        public string ToBinNum { get; set; }
        public string OrderByType { get; set; }
        public string OrderByField { get; set; }
        public string OrderByDate { get; set; }
    }
}