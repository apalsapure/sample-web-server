using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Infrastructure
{
    public static class Errors
    {
        public static Exception ResourceNotFound()
        {
            return new ResourceNotFoundException();
        }
    }
}
