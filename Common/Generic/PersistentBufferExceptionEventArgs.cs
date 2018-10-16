using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Generics
{
    /// <summary>
    /// Events arg class for PersistentBuffer Exceptions
    /// </summary>
    public class PersistentBufferExceptionEventArgs: EventArgs
    {
        /// <summary>
        /// The originating member name for the exception
        /// </summary>
        public string OriginMember { get; set; }

        /// <summary>
        /// the exception that was raised
        /// </summary>
        public Exception Exception { get; set; }
    }
}
