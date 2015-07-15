using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Model
{
    public interface IHandler
    {
        string SupportedTypes { get; }
        Response Process(Request request);
    }
}
