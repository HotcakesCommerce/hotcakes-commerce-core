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
using System.Linq;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.Controls;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    public class BaseCustomerPage : BaseAdminPage
    {
        public CustomerAccount Customer { get; set; }

        public virtual string CustomerId
        {
            get { return Request.QueryString["id"]; }
        }

        public virtual string ReturnUrl
        {
            get { return Request.QueryString["returnUrl"]; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.PeopleView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!string.IsNullOrEmpty(CustomerId))
            {
                Customer = HccApp.MembershipServices.Customers.Find(CustomerId);
            }
        }

        protected void InitNavMenu(NavMenu navMenu)
        {
            navMenu.ProcessItems = items => items
                .Select(i => new MenuItem
                {
                    Text = i.Text,
                    Url = Customer != null || i.Name == "Profile" ? i.Url + "?id=" + CustomerId : null
                })
                .ToList();
        }
    }
}