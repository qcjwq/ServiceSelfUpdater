using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using IServiceSelfUpdater;
using unirest_net.http;

namespace ServiceProcess
{
    public class ServiceCore
    {
        private int localVersion = 0;
        private readonly string Host = "http://localhost:8001/WebServiceTestTools4J/";
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
        /// 构造函数
        /// </summary>
        public ServiceCore()
        {
            var guid = Guid.NewGuid();
            this.ReceiveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upgrade");
            this.ReceiveTempDir = Path.Combine(this.ReceiveDir, guid.ToString());
            this.StartUpDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StartUp");
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
                this.LogError(ex.Message);
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
            var method = GetMethodName(ServiceMethodEnum.ServerVersion);
            var response = Unirest.get(method).asString().Body;
            var result = Convert.ToInt32(response) > localVersion;
            if (result)
            {
                this.LogInfo("有新版本文件");
            }

            return result;
        }

        /// <summary>
        /// 获取完整服务方法名称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GetMethodName(ServiceMethodEnum method)
        {
            var type = typeof(ServiceMethodEnum);
            var field = type.GetField(method.ToString());
            var attributes = field.GetCustomAttributes(false).Cast<ServiceMethodAttribute>().ToList();
            if (attributes == null || !attributes.Any())
            {
                throw new Exception("获取服务方法名称异常" + method);
            }

            var methodName = attributes[0].MethodName;
            return Path.Combine(this.Host, "api", methodName);
        }

        /// <summary>
        /// 清理更新文件夹
        /// </summary>
        public void CleanUpgradeDir()
        {
            Directory.Delete(this.ReceiveDir, true);
            this.LogInfo(string.Format("清理{0}文件夹成功", this.GetDirName(this.ReceiveDir)));
        }

        /// <summary>
        /// 下载更新文件包
        /// </summary>
        public void DownloadFile()
        {
            var urlAddress = Path.Combine(this.UpgradeHost, "upgrade", this.UpgradeFileName);
            if (!Directory.Exists(ReceiveTempDir))
            {
                Directory.CreateDirectory(ReceiveTempDir);
            }

            var receivePath = Path.Combine(ReceiveTempDir, Path.GetFileName(urlAddress));
            new WebClient().DownloadFile(urlAddress, receivePath);

            this.LogInfo("下载文件完毕");
        }

        /// <summary>
        /// 解压更新包
        /// </summary>
        public void Archive()
        {
            var filePath = Path.Combine(this.ReceiveTempDir, this.UpgradeFileName);
            using (var zip = new ZipFile(filePath))
            {
                zip.ExtractAll(this.ReceiveTempDir, ExtractExistingFileAction.OverwriteSilently);
            }

            this.LogInfo("解压文件完毕");
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        public void CopyFile()
        {
            var sourceFiles = Directory.GetFileSystemEntries(this.ReceiveTempDir, "*.*")
                .Where(a => !a.EndsWith("zip") && !a.EndsWith("rar") && !a.EndsWith("7z")).ToList();
            if (!Directory.Exists(this.StartUpDir))
            {
                this.LogInfo("创建启动目录成功");
                Directory.CreateDirectory(this.StartUpDir);
            }

            foreach (var sourceFileName in sourceFiles)
            {
                var fileName = new FileInfo(sourceFileName).Name;
                var destFileName = Path.Combine(this.StartUpDir, fileName);

                this.HandlerAction(() =>
                {
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                        this.LogInfo(string.Format("{0}文件删除完毕", fileName));
                    }

                    File.Copy(sourceFileName, destFileName);
                    this.LogInfo(string.Format("{0}文件复制完毕", fileName));
                });
            }
        }

        /// <summary>
        /// 删除更新文件夹
        /// </summary>
        public void DeleteUpgradeDir()
        {
            Directory.Delete(this.ReceiveTempDir, true);
            this.LogInfo(string.Format("文件夹{0}删除成功", this.GetDirName(this.ReceiveTempDir)));
        }

        /// <summary>
        /// 启动子程序
        /// </summary>
        public void RunSubProcess()
        {
            var allDlls = Directory.GetFileSystemEntries(this.StartUpDir, "*.*").ToList();
            allDlls.ForEach(this.Invoke);
        }

        private void Invoke(string assemblyPath)
        {
            var subAppDomain = AppDomain.CreateDomain("SubProcess");
            var proxy = (ProxyObject)subAppDomain.CreateInstanceFromAndUnwrap(this.GetType().Module.Name, typeof(ProxyObject).FullName);
            proxy.LoadAssembly(assemblyPath);
            proxy.Invoke();
            AppDomain.Unload(subAppDomain);
        }

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="info"></param>
        public void LogInfo(string info)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture).PadRight(10, ' '), info);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="errorMessage"></param>
        public void LogError(string errorMessage)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture).PadRight(10, ' '), errorMessage);
        }

        /// <summary>
        /// 打印新行
        /// </summary>
        public void NewLine()
        {
            Console.Write(Environment.NewLine);
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