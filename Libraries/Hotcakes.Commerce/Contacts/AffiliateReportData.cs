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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Contains the projected affiliate fields and metrics returned by an affiliate report.</summary>
    /// <remarks>Report repositories populate this DTO-shaped projection rather than returning the full <see cref="Affiliate"/> entity.</remarks>
    public class AffiliateReportData
    {
        /// <summary>Gets or sets the affiliate's database identifier.</summary>
        public long Id { get; set; }
        /// <summary>Gets or sets the affiliate's referral identifier.</summary>
        public string AffiliateId { get; set; }
        /// <summary>Gets or sets the affiliate's first name.</summary>
        public string FirstName { get; set; }
        /// <summary>Gets or sets the affiliate's last name.</summary>
        public string LastName { get; set; }

        /// <summary>Gets the display name formed by concatenating first and last names with a space.</summary>
        /// <remarks>The concatenation is intentionally retained, including its behavior for empty or null name parts.</remarks>
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        /// <summary>Gets or sets the affiliate's email address.</summary>
        public string Email { get; set; }
        /// <summary>Gets or sets the CMS user identifier associated with the affiliate.</summary>
        public int UserId { get; set; }
        /// <summary>Gets or sets the affiliate's company name.</summary>
        public string Company { get; set; }
        /// <summary>Gets or sets the count of referred orders.</summary>
        public int OrdersCount { get; set; }
        /// <summary>Gets or sets the count of payments.</summary>
        public int PaymentsCount { get; set; }
        /// <summary>Gets or sets the count of referrals that became signups.</summary>
        public int SignupsCount { get; set; }
        /// <summary>Gets or sets the total sales attributed to the affiliate.</summary>
        public decimal SalesAmount { get; set; }
        /// <summary>Gets or sets the commission attributed to the affiliate.</summary>
        public decimal Commission { get; set; }
        /// <summary>Gets or sets the commission already paid. The <c>Payed</c> spelling is compatibility-sensitive.</summary>
        public decimal CommissionPayed { get; set; }

        /// <summary>Gets the commission still owed, calculated as commission minus <see cref="CommissionPayed"/>.</summary>
        public decimal CommissionOwed
        {
            get { return Commission - CommissionPayed; }
        }
    }
}
