using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Webserver.Infrastructure;

namespace Webserver.Model
{
    public class Response
    {
        public Response(Socket socket)
        {
            if (socket == null) throw new ArgumentException();

            this.Body = new byte[] { };
            this.Socket = socket;
        }

        private string _contentType = "text/html";

        public Socket Socket { get; private set; }
        public int Status { get; set; }
        public string ReasonPhrase { get; set; }
        public string ContentType
        {
            get { return _contentType; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                _contentType = value;
            }
        }

        public string HttpVersion { get; set; }
        public string Date { get { return DateTime.Now.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"); } }
        public int ContentLength { get { return this.Body != null ? this.Body.Length : 0; } }
        public byte[] Body { get; set; }

        public void Send()
        {
            try
            {
                byte[] bytes = GetBytes(BuildResponse());

                var socket = this.Socket;
                socket.Send(bytes);
                socket.Close();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex);
            }
        }

        private string BuildResponse()
        {
            StringBuilder httpResponse = new StringBuilder();
            //Adding the Status-Line
            httpResponse.Append(this.HttpVersion).Append(" ");
            httpResponse.Append(this.Status).Append(" ");
            httpResponse.Append(this.ReasonPhrase).Append(" ");
            httpResponse.Append(Constants.CARRIAGE_RETURN_LINE_FEED);

            //Adding the Current Date and time 
            httpResponse.Append(this.Date);
            httpResponse.Append(Constants.CARRIAGE_RETURN_LINE_FEED);

            //Adding the Content Type to the response header
            httpResponse.Append("Content-Type:").Append(this.ContentType);
            httpResponse.Append(Constants.CARRIAGE_RETURN_LINE_FEED).Append(Constants.CARRIAGE_RETURN_LINE_FEED);

            return httpResponse.ToString();
        }

        private byte[] GetBytes(string response)
        {
            var bytes = Encoding.UTF8.GetBytes(response);

            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            if (this.Body != null)
                stream.Write(this.Body, 0, this.Body.Length);
            return stream.ToArray();
        }

        public void SendMaxQueueSizeResponse(Socket socket)
        {

        }
    }
}