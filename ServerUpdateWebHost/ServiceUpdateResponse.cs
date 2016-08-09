namespace ServerUpdateWebHost
{
    public class ServiceUpdateResponse
    {
        /// <summary>
        /// 是否需要更新
        /// </summary>
        public bool NeedUpgrade { get; set; }

        /// <summary>
        /// 更新文件流
        /// </summary>
        public byte[] FileBytes { get; set; }
    }
}