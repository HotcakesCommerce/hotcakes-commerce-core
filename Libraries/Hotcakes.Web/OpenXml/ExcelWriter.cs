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
using System.IO;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Hotcakes.Web.OpenXml
{
    /// <summary>Creates and writes Open XML spreadsheets.</summary>
    [Serializable]
    public class ExcelWriter
    {
        private readonly SpreadsheetDocument _doc;
        private readonly MemoryStream _stream;
        private WorksheetWriter _writer;

        /// <summary>Initializes a workbook with a default worksheet.</summary>
        public ExcelWriter()
        {
            _stream = SpreadsheetReader.Create();
            _doc = SpreadsheetDocument.Open(_stream, true);
            var worksheetPart = SpreadsheetReader.GetWorksheetPartByName(_doc, "Sheet1");
            _writer = new WorksheetWriter(_doc, worksheetPart);
        }

        /// <summary>Initializes a workbook with one named worksheet.</summary>
        /// <param name="firstSheetName">The name of the first worksheet.</param>
        public ExcelWriter(string firstSheetName) : this()
        {
            var sheet = _doc.WorkbookPart.Workbook.Descendants<Sheet>().First(s => s.Name == "Sheet1");
            if (sheet != null)
                sheet.Name = firstSheetName;

            SpreadsheetWriter.RemoveWorksheet(_doc, "Sheet2");
            SpreadsheetWriter.RemoveWorksheet(_doc, "Sheet3");
        }

        /// <summary>Sets the widths of columns beginning with column A.</summary>
        /// <param name="colWidths">The column widths in order.</param>
        public void SetColumnsWidths(double[] colWidths)
        {
            var colName = "A";
            foreach (var width in colWidths)
            {
                _writer.SetColumnWidth(colName, width);
                colName = SpreadsheetReader.GetColumnName(colName, 1);
            }
        }

        /// <summary>Adds and selects a worksheet.</summary>
        /// <param name="sheetName">The worksheet name.</param>
        public void AddWorksheet(string sheetName)
        {
            var wPart = SpreadsheetWriter.InsertWorksheet(_doc, sheetName);
            _writer = new WorksheetWriter(_doc, wPart);
        }

        /// <summary>Gets the default spreadsheet style for this workbook.</summary>
        /// <returns>The default <see cref="SpreadsheetStyle"/>.</returns>
        public SpreadsheetStyle GetStyle()
        {
            return SpreadsheetStyle.GetDefault(_doc);
        }

        /// <summary>Writes a row of string values and optionally applies a style.</summary>
        /// <param name="column">The starting column.</param><param name="row">The row number.</param><param name="values">The values to write.</param><param name="style">The optional style.</param>
        public void WriteRow(string column, int row, List<string> values, SpreadsheetStyle style = null)
        {
            var startCell = column + row;
            _writer.PasteValues(startCell, values, CellValues.String);

            if (style != null)
            {
                var endCell = SpreadsheetReader.GetColumnName(column, values.Count - 1) + row;
                _writer.SetStyle(style, startCell, endCell);
            }
        }

        /// <summary>Writes and optionally styles a cell, then merges the requested span.</summary>
        /// <param name="column">The starting column.</param><param name="row">The row number.</param><param name="value">The value to write.</param><param name="style">The optional style.</param><param name="colSpan">The number of columns to span.</param>
        public void WriteCellFormatted(string column, int row, string value, SpreadsheetStyle style = null,
            int colSpan = 0)
        {
            WriteCell(column, row, value, style);

            if (colSpan > 0)
            {
                var endCell = SpreadsheetReader.GetColumnName(column, colSpan - 1) + row;
                _writer.MergeCells(column + row, endCell);
            }
        }

        /// <summary>Writes a string value to a cell and optionally applies a style.</summary>
        /// <param name="column">The column.</param><param name="row">The row number.</param><param name="value">The value to write.</param><param name="style">The optional style.</param>
        public void WriteCell(string column, int row, string value, SpreadsheetStyle style = null)
        {
            var startCell = column + row;
            _writer.PasteValue(startCell, value, CellValues.String);

            if (style != null)
            {
                var endCell = column + row;
                _writer.SetStyle(style, startCell, endCell);
            }
        }

        /// <summary>Saves pending workbook changes.</summary>
        public void Save()
        {
            SpreadsheetWriter.Save(_doc);
        }

        /// <summary>Writes the workbook to an HTTP response as an Excel download.</summary>
        /// <param name="response">The response receiving the workbook.</param><param name="filename">The download filename.</param>
        public void WriteToResponse(HttpResponse response, string filename)
        {
            response.Clear();
            response.AddHeader("content-disposition", string.Format("attachment;filename={0}", filename));
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            _stream.WriteTo(response.OutputStream);
            response.End();
        }

        /// <summary>Writes the workbook stream to a file.</summary>
        /// <param name="path">The destination file path.</param>
        public void StreamToFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            AbstractWriter.StreamToFile(path, _stream);
        }

        /// <summary>Applies a style to a rectangular cell range.</summary>
        /// <param name="startCol">The starting column.</param><param name="startRow">The starting row.</param><param name="endCol">The ending column.</param><param name="endRow">The ending row.</param><param name="style">The style to apply.</param>
        public void SetStyle(string startCol, int startRow, string endCol, int endRow, SpreadsheetStyle style)
        {
            _writer.SetStyle(style, startCol + startRow, endCol + endRow);
        }
    }
}
