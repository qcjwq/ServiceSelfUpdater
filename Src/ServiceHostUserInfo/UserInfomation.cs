using SelfUpdate.Interface;
using SelfUpdateHelper;
using ServiceProcess;

namespace CustomerInfo.Plugin
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
            Helper.LogInfo("获取用户信息");
        }
    }
}
