using System;
using Microsoft.Win32;

namespace SelfUpdate.Contract
{
    [Serializable]
    public class EsExtensionInfo
    {
        public string EsExtensionPath { get; set; }

        public string EsExtensionKey { get; set; }

        public string EsExtensionValue { get; set; }
    }
}