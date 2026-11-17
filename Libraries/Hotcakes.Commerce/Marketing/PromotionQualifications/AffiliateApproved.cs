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
using System.Text;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Matches customers whose affiliate account is approved and enabled.</summary>
    /// <remarks>
    /// When <see cref="HasReffferalID"/> (or its correctly spelled alias <see cref="HasReferralId"/>) is enabled,
    /// the affiliate's referrer must also be found and enabled. The historic setting key and misspelled property
    /// are preserved for serialized compatibility.
    /// </remarks>
    public class AffiliateApproved : PromotionQualificationBase
    {
        /// <summary>Stable serialized type identifier for this qualification.</summary>
        public const string TypeIdString = "BC68C7FB-A1A5-4CBB-81BE-16DF5FEA780C";
            
        #region Constructor

        /// <summary>Initializes the qualification with a lower relative processing cost.</summary>
        public AffiliateApproved()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        #endregion

        #region Properties

        /// <summary>Gets the stable serialized type identifier for this qualification.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        /// <summary>
        /// Gets or sets whether the current affiliate must have a referral affiliate.
        /// </summary>
        /// <remarks>The historic <c>HasReffferalID</c> setting key and property spelling are intentionally preserved.</remarks>
        public bool HasReffferalID
        {
            get { return GetSettingAsBool("HasReffferalID"); }
            set { SetSetting("HasReffferalID", value); }
        }

        /// <summary>Gets or sets the correctly spelled alias for <see cref="HasReffferalID"/>.</summary>
        /// <remarks>This convenience property maps to the same persisted setting and does not change matching behavior.</remarks>
        public bool HasReferralId
        {
            get { return HasReffferalID; }
            set { HasReffferalID = value; }
        }

        #endregion

        #region Public methods

        /// <summary>Builds the display description, including whether a referral is required.</summary>
        /// <param name="app">The application context; it is not used by this qualification.</param>
        /// <returns>The qualification description as display text.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var sb = new StringBuilder();

            sb.Append("When Affiliate is Approved");

            if (HasReffferalID)
            {
                sb.Append(" and Has Referral");
            }

            return sb.ToString();
        }

        /// <summary>Checks the current customer's affiliate approval, enabled state, and optional referral.</summary>
        /// <param name="context">The promotion context containing the current customer and affiliate services.</param>
        /// <param name="mode">The requested promotion evaluation mode; this rule does not branch on the mode.</param>
        /// <returns>
        /// <c>true</c> when the customer has a non-empty numeric customer identifier, the corresponding affiliate
        /// is approved and enabled, and, when configured, its referral affiliate is found and enabled; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>The implementation converts the customer BVIN with <see cref="Convert.ToInt32(object)"/> and therefore retains its existing numeric-ID assumption.</remarks>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.CurrentCustomer == null) return false;
            if (string.IsNullOrEmpty(context.CurrentCustomer.Bvin)) return false;

            var userId = Convert.ToInt32(context.CurrentCustomer.Bvin);
            var affiliate = context.HccApp.ContactServices.Affiliates.FindByUserId(userId);

            if (HasReffferalID)
            {
                var refAffiliate =
                    context.HccApp.ContactServices.Affiliates.FindByAffiliateId(affiliate.ReferralAffiliateId);
                return refAffiliate != null && refAffiliate.Enabled && affiliate.Approved && affiliate.Enabled;
            }

            return affiliate.Approved && affiliate.Enabled;
        }

        #endregion
    }
}