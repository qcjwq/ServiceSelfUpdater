using System;
using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ionic.Zip;
using IServiceSelfUpdater;
using ServiceSelfUpdate.Contract;
using unirest_net.http;

namespace ServiceProcess
{
    public class ServiceCore
    {
        private int Version = 0;
        private readonly string UpgradeHost = "http://localhost/ServerUpdateWebHost";

        /// <summary>
        /// 更新包文件名
        /// </summary>
        private readonly string UpgradeFileName = "upgrade.zip";

        /// <summary>
        /// 接收文件夹
        /// </summary>
        public string ReceiveDir;

        /// <summary>
        /// 接收临时文件夹
        /// </summary>
        public string ReceiveTempDir;

        /// <summary>
        /// 启动文件夹
        /// </summary>
        public string StartUpDir;

        /// <summary>
        /// 配置文件夹
        /// </summary>
        public string ConfigDir;

        /// <summary>
        /// 文件和文件夹复制事件
        /// </summary>
        private readonly EventHandler<FileSystemInfo> CopyHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceCore()
        {
            var guid = Guid.NewGuid();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.ReceiveDir = Path.Combine(baseDirectory, "Upgrade");
            this.ReceiveTempDir = Path.Combine(this.ReceiveDir, guid.ToString());
            this.StartUpDir = Path.Combine(baseDirectory, "StartUp");
            this.ConfigDir = Path.Combine(this.StartUpDir, "Config");
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="requestAction"></param>
        public void HandlerAction(Action requestAction)
        {
            try
            {
                requestAction();
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="requestAction"></param>
        /// <param name="errorAction"></param>
        public void HandlerAction(Action requestAction, Action<Exception> errorAction)
        {
            try
            {
                requestAction();
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }

        /// <summary>
        /// 处理请求，采用异步
        /// </summary>
        /// <param name="requestAction"></param>
        /// <param name="errorAction"></param>
        public async void HandlerActionAsync(Action requestAction, Action<Exception> errorAction)
        {
            try
            {
                await Task.Factory.StartNew(requestAction);
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }

        /// <summary>
        /// 获取服务器最新版本，和本地版本比较是否需要更新
        /// </summary>
        /// <returns></returns>
        public bool NeedUpgrate()
        {
            var method = Helper.GetMethodName(ServiceMethodEnum.ServerVersion);
            var response = Unirest.get(method).asString().Body;
            int i;
            if (!int.TryParse(response, out i))
            {
                return false;
            }

            var result = int.Parse(response) > Version;
            if (result)
            {
                Helper.LogInfo("有新版本文件");
            }

            return result;
        }

        /// <summary>
        /// 获取配置更新文件
        /// </summary>
        /// <returns></returns>
        public UpgradeSetting GetUpgradeSetting()
        {
            var allDlls = Directory.GetFileSystemEntries(this.ConfigDir, "*.*");
            if (!allDlls.Any())
            {
                return null;
            }

            var result = this.Invoke(allDlls[0], typeof(IServiceSelfUpdateConfig), "GetUpgradeSetting");
            if (result == null || !(result is UpgradeSetting))
            {
                return null;
            }

            return result as UpgradeSetting;
        }

        /// <summary>
        /// 清理更新文件夹
        /// </summary>
        public void CleanUpgradeDir()
        {
            if (!Directory.Exists(this.ReceiveDir))
            {
                return;
            }

            Directory.Delete(this.ReceiveDir, true);
            Helper.LogInfo(string.Format("清理{0}文件夹成功", this.GetDirName(this.ReceiveDir)));
        }

        /// <summary>
        /// 下载更新文件包
        /// </summary>
        public void DownloadFile()
        {
            var urlAddress = Path.Combine(UpgradeHost, "upgrade", UpgradeFileName);
            if (!Directory.Exists(ReceiveTempDir))
            {
                Directory.CreateDirectory(ReceiveTempDir);
            }

            var receivePath = Path.Combine(ReceiveTempDir, Path.GetFileName(urlAddress));
            new WebClient().DownloadFile(urlAddress, receivePath);

            Helper.LogInfo("下载文件完毕");
        }

        /// <summary>
        /// 解压更新包
        /// </summary>
        public void Archive()
        {
            var filePath = Path.Combine(ReceiveTempDir, UpgradeFileName);
            using (var zip = new ZipFile(filePath))
            {
                zip.ExtractAll(ReceiveTempDir, ExtractExistingFileAction.OverwriteSilently);
            }

            Helper.LogInfo("解压文件完毕");
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        public void CopyFile()
        {
            if (!Directory.Exists(this.ReceiveTempDir))
            {
                return;
            }

            if (!Directory.Exists(StartUpDir))
            {
                Helper.LogInfo("创建启动目录成功");
                Directory.CreateDirectory(StartUpDir);
            }

            this.CopyDirectoryAndFile(this.ReceiveTempDir, this.StartUpDir);
        }

        /// <summary>
        /// 复制文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>
        public void CopyDirectoryAndFile(string sourcePath, string destinationPath)
        {
            var directoryInfo = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (var fsi in directoryInfo.GetFileSystemInfos()
                .Where(a => !a.Name.EndsWith("zip") && !a.Name.EndsWith("rar") && !a.Name.EndsWith("7z")))
            {
                var destName = Path.Combine(destinationPath, fsi.Name);
                if (fsi is FileInfo) //如果是文件，复制文件
                {
                    File.Copy(fsi.FullName, destName, true);
                    CopyHandler.Invoke(this, fsi);
                }
                else //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectoryAndFile(fsi.FullName, destName);
                    CopyHandler.Invoke(this, fsi);
                }
            }
        }

        /// <summary>
        /// 删除更新文件夹
        /// </summary>
        public void DeleteUpgradeDir()
        {
            if (!Directory.Exists(this.ReceiveDir))
            {
                return;
            }

            Directory.Delete(this.ReceiveTempDir, true);
            Helper.LogInfo(string.Format("文件夹{0}删除成功", this.GetDirName(this.ReceiveTempDir)));
        }

        /// <summary>
        /// 启动子程序
        /// </summary>
        public void RunSubProcess()
        {
            var allDlls = Directory.GetFileSystemEntries(this.StartUpDir, "*.*").ToList();
            allDlls.ForEach(a => Invoke(a, typeof(IServiceSelfUpdate), "Execute"));
        }

        /// <summary>
        /// 通过反射，进行调用
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private object Invoke(string assemblyPath, Type type, string methodName)
        {
            object result = null;
            if (!assemblyPath.EndsWith(".dll"))
            {
                return result;
            }

            var subAppDomain = AppDomain.CreateDomain("SubProcess");
            var proxy = (ProxyObject)subAppDomain.CreateInstanceFromAndUnwrap(this.GetType().Module.Name, typeof(ProxyObject).FullName);
            proxy.LoadAssembly(assemblyPath, type);
            result = proxy.Invoke(methodName);
            AppDomain.Unload(subAppDomain);
            return result;
        }

        /// <summary>
        /// 文件和文件夹复制事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyFileEvent(object sender, FileSystemInfo e)
        {
            if (e is FileInfo)
            {
                Helper.LogInfo(string.Format("{0}文件复制完毕", e.Name));
            }
            else if (e is DirectoryInfo)
            {
                Helper.LogInfo(string.Format("{0}文件文件创建成功", e.Name));
            }
        }


        /// <summary>
        /// 获取文件夹名称
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private string GetDirName(string dirPath)
        {
            return new DirectoryInfo(dirPath).Name;
        }
    }
}