using System;

namespace SelfUpdate.Contract
{
    [Serializable]
    public class DirectoryConfig
    {
        /// <summary>
        /// 更新地址
        /// </summary>
        public string UpgradeHost { get; set; }

        /// <summary>
        /// 更新包文件名
        /// </summary>
        public string UpgradeFileName { get; set; }

        /// <summary>
        /// 程序工作目录
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// 接收文件夹
        /// </summary>
        public string ReceiveDir { get; set; }

        /// <summary>
        /// 接收临时文件夹
        /// </summary>
        public string ReceiveTempDir { get; set; }

        /// <summary>
        /// 启动文件夹
        /// </summary>
        public string StartUpDir { get; set; }

        /// <summary>
        /// 配置文件夹
        /// </summary>
        public string ConfigDir { get; set; }
    }
}