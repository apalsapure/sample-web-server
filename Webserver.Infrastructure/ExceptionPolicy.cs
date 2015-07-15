using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Infrastructure
{
    public class ExceptionPolicy
    {
        public static void HandleException(Exception exception)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    ILogger logger = Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["logger"])) as ILogger;
                    logger.Log(FormatException(exception));
                });
            }
            catch { }
        }

        private static string FormatException(Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append("- - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            sb.Append(Environment.NewLine);
            sb.Append("Time Stamp: " + DateTime.UtcNow.ToString());
            sb.Append(Environment.NewLine);
            sb.Append("Exception: " + ex.ToString());
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}
