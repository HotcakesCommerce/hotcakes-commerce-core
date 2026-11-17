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
using System.Globalization;
using System.Text;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Metrics
{
    /// <summary>
    /// Renders Google Analytics ecommerce and Google Ads conversion markup from Hotcakes order data.
    /// </summary>
    /// <remarks>
    /// The methods return script fragments as strings; they do not register scripts with a page or
    /// perform any network request. The generated markup intentionally preserves the provider's
    /// historical formatting and concatenation behavior. In particular, only apostrophes are
    /// replaced in values passed through the internal Google-safe helper; this is not general
    /// JavaScript, HTML, or URL escaping.
    /// </remarks>
    public class GoogleAnalytics
    {
        private static string GoogleSafeString(string input)
        {
            var result = input.Replace("'", " ");
            return result;
        }

        /// <summary>
        /// Renders the legacy Google ecommerce tracker and, when an order is supplied, its transaction and item data.
        /// </summary>
        /// <param name="googleId">The Google tracking identifier. This compatibility parameter is accepted but is not rendered or otherwise used by the current implementation.</param>
        /// <param name="o">The order whose totals, shipping address, and line items are rendered; when <see langword="null"/>, only the opening and closing script tags are returned.</param>
        /// <param name="storeName">The affiliation or store name rendered in the transaction data.</param>
        /// <param name="categoryName">The category or variation rendered for each order item.</param>
        /// <returns>A JavaScript <c>script</c> element containing the ecommerce calls. The output includes one transaction, one item per order line, and an ecommerce send call when <paramref name="o"/> is not <see langword="null"/>.</returns>
        /// <remarks>
        /// Order number, store name, address names, category, product name, and SKU are passed through
        /// the internal apostrophe replacement before being concatenated into single-quoted JavaScript
        /// values. Other characters are not escaped, including backslashes, line breaks, HTML markup,
        /// or JavaScript syntax. Numeric values are concatenated using their normal string conversion,
        /// and the output uses the order's shipping address and item collection as observed at render time.
        /// </remarks>
        public static string RenderLatestTrackerAndTransaction(string googleId, Order o, string storeName,
            string categoryName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\">");

            if (o != null)
            {
                sb.AppendLine("ga('ecommerce:addTransaction', {");
                sb.AppendLine("'id': '" + GoogleSafeString(o.OrderNumber) + "',"); // Transaction ID. Required
                sb.AppendLine("'affiliation': '" + GoogleSafeString(storeName) + "',"); // Affiliation or store name
                sb.AppendLine("'revenue': '" + o.TotalGrand + "',"); // Grand Total
                sb.AppendLine("'shipping': '" + o.TotalShippingAfterDiscounts + "',"); // Shipping
                sb.AppendLine("'tax': '" + o.TotalTax + "', "); // Tax
                sb.AppendLine("'city': '" + GoogleSafeString(o.ShippingAddress.City) + "', "); // City
                sb.AppendLine("'state': '" + GoogleSafeString(o.ShippingAddress.RegionSystemName) + "', "); // State or Province
                sb.AppendLine("'country': '" + GoogleSafeString(o.ShippingAddress.CountrySystemName) + "' "); // country
                sb.AppendLine("});");

                foreach (var li in o.Items)
                {
                    // add item might be called for every item in the shopping cart
                    // where your ecommerce engine loops through each item in the cart and
                    // prints out addItem for each
                    sb.AppendLine("ga('ecommerce:addItem', {");
                    sb.AppendLine("'id': '" + GoogleSafeString(o.OrderNumber) + "',"); // Transaction ID. Required
                    sb.AppendLine("'name': '" + GoogleSafeString(li.ProductName) + "',"); // Product name. Required
                    sb.AppendLine("'sku': '" + GoogleSafeString(li.ProductSku) + "',"); // SKU/code
                    sb.AppendLine("'category': '" + GoogleSafeString(categoryName) + "',"); // Category or variation
                    sb.AppendLine("'price': '" + li.AdjustedPricePerItem + "',"); // Unit price
                    sb.AppendLine("'quantity': '" + li.Quantity + "'"); // Quantity
                    sb.AppendLine("});");
                }

                sb.AppendLine("ga('ecommerce:send');");
            }

            sb.AppendLine("</script>");

            return sb.ToString();
        }

        /// <summary>
        /// Renders Google Ads purchase-conversion markup for an order value.
        /// </summary>
        /// <param name="orderValue">The purchase amount. It is rounded to two decimal places and formatted with invariant culture for the JavaScript value and image query string.</param>
        /// <param name="conversionId">The Google conversion identifier, inserted into the JavaScript variable and tracking image path without escaping.</param>
        /// <param name="conversionFormat">The conversion format inserted into <c>google_conversion_format</c> without escaping.</param>
        /// <param name="conversionLabel">The conversion label inserted into the JavaScript variable and image query string without URL escaping.</param>
        /// <param name="backgroundColor">The conversion color inserted into <c>google_conversion_color</c> without JavaScript escaping.</param>
        /// <param name="https">Selects <c>https://</c> rather than <c>http://</c> for the noscript impression image. The external conversion script is always emitted with its fixed HTTPS URL.</param>
        /// <returns>A JavaScript conversion block followed by the fixed Google conversion script URL and a noscript tracking image.</returns>
        /// <remarks>
        /// The generated JavaScript and image URL are assembled by direct string concatenation. The
        /// conversion identifier, label, format, and background color are not escaped or encoded, so
        /// callers must provide values compatible with the provider markup. The method retains the
        /// historical malformed <c>/* &lt;!CDATA[ */</c> comment marker, emits a conversion-value
        /// conditional based on the formatted amount, and does not change the provider's script semantics.
        /// </remarks>
        public static string RenderGoogleAdwordTracker(decimal orderValue, string conversionId, string conversionFormat,
            string conversionLabel, string backgroundColor, bool https)
        {
            var sb = new StringBuilder();

            var total = Math.Round(orderValue, 2).ToString(CultureInfo.InvariantCulture);

            sb.AppendLine("<!-- Google Code for purchase Conversion Page -->");
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("/* <!CDATA[ */");
            sb.AppendLine("var google_conversion_id = " + conversionId + ";");
            sb.AppendLine("var google_conversion_language = \"en_US\";");
            sb.AppendLine("var google_conversion_format = \"" + conversionFormat + "\";");
            sb.AppendLine("var google_conversion_color = \"" + backgroundColor + "\";");
            sb.AppendLine("var google_conversion_label = \"" + conversionLabel + "\";");
            sb.AppendLine("if (" + total + ") {");
            sb.AppendLine("  var google_conversion_value = " + total + ";");
            sb.AppendLine("}");
            sb.AppendLine("/* ]]> */");
            sb.AppendLine("</script>");

            sb.AppendLine("<script type=\"text/javascript\" src=\"https://www.googleadservices.com/pagead/conversion.js\">");
            sb.AppendLine("</script>");

            sb.AppendLine("<noscript>");
            sb.Append("<img height=\"1\" width=\"1\" border=\"0\" src=\"");
            sb.Append(https ? "https://" : "http://");
            sb.AppendLine("www.googleadservices.com/pagead/conversion/" + conversionId + "/imp.gif?value=" + total +
                          "&label=" + conversionLabel + "&script=0\">");
            sb.AppendLine("</noscript>");

            return sb.ToString();
        }
    }
}
