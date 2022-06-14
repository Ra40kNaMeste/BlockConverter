using ModelConverterToBlock.Logger;
using System;
using System.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockConverter.Tools
{
    delegate void WriteBlockLoggerHandler(string message);
    class BlockMemoryLogger : IBlockLogger
    {
        public BlockMemoryLogger()
        {
            CleanAllMessages();
        }
        private void CleanAllMessages()
        {
            MemoryMessages = new();
            MemoryNumberId = new();
        }
        public LogLevels Level { get; set; }
        private object memoryMessagesLocker = new();
        private Dictionary<int, List<string>> MemoryMessages { get; set; }
        private object memoryNumberIdLocker = new();
        private List<string> MemoryNumberId { get; set; }

        public async void LogAsync(string message, LogLevels level, Thread thread)
        {
            await Task.Run(() => Log(message, level, thread));
        }
        public void Log(string message, LogLevels level, Thread thread)
        {
            if (level < Level)
                return;
            string nameThread = thread.Name;

            lock (memoryNumberIdLocker)
                if (!MemoryNumberId.Contains(nameThread))
                    MemoryNumberId.Add(nameThread);
            int id = MemoryNumberId.IndexOf(nameThread);

            lock (memoryMessagesLocker)
            {
                if (!MemoryMessages.ContainsKey(id))
                    MemoryMessages.Add(id, new());
                MemoryMessages[id].Add(message);
                if (level >= LogLevels.Warn)
                    WriteOneBlockMessages(id);
            }
        }
        private void WriteOneBlockMessages(int id)
        {
            var messages = MemoryMessages[id];
            MemoryMessages.Remove(id);
            lock (memoryNumberIdLocker)
                MemoryNumberId.RemoveAt(id);
            StringBuilder res = new(Resources.Resources.LoggingThreadName + " " + id.ToString() + "\n");
            foreach (var message in messages)
                res.Append(message + "\n");
            handler?.Invoke(res.ToString());
        }
        public void WriteMessagesByThread(Thread thread)
        {
            string name = thread.Name;

            if (!MemoryNumberId.Contains(name))
                return;

            int id = MemoryNumberId.IndexOf(name);
            lock (memoryMessagesLocker)
                WriteOneBlockMessages(id);
        }

        public void WriteAllMessages()
        {
            lock (memoryMessagesLocker)
            {
                int ids = MemoryNumberId.Count;
                for (int id = 0; id < ids; id++)
                    WriteOneBlockMessages(id);
                CleanAllMessages();
            }
        }

        private event WriteBlockLoggerHandler handler;
        public event WriteBlockLoggerHandler Handler
        {
            add { handler += value; }
            remove { handler -= value; }
        }
    }

    class BlockLogger : IBlockLogger
    {
        public LogLevels Level { get; set; }
        private event WriteBlockLoggerHandler handler;
        public event WriteBlockLoggerHandler Handler
        {
            add { handler += value; }
            remove { handler -= value; }
        }
        public void Log(string message, LogLevels level, Thread thread)
        {
            if (level >= Level)
                handler?.Invoke(message + "\n");
        }

        public async void LogAsync(string message, LogLevels level, Thread thread)
        {
            await Task.Run(() => Log(message, level, thread));
        }
    }
}
