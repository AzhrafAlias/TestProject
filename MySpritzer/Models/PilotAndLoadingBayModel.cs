using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PilotAndLoadingBayModel
    {
        public List<PilotIDList1> pilotIDList1s { get; set; }
        public List<LoadingBayList1> loadingBayList1s { get; set; }
    }
    public class PilotIDList1
    {
        public string PilotID { get; set; }
        public string Name { get; set; }
        public bool selected { get; set; }
    }
    public class LoadingBayList1
    {
        public string LoadingBay { get; set; }
        public string LoadingBayDesc { get; set; }
        public bool selected { get; set; }
    }

    //Use in FinishGoods View
    public class TruckList
    {
        public string TruckId { get; set; }
        public string TruckNo { get; set; }
    }
}