using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_webapi.Models
{
    public class CpuRam
    {
        public string Name { get; set; }
        public float CPU { get; set; }
        public float Ram { get; set; }
        public string ModifyDate { get; set; }
    }
}