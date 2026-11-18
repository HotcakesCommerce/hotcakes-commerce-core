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
using Hotcakes.Commerce.Membership;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    /// Exports customers associated with an order-product search to an Excel workbook.
    /// </summary>
    /// <remarks>
    /// The workbook uses a worksheet named <c>Main</c> and preserves the established twelve
    /// columns, including billing-address values. Customer values are written as text through
    /// <see cref="ExcelWriter"/>; the supplied product name is repeated for every customer row.
    /// </remarks>
    [Serializable]
    public class OrderCustomersExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an exporter with the application context used by the nested sheet writers.
        /// </summary>
        /// <param name="hccApp">The current Hotcakes application context.</param>
        public OrderCustomersExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        /// <summary>
        /// Writes the matching customers to an Excel attachment in the HTTP response.
        /// </summary>
        /// <param name="response">The response that receives the generated workbook.</param>
        /// <param name="fileName">The attachment file name supplied to the response.</param>
        /// <param name="customers">Customers to write, in the supplied order.</param>
        /// <param name="productName">The product name placed in each customer's row.</param>
        /// <remarks>
        /// The header columns are <c>ID</c>, <c>Product Name</c>, first and last name, email,
        /// phone, and the billing address fields through <c>Postal Code</c>. The implementation
        /// reads the billing address directly, including its display region and country names.
        /// </remarks>
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
            /// <summary>Gets or sets the first data row for this sheet.</summary>
            protected int _firstRow;
            /// <summary>Provides the application context used by derived writers.</summary>
            protected HotcakesApplication _hccApp;
            /// <summary>Style applied to the header row.</summary>
            protected SpreadsheetStyle _headerStyle;
            /// <summary>Style applied to customer data rows.</summary>
            protected SpreadsheetStyle _rowStyle;
            /// <summary>The workbook writer used to emit cells and rows.</summary>
            protected ExcelWriter _writer;

            internal BaseSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _hccApp = hccApp;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();
                _headerStyle.IsBold = true;
            }

            /// <summary>
            /// Writes the header and invokes the row extension point for each customer.
            /// </summary>
            /// <param name="customers">Customers to write.</param>
            /// <param name="productName">Product name passed to each row writer.</param>
            public void Write(List<CustomerAccount> customers, string productName)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var c in customers)
                {
                    rowIndex = WriteCustomerRow(c, rowIndex, productName);
                }
            }

            /// <summary>
            /// Writes or advances one customer row. Derived writers override this extension point
            /// to emit customer-specific columns and should return the next row index.
            /// </summary>
            /// <param name="c">The customer being written.</param>
            /// <param name="rowIndex">The current row index.</param>
            /// <param name="productName">The product name associated with the search.</param>
            /// <returns>The row index to use for the next customer.</returns>
            protected virtual int WriteCustomerRow(CustomerAccount c, int rowIndex, string productName)
            {
                return ++rowIndex;
            }

            /// <summary>
            /// Writes the sheet header. The base implementation intentionally emits no columns;
            /// concrete writers define their own export headings.
            /// </summary>
            protected virtual void WriteHeader()
            {
            }
        }

        internal class MainSheetWriter : BaseSheetWriter
        {
            /// <summary>Initializes the customer main-sheet writer.</summary>
            /// <param name="writer">The workbook writer.</param>
            /// <param name="hccApp">The current Hotcakes application context.</param>
            internal MainSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            /// <summary>
            /// Creates the header row defining the customer export columns and their compatibility
            /// column names.
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
            /// Writes one row containing the customer's identifier, product name, contact details,
            /// and billing address. Values are taken directly from the customer record and are not
            /// normalized by this writer.
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
