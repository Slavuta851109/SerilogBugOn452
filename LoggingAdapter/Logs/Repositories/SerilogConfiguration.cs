using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace LoggingAdapter.Logs.Repositories
{
    public static class SerilogConfiguration
    {
        #region SQL Columns Length Constants
        public const int COLUMN_SOURCE_LENGTH = 400;
        public const int COLUMN_LEVEL_LENGTH = 50;
        public const int COLUMN_METHOD_LENGTH = 255;
        public const int COLUMN_MACHINENAME_LENGTH = 32;
        public const int COLUMN_EXCEPTION_LENGTH = 8000;
        public const int COLUMN_PARAMETERS_LENGTH = 4000;
        #endregion

        public static LoggerConfiguration CreateDefault()
        {
            //// Serilog exceptions are written to self log, visible only under debug.
            //// Check that your VS write to debug by
             Debug.WriteLine("Hello World.");
             Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            return new LoggerConfiguration()
                .MinimumLevel.Verbose();
        }

        public static LoggerConfiguration AddLoggingToSQL(
            this LoggerConfiguration configuration,
            string logDbConnectionString,
            string logTableName,
            int insertBatchSize,
            TimeSpan insertPeriod)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrWhiteSpace(logDbConnectionString)) throw new ArgumentNullException(nameof(logDbConnectionString));
            if (string.IsNullOrWhiteSpace(logTableName)) throw new ArgumentNullException(nameof(logTableName));
            if (insertBatchSize < 1) throw new ArgumentException("Value less than 1 is not supported.", nameof(insertBatchSize));

            return configuration
                .WriteTo.MSSqlServer(
                    logDbConnectionString,
                    logTableName,
                    batchPostingLimit: insertBatchSize,
                    period: insertPeriod,
                    columnOptions: BuildColumnOptions());

            ColumnOptions BuildColumnOptions()
            {
                var columnOptions = new ColumnOptions();
                columnOptions.Store.Clear(); // Removing predefined by framework columns.
                columnOptions.AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "source", DataLength = COLUMN_SOURCE_LENGTH },
                    new SqlColumn { DataType = SqlDbType.DateTime, ColumnName = "timestamp" },
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "level", DataLength = COLUMN_LEVEL_LENGTH },
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "method", DataLength = COLUMN_METHOD_LENGTH },
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "machineName", DataLength = COLUMN_MACHINENAME_LENGTH },
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "message" }, // varchar(max) length.
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "exception", DataLength = COLUMN_EXCEPTION_LENGTH },
                    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "parameters", DataLength = COLUMN_PARAMETERS_LENGTH },
                    new SqlColumn { DataType = SqlDbType.UniqueIdentifier, ColumnName = "activityId" },
                };

                return columnOptions;
            }
        }
    }
}
