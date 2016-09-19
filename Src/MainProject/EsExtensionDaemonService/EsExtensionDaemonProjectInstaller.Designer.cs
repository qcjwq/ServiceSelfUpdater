namespace EsExtensionDaemonService
{
    partial class EsExtensionDaemonProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.DaemonServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // DaemonServiceProcessInstaller
            // 
            this.DaemonServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DaemonServiceProcessInstaller.Password = null;
            this.DaemonServiceProcessInstaller.Username = null;
            // 
            // serviceInstaller1
            // 
            this.serviceInstaller1.Description = "ES扩展插件守护进程，定期修改配置，保证ES插件正确运行！";
            this.serviceInstaller1.DisplayName = "EsExtensionDaemonService";
            this.serviceInstaller1.ServiceName = "EsExtensionDaemonService";
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // EsExtensionDaemonProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DaemonServiceProcessInstaller,
            this.serviceInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DaemonServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}