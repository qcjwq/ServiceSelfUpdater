using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using ServiceProcess;
using System.Threading;
using Ionic.Zip;
using unirest_net.http;

namespace ServiceTestHarness
{
    class Program
    {
        private readonly string Host = "http://localhost:8001/WebServiceTestTools4J/api/";
        private readonly string ReceiveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upgrade");

        static void Main(string[] args)
        {
            var fileName = "upgrade.zip";
            var p = new Program();

            var response = p.NeedUpgrate();

            p.DownloadFile(fileName);
            p.Archive(fileName);

            //ServiceTest();
            Console.ReadKey();
        }

        private bool NeedUpgrate()
        {
            var method = GetMethodName(ServiceMethodEnum.ServerVersion);
            var response = Unirest.get(method);

            return true;
        }

        /// <summary>
        /// 获取完整服务方法名称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private string GetMethodName(ServiceMethodEnum method)
        {
            var type = typeof(ServiceMethodEnum);
            var field = type.GetField(method.ToString());
            var attributes = field.GetCustomAttributes(false).Cast<ServiceMethodAttribute>().ToList();
            if (attributes == null || !attributes.Any())
            {
                throw new Exception("获取服务方法异常" + method);
            }

            var methodName = attributes[0].MethodName;
            return Path.Combine(this.Host, methodName);
        }

        private void DownloadFile(string fileName)
        {
            var urlAddress = Path.Combine(Host, "serverVersion", fileName);

            if (!Directory.Exists(ReceiveDir))
            {
                Directory.CreateDirectory(ReceiveDir);
            }

            var receivePath = Path.Combine(ReceiveDir, Path.GetFileName(urlAddress));
            new WebClient().DownloadFile(urlAddress, receivePath);
        }

        private void Archive(string fileName)
        {
            var filePath = Path.Combine(ReceiveDir, fileName);
            using (var zip = new ZipFile(filePath))
            {
                zip.ExtractAll(ReceiveDir, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        private static void ServiceTest()
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
        }
    }
}
