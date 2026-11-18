#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using Hotcakes.Web.Logging;
using log4net;

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Adapts the HCC event-logging contract to the log4net logger used by the MVC host.
    /// </summary>
    /// <remarks>
    /// Message logging uses the supplied source when one is provided, while exception logging
    /// uses the established <c>Hotcakes.System</c> logger. The severity mapping is intentionally
    /// retained by this adapter, including the existing treatment of fatal and none values.
    /// </remarks>
    public class MvcEventLog : ILogger
    {
        /// <summary>
        /// Logs an informational message using the adapter's default source.
        /// </summary>
        /// <param name="message">The message to write to the event log.</param>
        /// <remarks>This overload delegates to <see cref="LogMessage(string, string, EventLogSeverity)"/> with informational severity.</remarks>
        public void LogMessage(string message)
        {
            LogMessage("Logger STORE 0", message, EventLogSeverity.Information);
        }

        /// <summary>
        /// Logs a message with the specified source and severity.
        /// </summary>
        /// <param name="source">The log4net logger name to use.</param>
        /// <param name="message">The message to write to the event log.</param>
        /// <param name="severity">The event severity that determines the log4net method used.</param>
        /// <remarks>Fatal and none severities retain the adapter's established mappings.</remarks>
        public void LogMessage(string source, string message, EventLogSeverity severity)
        {
            var Log = LogManager.GetLogger(source);

            switch (severity)
            {
                case EventLogSeverity.Debug:
                    Log.Debug(message);
                    break;
                case EventLogSeverity.Error:
                    Log.Error(message);
                    break;
                case EventLogSeverity.Fatal:
                    Log.Error(message);
                    break;
                case EventLogSeverity.Information:
                    Log.Info(message);
                    break;
                case EventLogSeverity.None:
                    Log.Info(message);
                    break;
                case EventLogSeverity.Warning:
                    Log.Warn(message);
                    break;
            }
        }

        /// <summary>
        /// Logs an exception with error severity.
        /// </summary>
        /// <param name="ex">The exception to write to the event log.</param>
        /// <remarks>This overload delegates to <see cref="LogException(Exception, EventLogSeverity)"/> with error severity.</remarks>
        public void LogException(Exception ex)
        {
            LogException(ex, EventLogSeverity.Error);
        }

        /// <summary>
        /// Logs an exception with the specified severity.
        /// </summary>
        /// <param name="ex">The exception to write to the event log.</param>
        /// <param name="severity">The event severity that determines the log4net method used.</param>
        /// <remarks>Exceptions are written through the established <c>Hotcakes.System</c> logger; fatal and none severities retain their existing mappings.</remarks>
        public void LogException(Exception ex, EventLogSeverity severity)
        {
            var Log = LogManager.GetLogger("Hotcakes.System");
            switch (severity)
            {
                case EventLogSeverity.Debug:
                    Log.Debug(ex);
                    break;
                case EventLogSeverity.Error:
                    Log.Error(ex);
                    break;
                case EventLogSeverity.Fatal:
                    Log.Error(ex);
                    break;
                case EventLogSeverity.Information:
                    Log.Info(ex);
                    break;
                case EventLogSeverity.None:
                    Log.Info(ex);
                    break;
                case EventLogSeverity.Warning:
                    Log.Warn(ex);
                    break;
            }
        }
    }
}
