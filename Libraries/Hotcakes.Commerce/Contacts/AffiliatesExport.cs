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

using System.Collections.Generic;
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Contacts
{
    public class AffiliatesExport
    {
        public void ExportToExcel(HttpResponse response, string fileName, List<AffiliateReportData> affiliates)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new MainSheetWriter(writer);
            mainWriter.Write(affiliates);

            writer.Save();
            writer.WriteToResponse(response, fileName);
        }

        internal class MainSheetWriter
        {
            protected int _firstRow;
            protected SpreadsheetStyle _headerStyle;
            protected SpreadsheetStyle _rowStyle;
            protected ExcelWriter _writer;

            internal MainSheetWriter(ExcelWriter writer)
            {
                _writer = writer;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();
                _headerStyle.IsBold = true;
            }

            public void Write(List<AffiliateReportData> reportRows)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var o in reportRows)
                {
                    rowIndex = WriteAffiliatReporteRow(o, rowIndex);
                }
            }

            private void WriteHeader()
            {
                var centerStyle = _writer.GetStyle();
                centerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                _writer.WriteRow("A", 1, new List<string>
                {
                    "First Name",
                    "Last Name",
                    "ID",
                    "Company",
                    "Email",
                    "UserId",
                    "Sales",
                    "Orders",
                    "Commission",
                    "Owed",
                    "Signups"
                }, _headerStyle);

                _firstRow = 2;
            }

            private int WriteAffiliatReporteRow(AffiliateReportData ar, int rowIndex)
            {
                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    ar.FirstName,
                    ar.LastName,
                    ar.AffiliateId,
                    ar.Company,
                    ar.Email,
                    ar.UserId.ToString(),
                    GetCurrency(ar.SalesAmount),
                    ar.OrdersCount.ToString(),
                    GetCurrency(ar.Commission),
                    GetCurrency(ar.CommissionOwed),
                    ar.SignupsCount.ToString()
                }, _rowStyle);

                return ++rowIndex;
            }

            protected string GetCurrency(decimal val)
            {
                return val.ToString("C");
            }
        }
    }
}