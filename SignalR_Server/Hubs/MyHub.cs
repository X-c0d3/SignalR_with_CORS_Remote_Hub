using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR_webapi.Interfaces;
using System.Threading.Tasks;
using SignalR_webapi.Models;

namespace SignalR_webapi.Hubs
{
    [HubName("MyHub")]
    public class MyHub : Hub
    {
        protected static PerformanceCounter cpuCounter;
        protected static PerformanceCounter ramCounter;
        static List<PerformanceCounter> cpuCounters = new List<PerformanceCounter>();
        static int cores = 0;

        readonly IMyServices _myservice;
        public MyHub(IMyServices myservice)
        {
            this._myservice = myservice;

        }

        public override Task OnConnected()
        {
            Console.WriteLine("Client connected..");

            Task.Run(() => StartTask());
      

            return base.OnConnected();
        }

        void StartTask ()
        {
            Console.WriteLine("[+] Starting...");
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");


            try
            {
                System.Timers.Timer t = new System.Timers.Timer(1200);
                t.Elapsed += new ElapsedEventHandler(TimerElapsed);
                t.Start();
                Thread.Sleep(10000);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:" + e.Message);
            }
            Console.ReadLine();
        }

        public void Send(string name, string messag)
        {
            Clients.All.PrintMessage(name, messag);
        }

        public void Hello()
        {
            //Clients.All.hello();


            string message = this._myservice.GetServiceName(1);
            Clients.Caller.setMessage(message);
        }

        public static void ConsumeCPU()
        {
            int percentage = 60;
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("percentage");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                if (watch.ElapsedMilliseconds > percentage)
                {
                    Thread.Sleep(100 - percentage);
                    watch.Reset();
                    watch.Start();
                }
            }
        }

        public void TimerElapsed(object source, ElapsedEventArgs e)
        {
            float cpu = cpuCounter.NextValue();
            float sum = 0;
            foreach (PerformanceCounter c in cpuCounters)
            {
                sum = sum + c.NextValue();
            }
            sum = sum / (cores);
            float ram = ramCounter.NextValue();
            Console.WriteLine(string.Format("CPU Value 1: {0}, Cpu  2: {1} ,Ram : {2}", sum, cpu, ram));

            var res = new CpuRam
            {
                Name = "X-c0d3/" + Guid.NewGuid(),
                CPU = cpu,
                Ram = ram,
                ModifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff")
            };

            Clients.All.PrintMessage("Hi", res);
        }
    }
}