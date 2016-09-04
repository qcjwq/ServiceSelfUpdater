using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using SelfUpdate.Contract;
using SelfUpdateHelper;

namespace ServiceProcess
{
    public class ServiceRunner
    {
        private static bool serviceStarted;
        private static bool readyToStop = true;
        private ServiceCore serviceCore;

        /// <summary>
        /// 当前配置
        /// </summary>
        private UpgradeSetting upgradeSetting;

        /// <summary>
        /// 默认配置
        /// </summary>
        private readonly UpgradeSetting defaultUpgradeSetting;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceRunner()
        {
            this.defaultUpgradeSetting = new UpgradeSetting()
            {
                LocalVersion = 0,
                StartLoop = 500
            };
            this.upgradeSetting = this.defaultUpgradeSetting;
        }

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
            var log = LogManager.GetLogger("mylog");
            log.Info("system start");
            log.Info("hello world");

            return;

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

                //Helper.HandlerAction(this.SetUpgradeSetting, this.LogAction);
                //Helper.HandlerActionAsync(this.SubProcessUpgrade, this.LogAction);
                Helper.NewLine();

                Helper.LogInfo(string.Format("当前版本：{0}，服务器版本：{1}，轮询周期：{2}毫秒", upgradeSetting.LocalVersion, upgradeSetting.ServiceVersion, upgradeSetting.StartLoop));
                readyToStop = true;
                Thread.Sleep(upgradeSetting.StartLoop);
            }
        }

        /// <summary>
        /// 设置更新配置
        /// </summary>
        private void SetUpgradeSetting()
        {
            var config = serviceCore.GetUpgradeSetting();
            if (config == null)
            {
                this.upgradeSetting = this.defaultUpgradeSetting;
                return;
            }

            this.upgradeSetting = config;
        }

        /// <summary>
        /// 子程序更新
        /// </summary>
        private void SubProcessUpgrade()
        {
            var needUpgrade = Helper.HandlerAction(serviceCore.NeedUpgrate, upgradeSetting);
            if (needUpgrade)
            {
                Helper.HandlerAction(() =>
                {
                    serviceCore.CleanUpgradeDir();
                    serviceCore.DownloadFile();
                    serviceCore.Archive();
                    serviceCore.CopyFile();
                    serviceCore.DeleteUpgradeDir();
                });
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
            sb.Append(string.Format("Message：{0}", ex));
            if (ex.InnerException != null)
            {
                sb.Append(string.Format("，InnerException：{0}", ex.InnerException.Message));
            }

            Helper.LogError(sb.ToString());
        }
    }
}
