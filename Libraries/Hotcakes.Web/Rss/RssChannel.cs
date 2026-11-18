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
using System.Net;
using System.Xml;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Rss
{
    /// <summary>Represents an RSS channel and provides methods for loading its content.</summary>
    [Serializable]
    public class RSSChannel
    {
        /// <summary>Initializes a channel using the supplied logger for load failures.</summary>
        /// <param name="logger">The logger used while loading the feed.</param>
        public RSSChannel(ILogger logger)
        {
            Description = string.Empty;
            FeedUrl = string.Empty;
            Link = string.Empty;
            Title = string.Empty;

            Logger = logger;
        }

        /// <summary>Gets or sets the channel description.</summary>
        public string Description { get; set; }
        /// <summary>Gets or sets the feed URL.</summary>
        public string FeedUrl { get; set; }
        /// <summary>Gets or sets the channel link.</summary>
        public string Link { get; set; }
        /// <summary>Gets or sets the channel title.</summary>
        public string Title { get; set; }

        private ILogger Logger { get; set; }

        /// <summary>Loads channel metadata from an RSS feed URL.</summary>
        /// <param name="url">The RSS feed URL.</param>
        public void LoadFromFeed(string url)
        {
            FeedUrl = url;
            LoadChannel();
        }

        private XmlNodeList GetXMLDoc(string node)
        {
            XmlNodeList tempNodeList = null;
            var rssDoc = new XmlDocument();

            // Load From Web
            var request = WebRequest.Create(FeedUrl);
            var response = request.GetResponse();
            var rssStream = response.GetResponseStream();
            var sr = new StreamReader(rssStream);
            var Data = sr.ReadToEnd();
            rssDoc.Load(new StringReader(Data));

            tempNodeList = rssDoc.SelectNodes(node);
            return tempNodeList;
        }

        private void LoadChannel()
        {
            try
            {
                var rss = GetXMLDoc("rss/channel");
                Title = rss[0].SelectSingleNode("title").InnerText;
                Link = rss[0].SelectSingleNode("link").InnerText;
                Description = rss[0].SelectSingleNode("description").InnerText;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /// <summary>Gets up to the requested number of items from the loaded channel.</summary>
        /// <param name="maxitems">The maximum number of items to return.</param><returns>A list of <see cref="RSSItem"/> values.</returns>
        public List<RSSItem> GetChannelItems(int maxitems)
        {
            var result = new List<RSSItem>();

            try
            {
                var itemCounter = 0;

                var rssItems = GetXMLDoc("rss/channel/item");
                foreach (XmlNode item in rssItems)
                {
                    if (itemCounter >= maxitems) break;

                    var newItem = new RSSItem
                    {
                        Title = item.SelectSingleNode("title").InnerText,
                        Link = item.SelectSingleNode("link").InnerText,
                        Description = item.SelectSingleNode("description").InnerText
                    };
                    
                    result.Add(newItem);
                    itemCounter += 1;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return result;
        }
    }
}
