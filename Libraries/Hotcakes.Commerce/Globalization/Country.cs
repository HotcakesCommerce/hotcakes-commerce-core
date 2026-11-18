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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Hotcakes.Web.Geography;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     Represents a country and its localized display data used throughout the commerce application.
    /// </summary>
    /// <remarks>
    ///     <see cref="SystemName"/> and the ISO values are the stable, non-localized country data. <see cref="DisplayName"/>
    ///     uses the localized value when one is available and otherwise falls back to <see cref="SystemName"/>.
    ///     The object is serializable for compatibility with existing commerce and API payloads.
    /// </remarks>
    [Serializable]
    public class Country : ICountry
    {
        /// <summary>
        ///     Gets the compatibility identifier used for the built-in United States country.
        /// </summary>
        /// <remarks>This value is part of the persisted country identity and must not be changed.</remarks>
        public const string UnitedStatesCountryBvin = "bf7389a2-9b21-4d33-b276-23c9c18ea0c0";

        private string _displayName { get; set; }

        /// <summary>
        ///     Gets or sets the regions associated with this country.
        /// </summary>
        /// <remarks>
        ///     The property is ignored by <c>ScriptIgnore</c> serialization. Repositories populate it when loading
        ///     country sub-items; it may be null or empty when a country was constructed without repository sub-items.
        /// </remarks>
        [ScriptIgnore]
        public List<Region> Regions { get; set; }

        /// <summary>
        ///     Gets a sample currency value formatted with the culture identified by <see cref="CultureCode"/>.
        /// </summary>
        /// <remarks>
        ///     The sample amount is always 1.23. If the culture cannot be created or formatting fails, an empty string is returned.
        /// </remarks>
        /// <value>A culture-specific currency representation of 1.23, or an empty string when formatting is unavailable.</value>
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
        ///     Gets <see cref="DisplayName"/> followed by <see cref="SampleCurrency"/>.
        /// </summary>
        /// <value>The localized country name and sample currency text separated by a space.</value>
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
        ///     Gets or sets the string representation of the country's unique identifier.
        /// </summary>
        /// <value>The country identifier in the repository's bvin format.</value>
        public string Bvin { get; set; }

        /// <summary>
        ///     Gets or sets the culture code used when selecting localized country data and formatting samples.
        /// </summary>
        /// <value>The culture code associated with this country model.</value>
        public string CultureCode { get; set; }

        /// <summary>
        ///     Gets or sets the stable, non-localized country name.
        /// </summary>
        /// <value>The system name used as the localization fallback and global identity label.</value>
        public string SystemName { get; set; }

        /// <summary>
        ///     Gets or sets the country's two-letter ISO code.
        /// </summary>
        /// <remarks>This value is expected to follow ISO 3166-1 alpha-2 conventions.</remarks>
        public string IsoCode { get; set; }

        /// <summary>
        ///     Gets or sets the country's three-letter ISO code.
        /// </summary>
        /// <remarks>This value is expected to follow ISO 3166-1 alpha-3 conventions.</remarks>
        public string IsoAlpha3 { get; set; }

        /// <summary>
        ///     Gets or sets the country's three-digit ISO code.
        /// </summary>
        /// <remarks>This value is expected to follow ISO 3166-1 numeric conventions.</remarks>
        public string IsoNumeric { get; set; }

        /// <summary>
        ///     Gets or sets the regular expression used to validate postal codes for this country.
        /// </summary>
        /// <remarks><see cref="ValidatePostalCode(string)"/> uses this expression when it is not null or empty.</remarks>
        public string PostalCodeValidationRegex { get; set; }

        /// <summary>
        ///     Gets or sets the localized country name displayed to customers.
        /// </summary>
        /// <remarks>
        ///     When the localized value is null or empty, the getter returns <see cref="SystemName"/>. The setter stores
        ///     the localized value without changing the system name.
        /// </remarks>
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
        ///     Finds a region in <see cref="Regions"/> by its abbreviation.
        /// </summary>
        /// <param name="abbreviation">The region abbreviation to compare using ordinal string equality.</param>
        /// <returns>The matching <see cref="Region"/>, or <see langword="null"/> when no region matches.</returns>
        /// <remarks>The current implementation expects <see cref="Regions"/> to be initialized.</remarks>
        public Region FindRegion(string abbreviation)
        {
            return Regions.Where(y => y.Abbreviation == abbreviation).SingleOrDefault();
        }

        /// <summary>
        ///     Finds a region in <see cref="Regions"/> by its identifier.
        /// </summary>
        /// <param name="regionId">The region <see cref="Region.RegionId"/> to find.</param>
        /// <returns>The matching <see cref="Region"/>, or <see langword="null"/> when no region matches.</returns>
        /// <remarks>The current implementation expects <see cref="Regions"/> to be initialized.</remarks>
        public Region FindRegion(Guid regionId)
        {
            return Regions.SingleOrDefault(r => r.RegionId == regionId);
        }

        /// <summary>
        ///     Validates a postal or ZIP code using <see cref="PostalCodeValidationRegex"/>.
        /// </summary>
        /// <param name="postalCode">The postal or ZIP code to validate.</param>
        /// <returns><see langword="true"/> when no validation expression is configured or the expression matches; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        ///     Validation delegates to <see cref="Regex.IsMatch(string, string)"/>. Consequently, malformed expressions and
        ///     other argument errors retain the behavior of that API; this method does not normalize or trim the input.
        /// </remarks>
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
