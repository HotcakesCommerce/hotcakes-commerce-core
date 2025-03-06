#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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

namespace Hotcakes.Commerce
{
    public class EventLog
    {
        /// <summary>
        ///     Records and event to the Hotcakes Commerce event log
        /// </summary>
        /// <param name="source">The source of the event</param>
        /// <param name="message">The description or information about the event</param>
        /// <param name="severity">The severity.</param>
        /// <returns>
        ///     True if the event was recorded, otherwise false
        /// </returns>
        public static bool LogEvent(string source, string message, EventLogSeverity severity)
        {
            Factory.CreateEventLogger().LogMessage(source, message, severity);
            return true;
        }

        /// <summary>
        ///     Logs an exception to the Hotcakes Commerce log
        /// </summary>
        /// <param name="ex">Exception to be recorded</param>
        /// <returns>True if the exception was recorded, otherwise false</returns>
        public static bool LogEvent(Exception ex)
        {
            Factory.CreateEventLogger().LogException(ex);
            return true;
        }

        public static bool LogEvent(Exception ex, EventLogSeverity severity)
        {
            Factory.CreateEventLogger().LogException(ex, severity);
            return true;
        }
    }
}