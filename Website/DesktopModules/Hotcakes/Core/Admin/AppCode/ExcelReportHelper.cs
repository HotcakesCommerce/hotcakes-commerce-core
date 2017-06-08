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
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public static class ExcelReportHelper
    {
        public static SpreadsheetStyle GetHeadingStyle(this ExcelWriter w)
        {
            var style = w.GetStyle();
            var font = style.ToFont();
            font.SetFontSize(14);
            style.AddFont(font);
            style.IsBold = true;
            return style;
        }

        public static SpreadsheetStyle GetHeaderStyle(this ExcelWriter w)
        {
            var style = w.GetStyle();
            style.SetHorizontalAlignment(HorizontalAlignmentValues.Right);
            style.SetColor("FFFFFF");
            style.SetBackgroundColor("797C7C");
            style.IsBold = true;
            return style;
        }

        public static SpreadsheetStyle GetRowStyle(this ExcelWriter w)
        {
            var style = w.GetStyle();
            style.SetHorizontalAlignment(HorizontalAlignmentValues.Right);
            return style;
        }

        public static SpreadsheetStyle GetTotalStyle(this ExcelWriter w)
        {
            var style = w.GetStyle();
            style.SetHorizontalAlignment(HorizontalAlignmentValues.Right);
            style.IsBold = true;
            return style;
        }

        public static void SetFontSize(this Font font, double size)
        {
            font.FontSize = new FontSize {Val = size};
        }
    }
}