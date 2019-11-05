#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Globalization;
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Web.Logging;

namespace MyCompany.MyTaxProvider
{
    /// <summary>
    ///     This class is used to peform the operation around the results returned
    ///     from the core tax provider gateaway operations.
    ///     This is used for the seperation of logic to deal with the core operation and
    ///     business logic seperately on different class
    /// </summary>
    [Serializable]
    public class MyTaxProvider : ITaxProvider
    {
        public MyTaxProviderSettings settings;

        public MyTaxProvider()
        {
            settings = new MyTaxProviderSettings();
        }

        public TaxProviderSettings Basesettings
        {
            get { return settings; }
        }

        public void CancelTaxDocument(Order order, HccRequestContext hccContext)
        {
            try
            {
                //Set the current context and order service                
                var orderService = Factory.CreateService<OrderService>(hccContext);

                //Write down the business logic to cancel the tax document
                var companyCode = string.Empty;
                var docCode = order.CustomProperties.GetProperty("hcc", TaxProviderPropertyName);

                var hccAvaTax = MyTaxProviderGateWay(hccContext.CurrentStore);
                var result = hccAvaTax.CancelTax(companyCode, docCode, DocumentType.SalesInvoice);

                if (!result.Success)
                {
                    foreach (var m in result.Messages)
                    {
                        EventLog.LogEvent("CancelAvalaraTaxes", m, EventLogSeverity.Information);
                    }

                    var note = "Avalara - Cancel Tax Request Failed:";

                    foreach (var m in result.Messages)
                    {
                        note += "\n" + m;
                    }

                    order.Notes.Add(new OrderNote
                    {
                        IsPublic = false,
                        Note = note
                    });

                    orderService.Orders.Update(order);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        public void CommitTaxes(Order order, HccRequestContext hccContext)
        {
            try
            {
                var memberShipService = Factory.CreateService<MembershipServices>(hccContext);
                var contactService = Factory.CreateService<ContactService>(hccContext);
                var orderService = Factory.CreateService<OrderService>(hccContext);

                // write down the business logic to commit taxes

                var docCode = string.Empty;
                var customerCode = GetCustomerCode(order);
                var companyCode = string.Empty;

                var currencyCode = "USD";
                var ri = new RegionInfo(hccContext.CurrentStore.Settings.CurrencyCultureCode);

                if (ri != null)
                {
                    currencyCode = ri.ISOCurrencySymbol;
                }

                var taxExemptUser = false;
                var taxExemptionNumber = string.Empty;
                CustomerAccount customer = null;

                if (!string.IsNullOrEmpty(order.UserID))
                    customer = memberShipService.Customers.Find(order.UserID);
                else if (!string.IsNullOrEmpty(order.UserEmail))
                    customer = memberShipService.Customers.FindByEmail(order.UserEmail).FirstOrDefault();

                if (customer != null && customer.TaxExempt)
                {
                    taxExemptUser = true;
                    taxExemptionNumber = customer.TaxExemptionNumber;
                }

                //The only difference here is that we are using a SalesInvoice instead of SalesOrder
                var originationAddress = ConvertAddressToTaxProvider(contactService.Addresses.FindStoreContactAddress());
                Address destination;

                if (order.HasShippingItems)
                    destination = order.ShippingAddress;
                else
                    destination = order.BillingAddress;

                var destinationAddress = ConvertAddressToTaxProvider(destination);
                var applyVATRules = hccContext.CurrentStore.Settings.ApplyVATRules;
                var lines = ConvertOrderLines(order, orderService, applyVATRules, hccContext.CurrentStore.Id);

                var hccAvaTax = MyTaxProviderGateWay(hccContext.CurrentStore);

                var result = hccAvaTax.GetTax(DocumentType.SalesInvoice, companyCode, order.bvin,
                    originationAddress, destinationAddress, lines, order.TotalOrderDiscounts,
                    customerCode, currencyCode,
                    taxExemptUser, taxExemptionNumber, applyVATRules);

                if (result.Success)
                {
                    docCode = result.DocCode;
                }

                var totalTax = result.TotalTax;

                result = hccAvaTax.PostTax(companyCode, docCode, DocumentType.SalesInvoice, result.TotalAmount,
                    result.TotalTax);

                if (result != null)
                {
                    if (result.Success)
                    {
                        order.CustomProperties.SetProperty("hcc", TaxProviderPropertyName, result.DocCode);
                        orderService.Orders.Update(order);
                    }
                    else
                    {
                        var note = "MyTaxProvider - Commit Tax Failed (POST):";

                        foreach (var m in result.Messages)
                        {
                            note += "\n" + m;
                        }

                        order.Notes.Add(new OrderNote
                        {
                            IsPublic = false,
                            Note = note
                        });

                        orderService.Orders.Update(order);

                        EventLog.LogEvent("MyTaxProvider", note, EventLogSeverity.Error);
                    }
                }

                result = hccAvaTax.CommitTax(companyCode, docCode, DocumentType.SalesInvoice);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        public void GetTaxes(Order order, HccRequestContext hccContext)
        {
            try
            {
                var memberShipService = Factory.CreateService<MembershipServices>(hccContext);
                var contactService = Factory.CreateService<ContactService>(hccContext);
                var orderService = Factory.CreateService<OrderService>(hccContext);

                var customerCode = GetCustomerCode(order);
                var companyCode = string.Empty;

                var currencyCode = "USD";
                var ri = new RegionInfo(hccContext.CurrentStore.Settings.CurrencyCultureCode);

                if (ri != null)
                {
                    currencyCode = ri.ISOCurrencySymbol;
                }

                var taxExemptUser = false;
                var taxExemptionNumber = string.Empty;

                // write down the business logic to get taxes
                CustomerAccount customer = null;
                customer = memberShipService.Customers.Find(order.UserID);

                if (customer != null && customer.TaxExempt)
                {
                    taxExemptUser = true;
                    taxExemptionNumber = customer.TaxExemptionNumber;
                }

                var originAddress = ConvertAddressToTaxProvider(contactService.Addresses.FindStoreContactAddress());

                Address destination;

                if (order.HasShippingItems)
                    destination = order.ShippingAddress;
                else
                    destination = order.BillingAddress;

                var destinationAddress = ConvertAddressToTaxProvider(destination);

                var applyVATRules = hccContext.CurrentStore.Settings.ApplyVATRules;
                var lines = ConvertOrderLines(order, orderService, applyVATRules, hccContext.CurrentStore.Id);

                var hccTaxProvider = MyTaxProviderGateWay(hccContext.CurrentStore);

                var result = hccTaxProvider.GetTax(DocumentType.SalesOrder, companyCode, order.bvin,
                    originAddress, destinationAddress, lines, order.TotalOrderDiscounts,
                    customerCode, currencyCode,
                    taxExemptUser, taxExemptionNumber, applyVATRules);

                if (result != null)
                {
                    if (result.Success)
                    {
                        order.ItemsTax = result.ItemsTax;
                        order.ShippingTax = result.ShippingTax;
                        order.ShippingTaxRate = result.ShippingTaxRate;
                        order.TotalTax = result.TotalTax;

                        for (var i = 0; i < result.Items.Count; i++)
                        {
                            order.Items[i].TaxRate = result.Items[i].TaxRate;
                            order.Items[i].TaxPortion = result.Items[i].TaxValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        public string ProviderId
        {
            get { return "CCAD00ED-574E-4675-B788-49C4989A0795"; }
        }

        public string ProviderName
        {
            get { return "MyProvider"; }
        }

        public int SortIndex
        {
            get { return 1005; }
        }

        public bool TestConnection(HccRequestContext hccContext)
        {
            try
            {
                var hccTaxProvider = MyTaxProviderGateWay(hccContext.CurrentStore);
                var hccResult = hccTaxProvider.TestConnection();

                return hccResult.Success;
            }
            catch
            {
            }

            return false;
        }

        private MyTaxProviderGateway MyTaxProviderGateWay(Store appStore)
        {
            return new MyTaxProviderGateway(settings.TaxProviderProp1, settings.TaxProviderProp2);
        }

        #region Custom Order properties

        public string TaxProviderPropertyName
        {
            get { return "MyTaxProviderTaxCommitted"; }
        }

        public string TaxProviderGetTaxCountPropertyName
        {
            get { return "MyTaxProviderGetTaxCount"; }
        }

        #endregion

        #region "General Utilities methods"

        private string GetCustomerCode(Order order)
        {
            if (!string.IsNullOrWhiteSpace(order.UserID))
                return order.UserID;
            if (!string.IsNullOrWhiteSpace(order.UserEmail))
                return order.UserEmail;

            return "Anonymous";
        }


        private BaseAddress ConvertAddressToTaxProvider(Address address)
        {
            var result = new BaseAddress
            {
                Line1 = address.Street,
                Line2 = address.Street2,
                City = address.City,
                Region = address.RegionBvin,
                PostalCode = address.PostalCode,
                Country = address.CountryData.IsoCode
            };

            return result;
        }

        private List<Line> ConvertOrderLines(Order o, OrderService orderSerivce, bool ApplyVATRules, long StoreId)
        {
            var result = new List<Line>();

            var count = 1;

            var applyVATRules = ApplyVATRules;
            var taxSchedules = orderSerivce.TaxSchedules.FindAll(StoreId);

            foreach (var item in o.Items)
            {
                var newLine = new Line
                {
                    No = count.ToString(),
                    ItemCode = item.ProductSku,
                    Description = item.ProductName,
                    Amount = item.LineTotal,
                    Qty = item.Quantity,
                    TaxIncluded = applyVATRules
                };

                if (item.IsTaxExempt)
                    newLine.ExemptionNo = "taxexempt";
                if (item.IsNonShipping)
                    newLine.DestinationAddress = ConvertAddressToTaxProvider(o.BillingAddress);

                // Line item discount may not be included in the line
                // but document level discount should be splited equaly between all items
                newLine.Discounted = true;

                // Set Tax Code
                if (taxSchedules != null)
                {
                    var sched = taxSchedules.FirstOrDefault(y => y.Id == item.TaxSchedule);
                    if (sched != null)
                        newLine.TaxCode = sched.Name;
                }

                result.Add(newLine);
                count += 1;
            }

            var shipLine = new Line
            {
                Amount = o.TotalShippingAfterDiscounts,
                No = count.ToString(),
                ItemCode = o.ShippingMethodDisplayName,
                Description = "Shipping",
                Qty = 1,
                TaxIncluded = applyVATRules,
                Discounted = false
            };

            // Order level discount shouldn't be applied to shipping.
            // This is how Order discount is calculated when it is set in percentage
            result.Add(shipLine);

            return result;
        }

        #endregion
    }
}