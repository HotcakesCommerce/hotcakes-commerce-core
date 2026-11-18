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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hotcakes.Web
{
    /// <summary>
    /// Provides XML serialization, parsing, and writing helpers.
    /// </summary>
    [Serializable]
    public static class Xml
    {
        /// <summary>
        /// Deserializes an XML string to the requested type using <see cref="XmlSerializer"/>.
        /// </summary>
        /// <typeparam name="T">The target deserialization type.</typeparam>
        /// <param name="xml">The XML text to deserialize.</param>
        /// <returns>The deserialized value.</returns>
        public static T ObjectFromXml<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var xmlSerializer = new XmlSerializer(typeof (T));
                var result = (T) xmlSerializer.Deserialize(sr);
                return result;
            }
        }

        /// <summary>
        /// Reads XML from a <see cref="Stream"/> and deserializes it to the requested type.
        /// </summary>
        /// <typeparam name="T">The target deserialization type.</typeparam>
        /// <param name="stream">The stream containing the XML text.</param>
        /// <returns>The deserialized value.</returns>
        public static T ObjectFromXml<T>(Stream stream)
        {
            var rdr = new StreamReader(stream);
            return ObjectFromXml<T>(rdr.ReadToEnd());
        }

        /// <summary>
        /// Serializes an object to XML using <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <returns>The serialized XML text.</returns>
        public static string ObjectToXml(object toSerialize)
        {
            using (var textWriter = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(toSerialize.GetType());
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Gets the text value of a named child element by calling <see cref="ParseInnerText(XElement, string)"/>.
        /// </summary>
        /// <param name="x">The XML element to inspect.</param>
        /// <param name="elementName">The child element name.</param>
        /// <returns>The child element value, or an empty string when it is not present.</returns>
        public static string Parse(XElement x, string elementName)
        {
            return ParseInnerText(x, elementName);
        }

        /// <summary>
        /// Gets the inner text of a named node.
        /// </summary>
        /// <param name="n">The XML node to inspect.</param>
        /// <param name="nodeName">The XPath used to select the node.</param>
        /// <returns>The selected node's inner text, or an empty string when it is not present.</returns>
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

        /// <summary>
        /// Gets the value of a named child element.
        /// </summary>
        /// <param name="x">The XML element to inspect.</param>
        /// <param name="elementName">The child element name.</param>
        /// <returns>The child element value, or an empty string when it is not present.</returns>
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

        /// <summary>
        /// Parses a named node as a Boolean, accepting <c>1</c> and case-insensitive <c>true</c> as true.
        /// </summary>
        /// <param name="n">The XML node to inspect.</param>
        /// <param name="nodeName">The XPath used to select the node.</param>
        /// <returns><see langword="true"/> when the selected text represents true; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Parses a named child element as a Boolean, accepting <c>1</c> and case-insensitive <c>true</c> as true.
        /// </summary>
        /// <param name="x">The XML element to inspect.</param>
        /// <param name="elementName">The child element name.</param>
        /// <returns><see langword="true"/> when the child text represents true; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Parses a named node as a decimal using invariant culture.
        /// </summary>
        /// <param name="n">The XML node to inspect.</param>
        /// <param name="nodeName">The XPath used to select the node.</param>
        /// <returns>The parsed decimal, or zero when parsing fails.</returns>
        public static decimal ParseDecimal(XmlNode n, string nodeName)
        {
            var result = 0m;
            var temp = ParseInnerText(n, nodeName);
            decimal.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Parses a named child element as a decimal using invariant culture.
        /// </summary>
        /// <param name="x">The XML element to inspect.</param>
        /// <param name="elementName">The child element name.</param>
        /// <returns>The parsed decimal, or zero when parsing fails.</returns>
        public static decimal ParseDecimal(XElement x, string elementName)
        {
            var result = 0m;
            var temp = ParseInnerText(x, elementName);
            decimal.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Parses a named child element as a long integer using invariant culture.
        /// </summary>
        /// <param name="x">The XML element to inspect.</param>
        /// <param name="elementName">The child element name.</param>
        /// <returns>The parsed integer, or zero when parsing fails.</returns>
        public static long ParseLong(XElement x, string elementName)
        {
            long result = 0;
            var temp = ParseInnerText(x, elementName);
            long.TryParse(temp, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Parses a named node as an integer using invariant culture.
        /// </summary>
        /// <param name="n">The XML node to inspect.</param>
        /// <param name="nodeName">The XPath used to select the node.</param>
        /// <returns>The parsed integer, or zero when parsing fails.</returns>
        public static int ParseInteger(XmlNode n, string nodeName)
        {
            var result = 0;
            var temp = ParseInnerText(n, nodeName);
            int.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Parses a named child element as an integer using invariant culture.
        /// </summary>
        /// <param name="x">The XML element to inspect.</param>
        /// <param name="elementName">The child element name.</param>
        /// <returns>The parsed integer, or zero when parsing fails.</returns>
        public static int ParseInteger(XElement x, string elementName)
        {
            var result = 0;
            var temp = ParseInnerText(x, elementName);
            int.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Writes an element only when the supplied value contains non-whitespace text.
        /// </summary>
        /// <param name="xw">The XML writer to receive the element.</param>
        /// <param name="name">The element name.</param>
        /// <param name="value">The element value.</param>
        public static void WriteIfNotEmpty(XmlTextWriter xw, string name, string value)
        {
            if (value.Trim().Length > 0)
            {
                xw.WriteElementString(name, value);
            }
        }

        /// <summary>
        /// Writes a Boolean element to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="value">The Boolean value to write.</param>
        /// <param name="xw">The XML writer to receive the element.</param>
        public static void WriteBool(string name, bool value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes a date and time element to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="value">The date and time value to write.</param>
        /// <param name="xw">The XML writer to receive the element.</param>
        public static void WriteDate(string name, DateTime value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes an integer element to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="value">The integer value to write.</param>
        /// <param name="xw">The XML writer to receive the element.</param>
        public static void WriteInt(string name, int value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes a decimal element to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="value">The decimal value to write.</param>
        /// <param name="xw">The XML writer to receive the element.</param>
        public static void WriteDecimal(string name, decimal value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes a long-integer element to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="value">The long-integer value to write.</param>
        /// <param name="xw">The XML writer to receive the element.</param>
        public static void WriteLong(string name, long value, ref XmlWriter xw)
        {
            xw.WriteStartElement(name);
            xw.WriteValue(value);
            xw.WriteEndElement();
        }
    }
}
