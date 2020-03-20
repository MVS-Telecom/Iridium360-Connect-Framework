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

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            ///HACK: На андроиде рандромно проявыпадает исключение при вызове <see cref="Console.WriteLine"/>
            try
            {
                Console.WriteLine(message);
            }
            catch { }
        }

        public void Log(Exception exception)
        {
            Log(exception.ToString());
        }
    }
}
