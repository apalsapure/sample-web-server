using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Infrastructure
{
    public class MaxQueueSizeException: Exception
    {
        public MaxQueueSizeException() : base() { }

        public MaxQueueSizeException(string message) : base(message) { }

        public MaxQueueSizeException(string message, Exception exception) : base(message, exception) { }

        protected MaxQueueSizeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
