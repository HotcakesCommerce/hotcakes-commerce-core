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

using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Metrics
{
    public class ShopZilla
    {
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