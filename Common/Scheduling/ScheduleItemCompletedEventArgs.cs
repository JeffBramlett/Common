using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Scheduling
{
    /// <summary>
    /// Event args for ScheduleItem completion
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScheduleItemCompletedEventArgs<T>: EventArgs
    {
        /// <summary>
        /// When did this time get reached?
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The ScheduleItem that completed
        /// </summary>
        public IScheduleItem<T> ScheduleItem { get; set; }

        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="scheduledItem"></param>
        public ScheduleItemCompletedEventArgs(IScheduleItem<T> scheduledItem)
        {
            ScheduleItem = scheduledItem;
            Timestamp = DateTimeOffset.Now;
        }
    }
}
