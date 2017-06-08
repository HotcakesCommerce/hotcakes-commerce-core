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
using System.Drawing;
using System.Text;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Logging;
using Telerik.Web.UI;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductChoices : BaseProductPage
    {
        private Product localProduct = new Product();

        private string productBvin = string.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit Product Choices";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Request.QueryString["id"] != null)
            {
                productBvin = Request.QueryString["id"];
                localProduct = HccApp.CatalogServices.Products.Find(productBvin);
            }

            if (!Page.IsPostBack)
            {
                PopulateSharedChoices();
                CurrentTab = AdminTabType.Catalog;
                LoadItems();
            }
        }


        private void PopulateSharedChoices()
        {
            var items = HccApp.CatalogServices.ProductOptions.FindAllShared(1, 1000);
            if (items.Count > 0)
            {
                ChoiceTypes.Items.Add(CreateDisableListItem("------------------"));
                ChoiceTypes.Items.Add(CreateDisableListItem(" Shared Choices "));
                ChoiceTypes.Items.Add(CreateDisableListItem("------------------"));

                foreach (var opt in items)
                {
                    ChoiceTypes.Items.Add(new RadComboBoxItem(opt.Name, opt.Bvin));
                }
            }
        }

        private RadComboBoxItem CreateDisableListItem(string text)
        {
            var item = new RadComboBoxItem(text, string.Empty);
            item.Attributes["disabled"] = "disabled";
            item.Enabled = false;
            item.BackColor = Color.DarkGray;
            return item;
        }

        private void LoadItems()
        {
            RenderItems(localProduct.Options);
        }

        private void RenderItems(List<Option> items)
        {
            var sb = new StringBuilder();

            sb.Append("<div id=\"dragitem-list\">");
            foreach (var opt in items)
            {
                RenderSingleItem(sb, opt);
            }
            sb.Append("</div>");
            litResults.Text = sb.ToString();
        }

        private void RenderSingleItem(StringBuilder sb, Option o)
        {
            var destinationLink = "ProductChoices_Edit.aspx?cid=" + o.Bvin + "&id=" + productBvin;

            sb.Append("<div class=\"dragitem\" id=\"item_" + o.Bvin +
                      "\"><table class=\"formtable hcGrid\" width=\"100%\"><tbody class=\"ui-sortable\"><tr>");
            sb.AppendFormat(
                "<td width=\"30\"><a href=\"#\" class=\"handle\"><img class=\"hcIconMove\"  alt=\"Move\" /></a></td>");
            sb.Append("<td width=\"25%\"><a href=\"" + destinationLink + "\">");
            sb.Append(o.Name);
            sb.Append("</a></td>");
            sb.Append("<td class=\"hcProductEditChoice\">");
            sb.Append(o.Render());
            sb.Append("</td>");

            sb.Append("<td width=\"75\">");
            if (o.IsVariant)
            {
                sb.Append("VARIANT");
            }
            sb.Append("</td> <td width=\"105\">");

            if (o.IsShared)
            {
                sb.Append("SHARED");
            }
            else
            {
                sb.Append("<a href=\"" + destinationLink + "\"><img class=\"hcIconEdit\" alt=\"edit\" /></a>");
            }

            sb.Append("<a href=\"#\" class=\"trash\" id=\"rem" + o.Bvin + "\"");
            if (o.IsVariant)
            {
                sb.Append("  title=\"variant\" ");
            }
            sb.AppendFormat("><img  alt=\"Delete\" class=\"hcIconDelete\" /></a></td>");


            sb.Append("</tr></tbody></table></div>");
        }

        protected void NewChoiceButton_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();

            var opt = new Option();
            opt.SetProcessor(OptionTypes.DropDownList);

            var typeCode = 100;
            if (int.TryParse(ChoiceTypes.SelectedItem.Value, out typeCode))
            {
                opt.SetProcessor((OptionTypes) typeCode);
            }

            opt.IsShared = false;
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

            // Trick the new option to be an already existing option if shared
            if (ChoiceTypes.SelectedItem.Value.Trim().Length > 30)
            {
                opt.Bvin = ChoiceTypes.SelectedItem.Value;
                opt.IsShared = true;
                opt.Name = ChoiceTypes.SelectedItem.Text;
            }

            // Create Choice
            var created = false;

            if (opt.IsShared)
            {
                created = true;
            }
            else
            {
                localProduct.Options.Add(opt);
                HccApp.CatalogServices.Products.Update(localProduct);
                created = true;
            }

            // Associate Choice with Product
            if (created)
            {
                if (localProduct != null)
                {
                    HccApp.CatalogServices.ProductsAddOption(localProduct, opt.Bvin);
                    if (!opt.IsShared)
                    {
                        Response.Redirect("ProductChoices_Edit.aspx?cid=" + opt.Bvin + "&id=" + localProduct.Bvin);
                    }
                }
                else
                {
                    MessageBox1.ShowError(
                        "Unable to associate choice with product. An Administrator has been alerted to the issue.");
                    EventLog.LogEvent("ProductChoices.aspx",
                        "Could not associate choice " + opt.Bvin + " with product " + productBvin,
                        EventLogSeverity.Error);
                }
            }

            HccApp.CatalogServices.ProductsReloadOptions(localProduct);
            LoadItems();
        }
    }
}