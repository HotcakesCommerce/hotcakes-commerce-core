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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Taxes
{
    /// <summary>
    ///     This is the main object that is used for all tax management in the REST API
    /// </summary>
    /// <remarks>The main application equivalent to this is the Tax object</remarks>
    [DataContract]
    [Serializable]
    public class TaxDTO
    {
        /// <summary>
        /// Initializes a tax record with empty zone values and zero-valued rates and identifiers.
        /// </summary>
        public TaxDTO()
        {
            Id = 0;
            StoreId = 0;
            CountryIsoAlpha3 = string.Empty;
            RegionAbbreviation = string.Empty;
            PostalCode = string.Empty;
            TaxScheduleId = 0;
            Rate = 0;
            ShippingRate = 0;
            ApplyToShipping = false;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the tax record.
        /// </summary>
        /// <value>The tax record identifier.</value>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the store that owns the tax record.
        /// </summary>
        /// <value>The Hotcakes store identifier.</value>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-3 country code for the tax zone.
        /// </summary>
        /// <value>The three-letter country code.</value>
        [DataMember]
        public string CountryIsoAlpha3 { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation of the region matching this tax zone.
        /// </summary>
        /// <value>The region abbreviation.</value>
        [DataMember]
        public string RegionAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets the postal or ZIP code for the matching tax zone.
        /// </summary>
        /// <value>The postal code.</value>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="TaxScheduleDTO"/> containing this tax zone.
        /// </summary>
        /// <value>The tax schedule identifier.</value>
        [DataMember]
        public long TaxScheduleId { get; set; }

        /// <summary>
        /// Gets or sets the tax rate for this zone.
        /// </summary>
        /// <value>The decimal tax rate applied to taxable amounts in the zone.</value>
        [DataMember]
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the tax rate to apply to shipping in this zone.
        /// </summary>
        /// <value>The decimal shipping tax rate.</value>
        [DataMember]
        public decimal ShippingRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ShippingRate"/> is applied to matching orders.
        /// </summary>
        /// <value><see langword="true"/> to apply the shipping rate; otherwise, <see langword="false"/>.</value>
        [DataMember]
        public bool ApplyToShipping { get; set; }
    }
}
