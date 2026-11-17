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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Provides the administrative-role settings façade owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Each value is scoped to the current store and is staged in the parent façade when set. The caller controls when the staged settings are persisted.</remarks>
    [Serializable]
    public class StoreSettingsAdminRoles
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new administrative-role façade for <paramref name="s"/>.</summary>
        /// <param name="s">The store settings façade that owns the settings.</param>
        public StoreSettingsAdminRoles(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets the role name allowed to manage the catalog.</summary>
        /// <value>The value persisted under <c>RoleCatalogManagement</c>.</value>
        public string RoleCatalogManagement
        {
            get { return parent.GetProp("RoleCatalogManagement"); }
            set { parent.SetProp("RoleCatalogManagement", value); }
        }

        /// <summary>Gets or sets the role name allowed to manage orders and customers.</summary>
        /// <value>The value persisted under <c>RoleOrdersAndCustomers</c>.</value>
        public string RoleOrdersAndCustomers
        {
            get { return parent.GetProp("RoleOrdersAndCustomers"); }
            set { parent.SetProp("RoleOrdersAndCustomers", value); }
        }

        /// <summary>Gets or sets the role name allowed to administer the store.</summary>
        /// <value>The value persisted under <c>RoleStoreAdministration</c>.</value>
        public string RoleStoreAdministration
        {
            get { return parent.GetProp("RoleStoreAdministration"); }
            set { parent.SetProp("RoleStoreAdministration", value); }
        }

        /// <summary>Gets or sets the role name allowed to access the store administration experience on mobile devices.</summary>
        /// <value>The value persisted under <c>RoleMobileAccess</c>.</value>
        public string RoleMobileAccess
        {
            get { return parent.GetProp("RoleMobileAccess"); }
            set { parent.SetProp("RoleMobileAccess", value); }
        }
    }
}
