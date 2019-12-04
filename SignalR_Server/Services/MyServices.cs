using SignalR_webapi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_webapi.Services
{
    public class MyServices : IMyServices
    {
        public MyServices()
        {
    
        }
        public string GetServiceName(int id)
        {
            return "Service: " + id;
        }
    }

}