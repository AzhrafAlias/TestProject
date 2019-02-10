using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class InterCoRcvDtlModel
    {
        [DisplayName("Part")]
        public string PartNum { get; set; }
        [DisplayName("No.")]
        public int LineNo { get; set; }
        [DisplayName("Lot")]
        public string LotNum { get; set; }
        public decimal Qty { get; set; }
        [DisplayName("SUM")]
        public string SUOM { get; set; }
    }
}