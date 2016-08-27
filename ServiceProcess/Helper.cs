using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zip;

namespace ServiceProcess
{
    public class Helper
    {
        private static readonly string Host = "http://localhost:8001/WebServiceTestTools4J/";

        /// <summary>
        /// 获取完整服务方法名称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetMethodName(ServiceMethodEnum method)
        {
            var type = typeof(ServiceMethodEnum);
            var field = type.GetField(method.ToString());
            var attributes = field.GetCustomAttributes(false).Cast<ServiceMethodAttribute>().ToList();
            if (attributes == null || !attributes.Any())
            {
                throw new Exception("获取服务方法名称异常" + method);
            }

            var methodName = attributes[0].MethodName;
            return Path.Combine(Host, "api", methodName);
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="requestAction"></param>
        public static void HandlerAction(Action requestAction)
        {
            try
            {
                requestAction();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HandlerAction<T>(Func<T, bool> func, T t)
        {
            try
            {
                return func(t);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="requestAction"></param>
        /// <param name="errorAction"></param>
        public static void HandlerAction(Action requestAction, Action<Exception> errorAction)
        {
            try
            {
                requestAction();
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }

        /// <summary>
        /// 处理请求，采用异步
        /// </summary>
        /// <param name="requestAction"></param>
        /// <param name="errorAction"></param>
        public static async void HandlerActionAsync(Action requestAction, Action<Exception> errorAction)
        {
            try
            {
                await Task.Factory.StartNew(requestAction);
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }


        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="info"></param>
        public static void LogInfo(string info)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadRight(10, ' '), info);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void LogError(string errorMessage)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadRight(10, ' '), errorMessage);
        }

        /// <summary>
        /// 打印新行
        /// </summary>
        public static void NewLine()
        {
            Console.Write(Environment.NewLine);
        }
    }
}