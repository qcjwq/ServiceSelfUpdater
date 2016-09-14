using System;
using System.Collections.Generic;
using System.IO;
using SelfUpdate.Contract;
using SelfUpdate.Interface;
using SelfUpdateHelper;

namespace SelfUpdateSetting.Config.Implement
{
    [Serializable]
    public class SelfUpgradeConfig : IServiceSelfUpdateConfig
    {
        public UpgradeSetting GetUpgradeSetting()
        {
            var upgradeSetting = this.UpgradeSetting;
            Helper.LogInfo("【配置】：获取到更新配置文件");
            return upgradeSetting;
        }

        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <returns></returns>
        private UpgradeSetting UpgradeSetting
        {
            get
            {
                var guid = Guid.NewGuid();
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var receiveDir = Path.Combine(baseDirectory, "Upgrade1");
                var receiveTempDir = Path.Combine(receiveDir, guid.ToString());
                var startUpDir = Path.Combine(baseDirectory, "StartUp1");
                var configDir = Path.Combine(startUpDir, "Config1");

                return new UpgradeSetting()
                {
                    LocalVersion = 1,
                    StartLoop = 1000,
                    JavaHost = "http://10.32.151.141:8001/WebServiceTestTools4J/",
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
                        ReceiveDir = receiveDir,
                        ReceiveTempDir = receiveTempDir,
                        StartUpDir = startUpDir,
                        ConfigDir = configDir
                    }
                };
            }
        }
    }
}