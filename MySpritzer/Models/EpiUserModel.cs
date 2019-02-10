using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class EpiUserModel
    {
        public string Company { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        //public bool IsEpiUser { get; internal set; }
        public bool IsEpiUser { get; set; }
    }
}