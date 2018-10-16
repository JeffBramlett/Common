using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Scheduling
{
    /// <summary>
    /// Event args for scheduler ending
    /// </summary>
    public class SchedulerEndedEventArgs: EventArgs
    {
        /// <summary>
        /// When did this time get reached?
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        public SchedulerEndedEventArgs()
        {
            Timestamp = DateTimeOffset.Now;
        }
    }
}
