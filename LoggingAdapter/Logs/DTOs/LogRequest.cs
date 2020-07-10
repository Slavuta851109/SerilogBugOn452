using System;
using System.Collections.Generic;

namespace LoggingAdapter.Logs.DTOs
{
    public class LogRequest
    {
        public string Level { get; }
        public string Source { get; }
        public string Message { get; }
        public string Method { get; }
        public IEnumerable<object> Parameters { get; }
        public string MachineName { get; }
        public string Exception { get; }
        public Guid? ActivityId { get; }
        public DateTime TimeStamp { get; }

        public LogRequest(string level, string source, string message, string method, IEnumerable<object> parameters, string machineName, string exception, Guid? activityId, DateTime timeStamp)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            Source = source;
            Message = message;
            Method = method;
            Parameters = parameters;
            MachineName = machineName;
            Exception = exception;
            ActivityId = activityId;
            TimeStamp = timeStamp;
        }
    }
}
