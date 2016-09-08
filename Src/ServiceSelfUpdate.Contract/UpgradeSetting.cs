using System;

namespace SelfUpdate.Contract
{
    [Serializable]
    public class UpgradeSetting
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public int LocalVersion { get; set; }

        /// <summary>
        /// 服务器版本
        /// </summary>
        public int ServiceVersion { get; set; }

        /// <summary>
        /// 循环毫秒数
        /// </summary>
        public int StartLoop { get; set; }

        /// <summary>
        /// Java Host地址
        /// </summary>
        public string JavaHost { get; set; }
    }
}