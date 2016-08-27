using System;
using IServiceSelfUpdater;
using ServiceSelfUpdate.Contract;

namespace UpgradeConfig.Config.Implement
{
    [Serializable]
    public class SettingManager : IServiceSelfUpdateConfig
    {
        public UpgradeSetting GetUpgradeSetting()
        {
            var upgradeSetting = new UpgradeSetting
            {
                LocalVersion = 1,
                StartLoop = 3000
            };
            return upgradeSetting;
        }
    }
}