using System;
using SelfUpdate.Contract;
using SelfUpdate.Interface;

namespace SelfUpdateSetting.Config.Implement
{
    [Serializable]
    public class SelfUpgradeConfig : IServiceSelfUpdateConfig
    {
        public UpgradeSetting GetUpgradeSetting()
        {
            var upgradeSetting = new UpgradeSetting
            {
                LocalVersion = 1,
                StartLoop = 30000
            };
            return upgradeSetting;
        }
    }
}