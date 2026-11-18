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

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    /// <summary>
    /// Defines the administrative class TaxReportLine.
    /// </summary>
    [Serializable]
    public class TaxReportLine
    {
        /// <summary>
        /// Gets or sets the country bvin.
        /// </summary>
        public string CountryBvin { get; set; }
        /// <summary>
        /// Gets or sets the region bvin.
        /// </summary>
        public string RegionBvin { get; set; }
        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// Gets or sets the region name.
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// Gets or sets the total tax.
        /// </summary>
        public decimal TotalTax { get; set; }
    }
}
