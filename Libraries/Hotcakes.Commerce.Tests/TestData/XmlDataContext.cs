#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Configuration;
using System.Xml.Linq;

namespace Hotcakes.Commerce.Tests.TestData
{
    public class XmlDataContext
    {
        private readonly string _path = string.Empty;

        /// <summary>
        /// Initializes a XML file path.
        /// </summary>
        /// <param name="key">The key.</param>
        public XmlDataContext(string key)
        {
            this._path = Convert.ToString(ConfigurationManager.AppSettings[key]);
        }

        /// <summary>
        /// Get XML file data
        /// </summary>
        /// <returns></returns>
        public XElement GetXml()
        {
            try
            {
                if (string.IsNullOrEmpty(_path)) return null;
                var xmldoc = XElement.Load(_path);
                return xmldoc;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
