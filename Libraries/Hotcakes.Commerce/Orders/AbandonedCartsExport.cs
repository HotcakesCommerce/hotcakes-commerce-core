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
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    /// Exports abandoned-cart summaries and abandoned-cart line items to an Excel workbook.
    /// </summary>
    /// <remarks>
    /// The export uses the existing <see cref="ExcelWriter"/> response pipeline and preserves
    /// the established worksheet name, column headings, row layout, and culture-sensitive
    /// formatting. The cart export writes one cart row followed by one row per item, with item
    /// values beginning in column C.
    /// </remarks>
    public class AbandonedCartsExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an exporter using the application context needed to resolve customers and
        /// convert UTC order times to store time.
        /// </summary>
        /// <param name="hccApp">The current Hotcakes application context.</param>
        public AbandonedCartsExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        /// <summary>
        /// Writes an abandoned-product summary workbook to an HTTP response.
        /// </summary>
        /// <param name="response">The response that receives the generated workbook.</param>
        /// <param name="fileName">The attachment file name supplied to the response.</param>
        /// <param name="abandonedProducts">Product-level abandoned-cart results.</param>
        /// <remarks>
        /// The worksheet is named <c>Main</c> and contains the columns <c>Product</c>,
        /// <c>Quantity</c>, <c>Carts</c>, and <c>Contacts</c>. Numeric values are converted with
        /// their parameterless <see cref="object.ToString"/> behavior; no explicit culture is
        /// selected. The method saves the workbook before writing it to the response.
        /// </remarks>
        public void ExportToExcel(HttpResponse response, string fileName, List<AbandonedProduct> abandonedProducts)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new ProductsSheetWriter(writer, _hccApp);
            mainWriter.Write(abandonedProducts);

            writer.Save();
            writer.WriteToResponse(response, fileName);
        }

        /// <summary>
        /// Writes abandoned carts and their line items to an HTTP response as an Excel workbook.
        /// </summary>
        /// <param name="response">The response that receives the generated workbook.</param>
        /// <param name="fileName">The attachment file name supplied to the response.</param>
        /// <param name="abandonedCarts">Abandoned orders to write, in the supplied order.</param>
        /// <remarks>
        /// The worksheet is named <c>Main</c> and has the columns <c>Date</c>, <c>User</c>,
        /// <c>Product</c>, <c>Quantity</c>, and <c>Line Total</c>. Each cart occupies a row for
        /// its date and user, followed by one item row whose values start in column C. Dates are
        /// converted from UTC to store time and formatted with <see cref="DateTime.ToShortDateString"/>;
        /// quantities and line totals use parameterless <see cref="object.ToString"/> behavior.
        /// </remarks>
        public void ExportToExcel(HttpResponse response, string fileName, List<Order> abandonedCarts)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new CartsSheetWriter(writer, _hccApp);
            mainWriter.Write(abandonedCarts);

            writer.Save();
            writer.WriteToResponse(response, fileName);
        }

        internal class ProductsSheetWriter
        {
            /// <summary>Gets or sets the first data row written by this sheet writer.</summary>
            protected int _firstRow;
            private HotcakesApplication _hccApp;
            /// <summary>Style applied to the product summary header.</summary>
            protected SpreadsheetStyle _headerStyle;
            /// <summary>Style applied to product summary rows.</summary>
            protected SpreadsheetStyle _rowStyle;
            /// <summary>The workbook writer used to emit the product sheet.</summary>
            protected ExcelWriter _writer;

            internal ProductsSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();

                _writer.SetColumnsWidths(new double[] {50});

                _headerStyle.IsBold = true;
                _hccApp = hccApp;
            }

            /// <summary>Writes the header and one row for each abandoned product.</summary>
            /// <param name="abandonedProducts">Product summary values to write.</param>
            public void Write(List<AbandonedProduct> abandonedProducts)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var abandonedProduct in abandonedProducts)
                {
                    rowIndex = WriteRow(abandonedProduct, rowIndex);
                }
            }

            private void WriteHeader()
            {
                var centerStyle = _writer.GetStyle();
                centerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                _writer.WriteRow("A", 1, new List<string>
                {
                    "Product",
                    "Quantity",
                    "Carts",
                    "Contacts"
                }, _headerStyle);

                _firstRow = 2;
            }

            private int WriteRow(AbandonedProduct abandonedProduct, int rowIndex)
            {
                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    abandonedProduct.ProductName,
                    abandonedProduct.Quantity.ToString(),
                    abandonedProduct.CartsCount.ToString(),
                    abandonedProduct.ContactsCount.ToString()
                }, _rowStyle);

                return ++rowIndex;
            }
        }

        internal class CartsSheetWriter
        {
            /// <summary>Gets or sets the next row used by the cart and item output.</summary>
            protected int _firstRow;
            private readonly HotcakesApplication _hccApp;
            /// <summary>Style applied to the cart header.</summary>
            protected SpreadsheetStyle _headerStyle;
            /// <summary>Style applied to cart and line-item rows.</summary>
            protected SpreadsheetStyle _rowStyle;
            /// <summary>The workbook writer used to emit the cart sheet.</summary>
            protected ExcelWriter _writer;

            internal CartsSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();

                _writer.SetColumnsWidths(new double[] {10, 20, 50, 10, 16});

                _headerStyle.IsBold = true;
                _hccApp = hccApp;
            }

            /// <summary>
            /// Writes the cart header and each cart followed by its line-item rows.
            /// </summary>
            /// <param name="abandonedCarts">Abandoned carts to write.</param>
            public void Write(List<Order> abandonedCarts)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var abandonedCart in abandonedCarts)
                {
                    rowIndex = WriteRow(abandonedCart, rowIndex);
                }
            }

            private void WriteHeader()
            {
                var centerStyle = _writer.GetStyle();
                centerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                _writer.WriteRow("A", 1, new List<string>
                {
                    "Date",
                    "User",
                    "Product",
                    "Quantity",
                    "Line Total"
                }, _headerStyle);

                _firstRow = 2;
            }

            private int WriteRow(Order abandonedCart, int rowIndex)
            {
                var user = _hccApp.MembershipServices.Customers.Find(abandonedCart.UserID);
                var userName = user != null ? user.FirstName + " " + user.LastName : string.Empty;

                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    GetDateTimeString(abandonedCart.TimeOfOrderUtc),
                    userName
                }, _rowStyle);

                ++rowIndex;

                foreach (var lineItem in abandonedCart.Items)
                {
                    _writer.WriteRow("C", rowIndex, new List<string>
                    {
                        lineItem.ProductName,
                        lineItem.Quantity.ToString(),
                        lineItem.LineTotal.ToString()
                    }, _rowStyle);

                    ++rowIndex;
                }

                return rowIndex;
            }

            /// <summary>
            /// Converts a UTC timestamp to store time and formats it using the current culture's
            /// short-date pattern.
            /// </summary>
            /// <param name="utcTime">The timestamp recorded in UTC.</param>
            /// <returns>The store-local short date string used in the Excel cart export.</returns>
            protected string GetDateTimeString(DateTime utcTime)
            {
                return DateHelper.ConvertUtcToStoreTime(_hccApp, utcTime).ToShortDateString();
            }
        }
    }
}
