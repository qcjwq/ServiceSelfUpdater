﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Threading.Tasks;

namespace EsExtensionDaemonService
{
    [RunInstaller(true)]
    public partial class EsExtensionDaemonProjectInstaller : Installer
    {
        public EsExtensionDaemonProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
