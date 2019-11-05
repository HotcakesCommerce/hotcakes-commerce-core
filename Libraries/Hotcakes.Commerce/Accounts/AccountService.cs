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
using System.Web;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public abstract class AccountService : HccServiceBase, IAccountService
    {
        public AccountService(HccRequestContext context)
            : base(context)
        {
            ApiKeys = Factory.CreateRepo<ApiKeyRepository>(Context);
            Stores = Factory.CreateRepo<StoreRepository>(Context);
        }

        #region

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public AccountService(HccRequestContext c, bool isForMemoryOnly)
            : this(c)
        {
        }

        #endregion

        public ApiKeyRepository ApiKeys { get; protected set; }
        public StoreRepository Stores { get; protected set; }

        /// <summary>
        ///     Create store. This method is generally used during the first time application
        ///     installation and during first step of the wizard
        /// </summary>
        /// <returns></returns>
        public virtual Store CreateAndSetupStore()
        {
            var s = CreateStore();

            if (!Stores.Create(s))
            {
                throw new CreateStoreException(
                    "Unable to create store. Unknown error. Please contact an administrator for assistance.");
            }

            if (s != null)
            {
                SetupDefaultStoreSettings(s);

                // Save data to store 
                Stores.Update(s);
            }

            return s;
        }

        //Get Store instance by given URL
        public virtual Store GetStoreByUrl(string url)
        {
            var storeId = Stores.FindStoreIdByCustomUrl(url);

            if (storeId < 1)
            {
                // Check other custom domains
                var repo = new StoreDomainRepository(Context);
                var possible = repo.FindForAnyStoreByDomain(url);
                if (possible != null && possible.StoreId > 0)
                {
                    storeId = possible.StoreId;
                }
            }

            if (storeId > 0)
            {
                return Stores.FindById(storeId);
            }
            return null;
        }

        /// <summary>
        ///     Get current store instance. Site can have multiple store and this method
        ///     returns the current store instance the user is accessing.
        /// </summary>
        /// <returns>Store instance</returns>
        public virtual Store GetCurrentStore()
        {
            var url = HttpContext.Current.Request.Url;
            var host = url.DnsSafeHost.ToLowerInvariant();
            return GetStoreByUrl(host);
        }

        /// <summary>
        ///     Get Google Analytics settings for a current store.
        /// </summary>
        /// <returns>Returns <see cref="GoogleAnalyticsSettings" /> instance</returns>
        public virtual GoogleAnalyticsSettings GetGoogleAnalyticsSettings()
        {
            var store = Stores.FindByIdWithCache(Context.CurrentStore.Id);
            var result = new GoogleAnalyticsSettings();
            result.UseTracker = store.Settings.GetPropBool("UseGoogleTracker");
            result.TrackerId = store.Settings.GetProp("GoogleTrackerId");
            return result;
        }

        /// <summary>
        ///     Set the google analytics information for the current store.
        /// </summary>
        /// <param name="newSettings"></param>
        public virtual void SetGoogleAnalyticsSettings(GoogleAnalyticsSettings newSettings)
        {
            var store = Stores.FindById(Context.CurrentStore.Id);
            store.Settings.SetProp("UseGoogleTracker", newSettings.UseTracker);
            store.Settings.SetProp("GoogleTrackerId", newSettings.TrackerId);
            Stores.Update(store);
        }

        #region Implementation

        /// <summary>
        ///     Create store information with the current hostname in URL.
        ///     <para>
        ///         This method generally called when user there is no host entry exist for
        ///         given url in the Hosts table.
        ///     </para>
        /// </summary>
        /// <returns></returns>
        protected virtual Store CreateStore()
        {
            var host = HttpContext.Current.Request.Url.DnsSafeHost.ToLowerInvariant();

            var s = new Store();
            s.StoreName = host;
            s.DateCreated = DateTime.UtcNow;
            s.CustomUrl = host;
            return s;
        }


        /// <summary>
        ///     Set default settings when creating new store.
        /// </summary>
        /// <param name="s"></param>
        protected virtual void SetupDefaultStoreSettings(Store s)
        {
            s.Settings.FriendlyName = "My Hotcakes Store";
            s.Settings.LogoRevision = 0;
            s.Settings.UseLogoImage = false;
            s.Settings.LogoText = "My Hotcakes Store";
            s.Settings.MinumumOrderAmount = 0;
            s.Settings.MailServer.UseCustomMailServer = false;
            s.Settings.ProductReviewCount = 3;
            s.Settings.ProductReviewModerate = true;
            s.Settings.AllowProductReviews = true;
            s.Settings.PayPal.Currency = "USD";
            s.Settings.MaxItemsPerOrder = 999;
            s.Settings.MaxWeightPerOrder = 9999;
        }

        #endregion
    }
}