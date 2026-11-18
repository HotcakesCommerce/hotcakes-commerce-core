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
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Marketing
{
    /// <summary>
    /// Defines a promotion's schedule, qualifications, actions, and application behavior.
    /// A promotion is active only when enabled and its UTC date range contains the current time.
    /// </summary>
    public class Promotion
    {
        #region Constructor

        /// <summary>Initializes a promotion with a sale mode, a one-year default window, and empty rule collections.</summary>
        public Promotion()
        {
            Mode = PromotionType.Sale;
            LastUpdatedUtc = DateTime.UtcNow;
            Name = "New Promotion";
            CustomerDescription = string.Empty;
            StartDateUtc = Dates.ZeroOutTime(DateTime.Now).ToUniversalTime();
            EndDateUtc = Dates.MaxOutTime(DateTime.Now.AddMonths(12)).ToUniversalTime();
            _factory = Factory.Instance.CreatePromotionFactory();
        }

        #endregion

        #region Fields

        private readonly List<IPromotionQualification> _qualifications = new List<IPromotionQualification>();
        private readonly List<IPromotionAction> _actions = new List<IPromotionAction>();
        private readonly PromotionFactory _factory;
        private ReadOnlyCollection<IPromotionQualification> _qualificationsReadOnly;
        private ReadOnlyCollection<IPromotionAction> _actionsReadOnly;

        #endregion

        #region Properties

        /// <summary>Gets or sets the promotion identifier.</summary>
        public long Id { get; set; }
        /// <summary>Gets or sets the store identifier owning the promotion.</summary>
        public long StoreId { get; set; }
        /// <summary>Gets or sets the promotion mode, which controls application scope.</summary>
        public PromotionType Mode { get; set; }
        /// <summary>Gets or sets the last modification time in UTC.</summary>
        public DateTime LastUpdatedUtc { get; set; }
        /// <summary>Gets or sets the administrative promotion name.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the customer-facing description.</summary>
        public string CustomerDescription { get; set; }
        /// <summary>Gets or sets the inclusive UTC start boundary used by <see cref="GetStatus"/>.</summary>
        public DateTime StartDateUtc { get; set; }
        /// <summary>Gets or sets the inclusive UTC end boundary used by <see cref="GetStatus"/>.</summary>
        public DateTime EndDateUtc { get; set; }
        /// <summary>Gets or sets whether the promotion may be applied.</summary>
        public bool IsEnabled { get; set; }
        /// <summary>Gets or sets whether this promotion prevents later non-sale offers from combining with it.</summary>
        public bool DoNotCombine { get; set; }
        /// <summary>Gets or sets the ordering value used by repository queries and application.</summary>
        public int SortOrder { get; set; }

        /// <summary>Gets the read-only qualification collection, in promotion order.</summary>
        public ReadOnlyCollection<IPromotionQualification> Qualifications
        {
            get { return _qualificationsReadOnly ?? (_qualificationsReadOnly = _qualifications.AsReadOnly()); }
        }

        /// <summary>Gets the read-only action collection, in promotion order.</summary>
        public ReadOnlyCollection<IPromotionAction> Actions
        {
            get { return _actionsReadOnly ?? (_actionsReadOnly = _actions.AsReadOnly()); }
        }

        #endregion

        #region Public Methods

        /// <summary>Calculates the current status using <see cref="DateTime.UtcNow"/>, enablement, and UTC boundaries.</summary>
        /// <returns>The calculated promotion status.</returns>
        public PromotionStatus GetStatus()
        {
            var now = DateTime.UtcNow;

            if (!IsEnabled) return PromotionStatus.Disabled;
            if (StartDateUtc > now) return PromotionStatus.Upcoming;
            if (EndDateUtc < now) return PromotionStatus.Expired;

            return PromotionStatus.Active;
        }

        /// <summary>Serializes actions to the persisted <c>Actions</c>/<c>Action</c> XML format, including type IDs and settings.</summary>
        /// <returns>The action XML, or an empty <c>Actions</c> element when no actions exist.</returns>
        public string ActionsToXml()
        {
            var x = new XElement("Actions",
                from a in _actions
                select new XElement("Action",
                    new XElement("Id", a.Id),
                    new XElement("TypeId", a.TypeId),
                    new XElement("Settings",
                        from s in a.Settings
                        select new XElement("Setting",
                            new XElement("Key", s.Key),
                            new XElement("Value", s.Value)
                            )
                        )
                    )
                );
            return x.ToString(SaveOptions.None);
        }

        /// <summary>Replaces the action collection by deserializing the persisted action XML.</summary>
        /// <param name="xml">The <c>Actions</c> XML; blank input clears the collection.</param>
        /// <exception cref="FormatException">The XML or a contained type identifier is invalid.</exception>
        public void ActionsFromXml(string xml)
        {
            _actions.Clear();
            _actionsReadOnly = null;
            if (string.IsNullOrWhiteSpace(xml)) return;

            var doc = XDocument.Parse(xml, LoadOptions.None);
            var actions = doc.Descendants("Action").Select(xElem => ActionFactory(xElem.Descendants())).Where(a => a != null);
            _actions.AddRange(actions);
        }

        /// <summary>Serializes qualifications to the persisted <c>Qualifications</c>/<c>Qualification</c> XML format.</summary>
        /// <returns>The qualification XML, or an empty <c>Qualifications</c> element when none exist.</returns>
        public string QualificationsToXml()
        {
            var x = new XElement("Qualifications",
                from q in _qualifications
                select new XElement("Qualification",
                    new XElement("Id", q.Id),
                    new XElement("TypeId", q.TypeId),
                    new XElement("ProcessingCost", (int)q.ProcessingCost),
                    new XElement("Settings",
                        from s in q.Settings
                        select new XElement("Setting",
                            new XElement("Key", s.Key),
                            new XElement("Value", s.Value)
                            )
                        )
                    )
                );
            return x.ToString(SaveOptions.None);
        }

        /// <summary>Replaces the qualification collection by deserializing the persisted qualification XML.</summary>
        /// <param name="xml">The <c>Qualifications</c> XML; blank input clears the collection.</param>
        /// <exception cref="FormatException">The XML or a contained type identifier is invalid.</exception>
        public void QualificationsFromXml(string xml)
        {
            _qualifications.Clear();
            _qualificationsReadOnly = null;
            if (string.IsNullOrWhiteSpace(xml)) return;

            var doc = XDocument.Parse(xml, LoadOptions.None);
            var qualifications = doc.Descendants("Qualification").Select(xElem => QualificationFactory(xElem.Descendants())).Where(q => q != null);
            _qualifications.AddRange(qualifications);
        }

        /// <summary>Adds a qualification, assigning it the next identifier after the current maximum.</summary>
        /// <param name="q">The qualification to add.</param>
        /// <returns>Always <see langword="true"/> for the current implementation.</returns>
        public bool AddQualification(IPromotionQualification q)
        {
            long maxid = 0;
            if (_qualifications.Count > 0)
            {
                maxid = _qualifications.Max(y => y.Id);
            }
            if (maxid < 0) maxid = 0;

            q.Id = maxid + 1;
            _qualifications.Add(q);
            _qualificationsReadOnly = null;

            return true;
        }

        /// <summary>Removes the qualification with the specified identifier.</summary>
        /// <param name="id">The qualification identifier.</param>
        /// <returns><see langword="true"/> when a qualification was removed.</returns>
        public bool RemoveQualification(long id)
        {
            var index = FindQualificationIndex(id);
            if (index >= 0)
            {
                _qualifications.RemoveAt(index);
                _qualificationsReadOnly = null;
                return true;
            }
            return false;
        }

        /// <summary>Finds a qualification by identifier.</summary>
        /// <param name="id">The qualification identifier.</param>
        /// <returns>The qualification, or <see langword="null"/> when not found.</returns>
        public IPromotionQualification GetQualification(long id)
        {
            return _qualifications.FirstOrDefault(y => y.Id == id);
        }

        /// <summary>Adds an action, assigning it the next identifier after the current maximum.</summary>
        /// <param name="a">The action to add.</param>
        /// <returns>Always <see langword="true"/> for the current implementation.</returns>
        public bool AddAction(IPromotionAction a)
        {
            long maxid = 0;
            if (_actions.Count > 0)
            {
                maxid = _actions.Max(y => y.Id);
            }
            if (maxid < 0) maxid = 0;

            a.Id = maxid + 1;
            _actions.Add(a);
            _actionsReadOnly = null;

            return true;
        }

        /// <summary>Removes the action with the specified identifier.</summary>
        /// <param name="id">The action identifier.</param>
        /// <returns><see langword="true"/> when an action was removed.</returns>
        public bool RemoveAction(long id)
        {
            var index = FindActionIndex(id);
            if (index >= 0)
            {
                _actions.RemoveAt(index);
                _actionsReadOnly = null;
                return true;
            }
            return false;
        }

        /// <summary>Finds an action by identifier.</summary>
        /// <param name="id">The action identifier.</param>
        /// <returns>The action, or <see langword="null"/> when not found.</returns>
        public IPromotionAction GetAction(long id)
        {
            return _actions.FirstOrDefault(y => y.Id == id);
        }

        /// <summary>
        /// Applies the promotion to a product's user-specific price after checking product qualifications and actions.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="p">The p.</param>
        /// <param name="price">The price.</param>
        /// <param name="account">The account.</param>
        /// <returns><see langword="true"/> when the active promotion qualifies and its actions run.</returns>
        public bool ApplyToProduct(HccRequestContext requestContext, Product p, UserSpecificPrice price,
            CustomerAccount account)
        {
            if (requestContext == null) return false;
            if (p == null) return false;
            if (price == null) return false;

            var context = new PromotionContext(requestContext, Mode, Id)
            {
                Product = p,
                UserPrice = price,
                CurrentCustomer = account,
                CustomerDescription = CustomerDescription
            };

            return ApplyPromotion(context, PromotionQualificationMode.Products);
        }

        /// <summary>
        /// Applies the promotion to an order. Line-item and free-item offers evaluate eligible lines individually;
        /// other modes evaluate the order once. Actions mutate the supplied order and promotion context.
        /// </summary>
        /// <param name="requestContext">The commerce request context.</param>
        /// <param name="o">The order to evaluate and potentially mutate.</param>
        /// <returns><see langword="true"/> when the promotion qualifies.</returns>
        public bool ApplyToOrder(HccRequestContext requestContext, Order o)
        {
            if (requestContext == null) return false;
            if (o == null) return false;

            var membershipServices = Factory.CreateService<MembershipServices>(requestContext);
            var context = new PromotionContext(requestContext, Mode, Id)
            {
                Order = o,
                CurrentCustomer = string.IsNullOrEmpty(o.UserID) ? null : membershipServices.Customers.Find(o.UserID),
                CustomerDescription = CustomerDescription,
                OtherOffersApplied = o.HasAnyNonSaleDiscounts
            };

            var result = false;

            if (Mode == PromotionType.OfferForLineItems || Mode == PromotionType.OfferForFreeItems)
            {
                var itemCount = context.Order.Items.Count;
                for (var i = 0; i < itemCount; i++)
                {
                    var li = context.Order.Items[i];

                    if (!li.IsUserSuppliedPrice && (!li.IsGiftCard || Mode == PromotionType.OfferForFreeItems) &&
                        (!li.IsFreeItem || Mode == PromotionType.OfferForLineItems))
                    {
                        // don't try to add it again, if it's already there
                        if (HasPromotionDiscount(li.DiscountDetails, context.PromotionId)) continue;

                        context.CurrentlyProcessingLineItem = li;
                        result = ApplyPromotion(context, PromotionQualificationMode.LineItems);
                    }
                }
            }
            else
            {
                result = ApplyPromotion(context, PromotionQualificationMode.Orders);
            }

            return result;
        }

        /// <summary>
        /// Applies the promotion to a shipping rate and returns the context's adjusted rate when it qualifies.
        /// </summary>
        /// <param name="requestContext">The commerce request context.</param>
        /// <param name="o">The order whose shipping rate is being evaluated.</param>
        /// <param name="account">The customer account associated with the order.</param>
        /// <param name="shipMethodId">The shipping method identifier placed in the promotion context.</param>
        /// <param name="originalShippingMethodRate">The starting shipping rate.</param>
        /// <returns>The original rate when the context is invalid or the promotion does not qualify; otherwise the adjusted rate.</returns>
        public decimal ApplyToShippingRate(HccRequestContext requestContext, Order o, CustomerAccount account,
            string shipMethodId, decimal originalShippingMethodRate)
        {
            var result = originalShippingMethodRate;

            if (requestContext == null) return result;
            if (o == null) return result;

            var context = new PromotionContext(requestContext, Mode, Id)
            {
                Order = o,
                CurrentCustomer = account,
                CustomerDescription = CustomerDescription,
                CurrentShippingMethodId = shipMethodId,
                AdjustedShippingRate = originalShippingMethodRate,
                OtherOffersApplied = o.HasAnyNonSaleDiscounts
            };

            if (ApplyPromotion(context, PromotionQualificationMode.Orders))
            {
                result = context.AdjustedShippingRate;
            }

            return result;
        }

        /// <summary>Applies this promotion to an affiliate customer context.</summary>
        /// <param name="requestContext">The commerce request context.</param>
        /// <param name="account">The affiliate customer account.</param>
        /// <param name="date">The evaluation date supplied by the caller; the current implementation does not use it.</param>
        /// <returns><see langword="true"/> when the active promotion qualifies and its actions run.</returns>
        public bool ApplyToAffiliate(HccRequestContext requestContext, CustomerAccount account, DateTime date)
        {
            if (requestContext == null) return false;

            var context = new PromotionContext(requestContext, Mode, Id)
            {
                CurrentCustomer = account
            };

            return ApplyPromotion(context, PromotionQualificationMode.Affiliates);
        }

        /// <summary>Applies this promotion to each order line for free-shipping evaluation.</summary>
        /// <param name="promoContext">The existing mutable promotion context.</param>
        /// <returns><see langword="true"/> when the qualification evaluation succeeds.</returns>
        public bool ApplyForFreeShipping(PromotionContext promoContext)
        {
            var result = false;
            var itemCount = promoContext.Order.Items.Count;
            for (var i = 0; i < itemCount; i++)
            {
                var li = promoContext.Order.Items[i];

                promoContext.CurrentlyProcessingLineItem = li;

                //if (li.DiscountDetails.Any(d => d.PromotionId == promoContext.PromotionId)) continue;

                result = ApplyPromotion(promoContext, PromotionQualificationMode.LineItems);
            }
            return result;
        }

        #endregion

        #region Implementation

        private IPromotionAction ActionFactory(IEnumerable<XElement> nodes)
        {
            if (nodes == null) return null;

            var nodeTypeId = nodes.FirstOrDefault(y => y.Name.LocalName == "TypeId");
            if (nodeTypeId == null) return null;
            var typeId = new Guid(nodeTypeId.Value);

            var result = _factory.CreateAction(typeId);

            if (result == null) return null;

            var nodeId = nodes.FirstOrDefault(y => y.Name.LocalName == "Id");
            if (nodeId != null)
            {
                if (long.TryParse(nodeId.Value, out var temp))
                {
                    result.Id = temp;
                }
            }
            var nodeSettings = nodes.FirstOrDefault(y => y.Name.LocalName == "Settings");
            if (nodeSettings != null)
            {
                foreach (var setting in nodeSettings.Descendants("Setting"))
                {
                    var keyElem = setting.Element("Key");
                    if (keyElem == null) continue;
                    var valueElem = setting.Element("Value");
                    var key = keyElem.Value;
                    var value = valueElem?.Value ?? string.Empty;
                    result.Settings[key] = value;
                }
            }

            return result;
        }

        private IPromotionQualification QualificationFactory(IEnumerable<XElement> nodes)
        {
            if (nodes == null) return null;

            var nodeTypeId = nodes.FirstOrDefault(y => y.Name.LocalName == "TypeId");
            if (nodeTypeId == null) return null;
            var typeId = new Guid(nodeTypeId.Value);

            var result = _factory.CreateQualification(typeId);

            if (result == null) return null;

            var nodeId = nodes.FirstOrDefault(y => y.Name.LocalName == "Id");
            if (nodeId != null)
            {
                if (long.TryParse(nodeId.Value, out var temp))
                {
                    result.Id = temp;
                }
            }
            var nodeSettings = nodes.FirstOrDefault(y => y.Name.LocalName == "Settings");
            if (nodeSettings != null)
            {
                foreach (var setting in nodeSettings.Descendants("Setting"))
                {
                    var keyElem = setting.Element("Key");
                    if (keyElem == null) continue;
                    var valueElem = setting.Element("Value");
                    var key = keyElem.Value;
                    var value = valueElem?.Value ?? string.Empty;
                    result.Settings[key] = value;
                }
            }

            return result;
        }

        private bool ApplyPromotion(PromotionContext context, PromotionQualificationMode qualificationMode)
        {
            if (GetStatus() == PromotionStatus.Active)
            {
                var qualified = CheckQualifications(context, qualificationMode);

                if (qualified)
                {
                    RunActions(context);
                }

                return qualified;
            }

            return false;
        }

        /// <summary>
        ///     Make sure we meet all requirements
        /// </summary>
        /// <remarks>
        ///     NOTE: we order by processing cost which should allow us to check
        ///     the fastest items first. For example, checking userID is faster
        ///     than checking user group because ID is in the context and group
        ///     requires a database call.
        /// </remarks>
        /// <summary>Checks all qualifications in ascending <see cref="RelativeProcessingCost"/> order.</summary>
        /// <param name="context">The context to evaluate.</param>
        /// <param name="mode">The subject mode being evaluated.</param>
        /// <returns><see langword="true"/> only when every qualification succeeds.</returns>
        public bool CheckQualifications(PromotionContext context, PromotionQualificationMode mode)
        {
            var qualificationsSorted = _qualifications.OrderBy(y => y.ProcessingCost);
            return qualificationsSorted.All(q => q.MeetsQualification(context, mode));
        }

        private void RunActions(PromotionContext context)
        {
            var actionCount = _actions.Count;
            for (var i = 0; i < actionCount; i++)
            {
                _actions[i].ApplyAction(context);
            }
        }

        private int FindQualificationIndex(long id)
        {
            return _qualifications.FindIndex(q => q.Id == id);
        }

        private int FindActionIndex(long id)
        {
            return _actions.FindIndex(a => a.Id == id);
        }

        private bool HasPromotionDiscount(IEnumerable<DiscountDetail> discountDetails, long promotionId)
        {
            return discountDetails != null && discountDetails.Any(d => d.PromotionId == promotionId);
        }

        #endregion
    }
}