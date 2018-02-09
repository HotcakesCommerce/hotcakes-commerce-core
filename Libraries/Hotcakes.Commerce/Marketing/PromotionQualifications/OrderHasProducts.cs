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
using System.Linq;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{    
    public class OrderHasProducts : PromotionIdQualificationBase
    {
        public OrderHasProducts()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
            // Only supporting "Has At Least" mode for now
            HasMode = QualificationHasMode.HasAtLeast;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdOrderHasProducts); }
        }
        

        public QualificationHasMode HasMode
        {
            get
            {
                var result = QualificationHasMode.HasAtLeast;
                var temp = GetSettingAsInt("HasMode");
                  if (temp < 0) temp = 0;
                result = (QualificationHasMode) temp;
                  return result;
                }
            set { SetSetting("HasMode", (int) value); }
        }

        public QualificationSetMode SetMode
        {
            get
            {
                var result = QualificationSetMode.AnyOfTheseItems;
                var temp = GetSettingAsInt("SetMode");
                  if (temp < 0) temp = 0;
                result = (QualificationSetMode) temp;
                  return result;
                }
            set { SetSetting("SetMode", (int) value); }
        }

        public int Quantity
        {
            get { return GetSettingAsInt("Quantity"); }
            set { SetSetting("Quantity", value); }
        }

        public override string IdSettingName
        {
            get { return "ProductIds"; }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "";
            
            switch (HasMode)
            {
                case QualificationHasMode.HasAtLeast:
                    result += "When order has AT LEAST ";
                    break;
            }
            result += Quantity.ToString();
            switch (SetMode)
            {
                case QualificationSetMode.AllOfTheseItems:
                    result += " of ALL of these products";
                    break;
                case QualificationSetMode.AnyOfTheseItems:
                    result += " of ANY of these products";
                    break;
            }
            result += ":<ul>";
            
            foreach (string bvin in this.CurrentIds())
            {
				string pBvin = bvin;
				string[] aBvin = bvin.Split('#');
				if (aBvin.Count() > 1)
				{
					pBvin = aBvin[0];
				}

				Catalog.Product p = app.CatalogServices.Products.FindWithCache(pBvin);
                if (p != null)
                {
                    result += "<li>[" + p.Sku + "] " + p.ProductName + "</li>";
                }
            }
            result += "</ul>";
            return result;
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Orders && 
                mode != PromotionQualificationMode.LineItems) return false;
            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;
            

			List<string> currentIds = this.CurrentIds();

            switch (SetMode)
            {
                case QualificationSetMode.AnyOfTheseItems:
					return MatchAny(this.Quantity, context.Order.Items, currentIds, context);                    
                case QualificationSetMode.AllOfTheseItems:
					return MatchAll(this.Quantity, context.Order.Items, currentIds, context);                    
            }                                 

            return false;            
        }

		private bool MatchAny(int qty, List<LineItem> items, List<string> productIds, PromotionContext context)
        {
            int QuantityLeftToMatch = qty;

            foreach (LineItem li in items)
            {
				if (li.IsBundle)
				{
					if (productIds.Contains(li.ProductId.Trim().ToLowerInvariant()) ) 
					{
						QuantityLeftToMatch -= li.Quantity;
						if (QuantityLeftToMatch <= 0) return true;
					}

					Product p = context.HccApp.CatalogServices.Products.FindWithCache(li.ProductId);
					List<string> productOptions = new List<string>();
					List<string> selectedOptions = new List<string>();
					if(p != null) 
					{
						p.BundledProducts.ForEach(b => 
						{
							b.BundledProduct.Variants.ForEach(x =>
							{
								var selections = x.Selections.Select(y => y.SelectionData).ToList();
								productOptions.AddRange(selections);
							});
						});
						productOptions = productOptions.Distinct().ToList();

						li.SelectionData.BundleSelectionList.Values.ToList().ForEach(x =>
						{
							var selections = x.Select(y => y.SelectionData).ToList();
							selectedOptions.AddRange(selections);
						});
						selectedOptions = selectedOptions.Distinct().ToList();

						List<string> applicablePromotions = productIds.Where(x => x.Contains('#')).ToList();


						foreach (string promotionItem in applicablePromotions)
						{
							string productId = promotionItem.Split('#')[0];
							string variantId = promotionItem.Split('#')[1];

							bool isApplicable = productOptions.Intersect(selectedOptions).Count() == selectedOptions.Count;
							if (li.ProductId == productId && isApplicable)
							{
								QuantityLeftToMatch -= li.Quantity;
								if (QuantityLeftToMatch <= 0) return true;
							}
						}
					}
					return false;
				}
				else
				{
					if (productIds.Contains(li.ProductId.Trim().ToLowerInvariant()) || productIds.Contains(li.VariantId.Trim().ToLowerInvariant()))
                {
                    QuantityLeftToMatch -= li.Quantity;
                    if (QuantityLeftToMatch <= 0) return true;
                }
            }
            }

            return false;
        }
        private bool MatchAll(int qty, List<LineItem> items, List<string> productIds,PromotionContext context)
        {
            // Build up dictionary of items to match with quantities
			Dictionary<Tuple<string, string>, int> itemsToFind = new Dictionary<Tuple<string, string>, int>();
			List<Tuple<string, string>> bvins = new List<Tuple<string, string>>();

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
				itemsToFind.Add(pr, qty);
				bvins.Add(pr);
            }

            // Subtract each quantity found for items
            foreach (LineItem li in items)
            {

				if (li.IsBundle)
				{
					var allVariantsLid = Tuple.Create<string, string>(li.ProductId, string.Empty);
					if(itemsToFind.Keys.Contains(allVariantsLid)) 
					{
						itemsToFind[allVariantsLid] -= li.Quantity;
						continue;
					}

					Product p = context.HccApp.CatalogServices.Products.FindWithCache(li.ProductId);
					if (p != null)
					{
						List<Tuple<string, string>> pv = new List<Tuple<string, string>>();

						List<string> selectedOptions = new List<string>();
						li.SelectionData.BundleSelectionList.Values.ToList().ForEach(x =>
						{
							var selections = x.Select(y => y.SelectionData).ToList();
							selectedOptions.AddRange(selections);
						});
						selectedOptions = selectedOptions.Distinct().ToList();

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

						foreach (var item in pv)
						{
							Tuple<string, string> lidKey = itemsToFind.Keys.Where(x => x.Item1.Trim().ToLowerInvariant() == item.Item1.ToLowerInvariant() && x.Item2.ToLowerInvariant() == item.Item2.ToLowerInvariant()).FirstOrDefault();
							if (lidKey != null)
							{
								itemsToFind[lidKey] -= li.Quantity;
							}
						}
					}
				}
				else
				{
					string pid = li.ProductId.Trim().ToLowerInvariant();
					string vid = li.VariantId.Trim().ToLowerInvariant();

					Tuple<string, string> lidKey = itemsToFind.Keys.Where(x => x.Item1.Trim().ToLowerInvariant() == pid).FirstOrDefault();
					Tuple<string, string> vidKey = itemsToFind.Keys.Where(x => x.Item1.Trim().ToLowerInvariant() == vid).FirstOrDefault();

					if (lidKey != null)
					{
						itemsToFind[lidKey] -= li.Quantity;
					}
					else if (vidKey != null)
                {
						itemsToFind[vidKey] -= li.Quantity;
					}
                }

				
            }

			foreach (var bvin2 in bvins)
            {
                // If we didn't get enough quantity found, return false;
				if (itemsToFind[bvin2] > 0)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}