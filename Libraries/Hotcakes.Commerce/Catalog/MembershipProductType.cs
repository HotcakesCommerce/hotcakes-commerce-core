#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    public class MembershipProductType
    {
        public string ProductTypeId { get; set; }
        public long StoreId { get; set; }
        public string ProductTypeName { get; set; }
        public string RoleName { get; set; }
        public int ExpirationPeriod { get; set; }
        public ExpirationPeriodType ExpirationPeriodType { get; set; }
        public bool Notify { get; set; }

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
        public Guid ProductTypeId { get; set; }
        public long StoreId { get; set; }
        public string ProductTypeName { get; set; }
        public string RoleName { get; set; }
        public int ExpirationPeriod { get; set; }
        public ExpirationPeriodType ExpirationPeriodType { get; set; }
        public bool Notify { get; set; }

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

    public enum ExpirationPeriodType
    {
        Days = 0,
        Months = 1,
        Years = 2
    }
}