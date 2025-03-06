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
using log4net;

namespace Hotcakes.Commerce.NoCms
{
    public class MvcEventLog : ILogger
    {
        public void LogMessage(string message)
        {
            LogMessage("Logger STORE 0", message, EventLogSeverity.Information);
        }

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

        public void LogException(Exception ex)
        {
            LogException(ex, EventLogSeverity.Error);
        }

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