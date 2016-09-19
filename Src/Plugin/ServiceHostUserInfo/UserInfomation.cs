using System;
using SelfUpdate.Contract;
using SelfUpdate.Interface;
using ServiceProcessCore;

namespace CustomerInfo.Plugin
{
    public class UserInfomation : IServiceSelfUpdate
    {
        private readonly ServiceCore serviceCore;
        private UpgradeSetting upgradeSetting;

        public UserInfomation()
        {
            serviceCore = new ServiceCore();
        }

        public void Execute(UpgradeSetting upgradeSetting)
        {
            
        }
    }
}
