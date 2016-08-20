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
            serviceCore.LogInfo("ES插件运行");
            serviceCore.LogInfo("追加信息");
            serviceCore.LogInfo("追加信息2");
        }
    }
}
