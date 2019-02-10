using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PalletSelectionModel
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
        public decimal PrevQty { get; set; }
        public int PalletNum { get; set; }
        public string LotNum { get; set; }
        public decimal PalletQty { get; set; }
        public string UomCode { get; set; }
        public string FromWareHouse { get; set; }
        public string FromBinNum { get; set; }
        public string ToWareHouse { get; set; }
        public string ToBinNum { get; set; }
        public bool VerifyStatus { get; set; }
        public string Remark { get; set; }
        public string LoosePickStatus { get; set; }
        public string UserId { get; set; }
        public string RowMod { get; set; }
        public string DPNum { get; set; }

        public string LotCheck { get; set; } //if empty or null, no need to display msg, else display
    }
}