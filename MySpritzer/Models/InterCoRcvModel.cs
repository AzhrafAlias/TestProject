using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace MySpritzer.Models
{
    public class InterCoRcvModel
    {
        public string Company { get; set; }
        public int intQue { get; set; }
        [DisplayName("PO")]
        public int PONum { get; set; }
        [DisplayName("DO")]
        public string DONum { get; set; }
        public string RejComment { get; set; }
        public string TranStatus { get; set; }
        public int VendorNum { get; set; }
        [DisplayName("Supplier")]
        public string SupName { get; set; }
        public List<InterCoRcvDtlModel> Detail { get; set; }
    }
}