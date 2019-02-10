using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class RetrieveModel
    {
        public int Id { get; set; }
        public string UD108Key1 { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public int SONum { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public string LotNum { get; set; }
        public decimal Qty { get; set; }
        public string UOMCode { get; set; }
        public string FromWareHouse { get; set; }
        public string FromBinNum { get; set; }
        public string ToWareHouse { get; set; }
        public string ToBinNum { get; set; }
        public string UserId { get; set; }
    }
}