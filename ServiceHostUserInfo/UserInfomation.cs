using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IServiceSelfUpdater;
using ServiceProcess;

namespace ServiceHostUserInfo
{
    public class UserInfomation : IServiceSelfUpdate
    {
        private readonly ServiceCore serviceCore;

        public UserInfomation()
        {
            serviceCore = new ServiceCore();
        }

        public void Execute()
        {
            serviceCore.LogInfo("获取用户信息");
        }
    }
}
