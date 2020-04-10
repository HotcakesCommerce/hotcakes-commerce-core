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

using System;
using System.Collections.Generic;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Reporting
{
	public class PerformanceInfo
	{
		public PerformanceInfo()
		{
			BouncedData = new List<int>();
			AbandonedData = new List<int>();
			PurchasedData = new List<int>();

			ChartLabels = new List<string>();
			Events = new List<string[]>();
		}

		public List<int> BouncedData { get; set; }
		public List<int> AbandonedData { get; set; }
		public List<int> PurchasedData { get; set; }

		public List<string> ChartLabels { get; set; }
		public List<string[]> Events { get; set; }

		public int Views { get; set; }
		public int AddsToCart { get; set; }
		public int Purchases { get; set; }

		public int ViewsPrev { get; set; }
		public int AddsToCartPrev { get; set; }
		public int PurchasesPrev { get; set; }
	}

	public class PerformanceInfoJson
	{
		public PerformanceInfoJson()
		{
		}

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

		public string BouncedName { get; set; }
		public string AbandonedName { get; set; }
		public string PurchasedName { get; set; }

		public List<int> BouncedData { get; set; }
		public List<int> AbandonedData { get; set; }
		public List<int> PurchasedData { get; set; }

		public List<string> ChartLabels { get; set; }
		public List<string[]> Events { get; set; }

		public string Views { get; set; }
		public string ViewsPercentageChange { get; set; }
		public string ViewsComparison { get; set; }
		public bool? IsViewsGrowing { get; set; }

		public string AddsToCart { get; set; }
		public string AddsToCartPercentageChange { get; set; }
		public string AddsToCartComparison { get; set; }
		public bool? IsAddsToCartGrowing { get; set; }

		public string Purchases { get; set; }
		public string PurchasesPercentageChange { get; set; }
		public string PurchasesComparison { get; set; }
		public bool? IsPurchasesGrowing { get; set; }
	}
}