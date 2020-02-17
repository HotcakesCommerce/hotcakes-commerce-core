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
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Contacts;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    ///     This class is the main object that helps manage price groups throughout the application
    /// </summary>
    /// <remarks>The REST API equivalent is PriceGroupDTO.</remarks>
    [Serializable]
    public class PriceGroup
    {
        public PriceGroup()
        {
            StoreId = 0;
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            Name = string.Empty;
            PricingType = PricingTypes.PercentageOffListPrice;
            AdjustmentAmount = 0m;
        }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     This is the ID of the price group.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the category was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The name is used primarily for administrative purposes to give a meaningful label to the created groups which makes
        ///     for easier management.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The pricing type determines how the pricing is adjusted for customers.
        /// </summary>
        public PricingTypes PricingType { get; set; }

        /// <summary>
        ///     Depending on the pricing type, this amount is used to adjust the pricing for the customer.
        /// </summary>
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        ///     This method returns the correct price for the customer based upon the specified PricingType
        /// </summary>
        /// <param name="price">Price of the product</param>
        /// <param name="msrp">The suggested retail price of the product</param>
        /// <param name="cost">The cost of the product</param>
        /// <returns>Decimal - the adjusted price</returns>
        public decimal GetAdjustedPriceForThisGroup(decimal price, decimal msrp, decimal cost)
        {
            var result = price;

            switch (PricingType)
            {
                case PricingTypes.AmountAboveCost:
                    result = Money.ApplyIncreasedAmount(cost, AdjustmentAmount);
                    break;
                case PricingTypes.AmountOffListPrice:
                    result = Money.ApplyDiscountAmount(msrp, AdjustmentAmount);
                    break;
                case PricingTypes.AmountOffSitePrice:
                    result = Money.ApplyDiscountAmount(price, AdjustmentAmount);
                    break;
                case PricingTypes.PercentageAboveCost:
                    result = Money.ApplyIncreasedPercent(cost, AdjustmentAmount);
                    break;
                case PricingTypes.PercentageOffListPrice:
                    result = Money.ApplyDiscountPercent(msrp, AdjustmentAmount);
                    break;
                case PricingTypes.PercentageOffSitePrice:
                    result = Money.ApplyDiscountPercent(price, AdjustmentAmount);
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Allows you to convert the current price group object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of PriceGroupDTO</returns>
        public PriceGroupDTO ToDto()
        {
            var dto = new PriceGroupDTO
            {
                AdjustmentAmount = AdjustmentAmount,
                Bvin = Bvin,
                LastUpdated = LastUpdated,
                Name = Name,
                PricingType = (PricingTypesDTO) (int) PricingType,
                StoreId = StoreId
            };

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current price group object using a PriceGroupDTO instance
        /// </summary>
        /// <param name="dto">An instance of the price group from the REST API</param>
        public void FromDto(PriceGroupDTO dto)
        {
            AdjustmentAmount = dto.AdjustmentAmount;
            Bvin = dto.Bvin;
            LastUpdated = dto.LastUpdated;
            Name = dto.Name;
            PricingType = (PricingTypes) (int) dto.PricingType;
            StoreId = dto.StoreId;
        }
    }
}