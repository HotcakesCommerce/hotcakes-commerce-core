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

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>
    /// Exposes the tax-schedule values consumed by tax providers.
    /// </summary>
    /// <remarks>The method names are compatibility-sensitive and intentionally use the existing <c>TaxSchedule</c> prefix.</remarks>
    public interface ITaxSchedule
    {
        /// <summary>Gets the schedule identifier.</summary>
        /// <returns>The unique identifier of the tax schedule.</returns>
        long TaxScheduleId();

        /// <summary>Gets the schedule display name.</summary>
        /// <returns>The name of the tax schedule.</returns>
        string TaxScheduleName();

        /// <summary>Gets the default item tax rate used by applicable VAT integrations.</summary>
        /// <returns>The rate expressed as a percentage, not as a fraction; consumers that calculate a fraction divide it by 100.</returns>
        decimal TaxScheduleDefaultRate();

        /// <summary>Gets the default shipping tax rate used by applicable VAT integrations.</summary>
        /// <returns>The rate expressed as a percentage, not as a fraction; consumers that calculate a fraction divide it by 100.</returns>
        decimal TaxScheduleDefaultShippingRate();
    }
}
