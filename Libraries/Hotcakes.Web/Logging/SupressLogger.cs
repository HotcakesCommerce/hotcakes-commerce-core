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
    /// <summary>Provides a console-backed implementation of <see cref="ILogger"/> that does not propagate logging failures.</summary>
    [Serializable]
    public class SupressLogger : ILogger
    {
        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message to write.</param>
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>Writes an exception message and stack trace to the console.</summary>
        /// <param name="ex">The exception to write.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="ex"/> is <see langword="null"/>.</exception>
        public void LogException(Exception ex)
        {
            Console.WriteLine(ex.Message + " | " + ex.StackTrace);
        }

        /// <summary>Writes a source-qualified message and its severity to the console.</summary>
        /// <param name="source">The component or source that produced the message.</param>
        /// <param name="message">The message to write.</param>
        /// <param name="severity">The severity assigned to the message.</param>
        public void LogMessage(string source, string message, EventLogSeverity severity)
        {
            Console.WriteLine(source + " | " + message);
        }

        /// <summary>Writes an exception message and stack trace to the console with its severity.</summary>
        /// <param name="ex">The exception to write.</param>
        /// <param name="severity">The severity assigned to the exception.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="ex"/> is <see langword="null"/>.</exception>
        public void LogException(Exception ex, EventLogSeverity severity)
        {
            Console.WriteLine(ex.Message + " | " + ex.StackTrace);
        }
    }
}
