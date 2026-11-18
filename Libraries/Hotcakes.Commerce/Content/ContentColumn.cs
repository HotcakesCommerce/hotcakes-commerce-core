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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Represents a store-scoped content column containing ordered <see cref="ContentBlock"/> instances.
    /// </summary>
    /// <remarks>The XML element names written by this type are compatibility-sensitive.</remarks>
    public class ContentColumn
    {
        private XmlReaderSettings _hccXmlReaderSettings = new XmlReaderSettings();

        private readonly XmlWriterSettings _hccXmlWriterSettings = new XmlWriterSettings();

        /// <summary>
        /// Initializes an empty content column with default display and system values.
        /// </summary>
        public ContentColumn()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            DisplayName = string.Empty;
            SystemColumn = false;
            Blocks = new List<ContentBlock>();
        }

        /// <summary>Gets or sets the column's business identifier.</summary>
        public string Bvin { get; set; }
        /// <summary>Gets or sets the store that owns the column.</summary>
        public long StoreId { get; set; }
        /// <summary>Gets or sets the UTC timestamp of the last persistence update.</summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>Gets or sets the display name used to identify the column.</summary>
        public string DisplayName { get; set; }
        /// <summary>Gets or sets whether the column is a system-provided column.</summary>
        public bool SystemColumn { get; set; }
        /// <summary>Gets or sets the ordered blocks contained by the column.</summary>
        public List<ContentBlock> Blocks { get; set; }

        /// <summary>Serializes the column and its blocks using the established XML element names.</summary>
        /// <param name="omitDeclaration">Retained for API compatibility; fragment output is produced by the current implementation.</param>
        /// <returns>The serialized content column XML.</returns>
        public virtual string ToXml(bool omitDeclaration)
        {
            var response = string.Empty;
            var sb = new StringBuilder();
            _hccXmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
            var xw = XmlWriter.Create(sb, _hccXmlWriterSettings);
            ToXmlWriter(ref xw);
            xw.Flush();
            xw.Close();
            response = sb.ToString();
            return response;
        }

        /// <summary>Deserializes the column and its blocks from an XML string.</summary>
        /// <param name="x">The content-column XML to read.</param>
        /// <returns><see langword="true"/> when parsing completes; malformed XML is logged and returns <see langword="false"/>.</returns>
        public virtual bool FromXmlString(string x)
        {
            var sw = new StringReader(x);
            var xr = XmlReader.Create(sw);
            var result = FromXml(ref xr);
            sw.Dispose();
            xr.Close();
            return result;
        }

        /// <summary>Reads compatibility-sensitive column and block elements from an XML reader.</summary>
        /// <param name="xr">The reader positioned at content-column XML.</param>
        /// <returns><see langword="true"/> when reading completes; malformed XML is logged and returns <see langword="false"/>.</returns>
        public bool FromXml(ref XmlReader xr)
        {
            var results = false;

            try
            {
                while (xr.Read())
                {
                    if (xr.IsStartElement())
                    {
                        if (!xr.IsEmptyElement)
                        {
                            switch (xr.Name)
                            {
                                case "Bvin":
                                    xr.Read();
                                    Bvin = xr.ReadString();
                                    break;
                                case "DisplayName":
                                    xr.Read();
                                    DisplayName = xr.ReadString();
                                    break;
                                case "SystemColumn":
                                    xr.Read();
                                    SystemColumn = bool.Parse(xr.ReadString());
                                    break;
                                case "Blocks":
                                    if (xr.ReadToDescendant("ContentBlock"))
                                    {
                                        var blocks = new List<ContentBlock>();
                                        do
                                        {
                                            var block = new ContentBlock();
                                            block.FromXml(ref xr);
                                            blocks.Add(block);
                                        } while (xr.ReadToNextSibling("ContentBlock"));

                                        Blocks = blocks;
                                    }
                                    break;
                            }
                        }
                    }
                }

                results = true;
            }

            catch (XmlException XmlEx)
            {
                EventLog.LogEvent(XmlEx);
                results = false;
            }

            return results;
        }

        /// <summary>Writes this column and its blocks to an XML writer.</summary>
        /// <param name="xw">The writer that receives the column, or <see langword="null"/> to perform no work.</param>
        public void ToXmlWriter(ref XmlWriter xw)
        {
            if (xw != null)
            {
                xw.WriteStartElement("ContentColumn");

                xw.WriteElementString("Bvin", Bvin);
                xw.WriteElementString("DisplayName", DisplayName);

                xw.WriteStartElement("SystemColumn");
                xw.WriteValue(SystemColumn);
                xw.WriteEndElement();

                xw.WriteStartElement("Blocks");
                foreach (var b in Blocks)
                {
                    b.ToXmlWriter(ref xw);
                }
                xw.WriteEndElement();


                xw.WriteEndElement(); // end Column
            }
        }
    }
}
