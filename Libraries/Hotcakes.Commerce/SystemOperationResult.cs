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

namespace Hotcakes.Commerce
{
    /// <summary>
    /// Carries an operation success flag and its unnormalized message.
    /// </summary>
    public class SystemOperationResult
    {
        private string _Message = string.Empty;

        /// <summary>
        /// Initializes an unsuccessful result with an empty message.
        /// </summary>
        public SystemOperationResult()
        {
            Success = false;
        }

        /// <summary>
        /// Public API member: <c>public SystemOperationResult(bool resultSuccess, string resultMessage)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public SystemOperationResult(bool resultSuccess, string resultMessage)
        {
            Success = resultSuccess;
            _Message = resultMessage;
        }

        /// <summary>
        /// Public API member: <c>public bool Success { get; set; }</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Public API member: <c>public string Message</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
    }
}
