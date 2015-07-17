using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Model
{
    public interface IQueue
    {
        int Length { get; }
        bool TryDequeue(out Socket socket);
        void Enqueue(Socket socket);
    }
}
