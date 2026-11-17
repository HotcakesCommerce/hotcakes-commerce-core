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

namespace Hotcakes.Commerce.Controls
{
    /// <summary>
    /// Provides the account identifiers associated with a completed login operation.
    /// </summary>
    /// <remarks>
    /// Instances are mutable data-transfer objects. Both values are initialized to an
    /// empty string, and the setters do not validate or normalize assigned values.
    /// </remarks>
    public class LoginCompleteEventArgs
    {
        private string _UserEmail = string.Empty;
        private string _userId = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the user whose login completed.
        /// </summary>
        /// <value>The user identifier, or the current value assigned by the caller.</value>
        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        /// <summary>
        /// Gets or sets the email address associated with the completed login.
        /// </summary>
        /// <value>The user email address, or the current value assigned by the caller.</value>
        public string UserEmail
        {
            get { return _UserEmail; }
            set { _UserEmail = value; }
        }
    }
}
