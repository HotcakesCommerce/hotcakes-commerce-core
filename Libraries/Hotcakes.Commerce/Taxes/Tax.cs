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
using Hotcakes.CommerceDTO.v1.Taxes;

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>
    /// Represents a store-scoped tax rate for a geographic location and tax schedule.
    /// </summary>
    /// <remarks>Rates are retained as percentage values. Tax calculation code converts them to decimal factors before applying them to taxable amounts.</remarks>
    [Serializable]
    public class Tax
    {
        /// <summary>Initializes a tax with zero identifiers, empty location values, zero rates, and shipping tax disabled.</summary>
        public Tax()
        {
            Id = 0;
            StoreId = 0;
            CountryIsoAlpha3 = string.Empty;
            RegionAbbreviation = string.Empty;
            PostalCode = string.Empty;
            TaxScheduleId = 0;
            Rate = 0m;
            ShippingRate = 0m;
            ApplyToShipping = false;
        }

        /// <summary>Gets or sets the unique identifier or primary key of the tax record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the Hotcakes store identifier that owns this tax record.</summary>
        /// <remarks>The repository assigns the current store when creating a record; this value is meaningful for multi-store installations.</remarks>
        public long StoreId { get; set; }

        /// <summary>Gets or sets the three-letter ISO alpha-3 country code for the tax zone.</summary>
        public string CountryIsoAlpha3 { get; set; }

        /// <summary>Gets or sets the region abbreviation for the tax zone.</summary>
        /// <remarks>The existing model uses this value when matching an address; an empty stored value acts as a wildcard for the region.</remarks>
        public string RegionAbbreviation { get; set; }

        /// <summary>Gets or sets the postal or ZIP code for the tax zone.</summary>
        /// <remarks>An empty stored value acts as a wildcard for postal-code matching.</remarks>
        public string PostalCode { get; set; }

        /// <summary>Gets or sets the identifier of the tax schedule to which this zone belongs.</summary>
        public long TaxScheduleId { get; set; }

        /// <summary>Gets or sets the item tax rate for this zone.</summary>
        /// <remarks>The value is a percentage, such as <c>7.5</c> for seven and one-half percent; calculation code divides it by 100 before applying it.</remarks>
        public decimal Rate { get; set; }

        /// <summary>Gets or sets the shipping tax rate for this zone.</summary>
        /// <remarks>The value is a percentage, such as <c>7.5</c> for seven and one-half percent; calculation code divides it by 100 before applying it.</remarks>
        public decimal ShippingRate { get; set; }

        /// <summary>Gets or sets a value indicating whether <see cref="ShippingRate"/> applies when an order matches this zone.</summary>
        public bool ApplyToShipping { get; set; }

        #region DTO

        /// <summary>Converts this tax model to its REST API DTO representation.</summary>
        /// <returns>A new <see cref="TaxDTO"/> containing the current property values.</returns>
        public TaxDTO ToDto()
        {
            var dto = new TaxDTO
            {
                ApplyToShipping = ApplyToShipping,
                CountryIsoAlpha3 = CountryIsoAlpha3,
                Id = Id,
                PostalCode = PostalCode,
                Rate = Rate,
                ShippingRate = ShippingRate,
                RegionAbbreviation = RegionAbbreviation,
                StoreId = StoreId,
                TaxScheduleId = TaxScheduleId
            };

            return dto;
        }

        /// <summary>Copies values from a REST API DTO into this tax model.</summary>
        /// <param name="dto">The DTO to copy, or <see langword="null"/>.</param>
        /// <remarks>If <paramref name="dto"/> is <see langword="null"/>, this method does nothing and preserves every current value.</remarks>
        public void FromDto(TaxDTO dto)
        {
            if (dto == null) return;

            ApplyToShipping = dto.ApplyToShipping;
            CountryIsoAlpha3 = dto.CountryIsoAlpha3;
            Id = dto.Id;
            PostalCode = dto.PostalCode;
            Rate = dto.Rate;
            ShippingRate = dto.ShippingRate;
            RegionAbbreviation = dto.RegionAbbreviation;
            StoreId = dto.StoreId;
            TaxScheduleId = dto.TaxScheduleId;
        }

        #endregion
    }
}
