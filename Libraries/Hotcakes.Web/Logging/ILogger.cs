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

namespace Hotcakes.Web.Logging
{
    /// <summary>Defines the logging operations used by Hotcakes Web components.</summary>
    public interface ILogger
    {
        /// <summary>Writes a message to the log.</summary>
        /// <param name="message">The message to write.</param>
        void LogMessage(string message);
        /// <summary>Writes a source-qualified message at the specified <see cref="EventLogSeverity"/>.</summary>
        /// <param name="source">The component or source that produced the message.</param><param name="message">The message to write.</param><param name="severity">The severity assigned to the message.</param>
        void LogMessage(string source, string message, EventLogSeverity severity);
        /// <summary>Writes an exception to the log.</summary>
        /// <param name="ex">The exception to write.</param>
        void LogException(Exception ex);
        /// <summary>Writes an exception at the specified <see cref="EventLogSeverity"/>.</summary>
        /// <param name="ex">The exception to write.</param><param name="severity">The severity assigned to the exception.</param>
        void LogException(Exception ex, EventLogSeverity severity);
    }
}
