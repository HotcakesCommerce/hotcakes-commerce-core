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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
	public abstract class HasProductsQualificationBase : PromotionQualificationBase
	{
		#region Constructor

		public HasProductsQualificationBase()
		{
			ProcessingCost = RelativeProcessingCost.Lower;
		}

		#endregion

		#region Public methods

		public List<string> GetProductIds()
		{
			return GetSettingArr("products");
		}

		public void AddProductIds(IEnumerable<string> bvins)
		{
			AddSettingItems("products", bvins);
		}

		public void RemoveProductId(string bvin)
		{
			RemoveSettingItem("products", bvin);
		}

		public override string FriendlyDescription(HotcakesApplication app)
		{
            var result = "When Line Item is:<ul>";

			foreach (var bvin in GetProductIds())
			{
				string pBvin = bvin;
				string[] aBvin = bvin.Split('#');
				if (aBvin.Count() > 1)
				{
					pBvin = aBvin[0];
				}


				var p = app.CatalogServices.Products.FindWithCache(pBvin);
				if (p != null)
				{
					result += "<li>[" + p.Sku + "] " + p.ProductName + "</li>";
				}
			}

			result += "</ul>";
			return result;
		}

		#endregion
	}

	public class ProductBvin : HasProductsQualificationBase
    {
        public ProductBvin()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        public ProductBvin(string bvin)
            : this(new List<string> {bvin})
        {
        }

        public ProductBvin(List<string> bvins)
            : this()
        {
            AddProductIds(bvins);
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdProductBvin); }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Product is:<ul>";
            foreach (var bvin in GetProductIds())
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
            if (mode != PromotionQualificationMode.Products) return false;
            if (context == null) return false;
            if (context.Product == null) return false;
            if (context.UserPrice == null) return false;

			List<string> ids = GetProductIds();
			List<string> match = new List<string>();
			match.Add(context.Product.Bvin.Trim().ToLowerInvariant());

			if (context.Product.IsBundle)
			{
				//All Variants
				if (ids.Contains(context.Product.Bvin))
				{
					return true;
				}

				List<Tuple<string, string>> itemsToFind = new List<Tuple<string, string>>();
				foreach (string bvin in ids)
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

				List<Tuple<string, string>> pv = new List<Tuple<string, string>>();
			
				context.Product.BundledProducts.ForEach(b =>
				{
					b.BundledProduct.Variants.ForEach(x =>
					{
						pv.Add(Tuple.Create<string, string>(context.Product.Bvin, x.Bvin));
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
				return matchCount > 0;
			}
			else
			{
				if (context.Product.HasVariants())
				{
					match.AddRange(context.Product.Variants.Select(x => x.Bvin.Trim().ToLowerInvariant()));
				}
				return ids.Intersect(match).Count() > 0;
			}


        }
    }
}
