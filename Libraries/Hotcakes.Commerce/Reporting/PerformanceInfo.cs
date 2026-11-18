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
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Reporting
{
	/// <summary>Contains funnel chart series, labels, events, and current/comparison counts.</summary>
	public class PerformanceInfo
	{
		/// <summary>Initializes empty chart series, labels, and event collections.</summary>
		public PerformanceInfo()
		{
			BouncedData = new List<int>();
			AbandonedData = new List<int>();
			PurchasedData = new List<int>();

			ChartLabels = new List<string>();
			Events = new List<string[]>();
		}

		/// <summary>Gets or sets the bounced counts for each chart interval.</summary>
		public List<int> BouncedData { get; set; }
		/// <summary>Gets or sets the abandoned counts for each chart interval.</summary>
		public List<int> AbandonedData { get; set; }
		/// <summary>Gets or sets the purchased counts for each chart interval.</summary>
		public List<int> PurchasedData { get; set; }
		/// <summary>Gets or sets the labels corresponding to the chart series.</summary>
		public List<string> ChartLabels { get; set; }
		/// <summary>Gets or sets event rows associated with the chart.</summary>
		public List<string[]> Events { get; set; }
		/// <summary>Gets or sets current views.</summary>
		public int Views { get; set; }
		/// <summary>Gets or sets current add-to-cart events.</summary>
		public int AddsToCart { get; set; }
		/// <summary>Gets or sets current purchases.</summary>
		public int Purchases { get; set; }
		/// <summary>Gets or sets comparison-period views.</summary>
		public int ViewsPrev { get; set; }
		/// <summary>Gets or sets comparison-period add-to-cart events.</summary>
		public int AddsToCartPrev { get; set; }
		/// <summary>Gets or sets comparison-period purchases.</summary>
		public int PurchasesPrev { get; set; }
	}

	/// <summary>Provides the localized, formatted JSON projection of <see cref="PerformanceInfo"/>.</summary>
	public class PerformanceInfoJson
	{
		/// <summary>Initializes an empty projection.</summary>
		public PerformanceInfoJson()
		{
		}

		/// <summary>Initializes a projection using a period-aware localized comparison.</summary>
		/// <param name="performanceInfo">The source funnel data.</param>
		/// <param name="period">The reporting period used in comparison text.</param>
		/// <param name="localization">The localization helper used for labels and messages.</param>
		public PerformanceInfoJson(PerformanceInfo performanceInfo, SalesPeriod period, ILocalizationHelper localization)
		{
			BouncedName = localization.GetString("Bounced");
			AbandonedName = localization.GetString("Abandoned");
			PurchasedName = localization.GetString("Purchased");

			BouncedData = performanceInfo.BouncedData;
			AbandonedData = performanceInfo.AbandonedData;
			PurchasedData = performanceInfo.PurchasedData;

			ChartLabels = performanceInfo.ChartLabels;
			Events = performanceInfo.Events;

			Views = performanceInfo.Views.ToString("N0");
			AddsToCart = performanceInfo.AddsToCart.ToString("N0");
			Purchases = performanceInfo.Purchases.ToString("N0");

			var bouncesPercentageChange = 0D;
			if (performanceInfo.ViewsPrev != 0)
			{
				bouncesPercentageChange = Math.Abs(performanceInfo.Views - performanceInfo.ViewsPrev) /
										  (double)performanceInfo.ViewsPrev;
			}
			else if (performanceInfo.Views != 0)
			{
				bouncesPercentageChange = 1;
			}
			bouncesPercentageChange = Math.Round(bouncesPercentageChange, 2);
			ViewsPercentageChange = bouncesPercentageChange.ToString("p0");

			var abandomentsPercentageChange = 0D;
			if (performanceInfo.AddsToCartPrev != 0)
			{
				abandomentsPercentageChange = Math.Abs(performanceInfo.AddsToCart - performanceInfo.AddsToCartPrev) /
											  (double)performanceInfo.AddsToCartPrev;
			}
			else if (performanceInfo.AddsToCart != 0)
			{
				abandomentsPercentageChange = 1;
			}
			abandomentsPercentageChange = Math.Round(abandomentsPercentageChange, 2);
			AddsToCartPercentageChange = abandomentsPercentageChange.ToString("p0");

			var purchasesPercentageChange = 0D;
			if (performanceInfo.PurchasesPrev != 0)
			{
				purchasesPercentageChange = Math.Abs(performanceInfo.Purchases - performanceInfo.PurchasesPrev) /
											(double)performanceInfo.PurchasesPrev;
			}
			else if (performanceInfo.Purchases != 0)
			{
				purchasesPercentageChange = 1;
			}
			purchasesPercentageChange = Math.Round(purchasesPercentageChange, 2);
			PurchasesPercentageChange = purchasesPercentageChange.ToString("p0");

			var periodString = LocalizationUtils.GetSalesPeriodLower(period);
			var noChange = localization.GetFormattedString("NoChangeSinceLast", periodString);
			var lessThan = localization.GetFormattedString("LessSinceLast", periodString);
			var moreThan = localization.GetFormattedString("MoreSinceLast", periodString);

			ViewsComparison = noChange;
			if (Math.Abs(bouncesPercentageChange) >= 0.01)
			{
				IsViewsGrowing = performanceInfo.Views >= performanceInfo.ViewsPrev;
				ViewsComparison = IsViewsGrowing.Value ? moreThan : lessThan;
			}

			AddsToCartComparison = noChange;
			if (Math.Abs(abandomentsPercentageChange) >= 0.01)
			{
				IsAddsToCartGrowing = performanceInfo.AddsToCart >= performanceInfo.AddsToCartPrev;
				AddsToCartComparison = IsAddsToCartGrowing.Value ? moreThan : lessThan;
			}

			PurchasesComparison = noChange;
			if (Math.Abs(purchasesPercentageChange) >= 0.01)
			{
				IsPurchasesGrowing = performanceInfo.Purchases >= performanceInfo.PurchasesPrev;
				PurchasesComparison = IsPurchasesGrowing.Value ? moreThan : lessThan;
			}
		}

		/// <summary>Gets or sets the localized bounced label.</summary>
		public string BouncedName { get; set; }
		/// <summary>Gets or sets the localized abandoned label.</summary>
		public string AbandonedName { get; set; }
		/// <summary>Gets or sets the localized purchased label.</summary>
		public string PurchasedName { get; set; }
		/// <summary>Gets or sets the bounced chart series.</summary>
		public List<int> BouncedData { get; set; }
		/// <summary>Gets or sets the abandoned chart series.</summary>
		public List<int> AbandonedData { get; set; }
		/// <summary>Gets or sets the purchased chart series.</summary>
		public List<int> PurchasedData { get; set; }
		/// <summary>Gets or sets chart labels.</summary>
		public List<string> ChartLabels { get; set; }
		/// <summary>Gets or sets chart event rows.</summary>
		public List<string[]> Events { get; set; }
		/// <summary>Gets or sets the formatted view count.</summary>
		public string Views { get; set; }
		/// <summary>Gets or sets the absolute view percentage change.</summary>
		public string ViewsPercentageChange { get; set; }
		/// <summary>Gets or sets the localized view comparison.</summary>
		public string ViewsComparison { get; set; }
		/// <summary>Gets whether views are growing; null indicates the threshold was not exceeded.</summary>
		public bool? IsViewsGrowing { get; set; }
		/// <summary>Gets or sets the formatted add-to-cart count.</summary>
		public string AddsToCart { get; set; }
		/// <summary>Gets or sets the absolute add-to-cart percentage change.</summary>
		public string AddsToCartPercentageChange { get; set; }
		/// <summary>Gets or sets the localized add-to-cart comparison.</summary>
		public string AddsToCartComparison { get; set; }
		/// <summary>Gets whether add-to-cart activity is growing; null indicates the threshold was not exceeded.</summary>
		public bool? IsAddsToCartGrowing { get; set; }
		/// <summary>Gets or sets the formatted purchase count.</summary>
		public string Purchases { get; set; }
		/// <summary>Gets or sets the absolute purchase percentage change.</summary>
		public string PurchasesPercentageChange { get; set; }
		/// <summary>Gets or sets the localized purchase comparison.</summary>
		public string PurchasesComparison { get; set; }
		/// <summary>Gets whether purchases are growing; null indicates the threshold was not exceeded.</summary>
		public bool? IsPurchasesGrowing { get; set; }
	}
}
