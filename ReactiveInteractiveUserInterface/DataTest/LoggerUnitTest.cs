using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class LoggerUnitTest
    {
        [TestMethod]
        public void LoggerWriteTestMethod()
        {
            Logger newInstance = new Logger();

            newInstance.ClearLog();

            DateTime date = DateTime.Now;

            newInstance.Log("Test log message", date);

            string logEntry = $"{date}: Test log message";

            string logFilePath = newInstance.GetLogFilePath();

            Assert.IsNotNull(logFilePath);

            newInstance.writeFile(null, null);

            string lastLine = File.ReadLines(logFilePath).LastOrDefault();

            Assert.AreEqual(logEntry, lastLine);

        }
    }
}