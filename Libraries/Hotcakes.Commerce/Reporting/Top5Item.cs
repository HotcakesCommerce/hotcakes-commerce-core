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
using System.Globalization;
using System.Linq;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Represents one item in a top-five reporting result before JSON formatting.</summary>
    public class Top5Item
    {
        /// <summary>Gets or sets the item's GUID used when <see cref="Id"/> is not supplied.</summary>
        public Guid Guid { get; set; }
        /// <summary>Gets or sets the display or business identifier.</summary>
        public string Id { get; set; }
        /// <summary>Gets or sets the display name.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the optional numeric amount associated with the item.</summary>
        public decimal? Amount { get; set; }
        /// <summary>Gets or sets the format passed to the friendly amount formatter; when absent, currency format is used.</summary>
        public string AmountFormat { get; set; }
        /// <summary>Gets or sets the count associated with the item.</summary>
        public int Count { get; set; }
    }

    /// <summary>Contains the localized string values emitted for a <see cref="Top5Item"/>.</summary>
    public class Top5ItemJson
    {
        /// <summary>Gets or sets the item identifier, falling back to its GUID when needed.</summary>
        public string Id { get; set; }
        /// <summary>Gets or sets the item name.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the friendly-formatted amount, or an empty string when no amount exists.</summary>
        public string Amount { get; set; }
        /// <summary>Gets or sets the friendly-formatted count.</summary>
        public string Count { get; set; }

        /// <summary>Converts top-five items to their current-UI-culture JSON representation.</summary>
        /// <param name="items">The items to format.</param>
        /// <returns>A new list whose amount and count values are formatted for display.</returns>
        /// <remarks>This method also fills a missing source <see cref="Top5Item.AmountFormat"/> with <c>c</c>.</remarks>
        public static List<Top5ItemJson> ToJsonList(List<Top5Item> items)
        {
            var ci = CultureInfo.CurrentUICulture;

            var jItems = items.Select(i =>
            {
                i.AmountFormat = i.AmountFormat ?? "c";
                var strAmount = i.Amount.HasValue ? Money.GetFriendlyAmount(i.Amount.Value, ci, 3, i.AmountFormat) : string.Empty;

                return new Top5ItemJson
                {
                    Id = i.Id != null ? i.Id : i.Guid.ToString(),
                    Name = i.Name,
                    Amount = strAmount,
                    Count = Money.GetFriendlyAmount(i.Count, ci)
                };
            }).ToList();

            return jItems;
        }
    }
}
