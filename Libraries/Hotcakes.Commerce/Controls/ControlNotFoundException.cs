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

namespace Hotcakes.Commerce.Controls
{
    /// <summary>
    /// Represents an application error raised when a requested control cannot be found.
    /// </summary>
    /// <remarks>
    /// This exception carries no additional state beyond <see cref="ApplicationException"/>.
    /// Callers should use the message and, when supplied, the inner exception to diagnose
    /// the missing control; construction itself does not perform a lookup.
    /// </remarks>
    public class ControlNotFoundException : ApplicationException
    {
        /// <summary>
        /// Initializes an instance with the default exception message.
        /// </summary>
        public ControlNotFoundException()
        {
        }

        /// <summary>
        /// Initializes an instance with a descriptive message.
        /// </summary>
        /// <param name="message">The message that describes the missing control condition.</param>
        public ControlNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes an instance with a descriptive message and the exception that caused it.
        /// </summary>
        /// <param name="message">The message that describes the missing control condition.</param>
        /// <param name="innerException">The exception that caused the current exception, or <see langword="null"/> if there is no underlying exception.</param>
        public ControlNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
