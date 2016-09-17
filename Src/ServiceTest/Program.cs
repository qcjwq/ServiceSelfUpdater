using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SelfUpdateHelper;
using ServiceProcess;

namespace ServiceTest
{
    class Program
    {
        static void Main(string[] args)
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

            Console.ReadKey();
        }

        private static void Test()
        {
            for (int i = 0; i < 1; i++)
            {
                Logger.Singleton.Debug("sdfsftest", new Exception("jkjskldjfksj"));
                Logger.Singleton.Info("info");
                Logger.Singleton.Warn("warn");

                try
                {
                    var ij = 0;
                    var j = 1 / ij;
                }
                catch (Exception e)
                {
                    Logger.Singleton.Error(e.Message, e);
                    Logger.Singleton.Fatal(e.Message, e);
                }
            }

            Console.ReadKey();
        }
    }
}
