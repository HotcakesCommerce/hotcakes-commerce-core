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

using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Commerce.Utilities
{
    public class HtmlRendering
    {
        public static string Logo(HotcakesApplication app, bool isSecureRequest)
        {
            var storeRootUrl = app.StoreUrl(isSecureRequest, false);
            var storeName = app.CurrentStore.Settings.FriendlyName;

            var logoImage = app.CurrentStore.Settings.LogoImageFullUrl(app, isSecureRequest);
            var logoText = app.CurrentStore.Settings.LogoText;

            var sb = new StringBuilder();

            sb.Append("<a href=\"" + storeRootUrl + "\" title=\"" + storeName + "\"");

            if (app.CurrentStore.Settings.UseLogoImage)
            {
                sb.Append("><img src=\"" + logoImage + "\" alt=\"" + storeName + "\" />");
            }
            else
            {
                sb.Append(" class=\"logo\">");
                sb.Append(HttpUtility.HtmlEncode(logoText));
            }
            sb.Append("</a>");

            return sb.ToString();
        }

        public static string LogoText(HotcakesApplication app)
        {
            var storeRootUrl = app.StoreUrl(false, true);
            var storeName = app.CurrentStore.Settings.FriendlyName;
            var logoText = app.CurrentStore.Settings.LogoText;

            var sb = new StringBuilder();

            sb.Append("<a href=\"" + storeRootUrl + "\" title=\"" + storeName + "\"");
            sb.Append(" class=\"logo\">");
            sb.Append(HttpUtility.HtmlEncode(logoText));
            sb.Append("</a>");

            return sb.ToString();
        }

        public static string HeaderLinks(HotcakesApplication app, string currentUserId)
        {
            var sb = new StringBuilder();

            var rootUrl = app.StoreUrl(false, true);
            var rootUrlSecure = app.StoreUrl(true, false);

            sb.Append("<ul>");

            sb.Append("<li><a class=\"myaccountlink\" href=\"" + rootUrlSecure + "account\"><span>");
            sb.Append("My Account");
            sb.Append("</span></a></li>");

            sb.Append("<li><a class=\"signinlink\"");

            if (currentUserId == string.Empty)
            {
                sb.Append(" href=\"" + rootUrlSecure + "SignIn\"><span>");
                sb.Append("Sign In");
            }
            else
            {
                var name = string.Empty;
                var a = app.MembershipServices.Customers.Find(currentUserId);
                if (a != null)
                {
                    name = a.Email;
                }
                sb.Append(" href=\"" + rootUrlSecure + "SignOut\" title=\"" + HttpUtility.HtmlEncode(name) + "\"><span>");
                sb.Append("Sign Out");
            }
            sb.Append("</span></a></li>");

            sb.Append("<li><a class=\"contactlink\" href=\"" + rootUrl + "Checkout\"><span>");
            sb.Append("Checkout");
            sb.Append("</span></a></li>");

            sb.Append("</ul>");

            return sb.ToString();
        }

        public static void ProductOptionsAsControls(Product product, PlaceHolder ph)
        {
            if (!product.IsBundle)
            {
                SingleProductOptionsAsControls(product, ph);
            }
            else
            {
                foreach (var bundledProductAdv in product.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;

                    var span = new HtmlGenericControl("span");
                    span.Attributes["class"] = "product";
                    span.EnableViewState = false;
                    span.InnerHtml = bundledProductAdv.Quantity + "X " + bundledProduct.ProductName;
                    ph.Controls.Add(span);
                    SingleProductOptionsAsControls(bundledProduct, ph, bundledProductAdv.Id.ToString());
                }
            }
        }

        private static void SingleProductOptionsAsControls(Product product, PlaceHolder ph, string prefix = null)
        {
            foreach (var opt in product.Options)
            {
                if (!opt.NameIsHidden)
                {
                    var label = new HtmlGenericControl("label");
                    label.EnableViewState = false;
                    label.Attributes["for"] = "opt" + opt.Bvin.Replace("-", string.Empty);
                    label.InnerHtml = opt.Name;
                    ph.Controls.Add(label);
                }

                var lit1 = new LiteralControl("<span class=\"choice hcProductEditChoice\">");
                lit1.EnableViewState = false;
                ph.Controls.Add(lit1);

                opt.RenderAsControl(ph, prefix);

                var lit2 = new LiteralControl("</span>");
                lit2.EnableViewState = false;
                ph.Controls.Add(lit2);
            }
        }

        public static string ShippingRatesToRadioButtons(SortableCollection<ShippingRateDisplay> rates, int tabIndex,
            string selectedMethodUniqueKey)
        {
            var sb = new StringBuilder();

            if (rates == null)
                return string.Empty;

            // Tab Index Settings
            var tabOffSet = 0;
            if (tabIndex > 0)
            {
                tabOffSet = tabIndex;
            }

            foreach (ShippingRateDisplay r in rates)
            {
                if (r.Rate >= 0)
                {
                    sb.Append("<label><input type=\"radio\" name=\"shippingrate\" value=\"" + r.UniqueKey +
                              "\" style=\"display: inline !important;\"");
                    sb.Append(" class=\"shippingratequote\" ");
                    if (r.UniqueKey == selectedMethodUniqueKey)
                    {
                        sb.Append(" checked=\"checked\" ");
                    }
                    sb.Append("/>" + r.RateAndNameForDisplay + "</label><br />");
                }
            }

            return sb.ToString();
        }
    }
}