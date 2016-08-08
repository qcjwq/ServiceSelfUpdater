using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;

namespace ServiceProcess
{
    public class ServiceRunner
    {
        static bool serviceStarted = false;
        static bool readyToStop = true;

        public static bool IsReadyToExit { get { return !serviceStarted && readyToStop; } }

        public static void StartService()
        {
            serviceStarted = true;
            ThreadPool.QueueUserWorkItem(StartServiceThread);
        }

        public static void StopService()
        {
            serviceStarted = false;
            while (!IsReadyToExit)
            {
                Thread.Sleep(1000);
            }
        }

        public static void StartServiceThread(object state)
        {
            while (serviceStarted)
            {
                readyToStop = false;

                // Perform actions
                SimulateLongRunningTask();

                readyToStop = true;
                Thread.Sleep(60000);
            }
        }

        private static void SimulateLongRunningTask()
        {
            for (int i = 0; i < 10; i++)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.txt"), true))
                {
                    sw.WriteLine(string.Format("{0} - Process iterrated", DateTime.Now.ToString().PadRight(30, ' ')));
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
