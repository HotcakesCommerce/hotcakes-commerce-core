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
using System.Xml.Linq;

namespace Hotcakes.Modules.Core.AppCode
{
    [Serializable]
    public static class XElementExtensions
    {
        public static string GetAttributeValue(this XElement element, string name)
        {
            var attr = element.Attribute(name);
            return attr != null ? attr.Value : null;
        }

        public static string TryGetElementValue(this XElement parent, string name, string defaultValue)
        {
            var child = parent.Element(name);
            if (child != null)
            {
                return child.Value;
            }
            return defaultValue;
        }

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