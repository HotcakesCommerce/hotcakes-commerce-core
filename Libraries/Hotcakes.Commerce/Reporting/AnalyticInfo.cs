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

using System.Collections.Generic;
using System.Globalization;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Contains analytics totals, device comparisons, loyalty activity, and active promotions.</summary>
    public class AnalyticInfo
    {
        /// <summary>Gets or sets the desktop sales count.</summary>
        public int SalesByDesktopCount { get; set; }
        /// <summary>Gets or sets the current desktop sales percentage.</summary>
        public decimal SalesByDesktopPercent { get; set; }
        /// <summary>Gets or sets the comparison-period desktop sales percentage.</summary>
        public decimal SalesByDesktopPercentPrev { get; set; }
        /// <summary>Gets or sets the tablet sales count.</summary>
        public int SalesByTabletCount { get; set; }
        /// <summary>Gets or sets the current tablet sales percentage.</summary>
        public decimal SalesByTabletPercent { get; set; }
        /// <summary>Gets or sets the comparison-period tablet sales percentage.</summary>
        public decimal SalesByTabletPercentPrev { get; set; }
        /// <summary>Gets or sets the phone sales count.</summary>
        public int SalesByPhoneCount { get; set; }
        /// <summary>Gets or sets the current phone sales percentage.</summary>
        public decimal SalesByPhonePercent { get; set; }
        /// <summary>Gets or sets the comparison-period phone sales percentage.</summary>
        public decimal SalesByPhonePercentPrev { get; set; }
        /// <summary>Gets or sets revenue.</summary>
        public decimal Revenue { get; set; }
        /// <summary>Gets or sets units sold.</summary>
        public int UnitsSold { get; set; }
        /// <summary>Gets or sets the average deal size.</summary>
        public decimal AverageDealSize { get; set; }
        /// <summary>Gets or sets the number of gift cards sold.</summary>
        public int GiftCardSold { get; set; }
        /// <summary>Gets or sets the number of gift cards used.</summary>
        public int GiftCardUsed { get; set; }
        /// <summary>Gets or sets the value of gift cards sold.</summary>
        public decimal GiftCardSoldAmount { get; set; }
        /// <summary>Gets or sets the value of gift cards used.</summary>
        public decimal GiftCardUsedAmount { get; set; }
        /// <summary>Gets or sets reward points accrued.</summary>
        public int RewardPointsAccrued { get; set; }
        /// <summary>Gets or sets reward points used.</summary>
        public int RewardPointsUsed { get; set; }
        /// <summary>Gets or sets the value of reward points accrued.</summary>
        public decimal RewardPointsAccruedAmount { get; set; }
        /// <summary>Gets or sets the value of reward points used.</summary>
        public decimal RewardPointsUsedAmount { get; set; }
        /// <summary>Gets or sets the active promotions.</summary>
        public List<PromoInfo> ActivePromotions { get; set; }

        /// <summary>Describes a promotion and its activity count.</summary>
        public class PromoInfo
        {
            /// <summary>Gets or sets the promotion URL.</summary>
            public string Url { get; set; }
            /// <summary>Gets or sets the promotion name.</summary>
            public string Name { get; set; }
            /// <summary>Gets or sets the promotion activity count.</summary>
            public int Count { get; set; }
        }
    }

    /// <summary>Provides the display-ready JSON projection of <see cref="AnalyticInfo"/>.</summary>
    public class AnalyticInfoJson
    {
        /// <summary>Initializes a localized projection using the current UI culture.</summary>
        /// <param name="info">The source analytics data.</param>
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

        /// <summary>Gets or sets the desktop sales count.</summary>
        public int SalesByDesktopCount { get; set; }
        /// <summary>Gets or sets the desktop comparison text.</summary>
        public string SalesByDesktopComparison { get; set; }
        /// <summary>Gets whether desktop sales are growing; null means no change.</summary>
        public bool? SalesByDesktopGrowing { get; set; }
        /// <summary>Gets or sets the tablet sales count.</summary>
        public int SalesByTabletCount { get; set; }
        /// <summary>Gets or sets the tablet comparison text.</summary>
        public string SalesByTabletComparison { get; set; }
        /// <summary>Gets whether tablet sales are growing; null means no change.</summary>
        public bool? SalesByTabletGrowing { get; set; }
        /// <summary>Gets or sets the phone sales count.</summary>
        public int SalesByPhoneCount { get; set; }
        /// <summary>Gets or sets the phone comparison text.</summary>
        public string SalesByPhoneComparison { get; set; }
        /// <summary>Gets whether phone sales are growing; null means no change.</summary>
        public bool? SalesByPhoneGrowing { get; set; }
        /// <summary>Gets or sets the localized revenue.</summary>
        public string Revenue { get; set; }
        /// <summary>Gets or sets the localized units sold.</summary>
        public string UnitsSold { get; set; }
        /// <summary>Gets or sets the localized average deal size.</summary>
        public string AverageDealSize { get; set; }
        /// <summary>Gets or sets the gift cards sold count.</summary>
        public int GiftCardSold { get; set; }
        /// <summary>Gets or sets the gift cards used count.</summary>
        public int GiftCardUsed { get; set; }
        /// <summary>Gets or sets the localized gift card sales amount.</summary>
        public string GiftCardSoldAmount { get; set; }
        /// <summary>Gets or sets the localized gift card usage amount.</summary>
        public string GiftCardUsedAmount { get; set; }
        /// <summary>Gets or sets the localized accrued reward points.</summary>
        public string RewardPointsAccrued { get; set; }
        /// <summary>Gets or sets the localized used reward points.</summary>
        public string RewardPointsUsed { get; set; }
        /// <summary>Gets or sets the localized accrued reward-point amount.</summary>
        public string RewardPointsAccruedAmount { get; set; }
        /// <summary>Gets or sets the localized used reward-point amount.</summary>
        public string RewardPointsUsedAmount { get; set; }
        /// <summary>Gets or sets the active promotions.</summary>
        public List<AnalyticInfo.PromoInfo> ActivePromotions { get; set; }
    }
}
