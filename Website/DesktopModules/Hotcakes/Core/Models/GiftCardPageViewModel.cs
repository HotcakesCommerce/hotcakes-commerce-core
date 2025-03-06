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
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotcakes.Modules.Core.Models
{
    public class GiftCardData
    {
        public string KeyName { get; set; }
        public decimal Amount { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Greeting { get; set; }
        public string Message { get; set; }
        public string Picture { get; set; }

        public GiftCardData()
        {
            this.KeyName = string.Empty;
            this.Amount = 0.0m;
            this.To = string.Empty;
            this.From = string.Empty;
            this.Greeting = string.Empty;
            this.Message = string.Empty;
            this.Picture = string.Empty;
        }
    }
    public class GiftCardPageViewModel
    {
        public string ButtonUrlNext { get; set; }
        public string ButtonUrlPrevious { get; set; }

        public int Count25 { get; set; }
        public int Count50 { get; set; }
        public int Count75 { get; set; }
        public int Count100 { get; set; }
        public int Count200 { get; set; }
        public int Count300 { get; set; }
        public int Count400 { get; set; }
        public int Count500 { get; set; }

        public Dictionary<string, GiftCardData> CardData { get; set; }

        public GiftCardPageViewModel()
        {            
            this.ButtonUrlPrevious = string.Empty;
            this.ButtonUrlNext = string.Empty;
            
            this.Count25 = 0;            
            this.Count50 = 0;
            this.Count75 = 0;
            this.Count100 = 0;
            this.Count200 = 0;
            this.Count300 = 0;
            this.Count400 = 0;
            this.Count500 = 0;

            this.CardData = new Dictionary<string, GiftCardData>();
        }

        public bool HasAnyCounts()
        {
            int sum = this.Count25 + this.Count50
                + this.Count75 + this.Count100
                + this.Count200 + this.Count300
                + this.Count400 + this.Count500;
            return sum > 0;
        }

        public void EnsureCardData()
        {
            GenerateCardData(this.Count25, "g25");
            GenerateCardData(this.Count50, "g50");
            GenerateCardData(this.Count75, "g75");
            GenerateCardData(this.Count100, "g100");
            GenerateCardData(this.Count200, "g200");
            GenerateCardData(this.Count300, "g300");
            GenerateCardData(this.Count400, "g400");
            GenerateCardData(this.Count500, "g500");            
        }

        private void GenerateCardData(int count, string prefix)
        {
            if (this.CardData == null) this.CardData = new Dictionary<string,GiftCardData>();
            if (count < 1) return;
            for (int i = 1; i <= count; i++)
            {
                string key = prefix + i.ToString();
                if (this.CardData.ContainsKey(key) == false)
                {
                    this.CardData.Add(key, new GiftCardData());
                    this.CardData[key].KeyName = key;
                }
            }
        }
    }
}