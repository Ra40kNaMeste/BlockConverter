using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Logger
{
    public enum LogLevels
    {
        Debug, Info, Warn, Error, Fatal
    }
    
    public interface IBlockLogger
    {
        public void Log(string message, LogLevels level, Thread thread);
        public void LogAsync(string message, LogLevels level, Thread thread);
    }

    public static class BlockLoggerManager
    {
        static BlockLoggerManager() => Loggers = new();
        private static List<IBlockLogger> Loggers { get; set; }

        public static void AddLogger(IBlockLogger logger) => Loggers.Add(logger);
        public static void RemoveLogger(IBlockLogger logger) => Loggers.Remove(logger);

        internal static void Log(string message, LogLevels level, Thread thread)
        {
            foreach (var logger in Loggers)
                logger.LogAsync(message, level, thread);
        }
        internal static async void LogAsync(string message, LogLevels level, Thread thread)
        {
            await Task.Run(() => Log(message, level, thread));
        }
    }
}
