using System;

namespace Common.Generics
{
    /// <summary>
    /// Public contract for the SlidingTimer
    /// </summary>
    public interface ISlidingTimer: IDisposable
    {
        /// <summary>
        /// Bump the timer to "slide" the dlegate execution
        /// </summary>
        void BumpTimer();
    }
}