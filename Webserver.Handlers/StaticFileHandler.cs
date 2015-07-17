using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webserver.Infrastructure;
using Webserver.Model;

namespace Webserver.Handlers
{
    public class StaticFileHandler : IHandler
    {
        private static string _supportedTypes = ConfigurationManager.AppSettings["static-file-handler"];

        public Response Process(Request request)
        {
            var response = new Response(request.Socket);

            if (string.IsNullOrWhiteSpace(request.AcceptTypes)) response.ContentType = "text/html";
            else response.ContentType = request.AcceptTypes.Split(',')[0];

            if (File.Exists(request.PhysicalPath) == false)
            {
                throw Errors.ResourceNotFound();
            }
            response.Body = File.ReadAllBytes(request.PhysicalPath);
            return response;
        }

        public string SupportedTypes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_supportedTypes)) throw new Exception("Supported types for Static File Handler not defined");
                return _supportedTypes;
            }
        }
    }
}
