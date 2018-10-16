using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Generic
{
    /// <summary>
    /// Contract for a message bus that uses a delegate
    /// </summary>
    public interface IDelegateMessageBus
    {
        /// <summary>
        /// Registration Dictionary for collecting delegates by type
        /// Author: Jeff Bramlett (jeffrey.bramlett@gmail.com)
        /// </summary>
        ConcurrentDictionary<Type, List<Func<object, bool>>> Registrations { get; set; }

        /// <summary>
        /// Register a delegate by a type
        /// </summary>
        /// <param name="key">the type for registration</param>
        /// <param name="funcDelegate">the delegate for registration</param>
        void Register(Type key, Func<object, bool> funcDelegate);

        /// <summary>
        /// Remove the registration type (and delegates)
        /// </summary>
        /// <param name="typ">the type as a key</param>
        void DropRegistrations(Type typ);

        /// <summary>
        /// Publish the object to all/any subscribing delegates
        /// </summary>
        /// <param name="objToPublish">the object to publish</param>
        void Publish(object objToPublish);

        /// <summary>
        /// Release all resources and remove
        /// </summary>
        void Dispose();
    }
}