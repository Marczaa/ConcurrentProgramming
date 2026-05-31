using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TP.ConcurrentProgramming.Data
{
    public class Logger {
        private string? logFilePath;
        private readonly object fileLock = new object();

        public Logger()
        {
            logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "log.txt");
        }


        public async void Log(string message, DateTime? data = null)
        {

            string logEntry = $"{data ?? DateTime.Now}: {message}{Environment.NewLine}";
            

            lock (fileLock)
            {
                File.AppendAllText(logFilePath, logEntry);
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
                File.WriteAllText(logFilePath, string.Empty);
            }
        }



        public void Dispose()
        {
        }
    };

}
