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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hotcakes.Web
{
    [Serializable]
    public static class Xml
    {
        public static T ObjectFromXml<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var xmlSerializer = new XmlSerializer(typeof (T));
                var result = (T) xmlSerializer.Deserialize(sr);
                return result;
            }
        }

        public static T ObjectFromXml<T>(Stream stream)
        {
            var rdr = new StreamReader(stream);
            return ObjectFromXml<T>(rdr.ReadToEnd());
        }

        public static string ObjectToXml(object toSerialize)
        {
            using (var textWriter = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(toSerialize.GetType());
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static string Parse(XElement x, string elementName)
        {
            return ParseInnerText(x, elementName);
        }

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

        public static string ParseInnerText(XElement x, string elementName)
        {
            var result = string.Empty;
            if (x != null)
            {
                if (x.Element(elementName) != null)
                {
                    result = x.Element(elementName).Value;
                }
            }
            return result;
        }

        public static bool ParseBoolean(XmlNode n, string nodeName)
        {
            var result = false;
            var temp = ParseInnerText(n, nodeName);
            if (temp == "1" || temp.ToLower() == "true")
            {
                result = true;
            }
            return result;
        }

        public static bool ParseBoolean(XElement x, string elementName)
        {
            var result = false;
            var temp = ParseInnerText(x, elementName);
            if (temp == "1" || temp.ToLower() == "true")
            {
                result = true;
            }
            return result;
        }

        public static decimal ParseDecimal(XmlNode n, string nodeName)
        {
            var result = 0m;
            var temp = ParseInnerText(n, nodeName);
            decimal.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static decimal ParseDecimal(XElement x, string elementName)
        {
            var result = 0m;
            var temp = ParseInnerText(x, elementName);
            decimal.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static long ParseLong(XElement x, string elementName)
        {
            long result = 0;
            var temp = ParseInnerText(x, elementName);
            long.TryParse(temp, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static int ParseInteger(XmlNode n, string nodeName)
        {
            var result = 0;
            var temp = ParseInnerText(n, nodeName);
            int.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static int ParseInteger(XElement x, string elementName)
        {
            var result = 0;
            var temp = ParseInnerText(x, elementName);
            int.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static void WriteIfNotEmpty(XmlTextWriter xw, string name, string value)
        {
            if (value.Trim().Length > 0)
            {
                xw.WriteElementString(name, value);
            }
        }

        public static void WriteBool(string name, bool value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        public static void WriteDate(string name, DateTime value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        public static void WriteInt(string name, int value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        public static void WriteDecimal(string name, decimal value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        public static void WriteLong(string name, long value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }
    }
}