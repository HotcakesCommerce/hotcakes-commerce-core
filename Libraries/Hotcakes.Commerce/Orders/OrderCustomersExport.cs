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
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Commerce.Membership;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Orders
{
    [Serializable]
    public class OrderCustomersExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        public OrderCustomersExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        public void ExportToExcel(HttpResponse response, string fileName, List<CustomerAccount> customers,
            string productName)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new MainSheetWriter(writer, _hccApp);
            mainWriter.Write(customers, productName);

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

            internal BaseSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _hccApp = hccApp;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();
                _headerStyle.IsBold = true;
            }

            public void Write(List<CustomerAccount> customers, string productName)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var c in customers)
                {
                    rowIndex = WriteCustomerRow(c, rowIndex, productName);
                }
            }

            protected virtual int WriteCustomerRow(CustomerAccount c, int rowIndex, string productName)
            {
                return ++rowIndex;
            }

            protected virtual void WriteHeader()
            {
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
                    "Product Name",
                    "First Name",
                    "Last Name",
                    "Email Address",
                    "Phone Number",
                    "Line 1",
                    "Line 2",
                    "City",
                    "Region",
                    "Country",
                    "Postal Code"
                }, _headerStyle);

                _firstRow = 2;
            }

            /// <summary>
            ///     Writes a row in the spreadsheet for each customer found in the search
            /// </summary>
            /// <param name="c">CustomerAccount - the customer record.</param>
            /// <param name="rowIndex">The position of the row in the spreadsheet</param>
            /// <param name="productName">The product name</param>
            /// <returns>An integer value indicating the next row index</returns>
            protected override int WriteCustomerRow(CustomerAccount c, int rowIndex, string productName)
            {
                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    c.Bvin,
                    productName,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.BillingAddress.Phone,
                    c.BillingAddress.Line1,
                    c.BillingAddress.Line2,
                    c.BillingAddress.City,
                    c.BillingAddress.RegionDisplayName,
                    c.BillingAddress.CountryDisplayName,
                    c.BillingAddress.PostalCode
                }, _rowStyle);

                return base.WriteCustomerRow(c, rowIndex, productName);
            }
        }

        #endregion
    }
}