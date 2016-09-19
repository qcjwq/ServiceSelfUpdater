using System;
using SelfUpdate.Contract;
using SelfUpdate.Interface;
using ServiceProcessCore;

namespace CustomerInfo.Plugin
{
    public class UserInfomation : IServiceSelfUpdate
    {
        private UpgradeSetting upgradeSetting;

        public UserInfomation()
        {
        }

        public void Execute(UpgradeSetting upgradeSetting)
        {
            
        }
    }
}
