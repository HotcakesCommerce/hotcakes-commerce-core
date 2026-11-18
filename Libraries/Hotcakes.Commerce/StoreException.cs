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

namespace Hotcakes.Commerce
{
    /// <summary>
    /// Public API member: <c>public enum ViewMessageType</c>. This member preserves the established Hotcakes Commerce contract and behavior.
    /// </summary>
    public enum ViewMessageType
    {
        /// <summary>
        /// Compatibility value <c>Success</c> in this public enumeration.
        /// </summary>
        Success = 0,
        /// <summary>
        /// Compatibility value <c>Warning</c> in this public enumeration.
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Compatibility value <c>Error</c> in this public enumeration.
        /// </summary>
        Error = 2,
        /// <summary>
        /// Compatibility value <c>Info</c> in this public enumeration.
        /// </summary>
        Info = 3
    }

    /// <summary>
    /// Exception carrying presentation header, content, and <see cref="ViewMessageType"/> metadata.
    /// </summary>
    public class StoreException : Exception
    {
        /// <summary>
        /// Public API member: <c>public StoreException() : this(null)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public StoreException() : this(null)
        {
        }

        /// <summary>
        /// Public API member: <c>public StoreException(string content) : this(null, content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public StoreException(string content) : this(null, content)
        {
        }

        /// <summary>
        /// Public API member: <c>public StoreException(string header, string content) : this(header, content, ViewMessageType.Info)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public StoreException(string header, string content) : this(header, content, ViewMessageType.Info)
        {
        }

        /// <summary>
        /// Initializes presentation metadata without changing the base exception message contract.
        /// </summary>
        public StoreException(string header, string content, ViewMessageType type)
        {
            Header = header;
            Content = content;
            Type = type;
        }

        /// <summary>
        /// Public API member: <c>public string Header { get; set; }</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Public API member: <c>public string Content { get; set; }</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Public API member: <c>public ViewMessageType Type { get; set; }</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public ViewMessageType Type { get; set; }
    }
}
