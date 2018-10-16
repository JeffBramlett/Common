using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Common.Generic
{
    /// <summary>
    /// Type to Delegate Message Bus implementation
    /// </summary>
    /// <remarks>
    /// "When you use a message bus, an application that sends a message no 
    /// longer has individual connections to all the applications that must 
    /// receive the message. Instead, the application merely passes the message
    /// to the message bus, and the message bus transports the message to all 
    /// the other applications that are listening for bus messages through a 
    /// shared infrastructure. Likewise, an application that receives a message 
    /// no longer obtains it directly from the sender. Instead, it takes the 
    /// message from the message bus. In effect, the message bus reduces the 
    /// fan-out of each application from many to one."
    /// From: https://msdn.microsoft.com/en-us/library/ff647328.aspx 
    /// And
    /// "Extend the communication infrastructure by creating topics or by 
    /// dynamically inspecting message content. Enable listening applications 
    /// to subscribe to specific messages. Create a mechanism that sends 
    /// messages to all interested subscribers"
    /// From: https://msdn.microsoft.com/en-us/library/ff649664.aspx 
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class DelegateMessageBus: IDisposable, IDelegateMessageBus
    {
        #region Fields
        bool _isDisposed;
        ConcurrentDictionary<Type, List<Func<object, bool>>> _registrations;
        #endregion

        #region Properties
        /// <summary>
        /// Registration Dictionary for collecting delegates by type
        /// </summary>
        public ConcurrentDictionary<Type, List<Func<object, bool>>> Registrations
        {
            get
            {
                _registrations = _registrations ?? new ConcurrentDictionary<Type, List<Func<object, bool>>>();
                return _registrations;
            }
            set { _registrations = value; }
        }
        #endregion

        #region Ctors and Dtors

        /// <summary>
        /// Destructor to insure release of resources
        /// </summary>
        ~DelegateMessageBus()
        {
            Close(false);
        }
        #endregion

        #region Registration
        /// <summary>
        /// Register a delegate by a type
        /// </summary>
        /// <param name="key">the type for registration</param>
        /// <param name="funcDelegate">the delegate for registration</param>
        public void Register(Type key, Func<object, bool> funcDelegate)
        {
            if (Registrations.ContainsKey(key))
            {
                Registrations[key].Add(funcDelegate);
            }
            else
            {
                List<Func<object, bool>> funcList = new List<Func<object, bool>> { funcDelegate };
                Registrations.TryAdd(key, funcList);
            }
        }

        /// <summary>
        /// Remove the registration type (and delegates)
        /// </summary>
        /// <param name="typ">the type as a key</param>
        public void DropRegistrations(Type typ)
        {
            if (Registrations.ContainsKey(typ))
            {
                Registrations[typ].Clear();
            }
        }
        #endregion

        #region Publishing
        /// <summary>
        /// Publish the object to all/any subscribing delegates
        /// </summary>
        /// <param name="objToPublish">the object to publish</param>
        public void Publish(object objToPublish)
        {
            ThreadPool.QueueUserWorkItem(PublishObjectInThread, objToPublish);
        }
        #endregion

        #region Privates
        private void PublishObjectInThread(object stateInfo)
        {
            if (Registrations.ContainsKey(stateInfo.GetType()))
            {
                foreach (Func<object, bool> deleg in Registrations[stateInfo.GetType()])
                {
                    if (!deleg(stateInfo))
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region disposing
        /// <summary>
        /// Release all resources prior to removal
        /// </summary>
        /// <param name="disposing">True for deterministic, false for finalization</param>
        private void Close(bool disposing)
        {
            if (_isDisposed) return;

            // General cleanup logic here
            if (_registrations != null)
            {
                foreach (Type typ in Registrations.Keys)
                {
                    DropRegistrations(typ);
                }
            }

            if (disposing)
            {
                // Deterministic only cleanup
            }
            else
            {
                // Finalizer only cleanup
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Release all resources and remove
        /// </summary>
        public void Dispose()
        {
            Close(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
