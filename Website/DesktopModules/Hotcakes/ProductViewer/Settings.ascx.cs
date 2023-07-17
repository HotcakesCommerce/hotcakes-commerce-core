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
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;

namespace Hotcakes.Modules.ProductViewer
{
    public partial class Settings : HotcakesSettingsBase
    {
        public override void LoadSettings()
        {
            if (!IsPostBack)
            {
                FillForm();

                var slugText = Convert.ToString(ModuleSettings["Slug"]);
                var viewText = Convert.ToString(ModuleSettings["View"]);

                ViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(viewText))
                {
                    ViewComboBox.SelectedValue = viewText;
                    ViewContentLabel.Text = viewText;
                }

                ProductContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(slugText))
                {
                    var productText = LookupProduct(slugText);
                    ProductContentLabel.Text = productText;
                }
            }
        }

        public override void UpdateSettings()
        {
            var slugText = Convert.ToString(ModuleSettings["Slug"]);
            var viewText = Convert.ToString(ModuleSettings["View"]);

            if (ProductComboBox.SelectedValue != null)
            {
                slugText = ProductComboBox.SelectedValue;
            }

            if (!string.IsNullOrEmpty(ViewComboBox.SelectedValue))
            {
                viewText = ViewComboBox.SelectedValue;
            }

            var controller = new ModuleController();
            controller.UpdateModuleSetting(ModuleId, "Slug", slugText);
            controller.UpdateModuleSetting(ModuleId, "View", viewText);
        }

        protected void BindProducts()
        {
            //TODO: Restore auto-complete functionality 
            /*
            var criteria = new ProductSearchCriteria();
            criteria.Keyword = e.Text;

            var products = HccApp.CatalogServices.Products.FindByCriteria(criteria);
            */
            // only getting the first 500 for now (until the TODO above is done)
            var products = HccApp.CatalogServices.Products.FindAllPaged(0, 500);

            ProductComboBox.Items.Clear();

            ProductComboBox.DataSource = products;
            ProductComboBox.DataTextField = "ProductName";
            ProductComboBox.DataValueField = "UrlSlug";
            ProductComboBox.DataBind();

            ProductComboBox.Items.Insert(0, new ListItem(LocalizeString("NoneSelectedText"), string.Empty));
        }

        #region Implementation

        /// <summary>
        ///     Fill dropdown lists
        /// </summary>
        private void FillForm()
        {
            ViewComboBox.Items.Add(new ListItem(LocalizeString("NoneSelectedText"), string.Empty));
            ViewComboBox.AppendDataBoundItems = true;
            ViewComboBox.DataSource = DnnPathHelper.GetViewNames("Products");
            ViewComboBox.DataBind();

            BindProducts();
        }

        private string LookupProduct(string slugText)
        {
            var productText = string.Empty;
            if (!string.IsNullOrEmpty(slugText))
            {
                CustomUrl customUrl;
                var product = HccApp.ParseProductBySlug(slugText, out customUrl);

                if (product != null)
                {
                    productText = product.ProductName;
                }
            }
            return productText;
        }

        #endregion
    }
}