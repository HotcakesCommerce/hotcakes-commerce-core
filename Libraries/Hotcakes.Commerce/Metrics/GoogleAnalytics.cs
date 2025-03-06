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
using System.Globalization;
using System.Text;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Metrics
{
    public class GoogleAnalytics
    {
        private static string GoogleSafeString(string input)
        {
            var result = input.Replace("'", " ");
            return result;
        }

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