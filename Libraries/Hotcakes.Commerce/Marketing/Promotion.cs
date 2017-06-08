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
    public class Promotion
    {
        #region Constructor

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

        #endregion

        #region Properties

        public long Id { get; set; }
        public long StoreId { get; set; }
        public PromotionType Mode { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string Name { get; set; }
        public string CustomerDescription { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public bool IsEnabled { get; set; }
        public bool DoNotCombine { get; set; }
        public int SortOrder { get; set; }

        public ReadOnlyCollection<IPromotionQualification> Qualifications
        {
            get { return _qualifications.AsReadOnly(); }
        }

        public ReadOnlyCollection<IPromotionAction> Actions
        {
            get { return _actions.AsReadOnly(); }
        }

        #endregion

        #region Public Methods

        public PromotionStatus GetStatus()
        {
            var now = DateTime.UtcNow;

            if (!IsEnabled) return PromotionStatus.Disabled;
            if (StartDateUtc > now) return PromotionStatus.Upcoming;
            if (EndDateUtc < now) return PromotionStatus.Expired;

            return PromotionStatus.Active;
        }

        public string ActionsToXml()
        {
            var x = new XElement("Actions",
                from a in Actions
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

        public void ActionsFromXml(string xml)
        {
            _actions.Clear();
            if (xml.Trim().Length < 1) return;

            var doc = XDocument.Parse(xml, LoadOptions.None);
            var query = from xElem in doc.Descendants("Action")
                select ActionFactory(xElem.Descendants());
            _actions.AddRange(query);
        }

        public string QualificationsToXml()
        {
            var x = new XElement("Qualifications",
                from q in Qualifications
                select new XElement("Qualification",
                    new XElement("Id", q.Id),
                    new XElement("TypeId", q.TypeId),
                    new XElement("ProcessingCost", (int) q.ProcessingCost),
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

        public void QualificationsFromXml(string xml)
        {
            _qualifications.Clear();
            if (xml.Trim().Length < 1) return;

            var doc = XDocument.Parse(xml, LoadOptions.None);
            var query = from xElem in doc.Descendants("Qualification")
                select QualificationFactory(xElem.Descendants());
            _qualifications.AddRange(query);
        }

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

            return true;
        }

        public bool RemoveQualification(long id)
        {
            var d = _qualifications.SingleOrDefault(y => y.Id == id);
            if (d != null)
            {
                _qualifications.Remove(d);
                return true;
            }
            return false;
        }

        public IPromotionQualification GetQualification(long id)
        {
            var d = _qualifications.SingleOrDefault(y => y.Id == id);
            return d;
        }

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

            return true;
        }

        public bool RemoveAction(long id)
        {
            var d = _actions.SingleOrDefault(y => y.Id == id);
            if (d != null)
            {
                _actions.Remove(d);
                return true;
            }
            return false;
        }

        public IPromotionAction GetAction(long id)
        {
            var d = _actions.SingleOrDefault(y => y.Id == id);
            return d;
        }

        /// <summary>
        ///     Applies to product.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="p">The p.</param>
        /// <param name="price">The price.</param>
        /// <param name="account">The account.</param>
        /// <returns></returns>
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
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="o"></param>
        /// <returns></returns>
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
                for (var i = 0; i < context.Order.Items.Count; i++)
                {
                    var li = context.Order.Items[i];

                    if (!li.IsUserSuppliedPrice && (!li.IsGiftCard || Mode == PromotionType.OfferForFreeItems) &&
                        (!li.IsFreeItem || Mode == PromotionType.OfferForLineItems))
                    {
                        // don't try to add it again, if it's already there
                        if (li.DiscountDetails.Any(d => d.PromotionId == context.PromotionId)) continue;

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
        ///     Applies to shipping rate.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="o">The o.</param>
        /// <param name="account">The account.</param>
        /// <param name="shipMethodId">The ship method identifier.</param>
        /// <param name="originalShippingMethodRate">The original shipping method rate.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Applies to affiliate.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="account">The account.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public bool ApplyToAffiliate(HccRequestContext requestContext, CustomerAccount account, DateTime date)
        {
            if (requestContext == null) return false;

            var context = new PromotionContext(requestContext, Mode, Id)
            {
                CurrentCustomer = account
            };

            return ApplyPromotion(context, PromotionQualificationMode.Affiliates);
        }

        public bool ApplyForFreeShipping(PromotionContext promoContext)
        {
            var result = false;
            for (var i = 0; i < promoContext.Order.Items.Count; i++)
            {
                var li = promoContext.Order.Items[i];

                promoContext.CurrentlyProcessingLineItem = li;

                //if (li.DiscountDetails.Any(d => d.PromotionId == promoContext.PromotionId)) continue;

                result = ApplyPromotion(promoContext, PromotionQualificationMode.LineItems);
            }
            return result;
        }

        #endregion

        #region Obsolete

        [Obsolete("Obsolete in 2.0.0. Use method with same name but other parameters instead")]
        public bool ApplyToProduct(HotcakesApplication app, Product p, UserSpecificPrice price, CustomerAccount account)
        {
            return ApplyToProduct(app.CurrentRequestContext, p, price, account);
        }

        [Obsolete("Obsolete in 2.0.0. Use method with same name but other parameters instead")]
        public bool ApplyToOrder(HotcakesApplication app, Order o)
        {
            return ApplyToOrder(app.CurrentRequestContext, o);
        }

        [Obsolete("Obsolete in 2.0.0. Use method with same name but other parameters instead")]
        public decimal ApplyToShippingRate(HotcakesApplication app, Order o, CustomerAccount account,
            string shipMethodId, decimal originalShippingMethodRate)
        {
            return ApplyToShippingRate(app.CurrentRequestContext, o, account, shipMethodId, originalShippingMethodRate);
        }

        [Obsolete("Obsolete in 2.0.0. Use method with same name but other parameters instead")]
        public bool ApplyToAffiliate(HotcakesApplication app, CustomerAccount account, DateTime date)
        {
            return ApplyToAffiliate(app.CurrentRequestContext, account, date);
        }

        #endregion

        #region Implementation

        private IPromotionAction ActionFactory(IEnumerable<XElement> nodes)
        {
            if (nodes == null) return null;

            var nodeTypeId = nodes.FirstOrDefault(y => y.Name == "TypeId");
            if (nodeTypeId == null) return null;
            var typeId = new Guid(nodeTypeId.Value);

            var result = _factory.CreateAction(typeId);

            if (result == null) return null;

            var nodeId = nodes.FirstOrDefault(y => y.Name == "Id");
            if (nodeId != null)
            {
                long temp = 0;
                long.TryParse(nodeId.Value, out temp);
                result.Id = temp;
            }
            var nodeSettings = nodes.FirstOrDefault(y => y.Name == "Settings");
            if (nodeSettings != null)
            {
                foreach (var setting in nodeSettings.Descendants("Setting"))
                {
                    var key = setting.Element("Key").Value;
                    var value = setting.Element("Value").Value;
                    result.Settings[key] = value;
                }
            }

            return result;
        }

        private IPromotionQualification QualificationFactory(IEnumerable<XElement> nodes)
        {
            if (nodes == null) return null;

            var nodeTypeId = nodes.FirstOrDefault(y => y.Name == "TypeId");
            if (nodeTypeId == null) return null;
            var typeId = new Guid(nodeTypeId.Value);

            var result = _factory.CreateQualification(typeId);

            if (result == null) return null;

            var nodeId = nodes.FirstOrDefault(y => y.Name == "Id");
            if (nodeId != null)
            {
                long temp = 0;
                long.TryParse(nodeId.Value, out temp);
                result.Id = temp;
            }
            var nodeSettings = nodes.FirstOrDefault(y => y.Name == "Settings");
            if (nodeSettings != null)
            {
                foreach (var setting in nodeSettings.Descendants("Setting"))
                {
                    var key = setting.Element("Key").Value;
                    var value = setting.Element("Value").Value;
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
        /// <param name="context"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool CheckQualifications(PromotionContext context, PromotionQualificationMode mode)
        {
            var qualificationsSorted = _qualifications.OrderBy(y => y.ProcessingCost);
            return qualificationsSorted.All(q => q.MeetsQualification(context, mode));
        }

        private void RunActions(PromotionContext context)
        {
            foreach (var a in _actions)
            {
                a.ApplyAction(context);
            }
        }

        #endregion
    }
}