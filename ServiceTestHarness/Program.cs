using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using ServiceProcess;
using System.Threading;

namespace ServiceTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().ServiceTest();
            Console.ReadKey();
        }

        private void ServiceTest()
        {
            var service = new ServiceRunner();

            Console.WriteLine("Starting Service.");
            service.StartService();
            Console.WriteLine("Service Started, press any key to stop the process.");
            Console.ReadKey();
            Console.Write("Stopping Service.");
            service.StopService();
            while (!service.IsReadyToExit)
            {
                Console.Write(".");
                Thread.Sleep(5000);
            }

            Console.Write(" Stopped." + Environment.NewLine);
            Console.WriteLine("Press any key to exit the test harness.");
        }
    }
}
