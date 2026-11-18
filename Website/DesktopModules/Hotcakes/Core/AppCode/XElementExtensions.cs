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
using System.Xml.Linq;

namespace Hotcakes.Modules.Core.AppCode
{
    /// <summary>
    /// Provides convenience accessors for XML element values.
    /// </summary>
    [Serializable]
    public static class XElementExtensions
    {
        /// <summary>
        /// Returns an XML attribute value, or an empty string when the attribute is absent.
        /// </summary>
        /// <param name="element">The value supplied for element.</param>
        /// <param name="name">The value supplied for name.</param>
        /// <returns>The result of the operation.</returns>
        public static string GetAttributeValue(this XElement element, string name)
        {
            var attr = element.Attribute(name);
            return attr != null ? attr.Value : null;
        }

        /// <summary>
        /// Returns an XML child element value, or the supplied default when it is absent.
        /// </summary>
        /// <param name="parent">The value supplied for parent.</param>
        /// <param name="name">The value supplied for name.</param>
        /// <param name="defaultValue">The value supplied for defaultValue.</param>
        /// <returns>The result of the operation.</returns>
        public static string TryGetElementValue(this XElement parent, string name, string defaultValue)
        {
            var child = parent.Element(name);
            if (child != null)
            {
                return child.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns an XML child element value parsed as an integer, or the supplied default when it is absent or invalid.
        /// </summary>
        /// <param name="parent">The value supplied for parent.</param>
        /// <param name="name">The value supplied for name.</param>
        /// <param name="defaultValue">The value supplied for defaultValue.</param>
        /// <returns>The result of the operation.</returns>
        public static int TryGetElementValueAsInt(this XElement parent, string name, int defaultValue)
        {
            var child = parent.Element(name);
            if (child != null)
            {
                var temp = child.Value;
                var tempResult = 0;
                if (int.TryParse(temp, out tempResult)) return tempResult;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns an XML child element value parsed as a long integer, or the supplied default when it is absent or invalid.
        /// </summary>
        /// <param name="parent">The value supplied for parent.</param>
        /// <param name="name">The value supplied for name.</param>
        /// <param name="defaultValue">The value supplied for defaultValue.</param>
        /// <returns>The result of the operation.</returns>
        public static long TryGetElementValueAsLong(this XElement parent, string name, long defaultValue)
        {
            var child = parent.Element(name);
            if (child != null)
            {
                var temp = child.Value;
                long tempResult = 0;
                if (long.TryParse(temp, out tempResult)) return tempResult;
            }
            return defaultValue;
        }
    }
}
