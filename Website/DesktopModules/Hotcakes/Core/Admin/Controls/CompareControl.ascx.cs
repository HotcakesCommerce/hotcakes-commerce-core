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
using System.Web.UI;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class CompareControl : UserControl
    {
        public CompareCriteria<decimal> GetCompareCriteria()
        {
            decimal val = 0;
            decimal.TryParse(txtValue.Text, out val);
            var res = new CompareCriteria<decimal> {Value = val};

            switch (ddlCompareType.SelectedValue)
            {
                case "G":
                    res.Operator = CompareCriteria<decimal>.CompareOperator.GreaterThan;
                    break;
                case "L":
                    res.Operator = CompareCriteria<decimal>.CompareOperator.LessThan;
                    break;
                case "E":
                    res.Operator = CompareCriteria<decimal>.CompareOperator.Equal;
                    break;
                default:
                    res.Operator = CompareCriteria<decimal>.CompareOperator.Any;
                    break;
            }

            return res;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            txtValue.Attributes["style"] = ddlCompareType.SelectedIndex == 0 ? "display: none" : string.Empty;
        }
    }
}