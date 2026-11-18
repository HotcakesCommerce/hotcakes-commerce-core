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
using System.IO;
using System.Xml.Serialization;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Provides convenience methods for XML serialization and deserialization.
    /// </summary>
    public class Serialization
    {
        /// <summary>
        /// Serializes an object using an <see cref="System.Xml.Serialization.XmlSerializer"/> created for its runtime type.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <returns>The serialized XML representation of <paramref name="item"/>.</returns>
        public static string SerializeToXml(object item)
        {
            var sw = new StringWriter();
            var formatter = new XmlSerializer(item.GetType());
            formatter.Serialize(sw, item);
            return sw.ToString();
        }

        /// <summary>
        /// Deserializes XML into an instance of the specified type.
        /// </summary>
        /// <param name="item">The XML text to deserialize.</param>
        /// <param name="type">The type of object represented by the XML.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeFromXml(string item, Type type)
        {
            var tr = new StringReader(item);
            var formatter = new XmlSerializer(type);
            return formatter.Deserialize(tr);
        }
    }
}
