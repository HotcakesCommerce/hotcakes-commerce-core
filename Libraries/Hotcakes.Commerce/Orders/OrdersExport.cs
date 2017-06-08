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
using System.Linq;
using System.Text;
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Orders
{
    [Serializable]
    public class OrdersExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        public OrdersExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        public void ExportToExcel(HttpResponse response, string fileName, List<OrderSnapshot> orders)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new MainSheetWriter(writer, _hccApp);
            mainWriter.Write(orders);

            var lineItemsWriter = new LineItemsSheetWriter(writer, _hccApp);
            lineItemsWriter.Write(orders);
            lineItemsWriter.WriteOptions();

            writer.Save();
            writer.WriteToResponse(response, fileName);
        }

        #region Internal declarations

        internal class BaseSheetWriter
        {
            protected int _firstRow;
            protected HotcakesApplication _hccApp;
            protected SpreadsheetStyle _headerStyle;
            protected SpreadsheetStyle _rowStyle;
            protected ExcelWriter _writer;

            private const string YES = "YES";
            private const string NO = "NO";

            internal BaseSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _hccApp = hccApp;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();
                _headerStyle.IsBold = true;
            }

            public void Write(List<OrderSnapshot> orders)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var o in orders)
                {
                    rowIndex = WriteOrderRow(o, rowIndex);
                }
            }

            protected virtual int WriteOrderRow(OrderSnapshot p, int rowIndex)
            {
                return ++rowIndex;
            }

            protected virtual void WriteHeader()
            {
            }

            protected string GetYesNo(bool val)
            {
                return val ? YES : NO;
            }

            protected string GetCurrency(decimal val)
            {
                return val.ToString("C");
            }
        }

        internal class MainSheetWriter : BaseSheetWriter
        {
            internal MainSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            /// <summary>
            ///     Creates the header row for the XLS to define the columns the spreadsheet will have.
            /// </summary>
            protected override void WriteHeader()
            {
                var centerStyle = _writer.GetStyle();
                centerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                _writer.WriteRow("A", 1, new List<string>
                {
                    "ID",
                    "Order #",
                    "Affiliate ID",

                    // billing contact & address
                    "Billing First Name",
                    "Billing Last Name",
                    "Billing Phone",
                    "Billing Address",
                    "Billing Address2",
                    "Billing City",
                    "Billing Region",
                    "Billing Postal Code",
                    "Billing Country",

                    // shipping contact & address
                    "Shipping First Name",
                    "Shipping Last Name",
                    "Shipping Phone",
                    "Shipping Address",
                    "Shipping Address2",
                    "Shipping City",
                    "Shipping Region",
                    "Shipping Postal Code",
                    "Shipping Country",

                    // shipping columns
                    "Shipping Method",
                    "Shipping Provider",
                    "Shipping Status",
                    "Shipping Discount",
                    "Shipping Discount Details",
                    "Shipping Total",
                    "Adjusted Shipping Total",
                    //end shipping columns

                    "Instructions",
                    "Order Discount",
                    "Order Discount Details",
                    "Handling Total",
                    "Subtotal",
                    "Items Tax",
                    "Shipping Tax",
                    "Tax Total",
                    "Total",
                    "Date",
                    "User Email",
                    "User ID",
                    "Status"
                }, _headerStyle);

                _firstRow = 2;
            }

            /// <summary>
            ///     Writes a row in the spreadsheet for each order found in the search
            /// </summary>
            /// <param name="o">OrderSnapshop - a truncated version of the order record used to write the row.</param>
            /// <param name="rowIndex">The position of the row in the spreadsheet</param>
            /// <returns>An integer value indicating the next row index</returns>
            protected override int WriteOrderRow(OrderSnapshot o, int rowIndex)
            {
                var order = _hccApp.OrderServices.Orders.FindForCurrentStore(o.bvin);

                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    o.bvin,
                    o.OrderNumber,
                    o.AffiliateID.ToString(),

                    // billing contact & address
                    o.BillingAddress.FirstName,
                    o.BillingAddress.LastName,
                    o.BillingAddress.Phone,
                    o.BillingAddress.Line1,
                    o.BillingAddress.Line2,
                    o.BillingAddress.City,
                    o.BillingAddress.RegionDisplayName,
                    o.BillingAddress.PostalCode,
                    o.BillingAddress.CountryDisplayName,

                    // shipping contact & address
                    o.ShippingAddress.FirstName,
                    o.ShippingAddress.LastName,
                    o.ShippingAddress.Phone,
                    o.ShippingAddress.Line1,
                    o.ShippingAddress.Line2,
                    o.ShippingAddress.City,
                    o.ShippingAddress.RegionDisplayName,
                    o.ShippingAddress.PostalCode,
                    o.ShippingAddress.CountryDisplayName,

                    // shipping columns
                    o.ShippingMethodDisplayName,
                    o.ShippingProviderServiceCode,
                    LocalizationUtils.GetOrderShippingStatus(o.ShippingStatus),
                    GetCurrency(o.TotalShippingDiscounts),
                    DiscountDetail.ListToXml(order.ShippingDiscountDetails),
                    GetCurrency(o.TotalShippingBeforeDiscounts),
                    GetCurrency(order.TotalShippingAfterDiscounts),
                    //end shipping columns

                    o.Instructions,
                    GetCurrency(o.TotalOrderDiscounts),
                    DiscountDetail.ListToXml(order.OrderDiscountDetails),
                    GetCurrency(o.TotalHandling),
                    GetCurrency(o.TotalOrderBeforeDiscounts),
                    GetCurrency(o.ItemsTax),
                    GetCurrency(o.ShippingTax),
                    GetCurrency(o.TotalTax),
                    GetCurrency(o.TotalGrand),
                    o.TimeOfOrderUtc.ToString(),
                    o.UserEmail,
                    o.UserID,
                    o.StatusName
                }, _rowStyle);

                return base.WriteOrderRow(o, rowIndex);
            }
        }

        internal class LineItemsSheetWriter : BaseSheetWriter
        {
            private int _lastRowIndex;

            private readonly List<Option> _options = new List<Option>();

            internal LineItemsSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            protected override void WriteHeader()
            {
                _writer.AddWorksheet("Line Items");
                _writer.SetColumnsWidths(new double[] {50});

                _writer.WriteRow("A", 1, new List<string>
                {
                    "Order ID",
                    "Order #",
                    "Product ID",
                    "Product Name",
                    "Product SKU",
                    "Description",
                    "Is Non Shipping",
                    "Tax Schedule Name",
                    "Ship From Address",
                    "Ship From Mode",
                    "Ship Separately",
                    "Extra Ship Charge",
                    "User Suplied Price",
                    "Discount Details",
                    "Base Price",
                    "Adjusted Price",
                    "Shipping Portion",
                    "Tax Items Portion",
                    "Quantity",
                    "Line Total",
                    "Gift Card", // U column
                    "Recurring",
                    "Recurring Interval",
                    "Recurring Interval Type" // X column
                }, _headerStyle);

                _firstRow = 2;
            }

            protected override int WriteOrderRow(OrderSnapshot o, int rowIndex)
            {
                var items = _hccApp.OrderServices.Orders.FindLineItemsForOrders(new List<OrderSnapshot> {o});

                foreach (var item in items)
                {
                    _writer.WriteRow("A", rowIndex, new List<string>
                    {
                        o.bvin,
                        o.OrderNumber,
                        item.ProductId,
                        item.ProductName,
                        item.ProductSku,
                        item.ProductShortDescription,
                        GetYesNo(item.IsNonShipping),
                        GetTaxScheduleName(item.TaxSchedule),
                        GettAddress(item.ShipFromAddress),
                        item.ShipFromMode.ToString(),
                        GetYesNo(item.ShipSeparately),
                        GetCurrency(item.ExtraShipCharge),
                        GetYesNo(item.IsUserSuppliedPrice),
                        GetDiscountDetails(item.DiscountDetails),
                        GetCurrency(item.BasePricePerItem),
                        GetCurrency(item.AdjustedPricePerItem),
                        GetCurrency(item.ShippingPortion),
                        GetCurrency(item.TaxPortion),
                        item.Quantity.ToString(),
                        GetCurrency(item.LineTotal),
                        GetYesNo(item.IsGiftCard),
                        GetYesNo(item.IsRecurring),
                        GetRecurringInterval(item),
                        GetRecurringIntervalType(item)
                    });

                    AddLineOptions(rowIndex, item);
                    rowIndex++;
                    _lastRowIndex = rowIndex;
                }

                return rowIndex;
            }

            public void WriteOptions()
            {
                var column = "Y";

                foreach (var o in _options)
                {
                    _writer.WriteCell(column, 1, o.OptionName, _headerStyle);

                    foreach (var ov in o.RowValues)
                    {
                        _writer.WriteCell(column, ov.RowIndex, ov.Value);
                    }

                    column = SpreadsheetReader.GetColumnName(column, 1);
                }
            }

            private void AddLineOptions(int rowIndex, LineItem item)
            {
                var options = _hccApp.CatalogServices.ProductOptions.FindByProductId(item.ProductId);

                foreach (var op in options)
                {
                    var vals = op.Processor.GetSelectionValues(op, item.SelectionData.OptionSelectionList);

                    if (vals.Count > 0)
                    {
                        var optionCols =
                            _options.Where(o => o.OptionName.Equals(op.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        var optColIndex = _options.Count;
                        var colIndex = 0;

                        foreach (var val in vals)
                        {
                            if (optionCols.Count < colIndex + 1)
                            {
                                var optionCol = new Option
                                {
                                    OptionName = op.Name.Trim(),
                                    RowValues = new List<OptionValue>()
                                };
                                optColIndex =
                                    _options.FindLastIndex(
                                        o => o.OptionName.Equals(op.Name.Trim(), StringComparison.OrdinalIgnoreCase));
                                if (optColIndex > 0)
                                {
                                    _options.Insert(optColIndex, optionCol);
                                }
                                else
                                {
                                    _options.Add(optionCol);
                                }
                                optionCols.Add(optionCol);
                            }

                            optionCols[colIndex].RowValues.Add(new OptionValue
                            {
                                RowIndex = rowIndex,
                                Value = vals.First()
                            });
                            colIndex++;
                        }
                    }
                }
            }

            private string GetDiscountDetails(List<DiscountDetail> list)
            {
                var sb = new StringBuilder();

                foreach (var dd in list)
                {
                    sb.AppendFormat("{0}:{1:C}, ", dd.Description, dd.Amount);
                }

                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }

            private string GettAddress(Address address)
            {
                return address.ToHtmlString().Replace("<br />", ", ");
            }

            private string GetTaxScheduleName(long taxId)
            {
                var taxSchedule = _hccApp.OrderServices.TaxSchedules.FindForThisStore(taxId);
                return taxSchedule != null ? taxSchedule.Name : string.Empty;
            }

            private string GetRecurringInterval(LineItem lineItem)
            {
                var interval = lineItem.RecurringBilling.Interval;
                return lineItem.IsRecurring ? interval.ToString() : string.Empty;
            }

            private string GetRecurringIntervalType(LineItem lineItem)
            {
                var intervalType = lineItem.RecurringBilling.IntervalType;
                return lineItem.IsRecurring ? intervalType.ToString() : string.Empty;
            }

            internal class OptionValue
            {
                public int RowIndex { get; set; }
                public string Value { get; set; }
            }

            internal class Option
            {
                public string OptionName { get; set; }
                public List<OptionValue> RowValues { get; set; }
            }
        }

        #endregion
    }
}