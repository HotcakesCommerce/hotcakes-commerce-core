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
using System.Linq;
using System.Web;

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Represents AccountService data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public abstract class AccountService : HccServiceBase, IAccountService
    {
        /// <summary>Performs the AccountService operation.</summary>
        /// <param name="context">The context value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public AccountService(HccRequestContext context)
            : base(context)
        {
            ApiKeys = Factory.CreateRepo<ApiKeyRepository>(Context);
            Stores = Factory.CreateRepo<StoreRepository>(Context);
        }

        /// <summary>Gets or sets the ApiKeys value for this instance.</summary>
        /// <value>The current ApiKeys value. The meaning and persistence scope are defined by the containing type.</value>
        public ApiKeyRepository ApiKeys { get; protected set; }
        /// <summary>Gets or sets the Stores value for this instance.</summary>
        /// <value>The current Stores value. The meaning and persistence scope are defined by the containing type.</value>
        public StoreRepository Stores { get; protected set; }

        /// <summary>Creates a store for the current commerce context and applies the default store settings.</summary>
        /// <returns>The newly persisted <see cref="Store"/>.</returns>
        /// <remarks>The store is created through <see cref="Stores"/>, initialized with generated encryption material and other defaults, then updated so its settings are persisted. If creation fails, a <see cref="CreateStoreException"/> is thrown.</remarks>
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
        /// <summary>Resolves a store from a request URL or host name.</summary>
        /// <param name="url">The URL or host name to normalize and match.</param>
        /// <returns>The matching store, or <see langword="null"/> when the input is blank or no custom URL/domain matches.</returns>
        /// <remarks>The lookup first checks the store custom URL and then alternate domains. Matching is case-insensitive after trimming and lower-casing.</remarks>
        public virtual Store GetStoreByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var host = url.Trim().ToLowerInvariant();

            var storeId = Stores.FindStoreIdByCustomUrl(host);

            if (storeId < 1)
            {
                // Check other custom domains
                var repo = new StoreDomainRepository(Context);
                var possible = repo.FindForAnyStoreByDomain(host);
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

        /// <summary>Gets the store associated with the current HTTP request host.</summary>
        /// <returns>The current <see cref="Store"/>, or <see langword="null"/> when no request URL or matching store exists.</returns>
        /// <remarks>This method resolves the request's DNS-safe host through <see cref="GetStoreByUrl(string)"/> and therefore supports alternate store domains.</remarks>
        public virtual Store GetCurrentStore()
        {
            var context = HttpContext.Current;
            if (context?.Request?.Url == null) return null;

            var host = context.Request.Url.DnsSafeHost.ToLowerInvariant();
            return GetStoreByUrl(host);
        }

        /// <summary>Gets Google Analytics settings for the current store.</summary>
        /// <returns>A settings object populated from the current store; when the store cannot be found, the default-initialized object is returned.</returns>
        /// <remarks>The read uses the store repository cache and the serialized keys <c>UseGoogleTracker</c> and <c>GoogleTrackerId</c>.</remarks>
        public virtual GoogleAnalyticsSettings GetGoogleAnalyticsSettings()
        {
            var store = Stores.FindByIdWithCache(Context.CurrentStore.Id);
            var result = new GoogleAnalyticsSettings();
            if (store == null) return result;

            result.UseTracker = store.Settings.GetPropBool("UseGoogleTracker");
            result.TrackerId = store.Settings.GetProp("GoogleTrackerId");
            return result;
        }

        /// <summary>Persists Google Analytics settings for the current store.</summary>
        /// <param name="newSettings">The settings to save. This value cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newSettings"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the current store cannot be found.</exception>
        /// <remarks>The values are saved under <c>UseGoogleTracker</c> and <c>GoogleTrackerId</c>; repository update invalidates the store cache.</remarks>
        public virtual void SetGoogleAnalyticsSettings(GoogleAnalyticsSettings newSettings)
        {
            if (newSettings == null) throw new ArgumentNullException(nameof(newSettings));

            var store = Stores.FindById(Context.CurrentStore.Id);
            if (store == null) throw new InvalidOperationException("Current store could not be found.");

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
            var context = HttpContext.Current;
            string host = "localhost";

            if (context?.Request?.Url != null)
            {
                try
                {
                    host = context.Request.Url.DnsSafeHost.ToLowerInvariant();
                }
                catch
                {
                    host = "localhost";
                }
            }

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
            if (s == null) throw new ArgumentNullException(nameof(s));

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
            var newKey = Utilities.RandomNumbers.Create16DigitString();
            s.Settings.AesKey = newKey;
            s.Settings.AesInitVector = newKey;
        }

        #endregion
    }
}