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
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionQualifications;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    /// <summary>
    /// Defines the administrative class BaseQualificationControl.
    /// </summary>
    [Serializable]
    public abstract class BaseQualificationControl : HccUserControl
    {
        /// <summary>
        /// Performs the administrative operation represented by LoadQualification.
        /// </summary>
        public Promotion Promotion { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by LoadQualification.
        /// </summary>
        public IPromotionQualification Qualification { get; set; }

        /// <summary>
        /// Performs the administrative operation represented by LoadQualification.
        /// </summary>
        public abstract void LoadQualification();
        public abstract bool SaveQualification();

        /// <summary>
        /// Defines the administrative class FriendlyBvinDisplay.
        /// </summary>
        protected bool UpdatePromotion()
        {
            return HccApp.MarketingServices.Promotions.Update(Promotion);
        }

        protected class FriendlyBvinDisplay
        {
            /// <summary>
            /// Gets or sets the bvin.
            /// </summary>
            public string bvin { get; set; }
            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            public string DisplayName { get; set; }
        }
    }
}
