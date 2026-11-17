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

using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Generates HTML fragments for common storefront controls.
    /// </summary>
    /// <remarks>
    /// The returned strings are intended for direct insertion into the storefront markup. Only selected text values
    /// are HTML-encoded by the existing implementation; callers should treat the output as HTML rather than plain text.
    /// </remarks>
    public class HtmlRendering
    {
        /// <summary>
        /// Generates a link to the store root using either the configured logo image or encoded logo text.
        /// </summary>
        /// <param name="app">The current Hotcakes application and store context.</param>
        /// <param name="isSecureRequest">Whether the generated store URLs should use the secure request scheme.</param>
        /// <returns>An HTML anchor containing the configured logo image or logo text.</returns>
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

        /// <summary>
        /// Generates an HTML link to the store root using the configured logo text.
        /// </summary>
        /// <param name="app">The current Hotcakes application and store context.</param>
        /// <returns>An HTML anchor containing HTML-encoded logo text.</returns>
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

        /// <summary>
        /// Generates storefront account, sign-in or sign-out, and checkout links.
        /// </summary>
        /// <param name="app">The current Hotcakes application and store context.</param>
        /// <param name="currentUserId">The current user identifier; an empty string causes a sign-in link to be emitted.</param>
        /// <returns>An HTML unordered list containing the storefront header links.</returns>
        /// <remarks>
        /// When a user is signed in, the method looks up the customer and HTML-encodes the email address used in the
        /// sign-out link title. The generated link labels are fixed English text.
        /// </remarks>
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

        /// <summary>
        /// Adds the product's visible options to a Web Forms placeholder as server controls.
        /// </summary>
        /// <param name="product">The product whose options are rendered.</param>
        /// <param name="ph">The placeholder that receives the generated controls.</param>
        /// <remarks>
        /// Bundle products receive a product label and controls for each bundled product; ordinary products receive
        /// their options directly. Hidden option names are omitted, but their input controls are still rendered.
        /// </remarks>
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

        /// <summary>
        /// Generates radio-button markup for non-negative shipping rates.
        /// </summary>
        /// <param name="rates">The shipping rates to render, or <see langword="null"/> for no output.</param>
        /// <param name="tabIndex">The starting tab index; values less than or equal to zero do not add a tab index.</param>
        /// <param name="selectedMethodUniqueKey">The unique key of the rate that should be marked as selected.</param>
        /// <returns>HTML radio-button markup, or an empty string when <paramref name="rates"/> is <see langword="null"/>.</returns>
        /// <remarks>
        /// The current implementation calculates a tab offset but does not emit it in the generated markup. Rates with
        /// negative amounts are omitted.
        /// </remarks>
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
