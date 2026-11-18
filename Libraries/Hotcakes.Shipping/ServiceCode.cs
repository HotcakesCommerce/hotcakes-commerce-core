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
    /// Represents a shipping service code and its display name.
    /// </summary>
    /// <seealso cref="IServiceCode" />
    [Serializable]
    public class ServiceCode : IServiceCode
    {
        /// <summary>
        /// Initializes an empty service code.
        /// </summary>
        public ServiceCode()
        {
            Code = string.Empty;
            DisplayName = string.Empty;
        }

        /// <summary>
        /// Initializes a service code with its display name and code.
        /// </summary>
        /// <param name="name">The name shown to users.</param>
        /// <param name="code">The provider's service code.</param>
        public ServiceCode(string name, string code)
        {
            Code = code;
            DisplayName = name;
        }

        /// <summary>
        /// Gets or sets the provider's service code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name shown to users for this service.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
