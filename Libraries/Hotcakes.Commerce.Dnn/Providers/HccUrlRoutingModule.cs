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
using System.Net;
using System.Web;
using System.Web.Routing;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce.Dnn.Providers
{
    [Serializable]
    public class HccUrlRoutingModule : UrlRoutingModule
    {
        private static readonly object _contextKey = new object();

        protected override void Init(HttpApplication application)
        {
            application.BeginRequest += RegisterViewEngines;

            application.PostResolveRequestCache += InitServicePointManager;
            application.PostResolveRequestCache += InitializeHccContext;

            if (application.Context.Items[_contextKey] == null)
            {
                application.Context.Items[_contextKey] = _contextKey;
                application.PostResolveRequestCache += OnApplicationPostResolveRequestCache;
            }
        }

        private void RegisterViewEngines(object sender, EventArgs e)
        {
            MvcUtils.RegisterViewEngines();
        }

        private void InitServicePointManager(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
        }

        private void InitializeHccContext(object sender, EventArgs e)
        {
            var context = new HccRequestContext();

            var membershipServices = Factory.CreateService<MembershipServices>(context);
            var accountServices = Factory.CreateService<AccountService>(context);

            context.CurrentStore = accountServices.GetCurrentStore();
            var customerId = membershipServices.GetCurrentUserId();

            if (!string.IsNullOrEmpty(customerId))
            {
                context.CurrentAccount = membershipServices.Customers.Find(customerId);
            }

            HccRequestContext.Current = context;
        }

        private void OnApplicationPostResolveRequestCache(object sender, EventArgs e)
        {
            HttpContextBase context = new HccHttpContextWrapper(((HttpApplication) sender).Context);
            PostResolveRequestCache(context);
        }
    }
}