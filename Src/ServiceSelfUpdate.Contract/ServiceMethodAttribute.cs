using System;

namespace SelfUpdate.Contract
{
    /// <summary>
    /// 服务方法自定义特性
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field)]
    public class ServiceMethodAttribute : Attribute
    {
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
    }
}