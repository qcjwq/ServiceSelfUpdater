using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using ServiceProcess;

namespace MyWindowsService
{
    public partial class MyWindowsService : ServiceBase
    {
        public MyWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServiceRunner.StartService();
        }

        protected override void OnStop()
        {
            ServiceRunner.StopService();
        }
    }
}
