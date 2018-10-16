using Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Common.Generics
{
    #region Enums
    /// <summary>
    /// Enumeration of responses to AddItem
    /// </summary>
    public enum AddItemResults
    {
        /// <summary>
        /// Successfully added
        /// </summary>
        Ok,
        /// <summary>
        /// Ok to try to add the item again
        /// </summary>
        Retry,

        /// <summary>
        /// Do not try to add the item again, it will only fail again
        /// </summary>
        Fail
    }
    #endregion

    /// <summary>
    /// Executing class for using BufferBlock but persisting the unresolved items and resubmitting them
    /// </summary>
    public sealed class PersistentBuffer<T> : IPersistentBuffer<T>
    {
        #region Constants
        private readonly string MapName;
        #endregion

        #region Fields

        private bool _inited;

        private bool _isDisposing;

        private string _persistFilename;

        private BufferBlock<TrackingData> _bufferBlock;

        private ExecutionDataflowBlockOptions _bufferOptions;

        private SlidingTimer _slidingTimer;

        private OpenBinaryWriter _writer;

        private FileStream _stream;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get the count of the underlying BufferBlock
        /// </summary>
        public int Count
        {
            get
            {
                return _bufferBlock.Count;
            }
        }
        /// <summary>
        /// Can the buffer accept more items right now?
        /// </summary>
        public bool CanAcceptItems
        {
            get { return !IsCompleted && CurrentInFlightSize < MaxInFlightSize; }
        }

        /// <summary>
        /// Gets a Task that represents the asynchronous operation and completion of the Buffer.
        /// </summary>
        /// <remarks>
        /// Call Initialize first
        /// </remarks>
        /// <exception cref="InvalidOperationException">thrown if Initialize() has not been called first</exception>
        public Task Completion
        {
            get
            {
                if (!_inited)
                    throw new InvalidOperationException("Persistent Buffer has not been initialized, call Initialize first");

                return _bufferBlock.Completion;
            }
        }
        #endregion

        #region private Auto Properties
        private int CurrentIndex { get; set; }
        private int MaxItemSize { get; set; }
        private int SlidingMaxIterations { get; set; }
        private TimeSpan SlidingTimespan { get; set; }
        private int MaxInFlightSize { get; set; }
        private int CurrentInFlightSize { get; set; }
        private bool IsCompleted { get; set; }
        private Func<T, bool> PerItemFunction { get; set; }
        #endregion

        #region Private properties
        private ExecutionDataflowBlockOptions BufferOptions
        {
            get
            {
                if (_bufferOptions == null)
                {
                    _bufferOptions = new ExecutionDataflowBlockOptions()
                    {
                        // TODO: add any default options
                    };
                }
                return _bufferOptions;
            }
        }

        private BufferBlock<TrackingData> BufferBlock
        {
            get
            {
                if (_bufferBlock == null)
                {
                    _bufferBlock = new BufferBlock<TrackingData>();
                }
                return _bufferBlock;
            }
        }

        private SlidingTimer SlidingTimerForCleanup
        {
            get
            {
                _slidingTimer = _slidingTimer ?? new SlidingTimer(SlidingTimespan, SlidingMaxIterations, ReInputUnresolvedMessages);
                return _slidingTimer;
            }
        }

        private OpenBinaryWriter Writer
        {
            get
            {
                _writer = _writer ?? new OpenBinaryWriter(ViewStream);
                return _writer;
            }
        }

        private FileStream ViewStream
        {
            get
            {
                if (_stream == null)
                {
                    _stream = new FileStream(PersistFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }

                return _stream;
            }
        }

        private string PersistFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_persistFilename))
                {
                    _persistFilename = MapName + ".bin";
                }

                return _persistFilename;
            }
        }

        private Func<T, bool> PerItemAction
        {
            get { return PerItemFunction; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Events raising any exceptions while allowing the buffer to continue
        /// </summary>
        public event EventHandler PersistenceExceptionEncountered;

        /// <summary>
        /// Event raising any warnings while allowing the buffer to continue
        /// </summary>
        public event EventHandler PersistenceWarningEncountered;

        /// <summary>
        /// Event raising any information while allowing the buffer to continue
        /// </summary>
        public event EventHandler PersistenceInformationEncountered;
        #endregion

        #region Ctors and Dtors
        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="maxItemSize">the maximum size of each item in bytes</param>
        /// <param name="slidingTimespan">when idle the time to wait before retrying items</param>
        /// <param name="maxSlidingIterations">when retrying items, how many times to retry before stopping</param>
        /// <param name="maxInFlightSize">the maximum size of the buffer item count</param>
        /// <param name="mapName">the mapped name used for file naming if the filename is not specified (optional)</param>
        /// <param name="persistFilename">the filename for persistence (optional)</param>
        public PersistentBuffer(int maxItemSize, TimeSpan slidingTimespan, int maxSlidingIterations, int maxInFlightSize, string mapName = "", string persistFilename = "")
        {
            if (string.IsNullOrEmpty(mapName))
                MapName = typeof(T).Name;
            else
                MapName = mapName;

            MaxInFlightSize = maxInFlightSize;
            SlidingTimespan = slidingTimespan;
            SlidingMaxIterations = maxSlidingIterations;

            MaxItemSize = maxItemSize;
            _persistFilename = persistFilename;
        }

        /// <summary>
        /// Finalizer to insure the buffer is shutdown and resources released, even if Dispose is not called
        /// </summary>
        ~PersistentBuffer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }
        #endregion

        #region Publics
        /// <summary>
        /// Initialize the buffer with the delegate to use and data flow options
        /// </summary>
        /// <param name="functionForEachItem">the function to use with each item</param>
        /// <param name="bufferOptions">(optional) the options to use with the buffer</param>
        /// <exception cref="ArgumentNullException">throws if the functionForEachItem is null</exception>
        public void Initialize(Func<T, bool> functionForEachItem, ExecutionDataflowBlockOptions bufferOptions = null)
        {
            if (!_inited)
            {
                functionForEachItem.ThrowIfNull();

                _bufferOptions = bufferOptions;
                PerItemFunction = functionForEachItem;

                BufferBlock.LinkTo(new ActionBlock<TrackingData>((item) => PushItemToAction(item), BufferOptions));

                _inited = true;

                RaiseInformationEncountered("PersistentBuffer is Intialized");

                ReInputUnresolvedMessages();
            }
            else
                RaiseWarningEncountered("Calling Initialize after this is already initialized");
        }

        /// <summary>
        /// Persist the item and then submit it to the Buffer
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <returns>True if the item is successfully added, false otherwise (See PersistenceExceptionEncountered event)</returns>
        /// <remarks>
        /// Call Initialize before adding items
        /// </remarks>
        /// <exception cref="InvalidOperationException">throws if Initialize() has not been called first</exception>
        /// <exception cref="ArgumentNullException">throws if the item is null</exception>
        public AddItemResults AddItem(T item)
        {
            if (!_inited)
                throw new InvalidOperationException("Persistent Buffer has not been initialized, call Initialize before Adding Items");

            item.ThrowIfNull();

            TrackingData data = new TrackingData()
            {
                Item = item,
                IsResolved = false,
                Index = GetNextIndex()
            };

            if (!WriteTrackingData(data))
            {
                return AddItemResults.Fail;
            }

            BufferBlock.Post(data);

            CurrentInFlightSize++;

            return AddItemResults.Ok;
        }

        /// <summary>
        /// Signals to the Buffer that it should not 
        /// accept nor produce any more messages nor consume any more postponed messages.
        /// </summary>
        /// <remarks>
        /// Call Initialize first
        /// </remarks>
        /// <exception cref="InvalidOperationException">throws if Initialize() has not been called first</exception>
        public void Complete()
        {
            if (!_inited)
                throw new InvalidOperationException("Persistent Buffer has not been initialized, call Initialize first");

            _bufferBlock.Complete();
            IsCompleted = true;
        }
        #endregion

        #region Privates
        private int GetNextIndex()
        {
            CurrentIndex++;

            return CurrentIndex;
        }

        private bool WriteTrackingData(TrackingData data)
        {
            bool success = false;
            try
            {
                byte[] bytesToPersist;
                if (GetBytesForItem(data, out bytesToPersist))
                {
                    Writer.Write(bytesToPersist.ToArray(), 0, bytesToPersist.Length);
                    Writer.BaseStream.Flush();
                    success = true;
                }
                else
                {
                    RaiseExceptionEncountered(new ArgumentException("Item is not serializable"));
                }

                bytesToPersist = null;
            }
            catch (Exception ex)
            {
                RaiseExceptionEncountered(ex);
            }

            return success;
        }

        private bool GetBytesForItem(TrackingData data, out byte[] byteArray)
        {
            byteArray = new byte[MaxItemSize];

            BinaryFormatter formatter = new BinaryFormatter();
            using (var memstream = new MemoryStream(MaxItemSize))
            {
                formatter.Serialize(memstream, data);

                memstream.Flush();

                var binSize = memstream.Length;
                if (binSize > MaxItemSize)
                {
                    RaiseExceptionEncountered(new ArgumentException(string.Format("The item when serialized (size: {0}) exceeds the max item size of {1}", binSize, MaxItemSize)));
                    return false;
                }

                var itemBytes = memstream.GetBuffer();

                for (var i = 0; i < itemBytes.Length; i++)
                {
                    if (i < MaxItemSize)
                    {
                        byteArray[i] = itemBytes[i];
                    }
                }
            }

            return byteArray != null;
        }

        private bool GetItemFromBytes(byte[] inputBytes, out TrackingData data)
        {
            data = null;

            try
            {
                List<byte> byteList = new List<byte>();
                using (MemoryStream mem = new MemoryStream(inputBytes))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    var obj = binFormatter.Deserialize(mem);
                    data = obj as TrackingData;
                }
            }
            catch (Exception ex)
            {
                RaiseExceptionEncountered(ex);
            }

            return data != null;
        }

        private void PushItemToAction(TrackingData data)
        {
            try
            {
                var canRemoveItem = PerItemAction?.Invoke(data.Item);

                if (canRemoveItem.Value)
                {
                    data.IsResolved = true;
                    WriteTrackingData(data);
                }
            }
            catch (Exception ex)
            {
                RaiseExceptionEncountered(ex);
            }

            CurrentInFlightSize--;
            SlidingTimerForCleanup.BumpTimer();
        }

        private void ReInputUnresolvedMessages()
        {
            try
            {
                var unresolvedList = ReadPersistentFile();
                if (unresolvedList.Count == 0)
                {
                    CurrentIndex = 0;
                }
                else
                {
                    foreach (var data in unresolvedList)
                    {
                        AddItem(data.Item);
                    }
                }

                RaiseInformationEncountered("Unresolved messages have been resubmitted");
            }
            catch (Exception ex)
            {
                RaiseExceptionEncountered(ex);
            }
        }

        private List<TrackingData> ReadPersistentFile()
        {
            Writer.BaseStream.Flush();

            int pos = 0;
            List<TrackingData> dataList = new List<TrackingData>();
            OpenBinaryReader reader = new OpenBinaryReader(ViewStream);
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte[] buffer = new byte[MaxItemSize];
                int bytesRead = reader.Read(buffer, 0, buffer.Length);
                pos += bytesRead;

                if (bytesRead > 0)
                {
                    TrackingData data;
                    if (GetItemFromBytes(buffer, out data))
                    {
                        CurrentIndex = data.Index > CurrentIndex ? data.Index : CurrentIndex;

                        if (data.IsResolved)
                        {
                            var foundUnresolved = dataList.Find(t => t.Index == data.Index && !t.IsResolved);
                            if (foundUnresolved != null)
                            {
                                dataList.Remove(foundUnresolved);
                            }
                        }
                        else
                        {
                            dataList.Add(data);
                        }
                    }
                    else
                    {
                        RaiseExceptionEncountered(new Exception("Cannot get item from byte array"));
                    }
                }
                else
                {
                    break; // EOF
                }
            }

            reader.Close();

            dataList.Sort();

            ResetBinFile();

            return dataList;
        }

        private void ResetBinFile()
        {
            Writer.BaseStream.SetLength(0);
            Writer.Flush();
        }
        #endregion

        #region Event handling
        private void RaiseExceptionEncountered(Exception ex, [CallerMemberName] string callerName = null)
        {
            Task.Run(() =>
            {
                PersistentBufferExceptionEventArgs args = new PersistentBufferExceptionEventArgs()
                {
                    Exception = ex,
                    OriginMember = callerName
                };

                PersistenceExceptionEncountered?.Invoke(this, args);
            });
        }

        private void RaiseWarningEncountered(string message, [CallerMemberName] string callerName = null)
        {
            Task.Run(() =>
            {
                PersistentBufferWarningEventArgs args = new PersistentBufferWarningEventArgs()
                {
                    Message = message,
                    OriginMember = callerName
                };

                PersistenceWarningEncountered?.Invoke(this, args);
            });
        }

        private void RaiseInformationEncountered(string message, [CallerMemberName] string callerName = null)
        {
            Task.Run(() =>
            {
                PersistentBufferInformationEventArgs args = new PersistentBufferInformationEventArgs()
                {
                    Message = message,
                    OriginMember = callerName
                };

                PersistenceInformationEncountered?.Invoke(this, args);
            });
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (_isDisposing)
                return;

            _isDisposing = true;

            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_slidingTimer != null)
                    {
                        _slidingTimer.Dispose();
                    }

                    if (_writer != null)
                    {
                        _writer.Dispose();
                    }

                    if(_bufferBlock != null)
                    {
                        _bufferBlock.Complete();
                        _bufferBlock = null;
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Inner classes
        [Serializable]
        private class TrackingData : IComparable<TrackingData>, IComparer<TrackingData>
        {
            public T Item { get; set; }

            public bool IsResolved { get; set; }

            public int Index { get; set; }

            public int Compare(TrackingData x, TrackingData y)
            {
                return x.CompareTo(y);
            }

            public int CompareTo(TrackingData other)
            {
                var resolvedThis = IsResolved ? 1 : 0;
                var resolvedOther = other.IsResolved ? 1 : 0;
                return (Index + resolvedThis) - (other.Index + resolvedOther);
            }
        }
        #endregion
    }

    #region Internal Extended classes (necessary for stream handling without adding encoding)
    internal class OpenBinaryWriter: BinaryWriter
    {
        public OpenBinaryWriter(Stream inputStream):
            base(inputStream)
        {

        }
        
        public override void Close()
        {
            // Don't call base, it closes the underlying stream
        }
    }

    internal class OpenBinaryReader: BinaryReader
    {
        public OpenBinaryReader(Stream inputStream) :
            base(inputStream)
        {

        }

        public override void Close()
        {
            // Don't call base, it closes the underlying stream
        }
    }
    #endregion
}
