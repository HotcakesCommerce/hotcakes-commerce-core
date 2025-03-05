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
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.Settings;

namespace Hotcakes.Modules.CategoryViewer
{
    public partial class Settings : HotcakesSettingsBase
    {
        #region ModuleSettingsBase overrides

        public override void LoadSettings()
        {
            if (!IsPostBack)
            {
                FillForm();

                var slugText = Convert.ToString(ModuleSettings["Slug"]);
                var viewText = Convert.ToString(ModuleSettings["View"]);
                var productPageSize = Convert.ToString(ModuleSettings["ProductPageSize"]);

                ViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(viewText))
                {
                    ViewComboBox.SelectedValue = viewText;
                    ViewContentLabel.Text = viewText;
                }

                CategoryContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(slugText))
                {
                    var categoryText = LookupCategory(slugText);
                    CategoryContentLabel.Text = categoryText;

                    // check to see if there is a category in the DDL that matches the settings
                    if (CategoryComboBox.Items.FindByText(categoryText) != null)
                    {
                        // select the saved category if it exists
                        CategoryComboBox.Items.FindByText(categoryText).Selected = true;
                    }
                    else
                    {
                        // set no category if it doesn't exist
                        CategoryComboBox.SelectedIndex = 0;
                    }
                }
                ProductPageSizeTextBox.Text = productPageSize;
                ShowManufacturersCheckBox.Checked = Convert.ToBoolean(ModuleSettings["ShowManufactures"] ?? "false");
                ShowVendorsCheckBox.Checked = Convert.ToBoolean(ModuleSettings["ShowVendors"] ?? "false");
            }
        }

        public override void UpdateSettings()
        {
            var categorySettings = new CategoryModuleSettings(ModuleId);
            var controller = new ModuleController();
            controller.UpdateModuleSetting(ModuleId, "Slug", CategoryComboBox.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "View", ViewComboBox.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "ProductPageSize", ProductPageSizeTextBox.Text);
            controller.UpdateModuleSetting(ModuleId, "DefaultPreContentColumnId", ddlPreContentColumnId.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "DefaultPostContentColumnId", ddlPostContentColumnId.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "ShowManufactures", ShowManufacturersCheckBox.Checked.ToString());
            controller.UpdateModuleSetting(ModuleId, "ShowVendors", ShowVendorsCheckBox.Checked.ToString());

            var qOptions = SortingOptionsCheckBoxList.Items
                .OfType<ListItem>()
                .Where(i => i.Selected)
                .Select(i => (CategorySortOrder) Convert.ToInt32(i.Value));

            categorySettings.SortOrderOptions = qOptions.ToList();
        }

        #endregion

        #region Implementation

        /// <summary>
        ///     Looks up the category by its slug and returns the category name, if found
        /// </summary>
        /// <param name="slugText"></param>
        /// <returns></returns>
        private string LookupCategory(string slugText)
        {
            var categoryText = string.Empty;
            if (!string.IsNullOrEmpty(slugText))
            {
                CustomUrl customUrl;
                var category = HccApp.ParseCategoryBySlug(slugText, out customUrl);
                if (category != null)
                {
                    categoryText = category.Name;
                }
            }
            return categoryText;
        }

        /// <summary>
        ///     Fill dropdown lists
        /// </summary>
        private void FillForm()
        {
            ViewComboBox.Items.Add(new ListItem(LocalizeString("NoneSelectedText"), string.Empty));
            ViewComboBox.AppendDataBoundItems = true;
            ViewComboBox.DataSource = DnnPathHelper.GetViewNames("Category");
            ViewComboBox.DataBind();

            var allCats = HccApp.CatalogServices.Categories.FindAllSnapshotsPaged(1, 5000);
            var categories = CategoriesHelper.ListFullTreeWithIndentsForComboBox(allCats, false);

            CategoryComboBox.Items.Add(new ListItem(LocalizeString("NoneSelectedText"), string.Empty));
            CategoryComboBox.AppendDataBoundItems = true;
            CategoryComboBox.DataSource = categories;
            CategoryComboBox.DataTextField = "Text";
            CategoryComboBox.DataValueField = "Value";
            CategoryComboBox.DataBind();
            
            var columns = HccApp.ContentServices.Columns.FindAll();

            FillComboBox(ddlPreContentColumnId, columns, "DisplayName", "Bvin",
                Convert.ToString(ModuleSettings["DefaultPreContentColumnId"]));
            FillComboBox(ddlPostContentColumnId, columns, "DisplayName", "Bvin",
                Convert.ToString(ModuleSettings["DefaultPostContentColumnId"]));

            FillSortingCheckboxList();
        }


        private void FillSortingCheckboxList()
        {
            var categorySettings = new CategoryModuleSettings(ModuleId);
            var sortOptions = categorySettings.SortOrderOptions;
            var sortOrders = HccApp.CatalogServices.GetCategorySortOrderList();

            foreach (var sortOrder in sortOrders)
            {
                SortingOptionsCheckBoxList.Items.Add(new ListItem
                {
                    Value = ((int) sortOrder).ToString(),
                    Text = LocalizeString("SortOrder_" + sortOrder),
                    Selected = sortOptions.Contains(sortOrder)
                });
            }
        }

        private void FillComboBox(DropDownList comboBox, object dataSource, string textField, string valueField,
            string selectedValue)
        {
            comboBox.Items.Clear();
            comboBox.DataSource = dataSource;
            comboBox.DataTextField = textField;
            comboBox.DataValueField = valueField;
            comboBox.DataBind();
            comboBox.Items.Insert(0, new ListItem());
            var selectedItem = comboBox.Items.FindByValue(selectedValue) ??
                               comboBox.Items.FindByText(selectedValue);
            if (selectedItem != null)
                selectedItem.Selected = true;
        }

        #endregion
    }
}