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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Taxes.Providers.Avalara
{
    [Serializable]
    public class Avalara : ITaxProvider
    {
        public AvalaraSettings settings;

        private const string HCC_KEY = "hcc";

        public Avalara()
        {
            settings = new AvalaraSettings();
        }

        public string ProviderId
        {
            get { return Id(); }
        }

        public string ProviderName
        {
            get { return "Avalara"; }
        }

        public int SortIndex
        {
            get { return 700; }
        }

        public TaxProviderSettings Basesettings
        {
            get { return settings; }
        }

        public bool TestConnection(HccRequestContext hccContext)
        {
            try
            {
                var hccAvaTax = GetHccAvaTax(hccContext.CurrentStore);
                var avaResult = hccAvaTax.TestConnection();
                return avaResult.Success;
            }
            catch
            {
            }
            return false;
        }

        public void GetTaxes(Order order, HccRequestContext hccContext)
        {
            try
            {
                SubtractExistingTaxes(order, hccContext);

                var memberShipService = Factory.CreateService<MembershipServices>(hccContext);
                var contactService = Factory.CreateService<ContactService>(hccContext);
                var orderService = Factory.CreateService<OrderService>(hccContext);

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

                var originAddress = ConvertAddressToAvalara(contactService.Addresses.FindStoreContactAddress());
                Address destination;
                if (order.HasShippingItems)
                    destination = order.ShippingAddress;
                else
                    destination = order.BillingAddress;
                var destinationAddress = ConvertAddressToAvalara(destination);
                var customerCode = GetCustomerCode(order);
                var companyCode = settings.CompanyCode;
                var applyVATRules = hccContext.CurrentStore.Settings.ApplyVATRules;
                var lines = ConvertOrderLines(order, orderService, false, hccContext.CurrentStore.Id);
                var hccAvaTax = GetHccAvaTax(hccContext.CurrentStore);


                var result = hccAvaTax.GetTax(DocumentType.SalesOrder, companyCode, order.bvin,
                    originAddress, destinationAddress, lines, order.TotalOrderDiscounts,
                    customerCode, currencyCode,
                    taxExemptUser, taxExemptionNumber, applyVATRules);

                if (result != null)
                {
                    if (result.Success)
                    {
                        RecalculateOrder(order, applyVATRules, result);

                        if (settings.DebugMode)
                        {
                            var note = " Received Tax details from  Avalara as below : " + Environment.NewLine +
                                       "Total Tax :  " + result.TotalTax + currencyCode + Environment.NewLine +
                                       "Shipping Tax : " + result.ShippingTax + currencyCode + Environment.NewLine +
                                       "Items Tax : " + result.ItemsTax + currencyCode + Environment.NewLine +
                                       "Shipping Tax Rate : " + Math.Round(result.ShippingTaxRate*100, 3) + " % ";

                            order.Notes.Add(new OrderNote
                            {
                                IsPublic = false,
                                Note = "Avalara - Get Tax Request Succeeded" + Environment.NewLine + note
                            });
                            orderService.Orders.Update(order);

                            EventLog.LogEvent("Avalara -  GetTaxes ", note, EventLogSeverity.Information);
                        }
                    }
                    else
                    {
                        if (settings.DebugMode)
                        {
                            var note = "Avalara - Get Tax Request Failed:";
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

                            EventLog.LogEvent("Avalara -  CalculateTaxes", note, EventLogSeverity.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        public void CommitTaxes(Order order, HccRequestContext hccContext)
        {
            if (string.IsNullOrEmpty(order.CustomProperties.GetProperty(HCC_KEY, TaxProviderPropertyName)))
            {
                var memberShipService = Factory.CreateService<MembershipServices>(hccContext);
                var contactService = Factory.CreateService<ContactService>(hccContext);
                var orderService = Factory.CreateService<OrderService>(hccContext);

                var docCode = string.Empty;
                var customerCode = GetCustomerCode(order);
                var companyCode = settings.CompanyCode;

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
                var originationAddress = ConvertAddressToAvalara(contactService.Addresses.FindStoreContactAddress());
                Address destination;
                if (order.HasShippingItems)
                    destination = order.ShippingAddress;
                else
                    destination = order.BillingAddress;
                var destinationAddress = ConvertAddressToAvalara(destination);
                var applyVATRules = hccContext.CurrentStore.Settings.ApplyVATRules;

                // if VAT rules are used then tax are already included into lineitems otherwise no
                var lines = ConvertOrderLines(order, orderService, applyVATRules, hccContext.CurrentStore.Id);

                var orderIdentifier = GetOrderIdentifier(order, orderService);
                var hccAvaTax = GetHccAvaTax(hccContext.CurrentStore);

                var result = hccAvaTax.GetTax(DocumentType.SalesInvoice, companyCode, orderIdentifier,
                    originationAddress, destinationAddress, lines, order.TotalOrderDiscounts,
                    customerCode, currencyCode,
                    taxExemptUser, taxExemptionNumber, applyVATRules);

                if (result.Success)
                {
                    docCode = result.DocCode;
                }
                else
                {
                    if (settings.DebugMode)
                    {
                        var note = "Avalara - Commit Tax Failed:";
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

                        EventLog.LogEvent("Avalara", note, EventLogSeverity.Error);
                    }
                    return;
                }

                var totalTax = result.TotalTax;

                result = hccAvaTax.PostTax(companyCode, docCode, DocumentType.SalesInvoice, result.TotalAmount,
                    result.TotalTax);

                if (result != null)
                {
                    if (result.Success)
                    {
                        order.CustomProperties.SetProperty(HCC_KEY, TaxProviderPropertyName, result.DocCode);
                        orderService.Orders.Update(order);
                    }
                    else
                    {
                        if (settings.DebugMode)
                        {
                            var note = "Avalara - Commit Tax Failed (POST):";
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

                            EventLog.LogEvent("Avalara", note, EventLogSeverity.Error);
                        }
                        return;
                    }
                }

                result = hccAvaTax.CommitTax(companyCode, docCode, DocumentType.SalesInvoice);

                if (result.Success)
                {
                    if (settings.DebugMode)
                    {
                        var note = "Avalara - Committed " + totalTax.ToString("C") + ":";
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

                        EventLog.LogEvent("Avalara", note, EventLogSeverity.Error);
                    }
                }
                else
                {
                    if (settings.DebugMode)
                    {
                        var note = "Avalara - Commit Tax Failed (Commit Call):";
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

                        EventLog.LogEvent("Avalara", note, EventLogSeverity.Error);
                    }
                }
            }
        }

        public void CancelTaxDocument(Order order, HccRequestContext hccContext)
        {
            if (!string.IsNullOrEmpty(order.CustomProperties.GetProperty(HCC_KEY, TaxProviderPropertyName)))
            {
                var orderService = Factory.CreateService<OrderService>(hccContext);

                var companyCode = settings.CompanyCode;
                var docCode = order.CustomProperties.GetProperty(HCC_KEY, TaxProviderPropertyName);

                var hccAvaTax = GetHccAvaTax(hccContext.CurrentStore);
                var result = hccAvaTax.CancelTax(companyCode, docCode, DocumentType.SalesInvoice);

                if (!result.Success)
                {
                    foreach (var m in result.Messages)
                    {
                        EventLog.LogEvent("CancelAvalaraTaxes", m, EventLogSeverity.Information);
                    }

                    if (settings.DebugMode)
                    {
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
                else
                {
                    order.CustomProperties.SetProperty(HCC_KEY, TaxProviderPropertyName, string.Empty);

                    if (settings.DebugMode)
                    {
                        order.Notes.Add(new OrderNote
                        {
                            IsPublic = false,
                            Note = "Avalara - Cancel Tax Request Succeeded"
                        });
                        orderService.Orders.Update(order);

                        EventLog.LogEvent("CancelAvalaraTaxes",
                            "Avalara Taxes successfully cancelled. DocCode: " + docCode, EventLogSeverity.Information);
                    }
                }
            }
        }

        public static string Id()
        {
            return TaxProviders.AvataxServiceId;
        }

        #region Custom Order properties

        public string TaxProviderPropertyName
        {
            get { return "AvalaraTaxCommitted"; }
        }

        public string TaxProviderGetTaxCountPropertyName
        {
            get { return "AvalaraGetTaxCount"; }
        }

        #endregion

        #region Private

        private void SubtractExistingTaxes(Order order, HccRequestContext hccContext)
        {
            var applyVATRules = hccContext.CurrentStore.Settings.ApplyVATRules;
            if (applyVATRules)
            {
                order.TotalShippingAfterDiscounts = 0;

                foreach (ITaxable item in order.Items)
                {
                    if (item.IsTaxExempt)
                        continue;

                    var orderService = Factory.CreateService<OrderService>(hccContext);
                    ITaxSchedule schedule = orderService.TaxSchedules.FindForThisStore(item.TaxSchedule);

                    if (schedule == null)
                        continue;

                    var defaultRate = schedule.TaxScheduleDefaultRate()/100;
                    var defaultShippingRate = schedule.TaxScheduleDefaultShippingRate()/100;

                    if (defaultRate != 0)
                    {
                        //Subtract included tax
                        var lineTotal = item.LineTotal;
                        var lineTotalVAT = Money.RoundCurrency(lineTotal - lineTotal/(1 + defaultRate));
                        item.LineTotal = lineTotal - lineTotalVAT;
                    }

                    if (defaultShippingRate != 0)
                    {
                        //Subtract tax from shipping portion always since rates may differ
                        var shippingPortion = item.ShippingPortion;
                        var shippingPortionVAT =
                            Money.RoundCurrency(shippingPortion - shippingPortion/(1 + defaultShippingRate));
                        item.ShippingPortion = shippingPortion - shippingPortionVAT;
                    }

                    item.AdjustedPricePerItem = Money.RoundCurrency(item.LineTotal/item.Quantity);
                }

                foreach (ITaxable item in order.Items)
                {
                    // shipping portions have to be collected even if product is tax exempt or doesn't have tax schedule
                    order.TotalShippingAfterDiscounts += item.ShippingPortion;
                }
            }
        }

        private static void RecalculateOrder(Order order, bool applyVATRules, ITaxProviderResult result)
        {
            order.ItemsTax = result.ItemsTax;
            order.ShippingTax = result.ShippingTax;
            order.ShippingTaxRate = result.ShippingTaxRate;
            order.TotalTax = result.TotalTax;

            if (applyVATRules)
                order.TotalShippingAfterDiscounts += order.ShippingTax;

            for (var i = 0; i < result.Items.Count; i++)
            {
                order.Items[i].TaxRate = result.Items[i].TaxRate;
                order.Items[i].TaxPortion = result.Items[i].TaxValue;

                if (applyVATRules)
                    order.Items[i].LineTotal += order.Items[i].TaxPortion;
            }
        }

        private HccAvaTax GetHccAvaTax(Store appStore)
        {
            return new HccAvaTax(settings.ServiceUrl,
                settings.Account,
                settings.LicenseKey);
        }

        private BaseAddress ConvertAddressToAvalara(Address address)
        {
            var result = new BaseAddress();
            result.Line1 = address.Street;
            result.Line2 = address.Street2;
            result.City = address.City;
            result.Region = address.RegionBvin;
            result.PostalCode = address.PostalCode;
            result.Country = address.CountryData.IsoCode;

            return result;
        }

        private List<Line> ConvertOrderLines(Order o, OrderService orderSerivce, bool taxIncluded, long StoreId)
        {
            var result = new List<Line>();

            var count = 1;

            var taxSchedules = orderSerivce.TaxSchedules.FindAll(StoreId);

            foreach (var item in o.Items)
            {
                var newLine = new Line();
                newLine.No = count.ToString();
                newLine.ItemCode = item.ProductSku;
                newLine.Description = item.ProductName;
                newLine.Amount = item.LineTotal;
                newLine.Qty = item.Quantity;
                newLine.TaxIncluded = taxIncluded;
                if (item.IsTaxExempt)
                    newLine.ExemptionNo = "taxexempt";
                if (item.IsNonShipping)
                    newLine.DestinationAddress = ConvertAddressToAvalara(o.BillingAddress);

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

            var shipLine = new Line();
            shipLine.Amount = o.TotalShippingAfterDiscounts;
            shipLine.No = count.ToString();
            shipLine.ItemCode = o.ShippingMethodDisplayName;
            shipLine.Description = "Shipping";
            shipLine.Qty = 1;
            shipLine.TaxIncluded = taxIncluded;
            shipLine.TaxCode = settings.ShippingTaxCode;

            // Order level discount shouldn't be applied to shipping.
            // This is how Order discount is calculated when it is set in percentage
            shipLine.Discounted = false;
            result.Add(shipLine);

            return result;
        }

        private string GetOrderIdentifier(Order order, OrderService orderService)
        {
            var result = order.OrderNumber;
            if (string.IsNullOrEmpty(result))
            {
                result = order.bvin;
            }
            else
            {
                if (string.IsNullOrEmpty(order.CustomProperties.GetProperty(HCC_KEY, TaxProviderGetTaxCountPropertyName)))
                {
                    result += "-1";
                    order.CustomProperties.SetProperty(HCC_KEY, TaxProviderGetTaxCountPropertyName, "1");
                    orderService.Orders.Update(order);
                }
                else
                {
                    var count = order.CustomProperties.GetPropertyAsInt(HCC_KEY, TaxProviderGetTaxCountPropertyName);
                    count += 1;
                    order.CustomProperties.SetProperty(HCC_KEY, TaxProviderGetTaxCountPropertyName, count.ToString());
                    orderService.Orders.Update(order);
                    result += "-" + count;
                }
            }
            return result;
        }

        private string GetCustomerCode(Order order)
        {
            if (!string.IsNullOrWhiteSpace(order.UserID))
                return order.UserID;
            if (!string.IsNullOrWhiteSpace(order.UserEmail))
                return order.UserEmail;
            return "Anonymous";
        }

        #endregion
    }
}