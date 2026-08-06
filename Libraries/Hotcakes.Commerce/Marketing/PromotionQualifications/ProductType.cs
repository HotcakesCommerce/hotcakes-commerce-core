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
using System.Linq;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class ProductType : PromotionIdQualificationBase
    {
        #region Overrides

        public override Guid TypeId
        {
            get { return new Guid(TypeIdProductType); }
        }

        public bool IsNotMode
        {
            get
            {
                var all = GetSetting("PrTIsNotMode");

                return all == "1";
            }
            set { SetSetting("PrTIsNotMode", value ? "1" : "0"); }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var allTypes = app?.CatalogServices?.ProductTypes?.FindAll() ?? new System.Collections.Generic.List<Catalog.ProductType>();
            allTypes.Insert(0, new Catalog.ProductType { Bvin = "0", ProductTypeName = "Generic" });

            var result = "When Product Type is" + (IsNotMode ? " not" : string.Empty) + " :<ul>";
            var ids = CurrentIds();

            if (ids.Count > 0)
            {
                foreach (var bvin in ids)
                {
                    var p = allTypes.FirstOrDefault(y => string.Equals(y.Bvin?.Trim(), bvin, StringComparison.OrdinalIgnoreCase));
                    if (p != null)
                    {
                        result += "<li>" + p.ProductTypeName + "</li>";
                    }
                }
            }

            result += "</ul>";
            return result;
        }

        protected override void OnInit()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        public override string IdSettingName
        {
            get { return "TypeIds"; }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Products) return false;
            if (context == null) return false;
            if (context.Product == null) return false;
            if (context.UserPrice == null) return false;

            var ids = CurrentIds();
            var match = (context.Product.ProductTypeId ?? string.Empty).Trim().ToLowerInvariant();

            // for "generic" type we need to match 0 instead of empty string
            if (match == string.Empty) match = "0";

            if (IsNotMode)
            {
                return !ids.Contains(match);
            }
            return ids.Contains(match);
        }

        #endregion

        #region Init

        public ProductType()
        {
        }

        public ProductType(string id)
            : base(id)
        {
        }

        #endregion
    }
}