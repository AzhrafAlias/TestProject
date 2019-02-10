using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PilotModel
    {
        public string PilotID { get; set; }
        public string Name { get; set; }
    }

    public class LoadingBayModel
    {
        public string LoadingBay { get; set; }
    }

    public class UD110PilotAssgList
    {
        public string UD110Company { get; set; }
        public string UD110Key1 { get; set; }
        public string UD110Key2 { get; set; }
        public string UD110FS_TruckNo_c { get; set; }
        public string UD110FS_containerNo_c { get; set; }
        public string UD110FS_sealNo_c { get; set; }
        public string UD110FS_Type_c { get; set; }
        public string UD110FS_LoadingBay_c { get; set; }
    }
}