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
using System.Linq;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Hotcakes.Web.OpenXml
{
    [Serializable]
    public class ExcelReader
    {
        private readonly SpreadsheetDocument _doc;
        private WorksheetPart _worksheet;

        public ExcelReader(string filename)
        {
            var stream = AbstractReader.Copy(filename);
            _doc = SpreadsheetDocument.Open(stream, false);
        }

        public bool SetWorksheet(string sheetName)
        {
            _worksheet = SpreadsheetReader.GetWorksheetPartByName(_doc, sheetName);
            return _worksheet != null;
        }

        public int GetRowCount(string sheetName)
        {
            var wp = SpreadsheetReader.GetWorksheetPartByName(_doc, sheetName);
            var worksheet = wp.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            return sheetData.Elements<Row>().Count();
        }

        public Row GetRow(uint rowIndex)
        {
            if (_worksheet == null)
                throw new InvalidOperationException("Please choose worksheet by calling SetWorksheet() method");

            var sheetData = _worksheet.Worksheet.GetFirstChild<SheetData>();

            return WorksheetReader.GetRow(sheetData, rowIndex);
        }

        public string GetCellValue(Row row, string columnName)
        {
            if (_worksheet == null)
                throw new InvalidOperationException("Please choose worksheet by calling SetWorksheet() method");

            if (row == null)
                throw new ArgumentNullException("row");

            var cellReference = columnName + row.RowIndex;

            var cells = row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference);
            if (cells.Any())
            {
                return GetCellValue(cells.First());
            }

            return null;
        }

        private string GetCellValue(Cell theCell)
        {
            string value = null;


            // If the cell does not exist, return an empty string.
            if (theCell != null)
            {
                value = theCell.InnerText;

                // If the cell represents an integer number, you are done. 
                // For dates, this code returns the serialized value that 
                // represents the date. The code handles strings and 
                // Booleans individually. For shared strings, the code 
                // looks up the corresponding value in the shared string 
                // table. For Booleans, the code converts the value into 
                // the words TRUE or FALSE.
                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:

                            // For shared strings, look up the value in the
                            // shared strings table.
                            var stringTable =
                                _doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                            // If the shared string table is missing, something 
                            // is wrong. Return the index that is in
                            // the cell. Otherwise, look up the correct text in 
                            // the table.
                            if (stringTable != null)
                            {
                                value =
                                    stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                            }
                            break;

                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
            }

            return value;
        }
    }
}