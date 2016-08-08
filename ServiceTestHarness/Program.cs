using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceProcess;
using System.Threading;

namespace ServiceTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Service.");
            ServiceRunner.StartService();
            Console.WriteLine("Service Started, press any key to stop the process.");
            Console.ReadKey();
            Console.Write("Stopping Service.");
            ServiceRunner.StopService();
            while (!ServiceRunner.IsReadyToExit)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }

            Console.Write(" Stopped." + Environment.NewLine);
            Console.WriteLine("Press any key to exit the test harness.");
            Console.ReadKey();


        }
    }
}
