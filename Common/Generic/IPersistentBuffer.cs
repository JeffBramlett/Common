using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Common.Generics
{
    /// <summary>
    /// Public contract for the PersistentBuffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPersistentBuffer<T> : IDisposable
    {
        /// <summary>
        /// Signals to the Buffer that it should not 
        /// accept nor produce any more messages nor consume any more postponed messages.
        /// </summary>
        void Complete();

        /// <summary>
        /// Gets a Task that represents the asynchronous operation and completion of the Buffer.
        /// </summary>
        Task Completion { get; }

        /// <summary>
        /// return true if the buffer is not exceeding max buffer size
        /// </summary>
        bool CanAcceptItems { get; }

        /// <summary>
        /// Add and Item to this PersistentBuffer
        /// </summary>
        /// <returns>
        /// returns enumeration of the results of addin the item
        /// </returns>
        /// <param name="item">the item to add</param>
        AddItemResults AddItem(T item);

        /// <summary>
        /// Initialize the buffer with the delegate to use and data flow options
        /// </summary>
        /// <param name="functionForEachItem">the function to use with each item</param>
        /// <param name="bufferOptions">(optional) the options to use with the buffer</param>
        void Initialize(Func<T, bool> functionForEachItem, ExecutionDataflowBlockOptions bufferOptions = null);

        /// <summary>
        /// Events raising any exceptions while allowing the buffer to continue
        /// </summary>
        event EventHandler PersistenceExceptionEncountered;

        /// <summary>
        /// Event raising any warnings while allowing the buffer to continue
        /// </summary>
        event EventHandler PersistenceWarningEncountered;

        /// <summary>
        /// Event raising any information while allowing the buffer to continue
        /// </summary>
        event EventHandler PersistenceInformationEncountered;

    }
}