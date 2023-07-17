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
using DotNetNuke.Entities.Modules;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;
using System.Web.UI.WebControls;

namespace Hotcakes.Modules.Cart
{
    public partial class Settings : HotcakesSettingsBase
    {
        #region Implementation

        /// <summary>
        ///     Fill dropdown lists
        /// </summary>
        private void FillForm()
        {
            ViewComboBox.Items.Add(new ListItem(LocalizeString("NoneSelectedText"), string.Empty));
            ViewComboBox.AppendDataBoundItems = true;
            ViewComboBox.DataSource = DnnPathHelper.GetViewNames("Cart");
            ViewComboBox.DataBind();
        }

        #endregion

        #region ModuleSettingsBase overrides

        public override void LoadSettings()
        {
            if (!IsPostBack)
            {
                FillForm();

                var viewText = Convert.ToString(ModuleSettings["View"]);

                ViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(viewText))
                {
                    ViewComboBox.SelectedValue = viewText;
                    ViewContentLabel.Text = viewText;
                }
            }
        }

        public override void UpdateSettings()
        {
            var controller = new ModuleController();
            controller.UpdateModuleSetting(ModuleId, "View", ViewComboBox.SelectedValue);
        }

        #endregion
    }
}