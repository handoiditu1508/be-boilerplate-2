using Hamburger.Models.Common;
using Hamburger.Models.LoggingService;
using Hamburger.Models.Requests.LoggingService;
using Microsoft.Extensions.Logging;
using System;

namespace Hamburger.Helpers.Extensions
{
    public static class ExceptionExtension
    {
        /// <summary>
        /// Cast exception to more simple object so FE can understand.
        /// </summary>
        /// <param name="exception">Instance of Exception.</param>
        /// <returns>Instance of SimpleError.</returns>
        public static SimpleError ToSimpleError(this Exception exception)
        {
            SimpleError error;

            if (exception.GetType() == typeof(CustomException))
            {
                var customException = (CustomException)exception;
                error = customException.ToSimpleError();
            }
            else
            {
                var customException = CustomException.System.UnexpectedError();
                error = customException.ToSimpleError();
            }

            return error;
        }

        public static InsertLogRequest ToInsertLogRequest(this Exception exception, LogLevel logLevel)
        {
            InsertLogRequest request = new InsertLogRequest
            {
                CollectionName = AppSettings.LoggingService.DefaultCollectionName,
                Log = new LoggingModel
                {
                    LogLevel = logLevel
                }
            };

            if (exception is CustomException customException)
            {
                request.Log.Message = customException.Message;
                request.Log.StackTrace = customException.StackTrace;
                request.Log.Source = customException.Source;
                request.Log.Group = customException.Group;
                request.Log.Code = customException.Code;
            }
            else
            {
                customException = CustomException.System.UnexpectedError(exception.Message);

                request.Log.Message = customException.Message;
                request.Log.StackTrace = exception.StackTrace;
                request.Log.Source = exception.Source;
                request.Log.Group = customException.Group;
                request.Log.Code = customException.Code;
            }

            return request;
        }
    }
}
