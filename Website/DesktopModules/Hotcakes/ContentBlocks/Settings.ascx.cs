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

using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;
using Hotcakes.Modules.Core.Settings;
using Telerik.Web.UI;

namespace Hotcakes.Modules.ContentBlocks
{
    public partial class Settings : HotcakesSettingsBase
    {
        #region Implementation

        /// <summary>
        ///     Fills the form.
        /// </summary>
        private void FillForm()
        {
            ViewComboBox.Items.Add(new RadComboBoxItem(LocalizeString("NoneSelectedText"), string.Empty));
            ViewComboBox.AppendDataBoundItems = true;
            ViewComboBox.DataSource = DnnPathHelper.GetViewNames("ContentColumn");
            ViewComboBox.DataBind();

            ContentBlocksComboBox.Items.Add(new RadComboBoxItem(LocalizeString("NoneSelectedText"), string.Empty));
            ContentBlocksComboBox.AppendDataBoundItems = true;
            ContentBlocksComboBox.DataSource = HccApp.ContentServices.Columns.FindAll();
            ContentBlocksComboBox.DataTextField = "DisplayName";
            ContentBlocksComboBox.DataValueField = "Bvin";
            ContentBlocksComboBox.DataBind();
        }

        #endregion

        #region ModuleSettingsBase overrides

        public override void LoadSettings()
        {
            if (!IsPostBack)
            {
                FillForm();

                var moduleSettings = new ContentBlocksModuleSettings(ModuleId);

                ViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(moduleSettings.View))
                {
                    ViewComboBox.SelectedValue = moduleSettings.View;
                    ViewContentLabel.Text = moduleSettings.View;
                }
                ContentBlocksValueLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(moduleSettings.ContentBlockId))
                {
                    var contentBlocks = HccApp.ContentServices.Columns.Find(moduleSettings.ContentBlockId);

                    ContentBlocksComboBox.SelectedValue = moduleSettings.ContentBlockId;
                    ContentBlocksValueLabel.Text = contentBlocks.DisplayName;
                }
                ContentBlocksComboBox.Text = moduleSettings.ContentBlockId;
            }
        }

        public override void UpdateSettings()
        {
            var moduleSettings = new ContentBlocksModuleSettings(ModuleId)
            {
                View = ViewComboBox.SelectedValue,
                ContentBlockId = ContentBlocksComboBox.SelectedValue
            };
        }

        #endregion
    }
}