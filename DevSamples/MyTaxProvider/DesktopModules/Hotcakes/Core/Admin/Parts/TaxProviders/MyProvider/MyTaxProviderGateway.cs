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
using Hotcakes.Commerce.Taxes.Providers;

namespace MyCompany.MyTaxProvider
{
    /// <summary>
    ///     This class is used to write down the core process
    ///     to communicate with external tax provider and perform the different operation
    /// </summary>
    [Serializable]
    public class MyTaxProviderGateway
    {
        public MyTaxProviderGateway(string prop1, string prop2)
        {
            Prop1 = prop1;
            Prop2 = prop2;
        }

        public string Prop1 { get; set; }
        public string Prop2 { get; set; }

        public string ProviderGatewayID
        {
            get { return "6514AA2B-1C5F-4736-936C-665D1CC228E0"; }
        }

        public string ProviderGatewayName
        {
            get { return "MyProvider"; }
        }

        public string ProviderID
        {
            get { return "CCAD00ED-574E-4675-B788-49C4989A0795"; }
        }

        public ITaxProviderResult CancelTax(string companyCode, string docCode, DocumentType docType)
        {
            var result = new MyTaxProviderResult();

            // Write down business logic here to cancel the tax.

            var suceed = false;
            if (!suceed)
            {
                result.Success = false;
                result.Messages.Add("CancelTax Failed");
                // end user can add more messages whatever returned from tax provider as messages variable is list of string           
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        public ITaxProviderResult CommitTax(string companyCode, string docCode, DocumentType docType)
        {
            var result = new MyTaxProviderResult();

            // Write down business logic here to commit the tax.

            var suceed = false;
            if (!suceed)
            {
                result.Success = false;
                result.Messages.Add("CommitTax Failed");
                // end user can add more messages whatever returned from tax provider as messages variable is list of string
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        public ITaxProviderResult GetTax(DocumentType docType, string companyCode, string docCode,
            BaseAddress originationAddress, BaseAddress destinationAddress, List<Line> items, decimal orderDiscount,
            string customerCode, string currencyCode, bool taxExempt, string taxExemptionNumber, bool applyVATRules)
        {
            var result = new MyTaxProviderResult();

            // Write down business logic here to get the tax.

            var suceed = false;
            if (!suceed)
            {
                result.Success = false;
                result.Messages.Add("Get Tax Failed");
                // end user can add more messages whatever returned from tax provider as messages variable is list of string
            }
            else
            {
                result.Success = true;

                result.DocCode = "123";
                result.TotalAmount = 50;
                result.TotalTax = 5;

                // shipping line is always present ( even if 0 )
                // so the last item is shipping
                for (var i = 0; i < 2; i++)
                {
                    if (i < 2)
                    {
                        var lineResult = new MyTaxProviderLineResult
                        {
                            No = "abc",
                            TaxRate = 10,
                            TaxValue = 20
                        };
                        result.Items.Add(lineResult);
                    }
                    else
                    {
                        result.ShippingTax = 10;
                        result.ShippingTaxRate = 20;
                    }
                }
                result.ItemsTax = result.TotalTax - result.ShippingTax;
            }

            return result;
        }

        public ITaxProviderResult PostTax(string companyCode, string docCode, DocumentType docType, decimal totalAmount,
            decimal totalTax)
        {
            var result = new MyTaxProviderResult();

            // Write down business logic here to post the tax .
            var suceed = false;
            if (!suceed)
            {
                result.Success = false;
                result.Messages.Add("Post Tax Failed");
                // end user can add more messages whatever returned from tax provider as messages variable is list of string
            }
            else
            {
                result.DocCode = "123";
                result.TotalAmount = 123;
                result.TotalTax = 10;
            }

            return result;
        }

        public ITaxProviderResult TestConnection()
        {
            var result = new MyTaxProviderResult();

            // Write down business logic here to test the tax provider settings .
            var suceed = false;
            if (!suceed)
            {
                result.Success = false;
                result.Messages.Add("Test connection Failed");
                // end user can add more messages whatever returned from tax provider as messages variable is list of string
            }
            else
            {
                result.Success = true;
            }

            return result;
        }
    }
}