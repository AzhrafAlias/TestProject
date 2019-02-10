using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class LoosePickingDetailModel
    {
        public int Id { get; set; }
        public string UD108Key1 { get; set; }
        public string SysRowId { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public int SONum { get; set; }
        public DateTime ShipByDate { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public decimal ActQty { get; set; }
        public decimal PrevLPQty { get; set; }
        public decimal Qty { get; set; }
        public string UomCode { get; set; }
        public string Remark { get; set; }
        public decimal LooseAreaAvailableQty { get; set; }
        public string LoosePickStatus { get; set; }
        public bool IsLoosePickFull { get; set; }
        public string UserId { get; set; }
        public string DPNum { get; set; }
    }
}