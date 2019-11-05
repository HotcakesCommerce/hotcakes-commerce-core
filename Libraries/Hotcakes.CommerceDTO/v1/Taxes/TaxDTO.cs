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
        ///     The unique identifier or primary key of the tax object
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     A unique three letter code that identifies a country following the alpha 3 standard for this tax zone.
        /// </summary>
        [DataMember]
        public string CountryIsoAlpha3 { get; set; }

        /// <summary>
        ///     The two letter abbreviation for the region matching this tax zone.
        /// </summary>
        [DataMember]
        public string RegionAbbreviation { get; set; }

        /// <summary>
        ///     The postal or zip code for the matching tax zone.
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        ///     This is the ID of the tax schedule that this tax zone belongs to.
        /// </summary>
        [DataMember]
        public long TaxScheduleId { get; set; }

        /// <summary>
        ///     This is the decimal representation of the tax rate for this zone.
        /// </summary>
        [DataMember]
        public decimal Rate { get; set; }

        /// <summary>
        ///     This is the decimal representation of the tax rate to apply to this zone.
        /// </summary>
        [DataMember]
        public decimal ShippingRate { get; set; }

        /// <summary>
        ///     If true, the tax rate specified for ShippingRate will be applied to orders when they match this zone.
        /// </summary>
        [DataMember]
        public bool ApplyToShipping { get; set; }
    }
}