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
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;

namespace Hotcakes.Modules.CategoryMenu
{
    public partial class Settings : HotcakesSettingsBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ModeField.SelectedIndexChanged += ModeField_SelectedIndexChanged;
            btnSelectCategory.Click += btnSelectCategory_OnClick;
            btnRemoveCategory.Click += btnRemoveCategory_OnClick;
        }

        public override void UpdateSettings()
        {
            SaveData();
        }

        public override void LoadSettings()
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            ViewComboBox.Items.Add(new ListItem(LocalizeString("NoneSelectedText"), string.Empty));
            ViewComboBox.AppendDataBoundItems = true;
            ViewComboBox.DataSource = DnnPathHelper.GetViewNames("CategoryMenu");
            ViewComboBox.DataBind();

            var viewText = Convert.ToString(ModuleSettings["View"]);

            ViewContentLabel.Text = LocalizeString("NoneSelectedText");
            if (!string.IsNullOrEmpty(viewText))
            {
                ViewComboBox.SelectedValue = viewText;
                ViewContentLabel.Text = viewText;
            }

            TitleField.Text = Convert.ToString(ModuleSettings["Title"]);

            ModeField.Items.Clear();
            ModeField.Items.Add(new ListItem(LocalizeString("ModeField_ShowRootOnly"), "0"));
            ModeField.Items.Add(new ListItem(LocalizeString("ModeField_ShowAll"), "1"));
            ModeField.Items.Add(new ListItem(LocalizeString("ModeField_ShowChildrenPeerParents"), "2"));
            ModeField.Items.Add(new ListItem(LocalizeString("ModeField_ShowAllParent"), "3"));
            ModeField.Items.Add(new ListItem(LocalizeString("ModeField_ShowSelected"), "4"));
            ModeField.Items.Add(new ListItem(LocalizeString("ModeField_ShowChildrenSelected"), "5"));

            var mode = ModuleSettings["CategoryMenuMode"] != null ? ModuleSettings["CategoryMenuMode"].ToString() : "0";
            if (ModeField.Items.FindByValue(mode) != null)
            {
                ModeField.Items.FindByValue(mode).Selected = true;
            }

            ProductCountCheckBox.Checked = Convert.ToBoolean(ModuleSettings["ShowProductCount"]);
            SubCategoryCountCheckBox.Checked = Convert.ToBoolean(ModuleSettings["ShowCategoryCount"]);
            HomeLinkField.Checked = Convert.ToBoolean(ModuleSettings["HomeLink"]);
            MaximumDepth.Text = Convert.ToString(ModuleSettings["MaximumDepth"]);
            var listCategories = HccApp.CatalogServices.Categories.FindAll();
            var selectedCategories = Convert.ToString(ModuleSettings["SelectedCategories"])
                .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cat in listCategories)
            {
                ddlParentCategories.Items.Add(new ListItem(cat.Name, cat.Bvin));

                if (selectedCategories.Contains(cat.Bvin))
                {
                    lstSelectedCategories.Items.Add(new ListItem(cat.Name, cat.Bvin));
                }
                else
                {
                    lstCategories.Items.Add(new ListItem(cat.Name, cat.Bvin));
                }
            }
            ddlParentCategories.SelectedValue = Convert.ToString(ModuleSettings["ChildrenOfCategory"]);
            AdjustShowingFields();
        }

        private void SaveData()
        {
            var controller = new ModuleController();
            controller.UpdateModuleSetting(ModuleId, "View", ViewComboBox.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "Title", TitleField.Text.Trim());
            var mode = ModeField.SelectedValue ?? "0";
            controller.UpdateModuleSetting(ModuleId, "CategoryMenuMode", mode);
            switch (mode)
            {
                case "4":
                    var selectedCategories = new HashSet<string>();
                    foreach (ListItem item in lstSelectedCategories.Items)
                    {
                        if (!selectedCategories.Contains(item.Value))
                            selectedCategories.Add(item.Value);
                    }
                    controller.UpdateModuleSetting(ModuleId, "SelectedCategories", string.Join(",", selectedCategories));
                    break;
                case "5":
                    controller.UpdateModuleSetting(ModuleId, "ChildrenOfCategory", ddlParentCategories.SelectedValue);
                    break;
            }
            controller.UpdateModuleSetting(ModuleId, "ShowProductCount", ProductCountCheckBox.Checked.ToString());
            controller.UpdateModuleSetting(ModuleId, "ShowCategoryCount", SubCategoryCountCheckBox.Checked.ToString());
            controller.UpdateModuleSetting(ModuleId, "HomeLink", HomeLinkField.Checked.ToString());
            int maxDepth;
            controller.UpdateModuleSetting(ModuleId, "MaximumDepth",
                int.TryParse(MaximumDepth.Text.Trim(), out maxDepth) ? maxDepth.ToString() : "0");
        }

        protected void ModeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustShowingFields();
        }

        protected void btnSelectCategory_OnClick(object sender, EventArgs e)
        {
            MoveItems(lstCategories, lstSelectedCategories);
        }

        protected void btnRemoveCategory_OnClick(object sender, EventArgs e)
        {
            MoveItems(lstSelectedCategories, lstCategories);
        }

        private void MoveItems(ListBox fromList, ListBox toList)
        {
            for (var i = fromList.Items.Count - 1; i >= 0; i--)
            {
                var item = fromList.Items[i];
                if (item.Selected)
                {
                    toList.Items.Add(new ListItem(item.Text, item.Value));
                    fromList.Items.Remove(item);
                }
            }
        }

        private void AdjustShowingFields()
        {
            switch (ModeField.SelectedValue)
            {
                case "4":
                    pnlSelectedCategories.Visible = true;
                    pnlSelectedChildren.Visible = false;
                    break;
                case "5":
                    pnlSelectedChildren.Visible = true;
                    pnlSelectedCategories.Visible = false;
                    break;
                default:
                    pnlSelectedChildren.Visible = false;
                    pnlSelectedCategories.Visible = false;
                    break;
            }
        }
    }
}