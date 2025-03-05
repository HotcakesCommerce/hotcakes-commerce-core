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
using System.Globalization;
using DotNetNuke.Entities.Users;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Common.Dnn;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnContactService : ContactService
    {
        private readonly int _portalId;

        public DnnContactService(HccRequestContext context)
            : base(context)
        {
            _portalId = DnnGlobal.Instance.GetPortalId();
        }

        public override void UpdateProfileAffiliateId(long affiliateId)
        {
            var aff = Affiliates.Find(affiliateId);

            if (aff != null && aff.Enabled && aff.Approved)
            {
                var expireDate = DateTime.UtcNow;
                if (aff.ReferralDays > 0)
                {
                    expireDate = expireDate.AddDays(aff.ReferralDays);
                }
                else
                {
                    expireDate = DateTime.UtcNow.AddYears(50);
                }

                var dnnUser = DnnUserController.Instance.GetCurrentUserInfo();
                if (dnnUser != null)
                {
                    SetProfileAffiliateId(dnnUser, aff.Id, expireDate);
                }
            }
        }

        public override void SetAffiliateReferral(string affiliateId, string referralUrl)
        {
            var store = Context.CurrentStore;
            var currentAffId = GetCurrentAffiliateId();

            // Replace AffiliateId if Mode is Force New Affiliate
            var replaceAffiliateId = !currentAffId.HasValue ||
                                     (store.Settings.AffiliateConflictMode == AffiliateConflictMode.FavorNewAffiliate);

            if (replaceAffiliateId)
            {
                var aff = Affiliates.FindByAffiliateId(affiliateId);

                if (aff != null && aff.Enabled && aff.Approved)
                {
                    if (aff.Id != currentAffId)
                    {
                        var expireDate = DateTime.UtcNow;
                        if (aff.ReferralDays > 0)
                        {
                            expireDate = expireDate.AddDays(aff.ReferralDays);
                        }
                        else
                        {
                            expireDate = DateTime.UtcNow.AddYears(50);
                        }

                        var dnnUser = DnnUserController.Instance.GetCurrentUserInfo();
                        if (dnnUser != null)
                        {
                            SetProfileAffiliateId(dnnUser, aff.Id, expireDate);
                        }

                        SessionManager.SetCurrentAffiliateId(aff.Id, expireDate);
                    }

                    LogReferral(aff.Id, referralUrl);
                }
            }
            else
            {
                LogReferral(currentAffId.Value, referralUrl);
            }
        }

        public override long? GetCurrentAffiliateId()
        {
            long? affiliateId = null;

            var dnnUser = DnnUserController.Instance.GetCurrentUserInfo();
            if (dnnUser != null)
            {
                affiliateId = GetProfileAffiliateId(dnnUser);
            }

            if (!affiliateId.HasValue)
            {
                affiliateId = SessionManager.CurrentAffiliateID(Context.CurrentStore);
            }

            if (affiliateId.HasValue)
            {
                var aff = Affiliates.Find(affiliateId.Value);

                if (aff != null && aff.Enabled && aff.Approved)
                {
                    return affiliateId;
                }
            }

            return null;
        }

        #region Implementation

        private void SetProfileAffiliateId(UserInfo user, long affiliateId, DateTime expireDate)
        {
            user.Profile.EnsureProfileProperty("HccAffiliateID", "Hotcakes", _portalId, false);
            user.Profile.EnsureProfileProperty("HccAffiliateExpireDate", "Hotcakes", _portalId, false);
            user.Profile.SetProfileProperty("HccAffiliateID", affiliateId.ToString());
            user.Profile.SetProfileProperty("HccAffiliateExpireDate", expireDate.ToString(CultureInfo.InvariantCulture));
        }

        private long? GetProfileAffiliateId(UserInfo user)
        {
            var affid = user.Profile["HccAffiliateID"] as string;
            if (!string.IsNullOrEmpty(affid))
            {
                var expDateStr = user.Profile["HccAffiliateExpireDate"];
                var expDate = Convert.ToDateTime(expDateStr, CultureInfo.InvariantCulture);
                if (expDate > DateTime.UtcNow)
                {
                    return Convert.ToInt64(affid);
                }
            }

            return null;
        }

        #endregion
    }
}