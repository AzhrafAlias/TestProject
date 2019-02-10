using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class POHeaderModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public int PONum { get; set; }
        public string DONum { get; set; }
        public int SuppNum { get; set; }
        public string SuppId { get; set; }
        public string SuppName { get; set; }
        public string UserId { get; set; }
        public bool CartonLabel { get; set; }
        public List<PODetailModel> PODetails { get; set; }
    }
}