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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    /// <summary>Issues reward points to the current affiliate or that affiliate's referrer.</summary>
    /// <remarks>The compatibility-sensitive class name and <c>Ajustment</c> spelling are retained.</remarks>
    public class RewardPointsAjustment : PromotionActionBase
    {
        /// <summary>Identifies the recipient of issued reward points.</summary>
        public enum RecipientType
        {
            /// <summary>Issue points to the current affiliate.</summary>
            Self = 0,
            /// <summary>Issue points to the current affiliate's enabled referral affiliate.</summary>
            ReferralAffiliate = 1
        }

        /// <summary>Stable serialized identifier for the reward-points adjustment action.</summary>
        public const string TypeIdString = "3875f348-d0e4-4fd9-8d25-e5c506e50d00";

        // Cache the parsed Guid to avoid repeated allocations
        private static readonly Guid _typeId = new Guid(TypeIdString);

        #region Properties

        /// <summary>Gets the stable serialized action identifier.</summary>
        public override Guid TypeId
        {
            get { return _typeId; }
        }

        /// <summary>Gets or sets the reward-points recipient mode.</summary>
        public RecipientType Recipient
        {
            get { return (RecipientType) GetSettingAsInt("Recipient"); }
            set { SetSetting("Recipient", (int) value); }
        }

        /// <summary>Gets or sets the number of points to issue.</summary>
        public int Amount
        {
            get { return GetSettingAsInt("Amount"); }
            set { SetSetting("Amount", value); }
        }

        #endregion

        #region Constructor

        /// <summary>Initializes an action that issues zero points to the current affiliate.</summary>
        public RewardPointsAjustment()
            : this(RecipientType.Self, 0)
        {
        }

        /// <summary>Initializes a reward-points action with the specified recipient and amount.</summary>
        /// <param name="type">The recipient mode.</param>
        /// <param name="amount">The number of points to issue.</param>
        public RewardPointsAjustment(RecipientType type, int amount)
        {
            Id = -1;
            Settings = new Dictionary<string, string>();
            Amount = amount;
            Recipient = type;
        }

        #endregion

        #region Public methods

        /// <summary>Describes the reward-points recipient and amount.</summary>
        /// <param name="app">The application context; it is not used by this implementation.</param>
        /// <returns>The action description.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            if (Recipient == RecipientType.Self)
                return $"Issue <strong>{Amount}</strong> Reward Points to Affiliate";
            return $"Issue <strong>{Amount}</strong> Reward Points to Referral Affiliate";
        }

        /// <summary>Issues points for an affiliate promotion when the configured recipient is eligible.</summary>
        /// <param name="context">The affiliate promotion context.</param>
        /// <returns>The result returned by point issuance, or <see langword="false"/> when eligibility checks fail.</returns>
        /// <remarks>Referral issuance requires numeric current-customer identity and enabled current and referral affiliates.</remarks>
        public override bool ApplyAction(PromotionContext context)
        {
            if (context?.CurrentCustomer == null) return false;
            if (context.Mode != PromotionType.Affiliate) return false;

            if (Recipient == RecipientType.Self)
            {
                return context.HccApp.CustomerPointsManager.IssuePoints(context.CurrentCustomer.Bvin, Amount);
            }

            if (Recipient == RecipientType.ReferralAffiliate)
            {
                if (!int.TryParse(context.CurrentCustomer.Bvin, out var userId)) return false;

                var aff = context.HccApp.ContactServices.Affiliates.FindByUserId(userId);
                if (aff?.Enabled != true) return false;

                var refAff = context.HccApp.ContactServices.Affiliates.FindByAffiliateId(aff.ReferralAffiliateId);
                if (refAff?.Enabled != true) return false;

                return context.HccApp.CustomerPointsManager.IssuePoints(refAff.UserId.ToString(), Amount);
            }

            return false;
        }

        #endregion
    }
}