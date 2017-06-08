#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
	public class HccRequestContextUtils
	{
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

		public static void UpdateAdminContentCulture(HccRequestContext requestContext)
		{
			var locale = GetAdminContentCulture();

			requestContext.MainContentCulture = locale.Code;
			requestContext.FallbackContentCulture = locale.Fallback;

			UpdateContextStore(requestContext);
		}

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
