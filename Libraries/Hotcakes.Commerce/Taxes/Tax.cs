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
using Hotcakes.CommerceDTO.v1.Taxes;

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class Tax
    {
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

        /// <summary>
        ///     The unique identifier or primary key of the tax object
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     A unique three letter code that identifies a country following the alpha 3 standard for this tax zone.
        /// </summary>
        public string CountryIsoAlpha3 { get; set; }

        /// <summary>
        ///     The two letter abbreviation for the region matching this rax zone.
        /// </summary>
        public string RegionAbbreviation { get; set; }

        /// <summary>
        ///     The postal or zip code for the matching tax zone.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        ///     This is the ID of the tax schedule that this tax zone belongs to.
        /// </summary>
        public long TaxScheduleId { get; set; }

        /// <summary>
        ///     This is the devimal representation of the tax rate for this zone.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        ///     This is the decimal representation of the tax rate to apply to this zone.
        /// </summary>
        public decimal ShippingRate { get; set; }

        /// <summary>
        ///     If true, the tax rate specified for ShippingRate will be applied to orders when they match this zone.
        /// </summary>
        public bool ApplyToShipping { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current tax object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of TaxDTO</returns>
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

        /// <summary>
        ///     Allows you to populate the current tax object using an TaxDTO instance
        /// </summary>
        /// <param name="dto">An instance of the tax from the REST API</param>
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