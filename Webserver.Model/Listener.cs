using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Webserver.Infrastructure;

namespace Webserver.Model
{
    internal class Listener
    {
        private TcpListener _tcpListener;
        private static int _maxQueueSize = -1;

        public Listener(string host, int port)
        {
            this._tcpListener = new TcpListener(IPAddress.Parse(host), port);
        }

        public void Listen()
        {
            try
            {
                this.SetMaxQueueSize();
                this._tcpListener.Start();
                while (true)
                {
                    try
                    {
                        var socket = this._tcpListener.AcceptSocket();
                        if (socket.Connected == false) continue;

                        if (Application.RequestQueue.Length > _maxQueueSize)
                            throw Errors.MaxQueueSizeReached(Application.RequestQueue.Length, _maxQueueSize);

                        Application.RequestQueue.Enqueue(socket);
                    }
                    catch (ConfigurationErrorsException cEx)
                    {
                        ExceptionPolicy.HandleException(cEx);
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex);
            }
        }

        public void Stop()
        {
            this._tcpListener.Stop();
        }

        private void SetMaxQueueSize()
        {
            if (_maxQueueSize != -1) return;

            if (int.TryParse(ConfigurationManager.AppSettings["max-queue-size"], out _maxQueueSize) == false
                || _maxQueueSize < 0)
                throw Errors.MaxQueueSizeNotValid();
        }
    }
}
