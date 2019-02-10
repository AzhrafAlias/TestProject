using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//This is Loose Picking Model Dummy
namespace MySpritzer.Models
{
    public class LoosePickingHeaderModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public int SONum { get; set; }
        public DateTime ShipByDate { get; set; }
        public string UserId { get; set; }

        public string PickingStatus { get; set; }//
        public string TimeSlot { get; set; }
    }
}