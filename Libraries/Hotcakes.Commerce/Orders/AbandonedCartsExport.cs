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
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Orders
{
    public class AbandonedCartsExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        public AbandonedCartsExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        public void ExportToExcel(HttpResponse response, string fileName, List<AbandonedProduct> abandonedProducts)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new ProductsSheetWriter(writer, _hccApp);
            mainWriter.Write(abandonedProducts);

            writer.Save();
            writer.WriteToResponse(response, fileName);
        }

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
            protected int _firstRow;
            private HotcakesApplication _hccApp;
            protected SpreadsheetStyle _headerStyle;
            protected SpreadsheetStyle _rowStyle;
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
            protected int _firstRow;
            private readonly HotcakesApplication _hccApp;
            protected SpreadsheetStyle _headerStyle;
            protected SpreadsheetStyle _rowStyle;
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

            protected string GetDateTimeString(DateTime utcTime)
            {
                return DateHelper.ConvertUtcToStoreTime(_hccApp, utcTime).ToShortDateString();
            }
        }
    }
}