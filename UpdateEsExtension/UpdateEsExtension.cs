using System;
using System.Threading;
using IServiceSelfUpdater;
using ServiceProcess;

namespace UpdateEsExtension
{
    public class UpdateEsExtension : IServiceSelfUpdate
    {
        private readonly ServiceCore serviceCore;

        public UpdateEsExtension()
        {
            serviceCore = new ServiceCore();
        }

        public void Execute()
        {
            Helper.LogInfo("ES插件运行");
            Helper.LogInfo("追加信息");
            Helper.LogInfo("追加信息2");
        }
    }
}
