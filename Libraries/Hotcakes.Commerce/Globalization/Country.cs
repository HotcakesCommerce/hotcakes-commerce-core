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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Hotcakes.Web.Geography;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     This is the main object used to work with countries throughout the main application.
    /// </summary>
    [Serializable]
    public class Country : ICountry
    {
        /// <summary>
        ///     This is a global ID used for the United States since it is built in.
        /// </summary>
        public const string UnitedStatesCountryBvin = "bf7389a2-9b21-4d33-b276-23c9c18ea0c0";

        private string _displayName { get; set; }

        /// <summary>
        ///     Contains a listing of all of the regions that are part of this country.
        /// </summary>
        [ScriptIgnore]
        public List<Region> Regions { get; set; }

        /// <summary>
        ///     Returns the culture-specific version of 1.23 in currency.
        /// </summary>
        public string SampleCurrency
        {
            get
            {
                var result = string.Empty;

                try
                {
                    var tempCulture = CultureInfo.CreateSpecificCulture(CultureCode);
                    if (tempCulture != null)
                    {
                        result = string.Format(tempCulture, "{0:c}", 1.23);
                    }
                }
                catch (Exception)
                {
                }

                return result;
            }
        }

        /// <summary>
        ///     Returns the culture-specific name of the Country followed by the respective version of 1.23 in currency.
        /// </summary>
        public string SampleNameAndCurrency
        {
            get
            {
                var result = string.Empty;
                result = DisplayName + " " + SampleCurrency;
                return result;
            }
        }

        /// <summary>
        ///     The unique ID of the country.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     Specifies the language that this specific language object applies to.
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        ///     This is the global name of the country, which is immune to localization.
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        ///     This is the two-letter abbreviation of the country.
        /// </summary>
        /// <remarks>This value should follow ISO-3166-1 standards.</remarks>
        public string IsoCode { get; set; }

        /// <summary>
        ///     This is the three-letter abbreviation of the country.
        /// </summary>
        /// <remarks>This value should follow ISO-3166-1 standards.</remarks>
        public string IsoAlpha3 { get; set; }

        /// <summary>
        ///     This is the three-digit identifier of the country.
        /// </summary>
        /// <remarks>This value should follow ISO-3166-1 standards.</remarks>
        public string IsoNumeric { get; set; }

        /// <summary>
        ///     A regular expression that can be used to specify the format of the postal code that this country should follow.
        /// </summary>
        public string PostalCodeValidationRegex { get; set; }

        /// <summary>
        ///     The display name is the value that the customer will see in a localized website, per the CultureCode.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                    return SystemName;
                return _displayName;
            }
            set { _displayName = value; }
        }

        /// <summary>
        ///     Returns a specific region from the Regions property using the respective abbreviation.
        /// </summary>
        /// <param name="abbreviation">The two-letter abbreviation of the region to look for.</param>
        /// <returns>If found, a single instance of Region is returned. Otherwise, a null object will be returned.</returns>
        public Region FindRegion(string abbreviation)
        {
            return Regions.Where(y => y.Abbreviation == abbreviation).SingleOrDefault();
        }

        /// <summary>
        ///     Returns a specific region from the Regions property using the respective BVIN or ID.
        /// </summary>
        /// <param name="regionId">The unique ID or BVIN of the region to look for.</param>
        /// <returns>If found, a single instance of Region is returned. Otherwise, a null object will be returned.</returns>
        public Region FindRegion(Guid regionId)
        {
            return Regions.SingleOrDefault(r => r.RegionId == regionId);
        }

        /// <summary>
        ///     This method will validate the postal or zip code of the country using the PostalCodeValidationRegex property.
        /// </summary>
        /// <param name="postalCode">A string representation of the postal or zip code to validate.</param>
        /// <returns>If true, the postal code is valid. Otherwise, this method will return false.</returns>
        public bool ValidatePostalCode(string postalCode)
        {
            if (!string.IsNullOrEmpty(PostalCodeValidationRegex))
            {
                return Regex.IsMatch(postalCode, PostalCodeValidationRegex);
            }
            return true;
        }
    }
}