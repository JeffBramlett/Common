using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Extensions
{
    /// <summary>
    /// IList Extensions
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Task an Action (delegate) on a span (iterationSize) using parallel execution (Task)
        /// </summary>
        /// <typeparam name="T">the type to use in the Action</typeparam>
        /// <param name="collection">the collection of items of type</param>
        /// <param name="actionToTakeOnEach">the action (delegate) to apply to each item</param>
        /// <param name="throttleSize">the size of the parallel actions</param>
        /// <returns>true when all is complete</returns>
        public async static Task<bool> ApplyAction<T>(this IList<T> collection, Action<T> actionToTakeOnEach, int throttleSize)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (actionToTakeOnEach == null) throw new ArgumentNullException(nameof(actionToTakeOnEach));
            if (throttleSize <= 0) throw new ArgumentNullException(nameof(throttleSize));

            int count = collection.Count();
            int diffSize = count > throttleSize ? throttleSize : count;
            int pos = 0;

            while (diffSize > 0)
            {
                var cancellationTokenSource = new CancellationTokenSource();

                Task[] taskList = new Task[diffSize];

                int ti = 0;
                for (var i = pos; i < pos + diffSize; i++)
                {
                    var item = collection[i];
                    taskList[ti] = Task.Run(() => { actionToTakeOnEach.Invoke(item); }, cancellationTokenSource.Token);
                    ti++;
                }

                await Task.WhenAll(taskList.ToArray());

                pos += diffSize;
                diffSize = pos + throttleSize < count ? throttleSize : count - pos;
            }

            return true;
        }

        /// <summary>
        /// Apply a Func to each item in the collection async by throttlesize
        /// </summary>
        /// <typeparam name="I">the input collection type</typeparam>
        /// <typeparam name="R">the output collection type</typeparam>
        /// <param name="collection">the input collection</param>
        /// <param name="funcToTakeOnEach">the function to apply to each I in the collection</param>
        /// <param name="throttleSize">the size of the async throttle</param>
        /// <returns></returns>
        public async static Task<IList<R>> ApplyFunction<I, R>(this IList<I> collection, Func<I,R> funcToTakeOnEach, int throttleSize)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (funcToTakeOnEach == null) throw new ArgumentNullException("funcToTakeOnEach");
            if (throttleSize <= 0) throw new ArgumentNullException("iterationSize");

            IList<R> list = new List<R>();

            int count = collection.Count();
            int diffSize = count > throttleSize ? throttleSize : count;
            int pos = 0;

            while (diffSize > 0)
            {
                var cancellationTokenSource = new CancellationTokenSource();

                Task<R>[] taskList = new Task<R>[diffSize];

                int ti = 0;
                for (var i = pos; i < pos + diffSize; i++)
                {
                    var item = collection[i];
                    taskList[ti] = Task.Run(() => { return funcToTakeOnEach.Invoke(item); }, cancellationTokenSource.Token);
                    ti++;
                }

                var temp = await Task.WhenAll(taskList.ToArray()).ConfigureAwait(false);
                
                foreach(var r in temp)
                {
                    list.Add(r);
                }

                pos += diffSize;
                diffSize = pos + throttleSize < count ? throttleSize : count - pos;
            }
            return list;
        }
   }
}
