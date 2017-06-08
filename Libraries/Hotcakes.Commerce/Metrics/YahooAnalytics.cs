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

using System.Text;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Metrics
{
    public class YahooAnalytics
    {
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