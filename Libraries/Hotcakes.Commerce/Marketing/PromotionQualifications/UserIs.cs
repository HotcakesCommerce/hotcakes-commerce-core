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
using System.Linq;
using System.Text;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Matches the current customer against a configured list of user identifiers.</summary>
    /// <remarks>Configured identifiers are stored comma-separated, trimmed, and normalized to uppercase.</remarks>
    public class UserIs : PromotionQualificationBase
    {
        /// <summary>Initializes the qualification with a lower relative processing cost.</summary>
        public UserIs()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        /// <summary>Gets the stable serialized type identifier reserved for this qualification.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdUserIs); }
        }

        /// <summary>Gets the configured user identifiers after trimming and uppercase normalization.</summary>
        /// <returns>A new list of non-empty configured identifiers, or an empty list when none are configured.</returns>
        public List<string> UserIds()
        {
            var all = GetSetting("userids") ?? string.Empty;

            var parts = all
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.ToUpperInvariant())
                .ToList();

            return parts;
        }

        private void SaveUserIdsToSettings(IEnumerable<string> userIds)
        {
            if (userIds == null)
            {
                SetSetting("userids", string.Empty);
                return;
            }

            var cleaned = userIds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpperInvariant());

            SetSetting("userids", string.Join(",", cleaned));
        }

        /// <summary>Builds an HTML list containing the email address for each configured customer that can be found.</summary>
        /// <param name="app">The application context used to resolve customer identifiers.</param>
        /// <returns>Display text containing the configured-user description and matching customer email addresses.</returns>
        /// <remarks>Identifiers that do not resolve to a customer are omitted from the generated list.</remarks>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var sb = new StringBuilder();
            sb.Append("When User Is:<ul>");

            foreach (var userid in UserIds())
            {
                var c = app.MembershipServices.Customers.Find(userid);
                if (c != null)
                {
                    sb.Append("<li>");
                    sb.Append(c.Email);
                    sb.Append("</li>");
                }
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>Adds a user identifier to the configured list.</summary>
        /// <param name="uid">The identifier to trim, uppercase, and add; blank or duplicate values are ignored.</param>
        public void AddUserId(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid)) return;

            var candidate = uid.Trim().ToUpperInvariant();
            var ids = UserIds();

            if (ids.Contains(candidate)) return;

            ids.Add(candidate);
            SaveUserIdsToSettings(ids);
        }

        /// <summary>Removes a user identifier from the configured list.</summary>
        /// <param name="uid">The identifier to trim and remove; blank or absent values are ignored.</param>
        public void RemoveUserId(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid)) return;

            var candidate = uid.Trim().ToUpperInvariant();
            var ids = UserIds();

            if (!ids.Contains(candidate)) return;

            ids.RemoveAll(x => string.Equals(x, candidate, StringComparison.OrdinalIgnoreCase));
            SaveUserIdsToSettings(ids);
        }

        /// <summary>Checks whether the current customer's identifier is in the configured user list.</summary>
        /// <param name="context">The promotion context containing the current customer.</param>
        /// <param name="mode">The requested promotion evaluation mode; this rule uses the customer regardless of mode.</param>
        /// <returns><c>true</c> when the current customer's trimmed, uppercased BVIN matches a configured identifier; otherwise <c>false</c>.</returns>
        /// <remarks>A null customer or exactly empty BVIN fails; other malformed or whitespace-only values retain the implementation's existing comparison behavior.</remarks>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.CurrentCustomer == null) return false;
            if (context.CurrentCustomer.Bvin == string.Empty) return false;

            var currentId = context.CurrentCustomer.Bvin.Trim().ToUpperInvariant();

            foreach (var uid in UserIds())
            {
                if (currentId == uid) return true;
            }

            return false;
        }
    }
}