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
    /// Contains the stable permission literals used by Hotcakes Commerce system authorization.
    /// </summary>
    /// <remarks>
    /// These values are serialized and compared as strings. Do not change their casing, punctuation, or values.
    /// The role mapping exposed by <see cref="RolePermissionsMatrix.GetRoleNameByPermission(HotcakesApplication, string)"/> recognizes all literals except <see cref="PluginView"/>.
    /// </remarks>
    public class SystemPermissions
    {
        /// <summary>Permission literal for viewing catalog management.</summary>
        public const string CatalogView = "D0201557-356F-4ee6-A24C-3C0F85E2C472";
        /// <summary>Permission literal for viewing content management.</summary>
        public const string ContentView = "070F3D47-1182-4e08-92C4-F317544F0FA2";
        /// <summary>Permission literal for viewing marketing management.</summary>
        public const string MarketingView = "1085A525-F084-47df-B791-65DFA148D3C5";
        /// <summary>Permission literal for editing orders.</summary>
        public const string OrdersEdit = "4F708BA8-00B5-4171-B2AA-FC785319891D";
        /// <summary>Permission literal for viewing orders.</summary>
        public const string OrdersView = "D9900036-0794-4aa5-B662-28E99A23E9B1";
        /// <summary>Permission literal for viewing people and customer management.</summary>
        public const string PeopleView = "5519BC97-9CE4-4549-AFF1-56B6A54D449C";
        /// <summary>Permission literal for viewing plug-in management. This literal is not mapped by <see cref="RolePermissionsMatrix"/>.</summary>
        public const string PluginView = "403F3109-1A6D-48D6-8753-91EA59A8A318";
        /// <summary>Permission literal for viewing reports.</summary>
        public const string ReportsView = "C2B41076-A952-49ff-B2E2-DE62EFC2B554";
        /// <summary>Permission literal for viewing store administration.</summary>
        public const string SettingsView = "0984762F-9E31-4f6f-A087-138A69973AE1";
    }
}
