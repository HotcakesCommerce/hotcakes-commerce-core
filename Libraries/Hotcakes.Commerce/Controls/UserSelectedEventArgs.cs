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
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce.Controls
{
    /// <summary>
    /// Provides the customer account selected by a user-selection control.
    /// </summary>
    /// <remarks>
    /// This event-argument object follows the <see cref="EventArgs"/> convention and
    /// exposes the selected account directly. The property is mutable and is not
    /// validated by this type; it is <see langword="null"/> until assigned.
    /// </remarks>
    public class UserSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the customer account selected by the control.
        /// </summary>
        /// <value>The selected <see cref="CustomerAccount"/>, or <see langword="null"/> when no account is selected.</value>
        public CustomerAccount UserAccount { get; set; }
    }
}
