#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Marketing.PromotionQualifications;

namespace Hotcakes.Commerce.Marketing
{
    public class PromotionFactory
    {
        public virtual IPromotionQualification CreateQualification(Guid typeId)
        {
            switch (typeId.ToString().ToUpper())
            {
                case PromotionQualificationBase.TypeIdAnyProduct:
                    return new AnyProduct();
                case PromotionQualificationBase.TypeIdProductBvin:
                    return new ProductBvin();
                case PromotionQualificationBase.TypeIdProductCategory:
                    return new ProductCategory();
                case PromotionQualificationBase.TypeIdProductType:
                    return new ProductType();
                case PromotionQualificationBase.TypeIdOrderHasCoupon:
                    return new OrderHasCoupon();
                case PromotionQualificationBase.TypeIdAnyOrder:
                    return new AnyOrder();
                case PromotionQualificationBase.TypeIdOrderHasProducts:
                    return new OrderHasProducts();
                case PromotionQualificationBase.TypeIdOrderSubTotalIs:
                    return new OrderSubTotalIs();
                case PromotionQualificationBase.TypeIdUserIs:
                    return new UserIs();
                case PromotionQualificationBase.TypeIdUserIsInGroup:
                    return new UserIsInGroup();
                case PromotionQualificationBase.TypeIdShippingMethodIs:
                    return new ShippingMethodIs();
                case PromotionQualificationBase.TypeIdAnyShippingMethod:
                    return new AnyShippingMethod();
                case PromotionQualificationBase.TypeIdLineItemCategory:
                    return new LineItemCategory();
                case LineItemIsProduct.TypeIdString:
                    return new LineItemIsProduct();
                case AffiliateApproved.TypeIdString:
                    return new AffiliateApproved();
                case SumOrCountOfProducts.TypeIdString:
                    return new SumOrCountOfProducts();
                case ProductTypeIs.TypeIdString:
                    return new ProductTypeIs();
                case OrderHasNotProducts.TypeIdString:
                    return new OrderHasNotProducts();
                case VendorOrManufacturerIs.TypeIdString:
                    return new VendorOrManufacturerIs();
            }

            throw new InvalidOperationException(string.Format("Cannot create Qualification object based on typeId:",
                typeId));
        }

        public virtual IPromotionAction CreateAction(Guid typeId)
        {
            switch (typeId.ToString())
            {
                case ProductPriceAdjustment.TypeIdString:
                    return new ProductPriceAdjustment();
                case OrderTotalAdjustment.TypeIdString:
                    return new OrderTotalAdjustment();
                case OrderShippingAdjustment.TypeIdString:
                    return new OrderShippingAdjustment();
                case LineItemFreeShipping.TypeIdString:
                    return new LineItemFreeShipping();
                case LineItemAdjustment.TypeIdString:
                    return new LineItemAdjustment();
                case RewardPointsAjustment.TypeIdString:
                    return new RewardPointsAjustment();
                case ReceiveFreeProductAdjustment.TypeIdString:
                    return new ReceiveFreeProductAdjustment();
                case CategoryDiscountAdjustment.TypeIdString:
                    return new CategoryDiscountAdjustment();
            }

            throw new InvalidOperationException(string.Format("Cannot create Action object based on typeId:", typeId));
        }
    }
}