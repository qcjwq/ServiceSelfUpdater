using System.Configuration;
using IServiceSelfUpdater;
using ServiceSelfUpdate.Contract;

namespace UpgradeConfig.Config.Implement
{
    public class SettingManager : IServiceSelfUpdateConfig
    {
        public UpgradeSetting GetUpgradeSetting()
        {
            var upgradeSetting = new UpgradeSetting
            {
                Version = 0,
                StartLoop = 3000
            };
            return upgradeSetting;
        }
    }
}