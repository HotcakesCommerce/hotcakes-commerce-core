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

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Maps system permissions to the configured administrative role for the current store.
    /// </summary>
    public class RolePermissionsMatrix
    {
        /// <summary>
        /// Gets the configured role name associated with a system permission.
        /// </summary>
        /// <param name="hccApp">The commerce application whose current store supplies the role configuration.</param>
        /// <param name="permission">The permission literal to map, such as <see cref="SystemPermissions.CatalogView"/>.</param>
        /// <returns>The configured role name for the recognized permission; otherwise, <see cref="string.Empty"/>. If no current store is available, returns <see cref="string.Empty"/>.</returns>
        /// <remarks>
        /// <see cref="SystemPermissions.CatalogView"/>, <see cref="SystemPermissions.ContentView"/>, and <see cref="SystemPermissions.MarketingView"/> map to the catalog-management role.
        /// <see cref="SystemPermissions.PeopleView"/>, <see cref="SystemPermissions.OrdersView"/>, <see cref="SystemPermissions.OrdersEdit"/>, and <see cref="SystemPermissions.ReportsView"/> map to the orders-and-customers role.
        /// <see cref="SystemPermissions.SettingsView"/> maps to the store-administration role. Other permission literals, including <see cref="SystemPermissions.PluginView"/>, are not mapped.
        /// </remarks>
        public static string GetRoleNameByPermission(HotcakesApplication hccApp, string permission)
        {
            if (hccApp.CurrentStore != null)
            {
                //var rType = _permissionToRoles[permission];
                var adminRoles = hccApp.CurrentStore.Settings.AdminRoles;

                switch (permission)
                {
                    case SystemPermissions.CatalogView:
                    case SystemPermissions.ContentView:
                    case SystemPermissions.MarketingView:
                        return adminRoles.RoleCatalogManagement;

                    case SystemPermissions.PeopleView:
                    case SystemPermissions.OrdersView:
                    case SystemPermissions.OrdersEdit:
                    case SystemPermissions.ReportsView:
                        return adminRoles.RoleOrdersAndCustomers;

                    case SystemPermissions.SettingsView:
                        return adminRoles.RoleStoreAdministration;

                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
