using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Generics
{
    /// <summary>
    /// Events arg class for PersistentBuffer Information
    /// </summary>
    public class PersistentBufferInformationEventArgs: EventArgs
    {
        /// <summary>
        /// The originating member name for the exception
        /// </summary>
        public string OriginMember { get; set; }

        /// <summary>
        /// The message from the PersistentBuffer
        /// </summary>
        public string Message { get; set; }

    }
}
