using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Util
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception exception);
    }



    /// <summary>
    /// 
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message)
        {
            ///HACK: На андроиде рандромно выпадает исключение при вызове <see cref="Console.WriteLine"/>
            try
            {
                Console.WriteLine(message);
            }
            catch { }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            WriteLine(message);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void Log(Exception exception)
        {
            WriteLine(exception.ToString());
        }
    }
}
