using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PilotToTruckModel
    {
        public string UD25Company { get; set; }
        public string UD25Key1 { get; set; }
        public string UD25Key2 { get; set; }
        public string UD25Key3 { get; set; }
        public string UD25FS_TruckStatus_c { get; set; }
        public bool UD25FS_TruckActive_c { get; set; }
        public string UD25FS_PilotID_c { get; set; }
        public string UD25FS_LoadingBay_c { get; set; }
        public string UD25FS_SupervisorID_c { get; set; }
        public DateTime UD25FS_PilotAssigDateTime_c { get; set; }
        public string TruckNo { get; set; }
    }
}