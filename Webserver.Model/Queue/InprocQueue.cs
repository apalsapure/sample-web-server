using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Model
{
    public class InProcQueue : IQueue
    {
        private ConcurrentQueue<Socket> _queue = new ConcurrentQueue<Socket>();

        public bool TryDequeue(out Socket socket)
        {
            return this._queue.TryDequeue(out socket);
        }

        public void Enqueue(Socket socket)
        {
            this._queue.Enqueue(socket);
        }
    }
}
