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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>
    ///     Used to store Google analytics tracking configuration. Represents GoogleAnalyticsSettings data and behavior within the Hotcakes Commerce account and store boundary.
    /// </summary>
    [Serializable]
    public class GoogleAnalyticsSettings
    {
        /// <summary>Gets or sets UseTracker.</summary>
        /// <remarks>Whether Google Analytics tracking is enabled for the current store.</remarks>
        /// <value>Whether Google Analytics tracking is enabled for the current store.</value>
        public bool UseTracker { get; set; }
        /// <summary>Gets or sets TrackerId.</summary>
        /// <remarks>The Google Analytics tracker identifier stored with the current store settings.</remarks>
        /// <value>The Google Analytics tracker identifier stored with the current store settings.</value>
        public string TrackerId { get; set; }
    }
}
