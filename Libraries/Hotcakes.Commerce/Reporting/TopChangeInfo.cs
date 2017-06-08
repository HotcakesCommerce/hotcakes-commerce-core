#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Reporting
{
    public class TopChangeInfo
    {
        public TopChangeInfo()
        {
            Items = new List<TopChangeItemInfo>();
        }

        public int TotalCount { get; set; }
        public List<TopChangeItemInfo> Items { get; set; }
    }

    public class TopChangeItemInfo
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal BouncesChange { get; set; }
        public decimal AbandomentsChange { get; set; }
        public decimal PurchasesChange { get; set; }

        public decimal Change { get; set; }
    }

    public class TopChangeInfoJson
    {
        public TopChangeInfoJson()
        {
            Items = new List<TopChangeItemInfoJson>();
        }

        public TopChangeInfoJson(TopChangeInfo topChangeInfo, ILocalizationHelper localization, int pageSize)
            : this()
        {
            TotalCount = topChangeInfo.TotalCount;

            var noChangeString = localization.GetString("NoChange");

            foreach (var topChangeItemInfo in topChangeInfo.Items)
            {
                var topChangeItemInfoJson = new TopChangeItemInfoJson
                {
                    ProductId = topChangeItemInfo.ProductId,
                    ProductName = topChangeItemInfo.ProductName,
                    ProductUrl =
                        string.Format("/DesktopModules/Hotcakes/Core/Admin/catalog/Products_Performance.aspx?id={0}",
                            topChangeItemInfo.ProductId)
                };

                topChangeItemInfoJson.BouncesChange = noChangeString;
                topChangeItemInfoJson.AbandomentsChange = noChangeString;
                topChangeItemInfoJson.PurchasesChange = noChangeString;
                topChangeItemInfoJson.Change = noChangeString;

                if (Math.Abs(topChangeItemInfo.BouncesChange) >= 0.01m)
                {
                    topChangeItemInfoJson.BouncesChange = Math.Abs(topChangeItemInfo.BouncesChange).ToString("p0");
                    topChangeItemInfoJson.IsBouncesChangeGrowing = topChangeItemInfo.BouncesChange > 0;
                }

                if (Math.Abs(topChangeItemInfo.AbandomentsChange) >= 0.01m)
                {
                    topChangeItemInfoJson.AbandomentsChange =
                        Math.Abs(topChangeItemInfo.AbandomentsChange).ToString("p0");
                    topChangeItemInfoJson.IsAbandomentsChangeGrowing = topChangeItemInfo.AbandomentsChange > 0;
                }

                if (Math.Abs(topChangeItemInfo.PurchasesChange) >= 0.01m)
                {
                    topChangeItemInfoJson.PurchasesChange = Math.Abs(topChangeItemInfo.PurchasesChange).ToString("p0");
                    topChangeItemInfoJson.IsPurchasesChangeGrowing = topChangeItemInfo.PurchasesChange > 0;
                }

                if (Math.Abs(topChangeItemInfo.Change) >= 0.01m)
                {
                    topChangeItemInfoJson.Change = Math.Abs(topChangeItemInfo.Change).ToString("p0");
                    topChangeItemInfoJson.IsChangeGrowing = topChangeItemInfo.Change > 0;
                }

                Items.Add(topChangeItemInfoJson);
            }

            while (Items.Count < pageSize)
                Items.Add(new TopChangeItemInfoJson());
        }

        public int TotalCount { get; set; }
        public List<TopChangeItemInfoJson> Items { get; set; }
    }

    public class TopChangeItemInfoJson
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }

        public string BouncesChange { get; set; }
        public string AbandomentsChange { get; set; }
        public string PurchasesChange { get; set; }

        public bool? IsBouncesChangeGrowing { get; set; }
        public bool? IsAbandomentsChangeGrowing { get; set; }
        public bool? IsPurchasesChangeGrowing { get; set; }

        public string Change { get; set; }
        public bool? IsChangeGrowing { get; set; }
    }
}