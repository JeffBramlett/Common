﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Scheduling
{
    /// <summary>
    /// Data class for a scheduled item
    /// </summary>
    /// <typeparam name="T">the type of the scheduled item</typeparam>
    public class ScheduleItem<T> : IScheduleItem<T>
    {
        /// <summary>
        /// Assigned id of the schedule item
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The scheduled item 
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// Datetime when the item will be raised again
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Count of how many time this schedule item has been raised
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The interval to raise the item
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// The DateTime when the scheduled item is to be no longer raised
        /// </summary>
        public DateTime EndDateTime { get; set; }

        #region Comparison Implementation
        /// <summary>
        /// Compare two Schedule items
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>int of the comparison</returns>
        public int Compare(IScheduleItem<T> x, IScheduleItem<T> y)
        {
            return x.CompareTo(y);
        }

        /// <summary>
        /// Compare this to another Scheduled item
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IScheduleItem<T> other)
        {
            return Timestamp.CompareTo(other.Timestamp);
        }
        #endregion
    }
}
