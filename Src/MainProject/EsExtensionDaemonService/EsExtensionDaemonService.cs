using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EsExtensionDaemonService
{
    partial class EsExtensionDaemonService : ServiceBase
    {
        public EsExtensionDaemonService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Program.Start(args);
        }

        protected override void OnStop()
        {
            Program.Stop();
        }
    }
}
