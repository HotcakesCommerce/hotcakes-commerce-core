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

using System;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class HccShippingPart : HccPart
    {
        public ShippingMethod ShippingMethod { get; set; }

        public void AddHighlightColors(DropDownList lst)
        {
            var liNone = new ListItem("- None -", string.Empty);
            lst.CssClass = "hcOrderHighlight";
            lst.Items.Add(liNone);

            var liBlue = new ListItem("Blue", "Blue");
            liBlue.Attributes.Add("class", "hcBlue");
            lst.Items.Add(liBlue);

            var liYellow = new ListItem("Yellow", "Yellow");
            liYellow.Attributes.Add("class", "hcYellow");
            lst.Items.Add(liYellow);

            var liLime = new ListItem("Lime", "Lime");
            liLime.Attributes.Add("class", "hcLime");
            lst.Items.Add(liLime);

            var liOrange = new ListItem("Orange", "Orange");
            liOrange.Attributes.Add("class", "hcOrange");
            lst.Items.Add(liOrange);

            var liPurple = new ListItem("Purple", "Purple");
            liPurple.Attributes.Add("class", "hcPurple");
            lst.Items.Add(liPurple);

            var liTan = new ListItem("Tan", "Tan");
            liTan.Attributes.Add("class", "hcTan");
            lst.Items.Add(liTan);
        }
    }
}