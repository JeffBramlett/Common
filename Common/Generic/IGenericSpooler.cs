using System;

namespace Common.Generic
{
    /// <summary>
    /// Contract for a spooler, if you want to make your own.
    /// Author: Jeff Bramlett (jeffrey.bramlett@gmail.com)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericSpooler<T> : IDisposable
    {
        /// <summary>
        /// Add Item to the spool
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemCausesStop">if true cause the spooler to pause at this item, it must be resumed after this</param>
        void AddItem(T item, bool itemCausesStop = false);

        /// <summary>
        /// Stop spooling for now
        /// </summary>
        void Stop();

        /// <summary>
        /// Resume spooling after being stopped
        /// </summary>
        void Resume();

        /// <summary>
        /// Some exception happened in the spool, either in the spooler or in the message handler
        /// </summary>
        event GenericSpooler<T>.ExceptionEncounteredDelegate ExceptionEncountered;
    }
}
