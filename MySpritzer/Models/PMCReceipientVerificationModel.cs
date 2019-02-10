using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PMCReceipientVerificationModel
    {
        public int Id { get; set; }
        public int PickRawMtrlFKId { get; set; } // PickRawMtrl Id
        public string Company { get; set; }
        public string JobNum { get; set; }
        public string Plant { get; set; }
        public DateTime RequestDate { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public string LotNo { get; set; }
        public string TuId { get; set; }
        public string TranType { get; set; }
        public decimal ActQty { get; set; }
        public decimal Qty { get; set; }
        public string UOM { get; set; }
        public decimal PickQty { get; set; }
        public string PickUOM { get; set; }
        public string FromWarehse { get; set; }
        public string FromBinNum { get; set; }
        public string ToWarehse { get; set; }
        public string ToBinNum { get; set; }
        public string PalletType { get; set; }
        public decimal PalletQty { get; set; }
        public string UserId { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonDesc { get; set; }
        public decimal RejQty { get; set; }
        public string Status { get; set; } // A - Approve , R - Reject
        public bool IsEpiRow { get; set; }
        public byte[] ts { get; set; }
        //Input data for PalletType
        public List<ReasonModel> ReasonTypes { get; set; }
    }
}