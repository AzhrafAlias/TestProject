using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PalletModel
    {
        public int PId { get; set; }
        public decimal PQty { get; set; }
        public bool isVerified { get; set; }//
    }
}