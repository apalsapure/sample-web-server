using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Webserver.Infrastructure;

namespace Webserver.Model
{
    internal class Worker
    {
        private Request _request;

        public Worker(Request request)
        {
            if (request == null) throw new ArgumentException();

            this._request = request;
        }

        public void Process(IHandler handler)
        {
            if (handler == null) throw new ArgumentException();

            Response response;
            try
            {
                response = handler.Process(this._request);
                response.Status = Constants.STATUS_CODE_200;
                response.ReasonPhrase = "OK";
            }
            catch (ResourceNotFoundException reNtFdEx)
            {
                ExceptionPolicy.HandleException(reNtFdEx);

                response = new Response(this._request.Socket);
                response.Status = Constants.STATUS_CODE_404;
                response.ReasonPhrase = "Not Found";
            } 
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex);

                response = new Response(this._request.Socket);
                response.Status = Constants.STATUS_CODE_500;
                response.ReasonPhrase = "Internal Server Error";
            }

            response.Send();
        }
    }
}
