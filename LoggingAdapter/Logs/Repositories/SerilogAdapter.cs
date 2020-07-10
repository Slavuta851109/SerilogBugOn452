using System;
using System.Threading;
using LoggingAdapter.Logs.DTOs;
using LoggingAdapter.Logs.Serializers;
using Serilog.Events;

namespace LoggingAdapter.Logs.Repositories
{
    public class SerilogAdapter
    {
        private static readonly TimeSpan BATCH_PERIOD = TimeSpan.FromSeconds(1);  // Messages in buffer might be lost on app stop.
        private const int BATCH_SIZE = 100;
        private const string LOG_TABLE_NAME = "tlog";

        private static int _initialized = 0;

        public SerilogAdapter()
        {
            if(0 == Interlocked.Exchange(ref _initialized, 1))
            {
                string logDbConnectionString = "Server=.;Database=rewards;Integrated Security=true;";
                Serilog.Log.Logger = SerilogConfiguration
                    .CreateDefault()
                    .AddLoggingToSQL(logDbConnectionString, LOG_TABLE_NAME, BATCH_SIZE, BATCH_PERIOD)
                    .CreateLogger();
            }
        }

        public void Log(LogRequest request)
        {
            LogToDB(request);
        }

        private static void LogToDB(LogRequest request)
        {
            Serilog.Log.Write(
                ConvertTo_SerilogLogLevel(request.Level),
                "{source}{timestamp}{level}{method}{machineName}{message}{exception}{parameters}{activityId}",

                request.Source,
                request.TimeStamp,
                request.Level,
                request.Method, 
                request.MachineName, 
                request.Message, 
                request.Exception,
                ParametersSerializer.Serialize(request.Parameters),
                request.ActivityId);

            LogEventLevel ConvertTo_SerilogLogLevel(string logLevel)
            {
                switch (logLevel.ToLower())
                {
                    case "info": return LogEventLevel.Information;
                    default:
                        throw new NotImplementedException($"Mapping for {logLevel} is not implemented.");
                }
            }
        }
    }
}
