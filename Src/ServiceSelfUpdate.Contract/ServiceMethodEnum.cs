using System;

namespace SelfUpdate.Contract
{
    [Serializable]
    public enum ServiceMethodEnum
    {
        /// <summary>
        /// 获取服务器当前版本
        /// </summary>
        [ServiceMethod(MethodName = "serverVersion")]
        ServerVersion
    }
}