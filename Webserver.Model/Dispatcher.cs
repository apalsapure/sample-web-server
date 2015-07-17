using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Webserver.Infrastructure;

namespace Webserver.Model
{
    public class Dispatcher
    {
        private static Dictionary<string, Type> _handlerMapping = new Dictionary<string, Type>();
        static Dispatcher()
        {
            var type = typeof(IHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass);

            types.ToList().ForEach(handlerType =>
            {
                var instance = Activator.CreateInstance(handlerType) as IHandler;
                if (string.IsNullOrWhiteSpace(instance.SupportedTypes)) throw new ArgumentException("Invalid handler registered");
                instance.SupportedTypes.Split(',').ToList().ForEach(fileType =>
                {
                    _handlerMapping[fileType.ToLower()] = handlerType;
                });
            });
        }

        private bool _isRunning = true;

        internal void Start()
        {
            while (this._isRunning)
            {
                Socket socket;
                if (Application.RequestQueue.TryDequeue(out socket) == false) continue;
                Task.Factory.StartNew(() =>
               {
                   this.Dispatch(socket);
               });
            }
        }

        internal void Stop()
        {
            this._isRunning = false;
        }

        #region Private Helper Methods
        private void Dispatch(Socket socket)
        {
            var request = new Request(socket);
            try
            {
                if (string.IsNullOrEmpty(request.File))
                {
                    SendEmptyResponse(request);
                }
                else
                {
                    var handler = this.ResolveHandler(request.File);
                    new Worker(request).Process(handler);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex);
                SendInternalServerResponse(request);
            }
        }

        private void SendEmptyResponse(Request request)
        {
            var response = new Response(request.Socket);
            response.Status = Constants.STATUS_CODE_204;
            response.ReasonPhrase = "No Content";
            response.Send();
        }

        private void SendInternalServerResponse(Request request)
        {
            var response = new Response(request.Socket);
            response.Status = Constants.STATUS_CODE_500;
            response.ReasonPhrase = "Internal Server Error";
            response.Send();
        }

        private IHandler ResolveHandler(string extension)
        {
            if (_handlerMapping.ContainsKey(extension) == false) throw new Exception("Handler not found: Extension - " + extension);

            return Activator.CreateInstance(_handlerMapping[extension]) as IHandler;
        }
        #endregion
    }
}
