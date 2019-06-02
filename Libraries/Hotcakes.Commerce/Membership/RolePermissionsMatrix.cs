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

namespace Hotcakes.Commerce.Membership
{
    public class RolePermissionsMatrix
    {
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