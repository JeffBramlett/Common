using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    public class LogData
    {
        public DateTimeOffset Timestamp { get; set; }

        public string CodeFile { get; set; }

        public string MemberName { get; set; }

        public int LineNumber { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public Type TypeForLogging { get; set; }

        public LogLevels LogLevel { get; set; }
    }

}