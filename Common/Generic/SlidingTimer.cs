using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Generics
{
    /// <summary>
    /// Slide the timer when it is "Bumped" and then when the timer elasps call delegate
    /// </summary>
    public sealed class SlidingTimer : IDisposable, ISlidingTimer
    {
        #region Fields
        private int _numberOfTriesBeforeStopping;
        private TimeSpan _interval;
        private Timer _slidingTimer;
        private int _count;
        private Action _slidingAction;
        #endregion

        #region Properties
        private Timer InternalTimer
        {
            get
            {
                _slidingTimer = _slidingTimer ?? new Timer(TimerElasped);
                return _slidingTimer;
            }
        }
        #endregion

        #region Ctors and Dtors
        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="interval">interval to call delegate when the "slide" expires</param>
        /// <param name="numberOfTriesBeforeStopping">how many times the delegate is called before quiting</param>
        /// <param name="slidingAction">the delegate to call when the sliding elaspes</param>
        public SlidingTimer(TimeSpan interval, int numberOfTriesBeforeStopping, Action slidingAction)
        {
            _interval = interval;
            _numberOfTriesBeforeStopping = numberOfTriesBeforeStopping;
            _slidingAction = slidingAction;
        }

        /// <summary>
        /// Finalizer to insure that the sliding timer is disposed even if Dispose is not called
        /// </summary>
        ~SlidingTimer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        #endregion

        #region Publics
        /// <summary>
        /// Bump (a.k.a. slide) the delegate timer.  Resets the timer to the interval from the current time
        /// </summary>
        public void BumpTimer()
        {
            InternalTimer.Change(_interval, _interval);;
        }
        #endregion

        #region Privates
        private void TimerElasped(object stateNotUsed)
        {
            if (_count < _numberOfTriesBeforeStopping)
            {
                _slidingAction?.Invoke();
            }
            else
            {
                InternalTimer.Change(int.MaxValue, int.MaxValue);
            }
            _count++;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if(_slidingTimer != null)
                    {
                        _slidingTimer.Change(0, 0);
                        _slidingTimer.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }


        /// <summary>
        /// Dispose of this instance
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
        #endregion
    }
}
