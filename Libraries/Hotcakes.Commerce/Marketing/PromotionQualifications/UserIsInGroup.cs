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

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Matches the current customer against a configured pricing group.</summary>
    /// <remarks>Group identifiers are stored comma-separated, trimmed, and normalized to lowercase.</remarks>
    public class UserIsInGroup : PromotionQualificationBase
    {
        /// <summary>Initializes the qualification with a lower relative processing cost.</summary>
        public UserIsInGroup()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        /// <summary>Gets the stable serialized type identifier reserved for this qualification.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdUserIsInGroup); }
        }

        /// <summary>Gets the configured group identifiers after trimming and lowercase normalization.</summary>
        /// <returns>A new list of non-empty configured group identifiers, or an empty list when none are configured.</returns>
        public List<string> CurrentGroupIds()
        {
            var all = GetSetting("groupids") ?? string.Empty;
            var parts = all
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.ToLowerInvariant())
                .ToList();

            return parts;
        }

        private void SaveGroupIdsToSettings(List<string> groupids)
        {
            if (groupids == null || groupids.Count == 0)
            {
                SetSetting("groupids", string.Empty);
                return;
            }

            var cleaned = groupids
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant());

            var all = string.Join(",", cleaned);
            SetSetting("groupids", all);
        }

        /// <summary>Builds an HTML list containing the display name of each configured group that can be found.</summary>
        /// <param name="app">The application context used to resolve pricing groups.</param>
        /// <returns>Display text containing the current-user group description and matching group names.</returns>
        /// <remarks>Identifiers that do not resolve to a pricing group are omitted from the generated list.</remarks>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Current User Is In Group:<ul>";

            foreach (var gid in CurrentGroupIds())
            {
                var g = app.ContactServices.PriceGroups.Find(gid);
                if (g != null)
                {
                    result += "<li>" + g.Name + "</li>";
                }
            }
            result += "</ul>";
            return result;
        }

        /// <summary>Adds a pricing group identifier to the configured list.</summary>
        /// <param name="groupid">The identifier to trim, lowercase, and add; blank or duplicate values are ignored.</param>
        public void AddGroup(string groupid)
        {
            if (string.IsNullOrWhiteSpace(groupid)) return;

            var cleaned = groupid.Trim().ToLowerInvariant();
            var groups = CurrentGroupIds();

            if (groups.Contains(cleaned)) return;

            groups.Add(cleaned);
            SaveGroupIdsToSettings(groups);
        }

        /// <summary>Removes a pricing group identifier from the configured list.</summary>
        /// <param name="groupid">The identifier to trim and remove; blank or absent values are ignored.</param>
        public void RemoveGroup(string groupid)
        {
            if (string.IsNullOrWhiteSpace(groupid)) return;

            var cleaned = groupid.Trim().ToLowerInvariant();
            var groups = CurrentGroupIds();

            if (!groups.Contains(cleaned)) return;

            groups.RemoveAll(g => string.Equals(g, cleaned, StringComparison.OrdinalIgnoreCase));
            SaveGroupIdsToSettings(groups);
        }

        /// <summary>Checks whether the current customer's pricing group is configured for this qualification.</summary>
        /// <param name="context">The promotion context containing the current customer.</param>
        /// <param name="mode">The requested promotion evaluation mode; this rule uses the customer's pricing group regardless of mode.</param>
        /// <returns><c>true</c> when the customer's non-blank pricing group matches a configured group identifier; otherwise <c>false</c>.</returns>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            var customer = context?.CurrentCustomer;
            if (customer == null) return false;

            var pricingGroupId = customer.PricingGroupId;
            if (string.IsNullOrWhiteSpace(pricingGroupId)) return false;

            var target = pricingGroupId.Trim().ToLowerInvariant();
            var groups = CurrentGroupIds();

            return groups.Contains(target);
        }
    }
}