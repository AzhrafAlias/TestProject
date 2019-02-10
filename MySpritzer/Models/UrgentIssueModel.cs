using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class UrgentIssueModel
    {
        public string Company { get; set; }
        public string JobNum { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public int AssemblySeq { get; set; }
        public int MtlSeq { get; set; }
        public string IUM { get; set; }
        public string LotNum { get; set; }
        public decimal Qty { get; set; }
        public string FromWH { get; set; }
        public string FromBin { get; set; }
        public string ToWH { get; set; }
        public string ToBin { get; set; }
        public decimal UserQty { get; set; }

        public UrgentIssueModel()
        {
            UserQty = 0;
        }
    }
}