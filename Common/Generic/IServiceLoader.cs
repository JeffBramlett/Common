
using System.Collections.Generic;

namespace Common.Generics
{
    /// <summary>
    /// Interface to a late-binding service loader
    /// </summary>
    /// <typeparam name="T">the service type</typeparam>
    public interface IServiceLoader<T> where T : class
    {
        /// <summary>
        /// Load types (services) that are found in a file path
        /// </summary>
        /// <param name="folderPath">the top path to search</param>
        /// <remarks>
        /// The property FoundServices will contain all services (instantiated objects of type T)
        /// </remarks>
        void LoadServicesFrom(string folderPath);

        /// <summary>
        /// List of services found (if any)
        /// </summary>
        IList<T> FoundServices { get; }
    }
}
