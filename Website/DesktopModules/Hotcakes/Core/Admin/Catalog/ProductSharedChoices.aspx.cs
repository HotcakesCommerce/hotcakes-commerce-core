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
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductSharedChoices : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Shared Product Choices";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadItems();
            }
        }

        private void LoadItems()
        {
            var options = HccApp.CatalogServices.ProductOptions.FindAllShared(1, 1000);
            RenderItems(options);
        }

        private void RenderItems(List<Option> items)
        {
            var sb = new StringBuilder();

            var isAlternate = false;
            sb.Append("<table border=\"0\" class=\"formtable hcGrid\">");
            foreach (var opt in items)
            {
                RenderSingleItem(sb, opt, isAlternate);
                isAlternate = !isAlternate;
            }
            sb.Append("</table>");
            litResults.Text = sb.ToString();
        }

        private void RenderSingleItem(StringBuilder sb, Option o, bool isAlternate)
        {
            var destinationLink = "ProductSharedChoices_Edit.aspx?id=" + o.Bvin;

            sb.Append("<tr id=\"" + o.Bvin + "\"");
            if (isAlternate)
            {
                sb.Append(" class=\"alternaterow-padded\"");
            }
            else
            {
                sb.Append(" class=\"row-padded\"");
            }
            sb.Append("><td><a href=\"" + destinationLink + "\">");
            sb.Append(o.Name);
            sb.Append("</a></td>");
            sb.Append("<td class=\"hcProductEditChoice\">");
            sb.Append(o.Render());
            sb.Append("</td>");
            sb.Append("<td width='5%'><a href=\"" + destinationLink + "\">");
            sb.Append("<img class=\"hcIconEdit\" alt=\"Edit\" />");
            sb.Append("</a></td>");
            sb.Append("<td width='5%'>");
            sb.Append("<a id=\"rem" + o.Bvin +
                      "\"  class=\"trash\" href=\"ProductSharedChoices_Delete.aspx?redirect=y&id=");
            sb.Append(o.Bvin);
            sb.Append("\" >");
            sb.Append("<img alt=\"delete\" class=\"hcIconDelete\" />");
            sb.Append("</a></td>");
            sb.Append("</tr>");
        }

        protected void NewSharedChoiceImageButton_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();

            var opt = Option.Factory(OptionTypes.Html);

            var typeCode = 100;
            if (int.TryParse(SharedChoiceTypes.SelectedItem.Value, out typeCode))
            {
                opt.SetProcessor((OptionTypes) typeCode);
            }

            opt.IsShared = true;
            opt.IsVariant = false;
            opt.Name = "New Choice";

            switch (opt.OptionType)
            {
                case OptionTypes.CheckBoxes:
                    opt.Name = "New Checkboxes";
                    break;
                case OptionTypes.DropDownList:
                    opt.Name = "New Drop Down List";
                    break;
                case OptionTypes.FileUpload:
                    opt.Name = "New File Upload";
                    break;
                case OptionTypes.Html:
                    opt.Name = "New Html Description";
                    break;
                case OptionTypes.RadioButtonList:
                    opt.Name = "New Radio Button List";
                    break;
                case OptionTypes.TextInput:
                    opt.Name = "New Text Input";
                    break;
            }
            opt.StoreId = HccApp.CurrentStore.Id;

            if (HccApp.CatalogServices.ProductOptions.Create(opt))
            {
                Response.Redirect("ProductSharedChoices_Edit.aspx?id=" + opt.Bvin);
            }
            else
            {
                MessageBox1.ShowError("Unable to create choice. An Administrator has been alerted to the issue.");
            }
        }
    }
}