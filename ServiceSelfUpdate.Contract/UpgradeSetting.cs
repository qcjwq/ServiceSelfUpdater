using System;

namespace ServiceSelfUpdate.Contract
{
    [Serializable]
    public class UpgradeSetting
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 循环毫秒数
        /// </summary>
        public int Loop { get; set; }
    }
}