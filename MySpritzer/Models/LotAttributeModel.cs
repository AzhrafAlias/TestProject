using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class LotAttributeModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public int PONum { get; set; }
        public int POLine { get; set; }
        public string PartNum { get; set; }
        public string AttBatch { get; set; }
        public string Batch { get; set; }
        public string AttMfgBatch { get; set; }
        public string MBatch { get; set; }
        public string AttMfgLot { get; set; }
        public string MLot { get; set; }
        public string AttHeat { get; set; }
        public string Heat { get; set; }
        public string AttFirmware { get; set; }
        public string Firm { get; set; }
        public string AttBeforeDt { get; set; }
        public DateTime? BestBefore { get; set; }
        public string AttMfgDt { get; set; }
        public DateTime? OrigMfg { get; set; }
        public string AttCureDt { get; set; }
        public DateTime? Cure { get; set; }
        public string AttExpDt { get; set; }
        public DateTime? Expire { get; set; }

        public string LotNum { get; set; }
        public string DONum { get; set; }
    }
}