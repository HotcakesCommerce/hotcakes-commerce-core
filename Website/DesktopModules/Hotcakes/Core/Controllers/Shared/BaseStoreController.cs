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

using System;
using System.Web.Mvc;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Filters;

namespace Hotcakes.Modules.Core.Controllers.Shared
{
    [StoreClosedFilter]
    [Serializable]
    public class BaseStoreController : BaseAppController
    {
        /// <summary>
        ///     Gets the current cart.
        /// </summary>
        /// <value>
        ///     The current cart.
        /// </value>
        protected Order CurrentCart
        {
            get { return HccApp.OrderServices.CurrentShoppingCart(); }
        }

        /// <summary>
        ///     Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            RenderToBody("hccBottomAnalytics", HccApp.CurrentStore.Settings.Analytics.BottomAnalytics);
            RenderToHead("hccAdditionalMetaTags", HccApp.CurrentStore.Settings.Analytics.AdditionalMetaTags);
        }
    }
}