using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PODetailModel
    {
        public int Id { get; set; }//nd
        public string Company { get; set; }//nd
        public int PONum { get; set; }//nd
        public int POLine { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; } //need to add form field
        public bool TrackLot { get; set; }
        public decimal XOrdQty { get; set; }
        public string IUM { get; set; }
        public decimal OrdQty { get; set; }
        public string PUM { get; set; }
        public decimal PrevRcvQty { get; set; }
        public decimal Qty { get; set; } //nd isRF, isRS
        public string LotNum { get; set; } //nd
        public string TuId { get; set; } //nd
        public string UomCode { get; set; } //need to add form field
        public string WarehouseCode { get; set; }
        public string BinNum { get; set; }
        public string JobNum { get; set; }
        public int Assembly { get; set; }
        public int Sequence { get; set; }
        public LotAttributeModel LotAttribute { get; set; }
        public PrintTagModel PrintTag { get; set; }
        public bool IsReceivedFull { get; set; } //nd
        public string ReceiveStatus { get; set; }

        public string POInfo { get; set; }
        
        public int SuppNum { get; set; }
        public string DONum { get; set; }

        public decimal TuConv { get; set; }
        public int NumOfDecPUM { get; set; }
        public int NumOfDecIUM { get; set; }
    }
}