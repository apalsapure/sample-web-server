using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Webserver.Model
{
    public class Request
    {
        private Dictionary<string, string> _headers { get; set; }
        private static string _physicalPath = ConfigurationManager.AppSettings["virtual-directory"];
        private static string _scheme = "http";
        static Request()
        {
            var isSecure = false;
            bool.TryParse(ConfigurationManager.AppSettings["is-secure"], out isSecure);
            if (isSecure) _scheme = "https";
        }

        public Socket Socket { get; private set; }

        public string UserAgent
        {
            get
            {
                return this._headers["User-Agent"];
            }
        }
        public string AcceptTypes
        {
            get
            {
                return this._headers["Accept"];
            }
        }
        public string Connection
        {
            get
            {
                return this._headers["Connection"];
            }
        }
        public string CacheControl
        {
            get
            {
                return this._headers["Cache-Control"];
            }
        }
        public string AcceptEncoding
        {
            get
            {
                return this._headers["Accept-Encoding"];
            }
        }
        public string AcceptLanguage
        {
            get
            {
                return this._headers["Accept-Language"];
            }
        }

        public string Method { get; private set; }
        public string Version { get; private set; }
        public string Host { get; private set; }
        public string Body { get; private set; }
        public string Url { get; private set; }
        public Uri Uri { get; private set; }

        public string Query { get; set; }
        public string PhysicalApplicationPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_physicalPath)) throw new Exception("virtual-directory is not set in the configuration.");
                return _physicalPath;
            }
        }
        public string PhysicalPath { get; private set; }
        public string File { get; private set; }

        public Request(Socket socket)
        {
            if (socket == null) throw new ArgumentException();

            this.Socket = socket;

            // SAMPLE REQUEST
            //  GET /index.html HTTP/1.1
            //  Host: 127.0.0.1:8000
            //  Connection: keep-alive
            //  Cache-Control: max-age=0
            //  Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
            //  User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.132 Safari/537.36
            //  Accept-Encoding: gzip, deflate, sdch
            //  Accept-Language: en-US,en;q=0.8,en-GB;q=0.6,nb;q=0.4
            var request = Encoding.UTF8.GetString(ReadBytes());
            if (string.IsNullOrEmpty(request))
            {
                return;
            }

            this._headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string[] lineSplit = request.Split(new string[] { Constants.CARRIAGE_RETURN_LINE_FEED }, StringSplitOptions.None);
            string[] requestLine = lineSplit[0].Split(' ');

            ProcessRequest(requestLine);
            ProcessHeader(lineSplit);
            BuildUri();
            ResolvePath();
        }

        private byte[] ReadBytes()
        {
            var bucket = new byte[1024];
            using (var buffer = new MemoryStream())
            {
                while (true)
                {
                    var bytesRead = this.Socket.Receive(bucket);
                    if (bytesRead > 0)
                        buffer.Write(bucket, 0, bytesRead);

                    if (this.Socket.Available == 0)
                        break;
                }
                return buffer.ToArray();
            }
        }

        private void BuildUri()
        {
            var baseUrl = string.Format("{0}://{1}/{2}", _scheme, this.Host, this.Url);
            if (string.IsNullOrWhiteSpace(this.Query)) this.Uri = new Uri(baseUrl);
            else this.Uri = new Uri(baseUrl + this.Query);
        }

        private void ProcessRequest(string[] requestLine)
        {
            if (requestLine.Length != 3) throw new Exception("Invalid request");

            this.Method = requestLine[0].Trim();

            this.Url = requestLine[1].TrimStart('/');
            if (string.IsNullOrWhiteSpace(this.Url) && string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["default-document"]) == false)
            {
                this.Url = ConfigurationManager.AppSettings["default-document"];
            }

            var queryIndex = this.Url.IndexOf("?");
            if (queryIndex != -1)
            {
                this.Query = this.Url.Substring(queryIndex, this.Url.Length - queryIndex);
                this.Url = this.Url.Replace(this.Query, "");
            }

            this.Version = requestLine[2];
        }

        private void ProcessHeader(string[] lineSplit)
        {
            for (int index = 1; index < lineSplit.Length; index++)
            {
                //Last field of header is Body,so its handled separately
                if (index == lineSplit.Length - 1)
                    this.Body = lineSplit[index];

                string[] temp = lineSplit[index].Split(':');
                for (int index2 = 0; index2 < temp.Length - 1; index2++)
                {
                    ///Handling Host field of header separately
                    if (temp[0] == "Host")
                    {
                        if (temp.Length > 2)
                            this.Host = temp[1].Trim() + ":" + temp[2].Trim();
                        else
                            this.Host = temp[1];
                    }
                    else
                        this._headers[temp[0]] = temp[1];
                }

            }
        }

        private void ResolvePath()
        {
            var segments = new List<string>();
            for (int index = 0; index < this.Uri.Segments.Length; index++)
            {
                string segment = this.Uri.Segments[index];
                segments.Add(segment);
                var extIndex = segment.IndexOf(".");
                if (extIndex != -1)
                {
                    segment = segment.Trim('/').ToLower();
                    this.File = Path.GetExtension(segment).Trim('.');
                    break;
                }
            }
            var localPath = string.Join("", segments).Trim('/');
            this.PhysicalPath = System.IO.Path.Combine(this.PhysicalApplicationPath, localPath.Replace("/", "\\"));
        }
    }
}
