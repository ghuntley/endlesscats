using System.Diagnostics;
using Splat;

namespace EndlessCatsApp.Services.Droid.Logging
{
    public class LoggingService : ILogger
    {
        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level)
            {
                return;
            }

            Debug.WriteLine(message);
        }

        public LogLevel Level { get; set; }
    }
}
