#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductChoices : BaseProductPage
    {
        private Product localProduct = new Product();

        private string productBvin = string.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = Localization.GetString("PageTitle");
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
                ChoiceTypes.Items.Add(CreateDisableListItem(string.Format(" {0} ", Localization.GetString("SharedChoices"))));
                ChoiceTypes.Items.Add(CreateDisableListItem("------------------"));

                foreach (var opt in items)
                {
                    ChoiceTypes.Items.Add(new ListItem(opt.Name, opt.Bvin));
                }
            }
        }

        private ListItem CreateDisableListItem(string text)
        {
            var item = new ListItem(text, string.Empty);
            item.Attributes["disabled"] = "disabled";
            item.Enabled = false;
            item.Attributes["style"] = "background-color: #a9a9a9 !important;";
            return item;
        }

        private void LoadItems()
        {
            RenderItems(localProduct.Options);
        }

        private void RenderItems(List<Option> items)
        {
            var sb = new StringBuilder();

            if (items != null && items.Count > 0)
            {
                sb.Append("<div id=\"dragitem-list\">");
                foreach (var opt in items)
                {
                    RenderSingleItem(sb, opt);
                }

                sb.Append("</div>");
            }
            else
            {
                sb.AppendFormat("<div class=\"hcClearfix\"><p>{0}</p></div>", Localization.GetString("NoOptions"));
            }

            litResults.Text = sb.ToString();
        }

        private void RenderSingleItem(StringBuilder sb, Option o)
        {
            var destinationLink = "ProductChoices_Edit.aspx?cid=" + o.Bvin + "&id=" + productBvin;

            sb.AppendFormat("<div class=\"dragitem\" id=\"item_{0}\"><table class=\"formtable hcGrid\" width=\"100%\"><tbody class=\"ui-sortable\"><tr>", o.Bvin);
            sb.AppendFormat("<td width=\"30\" class=\"handle\"><a href=\"#\" class=\"hcIconMove\" title=\"{0}\"></a></td>", Localization.GetString("Move"));
            sb.AppendFormat("<td width=\"25%\"><a href=\"{0}\">", destinationLink);
            sb.Append(o.Name);
            sb.Append("</a></td>");
            sb.Append("<td class=\"hcProductEditChoice\">");
            sb.Append(o.Render());
            sb.Append("</td>");

            sb.Append("<td width=\"75\">");
            if (o.IsVariant)
            {
                sb.Append(Localization.GetString("VARIANT"));
            }
            sb.Append("</td> <td width=\"105\">");

            if (o.IsShared)
            {
                sb.Append(Localization.GetString("SHARED"));
            }
            else
            {
                sb.AppendFormat("<a href=\"{0}\" class=\"hcIconEdit\" title=\"{1}\"></a> ", destinationLink, Localization.GetString("Edit"));
            }

            sb.AppendFormat("<a href=\"#\" class=\"trash hcIconDelete\" id=\"rem{0}\" title=\"{1}\"", o.Bvin, Localization.GetString("Delete"));

            sb.AppendFormat("></a></td>");

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
            opt.Name = Localization.GetString("NewChoice");

            switch (opt.OptionType)
            {
                case OptionTypes.CheckBoxes:
                    opt.Name = Localization.GetString("NewCheckboxes");
                    break;
                case OptionTypes.DropDownList:
                    opt.Name = Localization.GetString("NewDropdownList");
                    break;
                case OptionTypes.FileUpload:
                    opt.Name = Localization.GetString("NewFileUpload");
                    break;
                case OptionTypes.Html:
                    opt.Name = Localization.GetString("NewHtmlDescription");
                    break;
                case OptionTypes.RadioButtonList:
                    opt.Name = Localization.GetString("NewRadioButtonList");
                    break;
                case OptionTypes.TextInput:
                    opt.Name = Localization.GetString("NewTextInput");
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
                    MessageBox1.ShowError(Localization.GetString("CreateChoiceFailure"));
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