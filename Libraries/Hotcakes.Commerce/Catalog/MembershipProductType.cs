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
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Gets or sets the membership product type.
    /// </summary>
    public class MembershipProductType
    {
        /// <summary>
        /// Gets or sets the product type id.
        /// </summary>
        public string ProductTypeId { get; set; }
        /// <summary>
        /// Gets or sets the store id.
        /// </summary>
        public long StoreId { get; set; }
        /// <summary>
        /// Gets or sets the product type name.
        /// </summary>
        public string ProductTypeName { get; set; }
        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Gets or sets the expiration period.
        /// </summary>
        public int ExpirationPeriod { get; set; }
        /// <summary>
        /// Gets or sets the expiration period type.
        /// </summary>
        public ExpirationPeriodType ExpirationPeriodType { get; set; }
        /// <summary>
        /// Gets or sets the notify.
        /// </summary>
        public bool Notify { get; set; }

        /// <summary>
        /// Performs the GetTimeSpan catalog operation.
        /// </summary>
        public TimeSpan GetTimeSpan(DateTime startDate, int quantity)
        {
            switch (ExpirationPeriodType)
            {
                case ExpirationPeriodType.Days:
                    return startDate.AddDays(ExpirationPeriod*quantity) - startDate;
                case ExpirationPeriodType.Months:
                    return startDate.AddMonths(ExpirationPeriod*quantity) - startDate;
                case ExpirationPeriodType.Years:
                    return startDate.AddYears(ExpirationPeriod*quantity) - startDate;
                default:
                    throw new Exception("ExpirationPeriodType is not supported");
            }
        }
    }

    internal class InternalMembershipProductType
    {
        /// <summary>
        /// Gets or sets the product type id.
        /// </summary>
        public Guid ProductTypeId { get; set; }
        /// <summary>
        /// Gets or sets the store id.
        /// </summary>
        public long StoreId { get; set; }
        /// <summary>
        /// Gets or sets the product type name.
        /// </summary>
        public string ProductTypeName { get; set; }
        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Gets or sets the expiration period.
        /// </summary>
        public int ExpirationPeriod { get; set; }
        /// <summary>
        /// Gets or sets the expiration period type.
        /// </summary>
        public ExpirationPeriodType ExpirationPeriodType { get; set; }
        /// <summary>
        /// Gets or sets the notify.
        /// </summary>
        public bool Notify { get; set; }

        /// <summary>
        /// Performs the Convert catalog operation.
        /// </summary>
        public MembershipProductType Convert()
        {
            return new MembershipProductType
            {
                ProductTypeId = DataTypeHelper.GuidToBvin(ProductTypeId),
                StoreId = StoreId,
                ProductTypeName = ProductTypeName,
                RoleName = RoleName,
                ExpirationPeriod = ExpirationPeriod,
                ExpirationPeriodType = ExpirationPeriodType,
                Notify = Notify
            };
        }
    }

    /// <summary>
    /// Gets or sets the expiration period type.
    /// </summary>
    /// <summary>
    /// Identifies the unit used for a membership expiration period. Numeric values are persisted and must remain compatible.
    /// </summary>
    public enum ExpirationPeriodType
    {
        /// <summary>Measures expiration in days.</summary>
        Days = 0,
        /// <summary>Measures expiration in calendar months.</summary>
        Months = 1,
        /// <summary>Measures expiration in calendar years.</summary>
        Years = 2
    }
}
