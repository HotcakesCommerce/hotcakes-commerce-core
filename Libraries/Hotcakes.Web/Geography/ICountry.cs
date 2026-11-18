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

namespace Hotcakes.Web.Geography
{
    /// <summary>Defines the descriptive and validation data for a country.</summary>
    public interface ICountry
    {
        /// <summary>Gets the country identifier.</summary>
        string Bvin { get; }
        /// <summary>Gets the culture code associated with the country.</summary>
        string CultureCode { get; }
        /// <summary>Gets the system name of the country.</summary>
        string SystemName { get; }
        /// <summary>Gets the ISO country code.</summary>
        string IsoCode { get; }
        /// <summary>Gets the three-letter ISO country code.</summary>
        string IsoAlpha3 { get; }
        /// <summary>Gets the numeric ISO country code.</summary>
        string IsoNumeric { get; }
        /// <summary>Gets the regular expression used to validate postal codes.</summary>
        string PostalCodeValidationRegex { get; }
        /// <summary>Gets the display name of the country.</summary>
        string DisplayName { get; }
    }
}
