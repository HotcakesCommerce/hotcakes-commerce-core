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

using System.Collections.Generic;
using System.Globalization;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    public class AnalyticInfo
    {
        public int SalesByDesktopCount { get; set; }
        public decimal SalesByDesktopPercent { get; set; }
        public decimal SalesByDesktopPercentPrev { get; set; }
        public int SalesByTabletCount { get; set; }
        public decimal SalesByTabletPercent { get; set; }
        public decimal SalesByTabletPercentPrev { get; set; }
        public int SalesByPhoneCount { get; set; }
        public decimal SalesByPhonePercent { get; set; }
        public decimal SalesByPhonePercentPrev { get; set; }

        public decimal Revenue { get; set; }
        public int UnitsSold { get; set; }
        public decimal AverageDealSize { get; set; }

        public int GiftCardSold { get; set; }
        public int GiftCardUsed { get; set; }
        public decimal GiftCardSoldAmount { get; set; }
        public decimal GiftCardUsedAmount { get; set; }

        public int RewardPointsAccrued { get; set; }
        public int RewardPointsUsed { get; set; }
        public decimal RewardPointsAccruedAmount { get; set; }
        public decimal RewardPointsUsedAmount { get; set; }

        public List<PromoInfo> ActivePromotions { get; set; }

        public class PromoInfo
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }

    public class AnalyticInfoJson
    {
        public AnalyticInfoJson(AnalyticInfo info)
        {
            var ci = CultureInfo.CurrentUICulture;
            SalesByDesktopCount = info.SalesByDesktopCount;
            SalesByDesktopComparison = string.Format("{0:F2}% vs {1:F2}%", info.SalesByDesktopPercent,
                info.SalesByDesktopPercentPrev);
            if (info.SalesByDesktopPercent != info.SalesByDesktopPercentPrev)
                SalesByDesktopGrowing = info.SalesByDesktopPercent > info.SalesByDesktopPercentPrev;

            SalesByTabletCount = info.SalesByTabletCount;
            SalesByTabletComparison = string.Format("{0:F2}% vs {1:F2}%", info.SalesByTabletPercent,
                info.SalesByTabletPercentPrev);
            if (info.SalesByTabletPercent != info.SalesByTabletPercentPrev)
                SalesByTabletGrowing = info.SalesByTabletPercent > info.SalesByTabletPercentPrev;

            SalesByPhoneCount = info.SalesByPhoneCount;
            SalesByPhoneComparison = string.Format("{0:F2}% vs {1:F2}%", info.SalesByPhonePercent,
                info.SalesByPhonePercentPrev);
            if (info.SalesByPhonePercent != info.SalesByPhonePercentPrev)
                SalesByPhoneGrowing = info.SalesByPhonePercent > info.SalesByPhonePercentPrev;

            Revenue = Money.GetFriendlyAmount(info.Revenue, ci, 4, "c");
            UnitsSold = Money.GetFriendlyAmount(info.UnitsSold, ci, 4);
            AverageDealSize = Money.GetFriendlyAmount(info.AverageDealSize, ci);
            GiftCardSold = info.GiftCardSold;
            GiftCardUsed = info.GiftCardUsed;
            GiftCardSoldAmount = Money.GetFriendlyAmount(info.GiftCardSoldAmount, ci, 6, "c");
            GiftCardUsedAmount = Money.GetFriendlyAmount(info.GiftCardUsedAmount, ci, 6, "c");
            RewardPointsAccrued = Money.GetFriendlyAmount(info.RewardPointsAccrued, ci);
            RewardPointsUsed = Money.GetFriendlyAmount(info.RewardPointsUsed, ci);
            RewardPointsAccruedAmount = Money.GetFriendlyAmount(info.RewardPointsAccruedAmount, ci, 6, "c");
            RewardPointsUsedAmount = Money.GetFriendlyAmount(info.RewardPointsUsedAmount, ci, 6, "c");
            ActivePromotions = info.ActivePromotions;
        }

        public int SalesByDesktopCount { get; set; }
        public string SalesByDesktopComparison { get; set; }
        public bool? SalesByDesktopGrowing { get; set; }

        public int SalesByTabletCount { get; set; }
        public string SalesByTabletComparison { get; set; }
        public bool? SalesByTabletGrowing { get; set; }

        public int SalesByPhoneCount { get; set; }
        public string SalesByPhoneComparison { get; set; }
        public bool? SalesByPhoneGrowing { get; set; }

        public string Revenue { get; set; }
        public string UnitsSold { get; set; }
        public string AverageDealSize { get; set; }

        public int GiftCardSold { get; set; }
        public int GiftCardUsed { get; set; }
        public string GiftCardSoldAmount { get; set; }
        public string GiftCardUsedAmount { get; set; }

        public string RewardPointsAccrued { get; set; }
        public string RewardPointsUsed { get; set; }
        public string RewardPointsAccruedAmount { get; set; }
        public string RewardPointsUsedAmount { get; set; }
        public List<AnalyticInfo.PromoInfo> ActivePromotions { get; set; }
    }
}