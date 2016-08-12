using System;
using System.Linq;
using System.Reflection;
using IServiceSelfUpdater;

namespace ServiceProcess
{
    public class ProxyObject : MarshalByRefObject
    {
        private Assembly assembly;

        public void LoadAssembly(string path)
        {
            assembly = Assembly.LoadFile(path);
            if (!assembly.GetTypes().Any(t => t.GetInterfaces().Contains(typeof(IServiceSelfUpdate))))
            {
                assembly = null;
            }
        }

        public object Invoke()
        {
            if (assembly == null)
            {
                return null;
            }

            var type = assembly.GetTypes()[0];
            var method = type.GetMethod("Execute");
            var obj = Activator.CreateInstance(type);
            return method.Invoke(obj, new object[] { });
        }
    }
}