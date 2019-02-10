using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class UpdateInspectionModel
    {
        public string Company { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string CriteriaType { get; set; }
        public bool Result { get; set; }
    }

    public class InspectionFormModel
    {
        public string Company { get; set; }
        public string DPNum { get; set; }
        public string Key2 { get; set; }
        public List<string> CriteriaType { get; set; }

    }
}