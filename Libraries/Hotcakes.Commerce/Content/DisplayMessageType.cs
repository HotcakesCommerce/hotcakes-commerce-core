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

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Defines the kinds of messages that can be displayed to a user.
    /// </summary>
    public enum DisplayMessageType
    {
        /// <summary>
        /// An informational message. The serialized numeric value is <c>0</c>.
        /// </summary>
        Information = 0,

        /// <summary>
        /// A message indicating that an operation completed successfully. The serialized numeric value is <c>1</c>.
        /// </summary>
        Success = 1,

        /// <summary>
        /// A warning message. The serialized numeric value is <c>2</c>.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// A message representing an exception condition. The serialized numeric value is <c>3</c>.
        /// </summary>
        Exception = 3,

        /// <summary>
        /// An error message. The serialized numeric value is <c>5</c>; the value <c>4</c> is intentionally unused.
        /// </summary>
        Error = 5
    }
}
