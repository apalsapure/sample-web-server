using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Infrastructure
{
    public static class Errors
    {
        public static Exception ResourceNotFound()
        {
            return new ResourceNotFoundException();
        }

        public static ConfigurationErrorsException MaxQueueSizeNotValid()
        {
            return new ConfigurationErrorsException(ErrorMessages.MaxQueueSizeNotValid);
        }

        public static ConfigurationErrorsException MaxQueueSizeReached(int currentLenght, int maxQueueSize)
        {
            throw new ConfigurationErrorsException(string.Format(ErrorMessages.MaxQueueSizeReached, maxQueueSize, currentLenght));
        }
    }
}