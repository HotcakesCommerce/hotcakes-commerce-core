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
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Catalog
{
    public class GiftCardExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        public GiftCardExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        /// <summary>
        ///     Export the gift card to excel.
        /// </summary>
        /// <param name="response">Response stream on which excel file will be writted</param>
        /// <param name="fileName">File name which exported</param>
        /// <param name="giftCards">List of <see cref="GiftCard" /> instances</param>
        public void ExportToExcel(HttpResponse response, string fileName, List<GiftCard> giftCards)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new MainSheetWriter(writer, _hccApp);
            mainWriter.Write(giftCards);

            writer.Save();
            writer.WriteToResponse(response, fileName);
        }


        internal class MainSheetWriter
        {
            protected int _firstRow;
            private readonly HotcakesApplication _hccApp;
            protected SpreadsheetStyle _headerStyle;
            protected SpreadsheetStyle _rowStyle;
            protected ExcelWriter _writer;

            internal MainSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();
                _headerStyle.IsBold = true;
                _hccApp = hccApp;
            }

            /// <summary>
            ///     Write <see cref="GiftCard" /> to file.
            /// </summary>
            /// <param name="cards">List of cards</param>
            public void Write(List<GiftCard> cards)
            {
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var gc in cards)
                {
                    rowIndex = WriteRow(gc, rowIndex);
                }
            }

            /// <summary>
            ///     Write excel file header.
            /// </summary>
            private void WriteHeader()
            {
                var centerStyle = _writer.GetStyle();
                centerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                _writer.WriteRow("A", 1, new List<string>
                {
                    "Issue Date",
                    "Expiration Date",
                    "Recipient Name",
                    "Recipient Email",
                    "Card Number",
                    "Amount",
                    "Balance",
                    "Enabled"
                }, _headerStyle);

                _firstRow = 2;
            }

            /// <summary>
            ///     Write individual GiftCard to the file.
            /// </summary>
            /// <param name="gc"><see cref="GiftCard" /> instance</param>
            /// <param name="rowIndex">Row index on which this record going to be written on excel file</param>
            /// <returns>Returns row index</returns>
            private int WriteRow(GiftCard gc, int rowIndex)
            {
                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    GetDateTimeString(gc.IssueDateUtc),
                    GetDateTimeString(gc.ExpirationDateUtc),
                    gc.RecipientName,
                    gc.RecipientEmail,
                    gc.CardNumber,
                    gc.Amount.ToString(),
                    (gc.Amount - gc.UsedAmount).ToString(),
                    GetYesNo(gc.Enabled)
                }, _rowStyle);

                return ++rowIndex;
            }

            /// <summary>
            ///     Returns Yes or No based on gift card enabled or not.
            /// </summary>
            /// <param name="val">Value of the Gift Card enabled</param>
            /// <returns>Returns string presentation of the Gift Card enabled</returns>
            protected string GetYesNo(bool? val)
            {
                if (val.HasValue)
                    return val.Value ? "YES" : "NO";
                return string.Empty;
            }

            /// <summary>
            ///     Get datetime string
            /// </summary>
            /// <param name="utcTime">Datetime instance</param>
            /// <returns>Returns the datetime string</returns>
            protected string GetDateTimeString(DateTime utcTime)
            {
                return DateHelper.ConvertUtcToStoreTime(_hccApp, utcTime).ToShortDateString();
            }
        }
    }
}