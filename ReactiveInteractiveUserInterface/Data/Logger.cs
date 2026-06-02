using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TP.ConcurrentProgramming.Data
{
    public class Logger {
        private string? logFilePath;
        private readonly object fileLock = new object();
        private System.Timers.Timer writeTimer;
        private int writeInterval = 1000;

        private ConcurrentQueue<string> writeQueue = new ConcurrentQueue<string>();

        public Logger()
        {
            logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "log.txt");

            writeTimer = new System.Timers.Timer(writeInterval);
            writeTimer.Elapsed += writeFile;
            writeTimer.AutoReset = true;
            writeTimer.Enabled = true;
        }

        public int GetWriteInterval()
        {
            return writeInterval;
        }


        public async void Log(string message, DateTime? data = null)
        {
            string logEntry = $"{data ?? DateTime.Now}: {message}{Environment.NewLine}";
            writeQueue.Enqueue(logEntry);
        }

        private async void writeFile(Object source, ElapsedEventArgs e)
        {
            lock (fileLock)
            {
                while (writeQueue.TryDequeue(out string? entry))
                {
                    File.AppendAllText(logFilePath, entry);
                }
            }
        }


        public string GetLogFilePath()
        {
            return logFilePath ?? "Log file path is not set.";
        }

        public void ClearLog()
        {
            lock (fileLock)
            {
                writeQueue.Clear();
                File.WriteAllText(logFilePath, string.Empty);
            }
        }

        public void Dispose()
        {
            writeTimer?.Dispose();
            Log("Logger disposed");
            writeFile(null, null);

        }
    };

}
