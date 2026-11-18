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

using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Metrics
{
    /// <summary>
    /// Renders ShopZilla and Bizrate conversion and receipt-survey tracking markup.
    /// </summary>
    /// <remarks>
    /// The class preserves the historical provider names used by the API: ShopZilla for the
    /// tracking identifier and script, and Bizrate for the receipt-survey block. Both methods
    /// return markup only and do not load the provider scripts themselves.
    /// </remarks>
    public class ShopZilla
    {
        /// <summary>
        /// Renders the Bizrate post-purchase receipt-survey variables, provider script, and ShopZilla tracker.
        /// </summary>
        /// <param name="o">The completed order supplying the order number, total, billing postal code, line items, and custom properties.</param>
        /// <param name="app">The Hotcakes application context supplying product lookup, product URLs, the current store, and the ShopZilla identifier.</param>
        /// <returns>Markup containing the Bizrate receipt-survey variables and script, followed by the output of <see cref="RenderTracker(Order, HotcakesApplication)"/>.</returns>
        /// <remarks>
        /// Up to five item slots are emitted. Each populated slot contains a rewritten product URL,
        /// SKU, an empty GTIN field, and the line total divided by quantity; remaining slots contain
        /// empty values. Values are concatenated directly into JavaScript and are not HTML, JavaScript,
        /// or URL escaped. The output retains the provider's observed malformed HTML comment
        /// delimiters, which use a non-ASCII dash character, and the trailing newline behavior of
        /// the End comment.
        /// </remarks>
        public static string RenderReceiptSurvey(Order o, HotcakesApplication app)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!– Start Bizrate POS Code –>");

            sb.AppendLine("<script language=\"JavaScript\">");
            sb.AppendLine("// var passin_x =;");
            sb.AppendLine("//var passin_y =500;");
            sb.AppendLine("var orderId='" + o.OrderNumber + "';");
            sb.AppendLine("// var z_index =; //default 9995");
            sb.AppendLine("var cartTotal=" + o.TotalGrand.ToString("0.00") + ";");
            sb.AppendLine("var billingZipCode='" + o.BillingAddress.PostalCode + "';");
            sb.Append("var productsPurchased='");

            for (var i = 0; i < 5; i++)
            {
                if (i < o.Items.Count)
                {
                    var p = app.CatalogServices.Products.FindBySku(o.Items[i].ProductSku);
                    if (p == null) p = new Product();
                    var url = UrlRewriter.BuildUrlForProduct(p);
                    sb.Append("URL=" + url);
                    sb.Append("^SKU=" + p.Sku);
                    sb.Append("^GTIN="); // UPC, EIN or other global unique number
                    sb.Append("^PRICE=" + o.Items[i].LineTotal/o.Items[i].Quantity);
                }
                else
                {
                    sb.Append("URL=^SKU=^GTIN=^PRICE=");
                }
                if (i < 4)
                {
                    sb.Append("|");
                }
            }
            sb.AppendLine("';");

            sb.AppendLine("</script>");
            sb.AppendLine("<script type=\"text/javascript\" src=\"https://eval.bizrate.com/js/pos_" +
                          app.CurrentStore.Settings.Analytics.ShopZillaId + ".js\">");
            sb.AppendLine("</script>");

            sb.AppendLine("<!– End Bizrate POS Code –>\n");


            sb.AppendLine(RenderTracker(o, app));

            return sb.ToString();
        }

        /// <summary>
        /// Renders the ShopZilla ROI tracker variables and its external tracking script.
        /// </summary>
        /// <param name="o">The order supplying the total, order number, line-item quantities, and customer-status custom property.</param>
        /// <param name="app">The Hotcakes application context supplying the current store's ShopZilla identifier.</param>
        /// <returns>Markup defining the provider variables and referencing the fixed HTTPS ShopZilla ROI tracker URL.</returns>
        /// <remarks>
        /// The <c>cust_type</c> value is <c>0</c> when the order contains the <c>hcc</c>/<c>allowpasswordreset</c>
        /// custom property with value <c>1</c>; otherwise it is <c>1</c>, despite the historical comment
        /// describing <c>0</c> as a new customer and <c>1</c> as an old customer. The method sums all
        /// item quantities for <c>units_ordered</c>. The provider identifier, order values, and order
        /// number are directly concatenated and are not escaped. The external script URL is always HTTPS.
        /// </remarks>
        public static string RenderTracker(Order o, HotcakesApplication app)
        {
            var sb = new StringBuilder();
            var mid = app.CurrentStore.Settings.Analytics.ShopZillaId;
            sb.AppendLine("<script language=\"javascript\">");
            sb.AppendLine("<!--");
            sb.AppendLine("	/* Performance Tracking Data */");
            sb.AppendLine("	var mid            = '" + mid + "';");
            if (o.CustomProperties.Where(y => (y.DeveloperId == "hcc")
                                              && (y.Key == "allowpasswordreset")
                                              && (y.Value == "1")
                ).Count() > 0)
            {
                sb.AppendLine("	var cust_type      = '0';");
            }
            else
            {
                sb.AppendLine("	var cust_type      = '1';"); // 0 = new customer, 1 = old customer
            }
            sb.AppendLine("	var order_value    = '" + o.TotalGrand.ToString("0.00") + "';");
            sb.AppendLine("	var order_id       = '" + o.OrderNumber + "';");
            var unitsOrdered = 0;
            unitsOrdered = o.Items.Sum(y => y.Quantity);
            sb.AppendLine("	var units_ordered  = '" + unitsOrdered + "';");
            sb.AppendLine("//-->");
            sb.AppendLine("</script>");
            sb.AppendLine(
                "<script language=\"javascript\" src=\"https://www.shopzilla.com/css/roi_tracker.js\"></script>");

            return sb.ToString();
        }
    }
}
