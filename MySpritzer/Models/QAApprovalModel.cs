using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class QAApprovalModel
    {
        public string UD110Company { get; set; }
        public string UD110Key1 { get; set; } //dp num
        public string UD110Key2 { get; set; } //running num
        public bool UD110FS_QAInspStatus { get; set; }
        public string UD110FS_TruckStaRemarks { get; set; }
        public string UD110FS_TruckQASignanture { get; set; }
        public string UD110FS_TruckLorryDriverSignature { get; set; }
    }
}