#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    public class Top5Item
    {
        public Guid Guid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public string AmountFormat { get; set; }
        public int Count { get; set; }
    }

    public class Top5ItemJson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Count { get; set; }

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