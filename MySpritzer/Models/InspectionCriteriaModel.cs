using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class InspectionCriteriaModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Criteria { get; set; }
        public bool Mandatory { get; set; }
    }
}