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

using System;
using System.Globalization;
using System.Xml;

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class XmlHelper
    {
        public static string ParseInnerText(XmlNode n, string nodeName)
        {
            var result = string.Empty;
            if (n != null)
            {
                if (n.SelectSingleNode(nodeName) != null)
                {
                    result = n.SelectSingleNode(nodeName).InnerText;
                }
            }
            return result;
        }

        public static bool ParseBoolean(XmlNode n, string nodeName)
        {
            var result = false;

            var temp = "0";
            if (n != null)
            {
                if (n.SelectSingleNode(nodeName) != null)
                {
                    temp = n.SelectSingleNode(nodeName).InnerText;
                }
            }

            if (temp == "1")
            {
                result = true;
            }

            return result;
        }

        public static decimal ParseDecimal(XmlNode n, string nodeName)
        {
            var result = 0m;

            var temp = "0";
            if (n != null)
            {
                if (n.SelectSingleNode(nodeName) != null)
                {
                    temp = n.SelectSingleNode(nodeName).InnerText;
                }
            }

            decimal.TryParse(temp, NumberStyles.Float,
                CultureInfo.InvariantCulture, out result);

            return result;
        }

        public static void WriteIfNotEmpty(XmlTextWriter xw, string name, string value)
        {
            if (value.Trim().Length > 0)
            {
                xw.WriteElementString(name, value);
            }
        }
    }
}