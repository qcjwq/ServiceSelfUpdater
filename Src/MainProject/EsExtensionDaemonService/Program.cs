using System;
using System.IO;
using System.ServiceProcess;
using SelfUpdateHelper;
using ServiceProcess;

namespace EsExtensionDaemonService
{
    public class Program
    {
        private static ServiceRunner serviceRunner;

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                var service = new EsExtensionDaemonService();
                ServiceBase.Run(service);
            }
            else
            {
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        public static void Start(string[] args)
        {
            Logger.Singleton.Info("Daemon service start");
            Helper.HandlerAction(() =>
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StartUp");
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
                Logger.Singleton.Info("服务启动时清空插件和配置目录完成");
            });
            serviceRunner = new ServiceRunner();
            serviceRunner.StartService();
        }

        public static void Stop()
        {
            serviceRunner.StopService();
            Logger.Singleton.Info("Daemon service stop");
        }
    }
}
