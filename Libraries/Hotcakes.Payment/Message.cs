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

namespace Hotcakes.Payment
{
    /// <summary>
    /// Represents a description, code, and severity returned by a payment operation.
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Initializes a new <see cref="Message"/> with empty text and <see cref="MessageType.Unknown"/> severity.
        /// </summary>
        public Message()
        {
            Description = string.Empty;
            Code = string.Empty;
            Severity = MessageType.Unknown;
        }

        /// <summary>
        /// Initializes a new <see cref="Message"/> with the specified details.
        /// </summary>
        /// <param name="description">The human-readable message description.</param>
        /// <param name="code">The provider or gateway message code.</param>
        /// <param name="severity">The classification of the message.</param>
        public Message(string description, string code, MessageType severity)
        {
            Description = description;
            Code = code;
            Severity = severity;
        }

        /// <summary>Gets or sets the human-readable message description.</summary>
        public string Description { get; set; }
        /// <summary>Gets or sets the provider or gateway message code.</summary>
        public string Code { get; set; }
        /// <summary>Gets or sets the message classification.</summary>
        /// <seealso cref="MessageType"/>
        public MessageType Severity { get; set; }
    }
}
