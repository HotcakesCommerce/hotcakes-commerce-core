#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    public class AffiliateApproved : PromotionQualificationBase
    {
        public const string TypeIdString = "BC68C7FB-A1A5-4CBB-81BE-16DF5FEA780C";

        #region Constructor

        public AffiliateApproved()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        #endregion

        #region Properties

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public bool HasReffferalID
        {
            get { return GetSettingAsBool("HasReffferalID"); }
            set { SetSetting("HasReffferalID", value); }
        }

        #endregion

        #region Public methods

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