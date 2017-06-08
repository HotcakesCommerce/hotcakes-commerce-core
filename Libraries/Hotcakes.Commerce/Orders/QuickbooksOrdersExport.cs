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
using System.Globalization;
using System.Web;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Payment;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    [Serializable]
    public class QuickbooksOrdersExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        public List<string[]> FileLines = new List<string[]>();

        #region Constructor

        public QuickbooksOrdersExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        public void ExportToQuickbooks(HttpResponse response, string fileName, List<OrderSnapshot> orders)
        {
            WriteHeader();

            foreach (var orderSnapshot in orders)
            {
                ProcessSingleOrder(orderSnapshot);
            }

            WriteToResponse(response, fileName);
        }

        private void WriteHeader()
        {
            WriteLine(new[]
            {
                "!TRNS", "TRNSID", "TRNSTYPE", "DATE", "DOCNUM", "ACCNT", "MEMO", "NAME", "AMOUNT", "NAMEISTAXABLE",
                "PAYMETH"
            });
            WriteLine(new[] {"!SPL", "SPLID", "TRNSTYPE", "DATE", "ACCNT", "MEMO", "NAME", "AMOUNT", "TAXABLE"});
            WriteLine(new[] {"!ENDTRNS"});
        }

        private void ProcessSingleOrder(OrderSnapshot orderSnapshot)
        {
            var order = _hccApp.OrderServices.Orders.FindForCurrentStore(orderSnapshot.bvin);
            var transactions = _hccApp.OrderServices.Transactions.FindAllTransactionsForOrder(orderSnapshot.bvin);

            foreach (var transaction in transactions)
            {
                var action = transaction.Action;
                if (ActionTypeUtils.BalanceChangingActions.Contains(action)
                    && !ActionTypeUtils.IsGiftCardTransaction(action)
                    && !ActionTypeUtils.IsRewardPointTransaction(action))
                    ProcessSingleTransaction(order, transaction);
            }
        }

        private void ProcessSingleTransaction(Order order, OrderTransaction transaction)
        {
            var date = transaction.TimeStampUtc.ToString("d", CultureInfo.InvariantCulture);
            var transactionType = GetTransactionType(transaction.Action);
            var customer = _hccApp.MembershipServices.Customers.Find(order.UserID);
            var customerName = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;

            var orderAccount = _hccApp.CurrentStore.Settings.QuickbooksOrderAccount;
            var shippingAccount = _hccApp.CurrentStore.Settings.QuickbooksShippingAccount;

            var memoKey = order.TotalOrderDiscounts == 0 ? "OrderMemo" : "OrderWithDiscountMemo";
            var orderName = LocalizationUtils.GetQuickbooksExportFormattedString(memoKey, order.OrderNumber);
            var payMeth = GetPaymentMethodName(transaction);

            WriteLine(new[]
            {
                "TRNS", transaction.IdAsString, transactionType, date, order.OrderNumber, orderAccount, orderName,
                customerName, transaction.Amount.ToString(CultureInfo.InvariantCulture), customer.TaxExempt ? "N" : "Y",
                payMeth
            });
            foreach (var item in order.Items)
            {
                var itemTaxPortion = order.ApplyVATRules ? 0 : item.TaxPortion;
                var itemPercentage = (item.LineTotal + itemTaxPortion)/order.TotalGrand;
                var amountApplied = itemPercentage*transaction.Amount;

                var product = item.GetAssociatedProduct(_hccApp);
                var productType = product != null
                    ? _hccApp.CatalogServices.ProductTypes.Find(product.ProductTypeId)
                    : null;
                var productTypeName = productType != null ? productType.ProductTypeName : string.Empty;

                WriteLine(new[]
                {
                    "SPL", item.Id.ToString(), transactionType, date, productTypeName, item.ProductName, customerName,
                    (-1*amountApplied).ToString(CultureInfo.InvariantCulture),
                    product != null && product.TaxExempt ? "N" : "Y"
                });
            }
            var shippingTaxPortion = order.ApplyVATRules ? 0 : order.ShippingTax;
            var shippingPercentage = (order.TotalShippingAfterDiscounts + shippingTaxPortion)/order.TotalGrand;
            var shippingAmountApplied = shippingPercentage*transaction.Amount;
            var shippingName = LocalizationUtils.GetQuickbooksExportFormattedString("ShippingMemo");
            WriteLine(new[]
            {
                "SPL", "shipping", transactionType, date, shippingAccount, shippingName, customerName,
                (-1*shippingAmountApplied).ToString(CultureInfo.InvariantCulture), "Y"
            });
            WriteLine(new[] {"ENDTRNS"});
        }

        private string GetPaymentMethodName(OrderTransaction transaction)
        {
            switch (transaction.Action)
            {
                case ActionType.CashReceived:
                case ActionType.CashReturned:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethCash");
                case ActionType.CheckReceived:
                case ActionType.CheckReturned:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethCheck");
                case ActionType.CompanyAccountAccepted:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethCompanyAccount");
                case ActionType.PurchaseOrderAccepted:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethPurchaseOrder");
                case ActionType.CreditCardCapture:
                case ActionType.CreditCardCharge:
                case ActionType.CreditCardRefund:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethCreditCard");
                case ActionType.PayPalCapture:
                case ActionType.PayPalCharge:
                case ActionType.PayPalRefund:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethPayPal");
                case ActionType.ThirdPartyPayMethodCapture:
                case ActionType.ThirdPartyPayMethodCharge:
                case ActionType.ThirdPartyPayMethodRefund:
                    var payMeth = PaymentMethods.Find(transaction.MethodId);
                    return LocalizationUtils.GetPaymentMethodFriendlyName(payMeth.MethodName);
                default:
                    return LocalizationUtils.GetQuickbooksExportString("PayMethUnknown");
            }
        }

        private string GetTransactionType(ActionType actionType)
        {
            return "DEPOSIT";
        }

        private void WriteLine(string[] line)
        {
            FileLines.Add(line);
        }

        private void WriteToResponse(HttpResponse response, string filename)
        {
            response.Clear();
            response.AddHeader("content-disposition", string.Format("attachment;filename={0}", filename));
            response.ContentType = "text/plain";

            foreach (var line in FileLines)
            {
                response.Write(string.Join("	", line));
                response.Write(Environment.NewLine);
            }

            response.End();
        }
    }
}