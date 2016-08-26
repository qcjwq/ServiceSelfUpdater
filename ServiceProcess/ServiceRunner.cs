using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ServiceProcess
{
    public class ServiceRunner
    {
        private static bool serviceStarted;
        private static bool readyToStop = true;

        private ServiceCore serviceCore;

        public bool IsReadyToExit
        {
            get
            {
                return !serviceStarted && readyToStop;
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void StartService()
        {
            serviceStarted = true;
            StartServiceThread();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopService()
        {
            serviceStarted = false;
            while (!IsReadyToExit)
            {
                Thread.Sleep(1000);
            }
        }

        public void StartServiceThread()
        {
            while (serviceStarted)
            {
                readyToStop = false;
                serviceCore = new ServiceCore();

                serviceCore.HandlerAction(serviceCore.Test);
                //serviceCore.HandlerActionAsync(SubProcessUpgrade, LogAction);
                serviceCore.NewLine();

                readyToStop = true;
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 子程序更新
        /// </summary>
        private void SubProcessUpgrade()
        {
            var needUpgrade = serviceCore.NeedUpgrate();

            if (needUpgrade)
            {
                serviceCore.CleanUpgradeDir();
                serviceCore.DownloadFile();
                serviceCore.Archive();
                serviceCore.CopyFile();
                serviceCore.DeleteUpgradeDir();
            }

            serviceCore.RunSubProcess();
        }

        /// <summary>
        /// 记录Log方法
        /// </summary>
        /// <param name="ex"></param>
        private void LogAction(Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("Message：{0}", ex.Message));
            if (ex.InnerException != null)
            {
                sb.Append(string.Format("，InnerException：{0}", ex.InnerException.Message));
            }

            serviceCore.LogError(sb.ToString());
        }
    }
}
