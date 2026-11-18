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

using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Utilities
{
	/// <summary>
	/// Provides helpers for selecting and applying the content culture associated with an HCC request.
	/// </summary>
	public class HccRequestContextUtils
	{
		/// <summary>
		/// Gets the administrator content culture from the current session, falling back to the page locale.
		/// </summary>
		/// <returns>The resolved administrator <see cref="HccLocale"/>.</returns>
		/// <remarks>
		/// When the session value is missing or does not resolve to a configured locale, this method stores the
		/// page locale name in <see cref="SessionManager.AdminCurrentLanguage"/> before returning its locale.
		/// </remarks>
		public static HccLocale GetAdminContentCulture()
		{
			var contentCulture = SessionManager.AdminCurrentLanguage;
			var storeSettingsProvider = Factory.Instance.CreateStoreSettingsProvider();
			if (!string.IsNullOrEmpty(contentCulture))
			{
				var locale = storeSettingsProvider.GetLocale(contentCulture);
				if (locale != null)
					return locale;
			}
			// if contentCulture or locale is null
			contentCulture = storeSettingsProvider.GetPageLocale().Name;
			SessionManager.AdminCurrentLanguage = contentCulture;
			return storeSettingsProvider.GetLocale(contentCulture);
		}

		/// <summary>
		/// Applies the administrator content culture to a request context and refreshes its current store.
		/// </summary>
		/// <param name="requestContext">The request context to update.</param>
		/// <remarks>
		/// The culture is obtained from <see cref="GetAdminContentCulture"/>. The context's main and fallback
		/// cultures are set, and the store is reloaded through the repository cache when a current store exists.
		/// </remarks>
		public static void UpdateAdminContentCulture(HccRequestContext requestContext)
		{
			var locale = GetAdminContentCulture();

			requestContext.MainContentCulture = locale.Code;
			requestContext.FallbackContentCulture = locale.Fallback;

			UpdateContextStore(requestContext);
		}

		/// <summary>
		/// Applies the current page locale to a user request context and refreshes its current store.
		/// </summary>
		/// <param name="requestContext">The request context to update; <see langword="null"/> is ignored.</param>
		/// <remarks>
		/// The page locale supplies both the main locale and its configured fallback. If the context has a current
		/// store, that store is reloaded through the repository cache.
		/// </remarks>
		public static void UpdateUserContentCulture(HccRequestContext requestContext)
		{
			if (requestContext != null)
			{
				var storeSettingsProvider = Factory.Instance.CreateStoreSettingsProvider();
			    var contentCulture = storeSettingsProvider.GetPageLocale().Name;
				var locale = storeSettingsProvider.GetLocale(contentCulture);

                requestContext.MainContentCulture = locale.Code;
				requestContext.FallbackContentCulture = locale.Fallback;

				UpdateContextStore(requestContext);
			}
		}

		/// <summary>
		/// Creates a request context using the specified culture while preserving the account and routing context.
		/// </summary>
		/// <param name="requestContext">The source context whose account, routing context, and store identify the new context.</param>
		/// <param name="culture">The culture code to use for both the main and fallback content cultures.</param>
		/// <returns>A new context with the specified culture and the source context's current store.</returns>
		/// <remarks>
		/// The store is looked up through <see cref="StoreRepository"/> using the source store identifier; the source
		/// context itself is not modified.
		/// </remarks>
		public static HccRequestContext GetContextWithCulture(HccRequestContext requestContext, string culture)
		{
			var newRequestContext = new HccRequestContext();
			newRequestContext.MainContentCulture = culture;
			newRequestContext.FallbackContentCulture = culture;
			newRequestContext.CurrentAccount = requestContext.CurrentAccount;
			newRequestContext.RoutingContext = requestContext.RoutingContext;

			var storeRepo = Factory.CreateRepo<StoreRepository>(newRequestContext);
			newRequestContext.CurrentStore = storeRepo.FindByIdWithCache(requestContext.CurrentStore.Id);

			return newRequestContext;
		}

		private static void UpdateContextStore(HccRequestContext requestContext)
		{
			if (requestContext.CurrentStore != null)
			{
                var storeRepo = Factory.CreateRepo<StoreRepository>(requestContext);                
				var store = storeRepo.FindByIdWithCache(requestContext.CurrentStore.Id);
				requestContext.CurrentStore = store;
			}
		}
	}
}

