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
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using System.Web.UI;
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public static class HccPartController
    {
        #region "Tax Providers"

        public static Control LoadTaxProviderEditor(string methodName, Page p)
        {
            var fullName = "Parts/TaxProviders/" + methodName + "/Edit.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        #endregion

        #region " Shipping "

        public static Control LoadShippingEditor(string shippingName, Page p)
        {
            var fullName = "Parts/Shipping/" + shippingName + "/Edit.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        #endregion

        #region " Have to be moved and converted to regular controls "

        public static Control LoadShippingZoneEditor(Page p)
        {
            const string fullName = "Parts/ShippingZones/Edit.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        #endregion

        #region " Admin Plugins "

        public static StringCollection FindAdminPlugins()
        {
            var result = new StringCollection();
            result = ListFolders("Plugins", "default.aspx");
            return result;
        }

        #endregion

        private static StringCollection ListFolders(string startingFolder, string checkForFileName)
        {
            var result = new StringCollection();

            var baseUrl = "~/DesktopModules/Hotcakes/Core/Admin/";
            var modulePath = HostingEnvironment.MapPath(string.Concat(baseUrl, startingFolder));
            if (Directory.Exists(modulePath))
            {
                var modules = Directory.GetDirectories(modulePath);
                if (modules != null)
                {
                    foreach (var module in modules)
                    {
                        if (File.Exists(Path.Combine(module, checkForFileName)) ||
                            string.IsNullOrWhiteSpace(checkForFileName))
                        {
                            result.Add(Path.GetFileName(module));
                        }
                    }
                }
            }

            return result;
        }

        private static Control LoadSingleControl(string blockName, ref Page p)
        {
            Control result = null;

            if (p != null)
            {
                var virtualPath = string.Concat("~/DesktopModules/Hotcakes/Core/Admin/", blockName);
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    result = p.LoadControl(virtualPath);
                }
            }

            return result;
        }

        #region " Content Blocks "

        public static StringCollection FindContentBlocks()
        {
            var result = new StringCollection();
            result = ListFolders("Parts/ContentBlocks", string.Empty);
            return result;
        }

        public static Control LoadContentBlockAdminView(string blockName, Page p)
        {
            var fullName = "Parts/ContentBlocks/" + blockName + "/Adminview.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        public static Control LoadContentBlockEditor(string blockName, Page p)
        {
            var fullName = "Parts/ContentBlocks/" + blockName + "/Editor.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        #endregion

        #region " Payment Methods "

        public static Control LoadPaymentMethodEditor(string methodName, Page p)
        {
            var fullName = "Parts/PaymentMethods/" + methodName + "/Edit.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        public static Control LoadCreditCardGatewayEditor(string gatewayName, Page p)
        {
            var fullName = "Parts/CreditCardGateways/" + gatewayName + "/Edit.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        public static Control LoadGiftCardGatewayEditor(string gatewayName, Page p)
        {
            var fullName = "Parts/GiftCardGateways/" + gatewayName + "/Edit.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        #endregion

        #region " Editors "

        public static Control LoadEditor(string editorName, Page p)
        {
            var fullName = "Parts/Editors/" + editorName + "/editor.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        public static Control LoadDefaultEditor(Page p)
        {
            return LoadEditor(WebAppSettings.DefaultTextEditor, p);
        }

        public static Control LoadInventoryEditor(Page p)
        {
            var fullName = "Orders/OrderItemInventory.ascx";
            return LoadSingleControl(fullName, ref p);
        }

        #endregion
    }
}