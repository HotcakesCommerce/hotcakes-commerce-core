#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Avalara.AvaTax.Adapter.AddressService;
using Avalara.AvaTax.Adapter.TaxService;
using avt = Avalara.AvaTax.Adapter;

namespace Hotcakes.Avalara
{
    [Serializable]
    public class HccAvaTax
    {
        public HccAvaTax(string url, string account, string license)
        {
            Url = url;
            Account = account;
            License = license;
        }

        public string Url { get; set; }
        public string Account { get; set; }
        public string License { get; set; }

        public AvaTaxResult TestConnection()
        {
            var result = new AvaTaxResult();

            var svc = GetTaxServiceProxy();

            var pingResult = svc.Ping("Test Connection Message at " + DateTime.Now);

            if (pingResult.ResultCode == avt.SeverityLevel.Success)
            {
                result.Success = true;
            }
            else
            {
                result.Success = false;
                ApplyMessagesToResult(result, pingResult.Messages);
            }

            return result;
        }

        public AvaTaxResult GetTax(DocumentType docType, string companyCode, string docCode,
            BaseAddress originationAddress, BaseAddress destinationAddress, List<Line> items, decimal orderDiscount,
            string customerCode, string currencyCode,
            bool taxExempt, string taxExemptionNumber, bool applyVATRules)
        {
            var result = new AvaTaxResult();

            var gtr = new GetTaxRequest
            {
                OriginAddress = ConvertBaseAddress(originationAddress),
                DestinationAddress = ConvertBaseAddress(destinationAddress),
                CurrencyCode = currencyCode
            };

            if (taxExempt)
            {
                if (!applyVATRules)
                    gtr.ExemptionNo = taxExemptionNumber;
                else
                    gtr.BusinessIdentificationNo = taxExemptionNumber;
            }

            gtr.CompanyCode = companyCode;
            gtr.CustomerCode = customerCode;
            gtr.DetailLevel = DetailLevel.Line;
            gtr.DocCode = docCode;
            gtr.DocType = ConvertDocType(docType);
            gtr.DocDate = DateTime.UtcNow;

            gtr.Discount = -1*orderDiscount;
            foreach (var l in items)
            {
                var nl = ConvertLine(l);
                gtr.Lines.Add(nl);
            }

            var svc = GetTaxServiceProxy();
            var gtres = svc.GetTax(gtr);
            
            if (gtres.ResultCode != avt.SeverityLevel.Success)
            {
                result.Success = false;
                result.Messages.Add("GetTax Failed");
                ApplyMessagesToResult(result, gtres.Messages);
            }
            else
            {
                result.Success = true;

                result.DocCode = gtres.DocCode;
                result.TotalAmount = gtres.TotalAmount;
                result.TotalTax = gtres.TotalTax;

                // shipping line is always present ( even if 0 )
                // so the last item is shipping
                for (var i = 0; i < gtres.TaxLines.Count; i++)
                {
                    var line = gtres.TaxLines[i];
                    if (i < gtres.TaxLines.Count - 1)
                    {
                        var lineResult = new AvaTaxLineResult
                        {
                            No = line.No,
                            TaxRate = (decimal) line.Rate,
                            TaxValue = line.Tax
                        };
                        result.Items.Add(lineResult);
                    }
                    else
                    {
                        result.ShippingTax = line.Tax;
                        result.ShippingTaxRate = (decimal) line.Rate;
                    }
                }
                result.ItemsTax = result.TotalTax - result.ShippingTax;
            }

            return result;
        }

        public AvaTaxResult PostTax(string companyCode,
            string docCode,
            DocumentType docType,
            decimal totalAmount,
            decimal totalTax)
        {
            var result = new AvaTaxResult();

            var ptr = new PostTaxRequest
            {
                CompanyCode = companyCode,
                DocCode = docCode,
                DocType = ConvertDocType(docType),
                DocDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                TotalTax = totalTax
            };

            var svc = GetTaxServiceProxy();

            var ptres = svc.PostTax(ptr);
            
            if (ptres.ResultCode != avt.SeverityLevel.Success)
            {
                result.Success = false;
                result.Messages.Add("PostTax Failed");
                ApplyMessagesToResult(result, ptres.Messages);
            }
            else
            {
                result.Success = true;
            }

            result.DocCode = docCode;
            result.TotalAmount = totalAmount;
            result.TotalTax = totalTax;

            return result;
        }

        public AvaTaxResult CommitTax(string companyCode, string docCode, DocumentType docType)
        {
            var result = new AvaTaxResult();

            var ctr = new CommitTaxRequest
            {
                CompanyCode = companyCode,
                DocCode = docCode,
                DocType = ConvertDocType(docType)
            };

            var svc = GetTaxServiceProxy();

            var ctres = svc.CommitTax(ctr);
            
            if (ctres.ResultCode != avt.SeverityLevel.Success)
            {
                result.Success = false;
                result.Messages.Add("CommitTax Failed");
                ApplyMessagesToResult(result, ctres.Messages);
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        public AvaTaxResult CancelTax(string companyCode, string docCode, DocumentType docType)
        {
            var result = new AvaTaxResult();

            var ctr = new CancelTaxRequest
            {
                CompanyCode = companyCode,
                DocCode = docCode,
                DocType = ConvertDocType(docType),
                CancelCode = CancelCode.DocDeleted
            };

            var svc = GetTaxServiceProxy();

            var ctres = svc.CancelTax(ctr);
            
            if (ctres.ResultCode != avt.SeverityLevel.Success)
            {
                result.Success = false;
                result.Messages.Add("CancelTax Failed");
                ApplyMessagesToResult(result, ctres.Messages);
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        #region Private

        private TaxSvc GetTaxServiceProxy()
        {
            var svc = new TaxSvc();

            svc.Profile.Client = "Hotcakes Commerce";
            if (Url != null)
            {
                svc.Configuration.Url = Url;
            }
            if (Account != null && Account.Length > 0)
            {
                svc.Configuration.Security.Account = Account;
            }
            if (License != null && License.Length > 0)
            {
                svc.Configuration.Security.License = License;
            }

            return svc;
        }

        private static void ApplyMessagesToResult(AvaTaxResult result, avt.Messages messages)
        {
            if (messages != null)
            {
                foreach (avt.Message item in messages)
                {
                    result.Messages.Add(item.Name + Environment.NewLine + item.Summary + Environment.NewLine +
                                        item.Details);
                }
            }
        }

        private static global::Avalara.AvaTax.Adapter.TaxService.DocumentType ConvertDocType(DocumentType localDoc)
        {
            switch (localDoc)
            {
                case DocumentType.PurchaseInvoice:
                    return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.PurchaseInvoice;
                case DocumentType.PurchaseOrder:
                    return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.PurchaseOrder;
                case DocumentType.ReturnInvoice:
                    return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.ReturnInvoice;
                case DocumentType.ReturnOrder:
                    return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.ReturnOrder;
                case DocumentType.SalesInvoice:
                    return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.SalesInvoice;
                case DocumentType.SalesOrder:
                    return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.SalesOrder;
            }
            return global::Avalara.AvaTax.Adapter.TaxService.DocumentType.PurchaseOrder;
        }

        private static Address ConvertBaseAddress(BaseAddress local)
        {
            var result = new Address
            {
                City = local.City,
                Country = local.Country,
                Line1 = local.Line1,
                Line2 = local.Line2,
                Line3 = local.Line3,
                PostalCode = local.PostalCode,
                Region = local.Region
            };

            return result;
        }

        private static global::Avalara.AvaTax.Adapter.TaxService.Line ConvertLine(Line l)
        {
            var result = new global::Avalara.AvaTax.Adapter.TaxService.Line
            {
                Amount = l.Amount,
                CustomerUsageType = l.CustomerUsageType,
                Description = l.Description,
                Discounted = l.Discounted,
                ExemptionNo = l.ExemptionNo,
                ItemCode = l.ItemCode,
                No = l.No,
                Qty = (double) l.Qty,
                Ref1 = l.Ref1,
                Ref2 = l.Ref2,
                RevAcct = l.RevAcct,
                TaxCode = l.TaxCode,
                TaxIncluded = l.TaxIncluded
            };

            if (l.OriginAddress != null)
                result.OriginAddress = ConvertBaseAddress(l.OriginAddress);
            if (l.DestinationAddress != null)
                result.DestinationAddress = ConvertBaseAddress(l.DestinationAddress);

            return result;
        }

        #endregion
    }
}