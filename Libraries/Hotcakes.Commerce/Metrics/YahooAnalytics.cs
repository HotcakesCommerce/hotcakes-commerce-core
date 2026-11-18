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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Metrics
{
    /// <summary>
    /// Renders Yahoo! conversion tracking markup from a Hotcakes order.
    /// </summary>
    /// <remarks>
    /// The returned fragment uses the provider's historical document-write bootstrap and does not
    /// load the Yahoo! script during rendering. The provider endpoint is assembled in the emitted
    /// browser-side JavaScript, preserving the compatibility-sensitive URL fragments and casing.
    /// </remarks>
    public class YahooAnalytics
    {
        /// <summary>
        /// Renders a Yahoo! conversion tracker for an order and account.
        /// </summary>
        /// <param name="o">The order supplying the transaction identifier and total amount. The currency is rendered as the literal <c>USD</c>.</param>
        /// <param name="accountId">The Yahoo! account identifier assigned to <c>ysm_accountid</c> without escaping.</param>
        /// <returns>A script fragment that sets Yahoo! custom conversion data and writes the provider script element in the browser.</returns>
        /// <remarks>
        /// The transaction value is built by direct concatenation as
        /// <c>transId=&lt;order number&gt;,currency=USD,amount=&lt;total&gt;</c>. Order number and account
        /// identifier are not JavaScript or HTML escaped, and the amount uses the order's normal
        /// string conversion. The provider host and <c>/script/ScriptServlet</c> path are embedded
        /// in the generated <c>document.write</c> expression, which uses a protocol-relative
        /// <c>//srv1.wa.marketingsolutions.yahoo.com</c> URL. The returned markup also preserves the
        /// historical HTML comment wrapper and split <c>SCR</c>/<c>IPT</c> strings used to avoid
        /// prematurely closing the outer script element.
        /// </remarks>
        public static string RenderYahooTracker(Order o, string accountId)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<SCRIPT language=\"JavaScript\" type=\"text/javascript\">");
            sb.AppendLine("<!-- Yahoo! Inc.");
            sb.AppendLine("window.ysm_customData = new Object();");
            sb.AppendLine("window.ysm_customData.conversion = \"transId=" + o.OrderNumber + ",currency=USD,amount=" +
                          o.TotalGrand + "\";");
            sb.AppendLine("var ysm_accountid  = \"" + accountId + "\";");
            sb.AppendLine("document.write(\"<SCR\" + \"IPT language='JavaScript' type='text/javascript' \"");
            sb.AppendLine(" + \"SRC=//\" + \"srv1.wa.marketingsolutions.yahoo.com\" + ");
            sb.AppendLine("\"/script/ScriptServlet\" + \"?aid=\" + ysm_accountid ");
            sb.AppendLine(" + \"></SCR\" + \"IPT>\");");
            sb.AppendLine("// -->");
            sb.AppendLine("</SCRIPT>");

            return sb.ToString();
        }
    }
}
