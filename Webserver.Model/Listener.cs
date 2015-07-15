using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Model
{
    internal class Listener
    {
        private TcpListener _tcpListener;

        public Listener(string host, int port)
        {
            this._tcpListener = new TcpListener(IPAddress.Parse(host), port);
        }

        public void Listen()
        {
            this._tcpListener.Start();
            while (true)
            {
                var socket = this._tcpListener.AcceptSocket();
                if (socket.Connected == false) continue;

                Application.RequestQueue.Enqueue(socket);
            }
        }

        public void Stop()
        {
            this._tcpListener.Stop();
        }
    }
}
