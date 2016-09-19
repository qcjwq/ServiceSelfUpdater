using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using SelfUpdate.Contract;
using SelfUpdateHelper;

namespace ServiceProcess
{
    public class ServiceRunner
    {
        private volatile bool serviceStarted;
        private volatile bool readyToStop = true;
        private ServiceCore serviceCore;

        /// <summary>
        /// 当前配置
        /// </summary>
        private UpgradeSetting upgradeSetting;

        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <returns></returns>
        private UpgradeSetting DefaultUpgradeSetting
        {
            get
            {
                var guid = Guid.NewGuid();
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var receiveDir = Path.Combine(baseDirectory, "Upgrade");
                var receiveTempDir = Path.Combine(receiveDir, guid.ToString());
                var startUpDir = Path.Combine(baseDirectory, "StartUp");
                var configDir = Path.Combine(startUpDir, "Config");

                return new UpgradeSetting()
                {
                    LocalVersion = 0,
                    StartLoop = 6 * 60 * 60,
                    JavaHost = "http://10.2.36.171:8001/",
                    EsExtension = new List<EsExtensionInfo>
                    {
                        new EsExtensionInfo()
                        {
                            EsExtensionPath = @"SOFTWARE\Policies\Google\Chrome\ExtensionInstallForcelist",
                            EsExtensionKey = "1",
                            EsExtensionValue = "iphnoceklekkgpafdggfgodicabghdje;http://10.2.9.80/ChromCrx/ESExtension.xml"
                        }
                    },
                    DirConfig = new DirectoryConfig()
                    {
                        UpgradeHost = "http://10.2.9.80/ChromCrx",
                        UpgradeFileName = "upgrade.zip",
                        BaseDirectory = baseDirectory,
                        ReceiveDir = receiveDir,
                        ReceiveTempDir = receiveTempDir,
                        StartUpDir = startUpDir,
                        ConfigDir = configDir
                    }
                };
            }
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
            serviceStarted = true;
            Helper.HandlerActionAsync(StartServiceThread, this.LogAction);
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

                Helper.HandlerAction(this.SetUpgradeSetting, this.LogAction);

#if DEBUG
                Helper.HandlerAction(this.SubProcessUpgrade, this.LogAction);
#else
                Helper.HandlerActionAsync(this.SubProcessUpgrade, this.LogAction);
#endif

                Helper.LogInfo(Environment.NewLine);
                readyToStop = true;
                Thread.Sleep(upgradeSetting.StartLoop);
            }
        }

        /// <summary>
        /// 设置更新配置
        /// </summary>
        private void SetUpgradeSetting()
        {
            var directoryConfig = this.DefaultUpgradeSetting.DirConfig;
            if (this.upgradeSetting != null && this.upgradeSetting.DirConfig != null)
            {
                directoryConfig = this.upgradeSetting.DirConfig;
            }

            var config = serviceCore.GetUpgradeSetting(directoryConfig);
            if (config == null)
            {
                this.upgradeSetting = this.DefaultUpgradeSetting;
                return;
            }

            this.upgradeSetting = config;
        }

        /// <summary>
        /// 子程序更新
        /// </summary>
        private void SubProcessUpgrade()
        {
            var needUpgrade = Helper.HandlerAction(serviceCore.NeedUpgrate, this.upgradeSetting);
            Helper.LogInfo(string.Format("【环境】：当前版本：{0}，服务器版本：{1}，轮询周期：{2}毫秒",
                this.upgradeSetting.LocalVersion, this.upgradeSetting.ServiceVersion, this.upgradeSetting.StartLoop));
            if (needUpgrade)
            {
                Helper.HandlerAction(() =>
                {
                    serviceCore.CleanUpgradeDir(upgradeSetting);
                    serviceCore.DownloadFile(upgradeSetting);
                    serviceCore.Archive(upgradeSetting);
                    serviceCore.CopyFile(upgradeSetting);
                    serviceCore.DeleteUpgradeDir(upgradeSetting);
                });
            }

            serviceCore.RunSubProcess(upgradeSetting);
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
