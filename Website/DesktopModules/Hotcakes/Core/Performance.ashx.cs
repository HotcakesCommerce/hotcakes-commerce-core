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
using System.Linq;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core
{
	/// <summary>
	///     Summary description for Performance
	/// </summary>
	public class Performance : BaseHandler, IHttpHandler
	{
		protected override object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            if (request.RequestContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                // not found
                request.RequestContext.HttpContext.Response.StatusCode = 404;
                request.RequestContext.HttpContext.Response.End();
                return null;
            }

            var method = request.Params["method"];

			switch (method)
			{
				case "GetProductPerformanceData":
					return GetProductPerformanceData(request, hccApp);
				case "GetCategoryPerformanceData":
					return GetCategoryPerformanceData(request, hccApp);
				case "GetProductPurchasedWithData":
					return GetProductPurchasedWithData(request, hccApp);
				case "CreateBundle":
					return CreateBundle(request, hccApp);
				default:
					break;
			}
			return true;
		}

		public object GetProductPerformanceData(HttpRequest request, HotcakesApplication hccApp)
		{
			var productId = request.Params["productId"];
			var period = (SalesPeriod)Enum.Parse(typeof(SalesPeriod), request.Params["period"]);

			// Update setting only if it is different
			var performanceUserSelections = new PerformanceUserSelections();
			if (performanceUserSelections.ProductsPerformacePeriod != period)
			{
				performanceUserSelections.ProductsPerformacePeriod = period;
			}

			var reportingService = Factory.CreateService<ReportingService>();
			var performanceInfo = reportingService.GetProductPerformance(productId, period);

			var performanceJson = new PerformanceInfoJson(performanceInfo, period, Localization);

			return performanceJson; 
		}

		public object GetCategoryPerformanceData(HttpRequest request, HotcakesApplication hccApp)
		{
			var categoryId = request.Params["categoryId"];
			var period = (SalesPeriod)Enum.Parse(typeof(SalesPeriod), request.Params["period"]);

			// Update setting only if it is different
			var performanceUserSelections = new PerformanceUserSelections();
			if (performanceUserSelections.CategoriesPerformacePeriod != period)
			{
				performanceUserSelections.CategoriesPerformacePeriod = period;
			}

			var reportingService = Factory.CreateService<ReportingService>();
			var performanceInfo = reportingService.GetCategoryPerformance(categoryId, period);

			return new PerformanceInfoJson(performanceInfo, period, Localization);
		}

		public object GetProductPurchasedWithData(HttpRequest request, HotcakesApplication hccApp)
		{
			var productId = request.Params["productId"];
			var period = (SalesPeriod)Enum.Parse(typeof(SalesPeriod), request.Params["period"]);

			var products = hccApp.CatalogServices.Products.GetMostPurchasedWith(productId, period, 4);

			var results = new List<ProductPurchasedWithInfo>();
			foreach (var product in products)
			{
				var reportingService = Factory.CreateService<ReportingService>();
				var performanceInfo = reportingService.GetPurchasedWithProductPerformance(product.Bvin, period);

				var result = new ProductPurchasedWithInfo(hccApp, product);

				result.QuantitySold = performanceInfo.QuantitySold;
				result.Revenue = performanceInfo.Revenue.ToString("C");

				result.IsQuantitySoldGrowing = performanceInfo.QuantitySold >= performanceInfo.QuantitySoldPrev;
				result.IsRevenueGrowing = performanceInfo.Revenue >= performanceInfo.RevenuePrev;

				var periodString = LocalizationUtils.GetSalesPeriodLower(period);
				var lessThan = Localization.GetFormattedString("LessSinceLast", periodString);
				var moreThan = Localization.GetFormattedString("MoreSinceLast", periodString);

				result.QuantitySoldComparison = result.IsQuantitySoldGrowing ? moreThan : lessThan;
				result.RevenueComparison = result.IsRevenueGrowing ? moreThan : lessThan;

				var quantitySoldPercentageChange = 1D;
				if (performanceInfo.QuantitySoldPrev != 0)
					quantitySoldPercentageChange = (performanceInfo.QuantitySold - performanceInfo.QuantitySoldPrev) /
												   performanceInfo.QuantitySoldPrev / 100.0;
				result.QuantitySoldPercentageChange = quantitySoldPercentageChange.ToString("p0");

				var revenuePercentageChange = 1D;
				if (performanceInfo.RevenuePrev != 0)
					revenuePercentageChange =
						(double)
							((performanceInfo.Revenue - performanceInfo.RevenuePrev) / performanceInfo.RevenuePrev / 100);
				result.RevenuePercentageChange = revenuePercentageChange.ToString("p0");

				results.Add(result);
			}

			return results;
		}

		/// <summary>
		///     Creates the bundle.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="hccApp">The HCC application.</param>
		/// <returns></returns>
		public object CreateBundle(HttpRequest request, HotcakesApplication hccApp)
		{
			var productId = request.Params["productId"];
			var productIdString = request.Params["productIds"];

			var productIds = Json.ObjectFromJson<string[]>(productIdString).ToList();

			productIds.Insert(0, productId);

			var mainProduct = hccApp.CatalogServices.Products.FindWithCache(productId);

			var p = new Product();
			p.Sku = CreateSku(mainProduct.Sku + "Bundle", hccApp);
			p.ProductName = mainProduct.ProductName + " Bundle";
			p.IsBundle = true;
			p.IsSearchable = true;
			p.Status = ProductStatus.Disabled;
			p.InventoryMode = ProductInventoryMode.AlwayInStock;
			p.UrlSlug = CreateUrlSlug(mainProduct.UrlSlug + "-bundle", hccApp);
			hccApp.CatalogServices.ProductsCreateWithInventory(p, true);

			foreach (var subProductId in productIds)
			{
				var bundledProduct = new BundledProduct
				{
					BundledProductId = subProductId,
					ProductId = p.Bvin,
					Quantity = 1
				};

				hccApp.CatalogServices.BundledProductCreate(bundledProduct);
			}

			return string.Format("/DesktopModules/Hotcakes/Core/Admin/catalog/Products_Edit.aspx?id={0}", p.Bvin);
		}

		#region Implementation

		private string CreateSku(string sku, HotcakesApplication hccApp)
		{
			// Try 10000 times to append to URL if in use
			var skuInUse = hccApp.CatalogServices.Products.IsSkuExist(sku);
			var baseSku = sku;

			for (var i = 2; i < 10000; i++)
			{
				if (skuInUse)
				{
					sku = string.Concat(baseSku, "-", i.ToString());
					skuInUse = hccApp.CatalogServices.Products.IsSkuExist(sku);

					if (!skuInUse)
						return sku;
				}
			}
			return string.Empty;
		}

		private string CreateUrlSlug(string ulrSlug, HotcakesApplication hccApp)
		{
			// Try 10000 times to append to URL if in use
			var rewriteUrlInUse = UrlRewriter.IsProductSlugInUse(ulrSlug, string.Empty, hccApp);
			var baseRewriteUrl = ulrSlug;

			for (var i = 2; i < 10000; i++)
			{
				if (rewriteUrlInUse)
				{
					ulrSlug = string.Concat(baseRewriteUrl, "-", i.ToString());
					rewriteUrlInUse = UrlRewriter.IsProductSlugInUse(ulrSlug, string.Empty, hccApp);

					if (!rewriteUrlInUse)
						return ulrSlug;
				}
			}
			return string.Empty;
		}

		#endregion
	}
}