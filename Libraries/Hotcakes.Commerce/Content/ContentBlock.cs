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
using System.Text;
using System.Xml;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Represents a content block assigned to a <see cref="ContentColumn"/>.
    /// </summary>
    /// <remarks>
    /// Base and text settings, and list values, are persisted as JSON inside the
    /// content-block record. The XML member names used by <see cref="ToXml(bool)"/>
    /// and <see cref="FromXml(ref XmlReader)"/> are compatibility-sensitive.
    /// </remarks>
    public class ContentBlock
    {
        private XmlReaderSettings _hccXmlReaderSettings = new XmlReaderSettings();

        private readonly XmlWriterSettings _hccXmlWriterSettings = new XmlWriterSettings();

        /// <summary>
        /// Initializes a content block with empty settings and default ordering values.
        /// </summary>
        public ContentBlock()
        {
            BaseSettings = new ContentBlockSettings();
            TextSettings = new ContentBlockSettings();
            Lists = new ContentBlockSettingList();
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            ColumnId = string.Empty;
            SortOrder = 1;
            ControlName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the block's business identifier.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        /// Gets or sets the store that owns the block.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp of the last persistence update.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the business identifier of the containing <see cref="ContentColumn"/>.
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// Gets or sets the block's ordering position within its column.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the content-control name used to render the block.
        /// </summary>
        public string ControlName { get; set; }

        /// <summary>
        /// Gets or sets non-translated settings for the block.
        /// </summary>
        public ContentBlockSettings BaseSettings { get; set; }

        /// <summary>
        /// Gets or sets translated text settings for the block.
        /// </summary>
        public ContentBlockSettings TextSettings { get; set; }

        /// <summary>
        /// Gets or sets the named lists associated with the block.
        /// </summary>
        public ContentBlockSettingList Lists { get; set; }

        /// <summary>
        /// Creates a copy of the block, including settings and list items.
        /// </summary>
        /// <returns>A new block with copied content and a current UTC <see cref="LastUpdated"/> value.</returns>
        /// <remarks>The clone keeps the column, control, and sort information but receives a new default identifier.</remarks>
        public ContentBlock Clone()
        {
            var clone = new ContentBlock();
            clone.StoreId = StoreId;
            clone.LastUpdated = DateTime.UtcNow;
            clone.ColumnId = ColumnId;
            clone.SortOrder = SortOrder;
            clone.ControlName = ControlName;
            foreach (var q in BaseSettings)
            {
                clone.BaseSettings.Add(q.Key, q.Value);
            }
            foreach (var q in TextSettings)
            {
                clone.TextSettings.Add(q.Key, q.Value);
            }
            foreach (var y in Lists.Items)
            {
                clone.Lists.AddItem(y.Clone());
            }
            return clone;
        }

        /// <summary>
        /// Serializes this block using the established content-block XML element names.
        /// </summary>
        /// <param name="omitDeclaration">Retained for API compatibility; fragment output is produced by the current implementation.</param>
        /// <returns>The serialized content block XML.</returns>
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

        /// <summary>
        /// Deserializes this block from an XML string.
        /// </summary>
        /// <param name="x">The content-block XML to read.</param>
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

        /// <summary>
        /// Reads compatibility-sensitive content-block elements from an XML reader.
        /// </summary>
        /// <param name="xr">The reader positioned at content-block XML.</param>
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
                                case "ControlName":
                                    xr.Read();
                                    ControlName = xr.ReadString();
                                    break;
                                case "SortOrder":
                                    xr.Read();
                                    SortOrder = int.Parse(xr.ReadString());
                                    break;
                                case "ColumnId":
                                    xr.Read();
                                    ColumnId = xr.ReadString();
                                    break;
                                case "SerializedSettings":
                                    xr.Read();
                                    var baseSettings = xr.ReadString();
                                    BaseSettings = Json.ObjectFromJson<ContentBlockSettings>(baseSettings);
                                    break;
                                case "TextSettings":
                                    xr.Read();
                                    var textSettings = xr.ReadString();
                                    TextSettings = Json.ObjectFromJson<ContentBlockSettings>(textSettings);
                                    break;
                                case "Lists":
                                    xr.Read();
                                    var jsonlist = xr.ReadString();
                                    Lists = Json.ObjectFromJson<ContentBlockSettingList>(jsonlist);
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

        /// <summary>
        /// Writes this block to an XML writer using the established serialized element names.
        /// </summary>
        /// <param name="xw">The writer that receives the block, or <see langword="null"/> to perform no work.</param>
        public void ToXmlWriter(ref XmlWriter xw)
        {
            if (xw != null)
            {
                xw.WriteStartElement("ContentBlock");

                xw.WriteElementString("Bvin", Bvin);
                xw.WriteElementString("ControlName", ControlName);
                xw.WriteElementString("SortOrder", SortOrder.ToString());
                xw.WriteElementString("ColumnId", ColumnId);
                xw.WriteElementString("SerializedSettings", Json.ObjectToJson(BaseSettings));
                xw.WriteElementString("TextSettings", Json.ObjectToJson(TextSettings));
                xw.WriteElementString("Lists", Json.ObjectToJson(Lists));

                xw.WriteEndElement(); // end Column
            }
        }
    }
}
