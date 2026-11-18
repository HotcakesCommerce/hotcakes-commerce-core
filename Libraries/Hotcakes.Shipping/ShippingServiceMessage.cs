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

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Carries informational, diagnostic, or error details reported by a shipping service.
    /// </summary>
    /// <seealso cref="ShippingServiceMessageType" />
    [Serializable]
    public class ShippingServiceMessage
    {
        /// <summary>
        /// Initializes an informational message with empty code and description values.
        /// </summary>
        public ShippingServiceMessage()
        {
            MessageType = ShippingServiceMessageType.Information;
            Code = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Gets or sets the category of this message.
        /// </summary>
        public ShippingServiceMessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the service-defined code for this message.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the human-readable message description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Sets this message's type to <see cref="ShippingServiceMessageType.Error" /> and assigns its details.
        /// </summary>
        /// <param name="code">The service-defined error code.</param>
        /// <param name="description">The error description.</param>
        public void SetError(string code, string description)
        {
            MessageType = ShippingServiceMessageType.Error;
            Code = code;
            Description = description;
        }

        /// <summary>
        /// Sets this message's type to <see cref="ShippingServiceMessageType.Diagnostics" /> and assigns its details.
        /// </summary>
        /// <param name="code">The service-defined diagnostic code.</param>
        /// <param name="description">The diagnostic description.</param>
        public void SetDiagnostics(string code, string description)
        {
            MessageType = ShippingServiceMessageType.Diagnostics;
            Code = code;
            Description = description;
        }

        /// <summary>
        /// Sets this message's type to <see cref="ShippingServiceMessageType.Information" /> and assigns its details.
        /// </summary>
        /// <param name="code">The service-defined information code.</param>
        /// <param name="description">The information description.</param>
        public void SetInfo(string code, string description)
        {
            MessageType = ShippingServiceMessageType.Information;
            Code = code;
            Description = description;
        }
    }
}
