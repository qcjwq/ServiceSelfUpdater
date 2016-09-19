using System;
using System.Linq;
using System.Reflection;
using SelfUpdate.Contract;

namespace ServiceProcess
{
    [Serializable]
    public class ProxyObject : MarshalByRefObject
    {
        private Assembly assembly;

        public void LoadAssembly(string path, Type type)
        {
            assembly = Assembly.LoadFile(path);
            if (!assembly.GetTypes().Any(t => t.GetInterfaces().Contains(type)))
            {
                assembly = null;
            }
        }

        public object Invoke(string methodName, object[] parameters)
        {
            if (assembly == null)
            {
                return null;
            }

            var type = assembly.GetTypes()[0];
            var method = type.GetMethod(methodName);
            var obj = Activator.CreateInstance(type);
            return method.Invoke(obj, parameters);
        }
    }
}