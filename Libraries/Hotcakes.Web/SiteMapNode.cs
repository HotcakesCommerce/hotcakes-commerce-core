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
using System.Text;
using System.Xml;

namespace Hotcakes.Web
{
    /// <summary>
    /// Represents a sitemap URL and its child sitemap entries.
    /// </summary>
    [Serializable]
    public class SiteMapNode
    {
        /// <summary>
        /// Initializes a sitemap node with empty values and an empty child collection.
        /// </summary>
        public SiteMapNode()
        {
            DisplayName = string.Empty;
            LastModified = null;
            Priority = SiteMapPriority.NotSet;
            ChangeFrequency = SiteMapChangeFrequency.NotSet;
            Url = string.Empty;
            Children = new List<SiteMapNode>();
        }

        /// <summary>Gets or sets the display name associated with this node.</summary>
        public string DisplayName { get; set; }
        /// <summary>Gets or sets the URL represented by this node.</summary>
        public string Url { get; set; }
        /// <summary>Gets or sets the optional last-modified timestamp for this node.</summary>
        public DateTime? LastModified { get; set; }
        /// <summary>Gets or sets the sitemap priority for this node.</summary>
        /// <see cref="SiteMapPriority" />
        public SiteMapPriority Priority { get; set; }
        /// <summary>Gets or sets the expected change frequency for this node.</summary>
        /// <see cref="SiteMapChangeFrequency" />
        public SiteMapChangeFrequency ChangeFrequency { get; set; }
        /// <summary>Gets or sets the child sitemap nodes.</summary>
        public List<SiteMapNode> Children { get; set; }

        /// <summary>
        /// Adds a child URL using <see cref="SiteMapPriority.p5" /> as its priority.
        /// </summary>
        /// <param name="url">The URL to add.</param>
        /// <see cref="AddUrl(string, SiteMapPriority)" />
        public void AddUrl(string url)
        {
            AddUrl(url, SiteMapPriority.p5);
        }

        /// <summary>
        /// Adds a child URL with the specified sitemap priority when the sitemap limit allows it.
        /// </summary>
        /// <param name="url">The URL to add.</param>
        /// <param name="priority">The priority to assign to the child node.</param>
        public void AddUrl(string url, SiteMapPriority priority)
        {
            var n = new SiteMapNode {Url = url, Priority = priority};

            // Site Map can't exceed 50,000 according to specs
            if (Children.Count < 49999)
            {
                Children.Add(n);
            }
        }

        /// <summary>
        /// Renders this node and its descendants as an XML sitemap document.
        /// </summary>
        /// <returns>The XML sitemap document.</returns>
        /// <see cref="RenderAsXmlSiteMapToStringBuilder(XmlWriter)" />
        public string RenderAsXmlSiteMap()
        {
            var sb = new StringBuilder();
            var result = string.Empty;

            sb.Append("<?xml version='1.0' encoding='UTF-8'?>");
            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" ");
            sb.Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            sb.Append("xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 ");
            sb.Append(" http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\">" + Environment.NewLine);


            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            var xw = XmlWriter.Create(sb, settings);

            RenderAsXmlSiteMapToStringBuilder(xw);

            xw.Flush();
            xw.Close();

            sb.Append(Environment.NewLine + "</urlset>");

            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Writes this node and its descendants to an XML writer.
        /// </summary>
        /// <param name="xw">The writer receiving the sitemap elements.</param>
        /// <see cref="RenderAsXmlSiteMap()" />
        public void RenderAsXmlSiteMapToStringBuilder(XmlWriter xw)
        {
            RenderThis(xw);

            if (Children != null)
            {
                foreach (var node in Children)
                {
                    node.RenderAsXmlSiteMapToStringBuilder(xw);
                }
            }
        }

        private void RenderThis(XmlWriter xw)
        {
            if (xw == null) return;

            if (Url.Trim().Length > 4)
            {
                xw.WriteStartElement("url");
                xw.WriteElementString("loc", Url);
                if (LastModified.HasValue)
                {
                    // write date in ISO 8601 Format
                    xw.WriteElementString("lastmod", LastModified.Value.ToString("s"));
                }
                if (Priority != SiteMapPriority.NotSet)
                {
                    switch (Priority)
                    {
                        case SiteMapPriority.p1:
                            xw.WriteElementString("priority", "0.1");
                            break;
                        case SiteMapPriority.p2:
                            xw.WriteElementString("priority", "0.2");
                            break;
                        case SiteMapPriority.p3:
                            xw.WriteElementString("priority", "0.3");
                            break;
                        case SiteMapPriority.p4:
                            xw.WriteElementString("priority", "0.4");
                            break;
                        case SiteMapPriority.p5:
                            xw.WriteElementString("priority", "0.5");
                            break;
                        case SiteMapPriority.p6:
                            xw.WriteElementString("priority", "0.6");
                            break;
                        case SiteMapPriority.p7:
                            xw.WriteElementString("priority", "0.7");
                            break;
                        case SiteMapPriority.p8:
                            xw.WriteElementString("priority", "0.8");
                            break;
                        case SiteMapPriority.p9:
                            xw.WriteElementString("priority", "0.9");
                            break;
                        case SiteMapPriority.p10:
                            xw.WriteElementString("priority", "1.0");
                            break;
                    }
                }
                if (ChangeFrequency != SiteMapChangeFrequency.NotSet)
                {
                    switch (ChangeFrequency)
                    {
                        case SiteMapChangeFrequency.always:
                            xw.WriteElementString("changefreq", "always");
                            break;
                        case SiteMapChangeFrequency.daily:
                            xw.WriteElementString("changefreq", "daily");
                            break;
                        case SiteMapChangeFrequency.hourly:
                            xw.WriteElementString("changefreq", "hourly");
                            break;
                        case SiteMapChangeFrequency.monthly:
                            xw.WriteElementString("changefreq", "monthly");
                            break;
                        case SiteMapChangeFrequency.never:
                            xw.WriteElementString("changefreq", "never");
                            break;
                        case SiteMapChangeFrequency.weekly:
                            xw.WriteElementString("changefreq", "weekly");
                            break;
                        case SiteMapChangeFrequency.yearly:
                            xw.WriteElementString("changefreq", "yearly");
                            break;
                    }
                }
                xw.WriteEndElement();
            }
        }
    }
}
