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
    /// <summary>Provides Twitter settings owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Values are store-scoped and staged through the parent façade. The historical default tweet text is retained for compatibility.</remarks>
    [Serializable]
    public class StoreSettingsTwitter
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new Twitter settings façade.</summary>
        /// <param name="s">The owning store settings façade.</param>
        public StoreSettingsTwitter(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether Twitter integration is enabled.</summary>
        /// <value>The <c>UseTwitter</c> setting; the compatibility default is <see langword="true"/>.</value>
        public bool UseTwitter
        {
            get { return parent.GetPropBoolWithDefault("UseTwitter", true); }
            set { parent.SetProp("UseTwitter", value); }
        }

        /// <summary>Gets or sets the store's Twitter handle.</summary>
        /// <value>The <c>TwitterHandle</c> setting.</value>
        public string TwitterHandle
        {
            get { return parent.GetProp("TwitterHandle"); }
            set { parent.SetProp("TwitterHandle", value); }
        }

        /// <summary>Gets or sets the default tweet text template.</summary>
        /// <value>The <c>DefaultTweetText</c> setting; an empty stored value is exposed as <c>Check This Out: {0} </c>.</value>
        public string DefaultTweetText
        {
            get
            {
                var prop = parent.GetProp("DefaultTweetText");
                if (prop == string.Empty)
                {
                    prop = "Check This Out: {0} ";
                }
                return prop;
            }
            set { parent.SetProp("DefaultTweetText", value); }
        }
    }
}
