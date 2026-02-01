using System;
using System.IO;

namespace receive_function_keys
{
    public static class Logger
    {
        private const string LogFileName = "debug.log";
        private static object _lock = new object();

        public static void Log(string message, bool enabled)
        {
            if (!enabled) return;

            try
            {
                lock (_lock)
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                    File.AppendAllText(path, logEntry);
                }
            }
            catch
            {
                // Optionally handle logging failure, but avoid crashing the app
            }
        }
    }
}
