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

using System.Web.Mvc;
using System.Web.Routing;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.NoCms.Urls
{
    internal class MvcHccUrlResolver : IHccUrlResolver
    {
        public string RouteHccUrl(HccRoute route, string actionName, string controllerName, string protocol,
            string hostName, string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection,
            RequestContext requestContext, bool includeImplicitMvcValues)
        {
            var routeName = GetRouteName(route);
            if (string.IsNullOrEmpty(hostName))
                hostName = GetHostName();
            return UrlHelper.GenerateUrl(routeName, null, null, protocol, hostName, null, routeValues, routeCollection,
                requestContext, false);
        }

        public string GetStoreDataVirtualPath(long storeId)
        {
            return string.Format("~/Images/sites/{0}/", storeId);
        }

        public string GetStoreRootUrl(Store store, bool secure)
        {
            var protocol = secure ? "https://" : "http://";
            return protocol + store.CustomUrl + "/";
        }

        private string GetRouteName(HccRoute route)
        {
            string routeName = null;
            switch (route)
            {
                case HccRoute.Product:
                case HccRoute.Category:
                    routeName = HccRouteNames.Custom;
                    break;
                case HccRoute.Cart:
                    routeName = HccRouteNames.Cart;
                    break;
                case HccRoute.ProductReview:
                    routeName = HccRouteNames.ProductReview;
                    break;
                case HccRoute.Checkout:
                    routeName = HccRouteNames.Checkout;
                    break;
                case HccRoute.CheckoutPayPal:
                    routeName = HccRouteNames.CheckoutPayPal;
                    break;
                case HccRoute.WishList:
                    routeName = HccRouteNames.WishList;
                    break;
                case HccRoute.Search:
                    routeName = HccRouteNames.Search;
                    break;
                case HccRoute.OrderHistory:
                case HccRoute.AddressBook:
                    routeName = HccRouteNames.AccountDefault;
                    break;
                case HccRoute.Login:
                    routeName = HccRouteNames.Login;
                    break;
                case HccRoute.Logoff:
                    routeName = HccRouteNames.Logoff;
                    break;
                case HccRoute.SendPassword:
                    routeName = HccRouteNames.SendPassword;
                    break;
                case HccRoute.Terms:
                    routeName = HccRouteNames.Terms;
                    break;
            }
            return routeName;
        }

        private string GetHostName()
        {
            return HccRequestContext.Current.CurrentStore.CustomUrl;
        }
    }
}