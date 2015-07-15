using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Infrastructure
{
    [Serializable]
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() : base() { }

        public ResourceNotFoundException(string message) : base(message) { }

        public ResourceNotFoundException(string message, Exception exception) : base(message, exception) { }

        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
