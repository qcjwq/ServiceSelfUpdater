using System.Linq;
using Microsoft.Win32;
using SelfUpdate.Contract;
using SelfUpdate.Interface;
using SelfUpdateHelper;
using ServiceProcessCore;

namespace EsExtension.Plugin
{
    public class UpdateEsExtension : IServiceSelfUpdate
    {
        private static RegistryKey RegistryKey;

        public UpdateEsExtension()
        {

        }

        public void Execute(UpgradeSetting upgradeSetting)
        {
            if (upgradeSetting == null || upgradeSetting.EsExtension == null || !upgradeSetting.EsExtension.Any())
            {
                return;
            }

            Logger.Singleton.Info("【子程序】：子程序ES插件更新开始执行");
            upgradeSetting.EsExtension.ForEach(a =>
            {
                string path = a.EsExtensionPath;
                string key = a.EsExtensionKey;
                string value = a.EsExtensionValue;

                RegistryKey = Registry.CurrentUser;
                if (!Exist(RegistryKey, path, key, value))
                {
                    Create(RegistryKey, path, key, value);
                }

                RegistryKey = Registry.LocalMachine;
                if (!Exist(RegistryKey, path, key, value))
                {
                    Create(RegistryKey, path, key, value);
                }
            });
            Logger.Singleton.Info("【子程序】：子程序ES插件更新运行完毕");
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <returns></returns>
        private bool Exist(RegistryKey registryKey, string path, string key, string value)
        {
            using (var registry = registryKey.OpenSubKey(path, true))
            {
                if (registry == null || registry.GetValueNames().Length <= 0)
                {
                    return false;
                }

                var kv = registry.GetValueNames();
                if (kv.All(a => !a.Equals(key)))
                {
                    return false;
                }

                var result = registry.GetValue(key);
                return value.Equals(result);
            }
        }

        /// <summary>
        /// 创建插件记录
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        /// <param name="path">The path.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void Create(RegistryKey registryKey, string path, string key, string value)
        {
            using (RegistryKey registry = registryKey.CreateSubKey(path))
            {
                if (registry != null)
                {
                    registry.SetValue(key, value);
                    registry.SetValue(key, value);
                    Helper.LogInfo(string.Format("【更新】：ES插件插入一条记录，path：{0}，key：{1},value:{2}", path, key, value));
                }
            }
        }
    }
}
