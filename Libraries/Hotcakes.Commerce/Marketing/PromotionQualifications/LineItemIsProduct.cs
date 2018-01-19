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
using System.Linq;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Catalog;
using System.Collections.Generic;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
	public class LineItemIsProduct : HasProductsQualificationBase
	{
		public const string TypeIdString = "CCB783E6-9CA3-42FF-A59F-E063D3EFEB99";

		public override Guid TypeId
		{
			get { return new Guid(TypeIdString); }
		}

		public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
		{
			if (context == null) return false;
			if (context.Order == null) return false;

			if (mode == PromotionQualificationMode.LineItems)
			{
				if (context.CurrentlyProcessingLineItem == null) return false;

				var li = context.CurrentlyProcessingLineItem;

				return MeetLineItem(context, li);
			}
            if (mode == PromotionQualificationMode.Orders)
			{
				var items = context.Order.Items;
				return items.Any(i => MeetLineItem(context, i));
			}

			return false;
		}

		private bool MeetLineItem(PromotionContext context, LineItem li)
		{
			var productIds =GetProductIds();
			if (li.IsBundle)
			{
				//if entire product (all variants were selected)
				if(productIds.Contains(li.ProductId))
				{
					return true;
				}

				List<Tuple<string, string>> itemsToFind = new List<Tuple<string, string>>();
				foreach (string bvin in productIds)
				{
					string productId = bvin;
					string variantId = string.Empty;

					if (bvin.Contains("#"))
					{
						productId = bvin.Split('#')[0];
						variantId = bvin.Split('#')[1];
					}
					var pr = Tuple.Create<string, string>(productId, variantId);
					itemsToFind.Add(pr);
				}

				Product p = context.HccApp.CatalogServices.Products.FindWithCache(li.ProductId);
				if (p != null)
				{
					List<string> selectedOptions = new List<string>();
					li.SelectionData.BundleSelectionList.Values.ToList().ForEach(x =>
					{
						var selections = x.Select(y => y.SelectionData).ToList();
						selectedOptions.AddRange(selections);
					});
					selectedOptions = selectedOptions.Distinct().ToList();

					List<Tuple<string, string>> pv = new List<Tuple<string, string>>();
					p.BundledProducts.ForEach(b =>
					{
						b.BundledProduct.Variants.ForEach(x =>
						{
							var selections = x.Selections.Select(y => y.SelectionData).ToList();

							bool isApplicable = selectedOptions.Intersect(selections).Count() > 0;
							if (isApplicable)
							{
								pv.Add(Tuple.Create<string, string>(li.ProductId, x.Bvin));
							}
						});
					});
					pv = pv.Distinct().ToList();

					int matchCount = 0;
					foreach (var item in pv)
					{
						Tuple<string, string> lidKey = itemsToFind.Where(x => x.Item1.Trim().ToLowerInvariant() == item.Item1.ToLowerInvariant() && x.Item2.ToLowerInvariant() == item.Item2.ToLowerInvariant()).FirstOrDefault();
						if (lidKey != null)
						{
							matchCount++;
						}
					}

					return matchCount == itemsToFind.Count;
				}
			}
			else
			{
				bool isInProductId = productIds.Contains(li.ProductId);
				bool isInVariantId = productIds.Contains(li.VariantId);
				return isInProductId || isInVariantId;
			}

			return false;
		}
	}
}
