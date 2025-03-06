#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class RewardPointsAjustment : PromotionActionBase
    {
        public enum RecipientType
        {
            Self = 0,
            ReferralAffiliate = 1
        }

        public const string TypeIdString = "3875f348-d0e4-4fd9-8d25-e5c506e50d00";

        #region Properties

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public RecipientType Recipient
        {
            get { return (RecipientType) GetSettingAsInt("Recipient"); }
            set { SetSetting("Recipient", (int) value); }
        }

        public int Amount
        {
            get { return GetSettingAsInt("Amount"); }
            set { SetSetting("Amount", value); }
        }

        #endregion

        #region Constructor

        public RewardPointsAjustment()
            : this(RecipientType.Self, 0)
        {
        }

        public RewardPointsAjustment(RecipientType type, int amount)
        {
            Id = -1;
            Settings = new Dictionary<string, string>();
            Amount = amount;
            Recipient = type;
        }

        #endregion

        #region Public methods

        public override string FriendlyDescription(HotcakesApplication app)
        {
            if (Recipient == RecipientType.Self)
                return string.Format("Issue <strong>{0}</strong> Reward Points to Affiliate", Amount);
            return string.Format("Issue <strong>{0}</strong> Reward Points to Referral Affiliate", Amount);
        }

        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.CurrentCustomer == null) return false;
            if (context.Mode != PromotionType.Affiliate) return false;

            if (Recipient == RecipientType.Self)
            {
                return context.HccApp.CustomerPointsManager.IssuePoints(context.CurrentCustomer.Bvin, Amount);
            }
            if (Recipient == RecipientType.ReferralAffiliate)
            {
                var userId = Convert.ToInt32(context.CurrentCustomer.Bvin);
                var aff = context.HccApp.ContactServices.Affiliates.FindByUserId(userId);
                if (aff != null && aff.Enabled)
                {
                    var refAff = context.HccApp.ContactServices.Affiliates.FindByAffiliateId(aff.ReferralAffiliateId);
                    if (refAff != null && refAff.Enabled)
                    {
                        return context.HccApp.CustomerPointsManager.IssuePoints(refAff.UserId.ToString(), Amount);
                    }
                }
            }

            return false;
        }

        #endregion
    }
}