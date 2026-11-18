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
    /// <summary>Provides the store's Facebook settings façade.</summary>
    /// <remarks>The class name and setting spellings retain their historical <c>FaceBook</c> compatibility form. Values are store-scoped and staged through <see cref="StoreSettings"/> until saved.</remarks>
    [Serializable]
    public class StoreSettingsFaceBook
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new Facebook settings façade.</summary>
        /// <param name="s">The owning store settings façade.</param>
        public StoreSettingsFaceBook(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether Facebook integration is enabled.</summary>
        /// <value>The <c>UseFaceBook</c> setting; the compatibility default is <see langword="true"/>.</value>
        public bool UseFaceBook
        {
            get { return parent.GetPropBoolWithDefault("UseFaceBook", true); }
            set { parent.SetProp("UseFaceBook", value); }
        }

        /// <summary>Gets or sets the Facebook application identifier.</summary>
        /// <value>The <c>FaceBookAppId</c> setting.</value>
        public string AppId
        {
            get { return parent.GetProp("FaceBookAppId"); }
            set { parent.SetProp("FaceBookAppId", value); }
        }

        /// <summary>Gets or sets the Facebook administrator identifiers.</summary>
        /// <value>The <c>FaceBookAdmins</c> setting.</value>
        public string Admins
        {
            get { return parent.GetProp("FaceBookAdmins"); }
            set { parent.SetProp("FaceBookAdmins", value); }
        }
    }
}
