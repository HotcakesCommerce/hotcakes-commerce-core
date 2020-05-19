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
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;
using Hotcakes.Modules.Core.Models;
using Newtonsoft.Json;

namespace Hotcakes.Modules.ProductGrid
{
    partial class Settings : HotcakesSettingsBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnAdd.Click += btnAdd_Click;
            rgProducts.RowDeleting += rgProducts_OnDeleteCommand;
            rgProducts.RowCommand += rgProducts_OnItemCommand;
        }

        public override void LoadSettings()
        {
            var viewText = Convert.ToString(ModuleSettings["View"]);

            ViewContentLabel.Text = LocalizeString("NoneSelectedText");

            if (!string.IsNullOrEmpty(viewText))
            {
                ViewComboBox.SelectedValue = viewText;
                ViewContentLabel.Text = viewText;
            }
        }

        public override void UpdateSettings()
        {
            var controller = new ModuleController();

            controller.UpdateModuleSetting(ModuleId, "GridColumns", GridColumnsField.Text.Trim());
            controller.UpdateModuleSetting(ModuleId, "View", ViewComboBox.SelectedValue);

            ModuleController.SynchronizeModule(ModuleId);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LocalizeView();

                var sortedProducts = GetSelectedProducts();
                LoadItems(sortedProducts);

                int gridColumns;
                if (!int.TryParse(Convert.ToString(ModuleSettings["GridColumns"]), out gridColumns))
                    gridColumns = 3;

                GridColumnsField.Text = gridColumns.ToString();

                // load the view names into the combobox
                ViewComboBox.Items.Add(new System.Web.UI.WebControls.ListItem(LocalizeString("NoneSelectedText"), string.Empty));
                ViewComboBox.AppendDataBoundItems = true;
                ViewComboBox.DataSource = DnnPathHelper.GetViewNames("ProductGrid");
                ViewComboBox.DataBind();
            }
        }

        private void LocalizeView()
        {
            Localization.LocalizeGridView(ref rgProducts, LocalResourceFile);

            valGridColumns.ErrorMessage = LocalizeString("valGridColumns.ErrorMessage");
        }

        private void LoadItems(SortedList<int, Product> list)
        {
            if (list != null)
            {
                // the list can contain null values for products in certain scenarios
                // (mainly in cases where there is orphaned or corrupt data)
                var safeList = list.Where(p => p.Value != null);

                // select products to bind to the UI
                var productModels =
                    safeList.Select(i => new {i.Key, Value = new SingleProductViewModel(i.Value, HccApp)})
                        .OrderBy(i => i.Key);

                rgProducts.DataSource = productModels;
                rgProducts.DataBind();
            }
        }

        private void SaveItems(SortedList<int, Product> sortedProducts)
        {
            var items = sortedProducts.ToDictionary(item => item.Key, item => item.Value.Bvin);
            SaveItems(items);
        }

        private void SaveItems(Dictionary<int, string> items)
        {
            var controller = new ModuleController();
            controller.UpdateModuleSetting(ModuleId, "ProductList", JsonConvert.SerializeObject(items));
        }

        private SortedList<int, Product> GetSelectedProducts()
        {
            var productBvins = GetProductBvins();
            return GetProducts(productBvins);
        }

        private SortedList<int, Product> GetProducts(Dictionary<int, string> productBvins)
        {
            var sortedProducts = new SortedList<int, Product>();

            if (productBvins.Count > 0)
            {
                var listProducts = HccApp.CatalogServices.Products.FindManyWithCache(productBvins.Values);
                foreach (var item in productBvins)
                {
                    sortedProducts.Add(item.Key, listProducts.FirstOrDefault(p => p.Bvin == item.Value));
                }
            }
            return sortedProducts;
        }

        private Dictionary<int, string> GetProductBvins()
        {
            Dictionary<int, string> productBvins = null;
            try
            {
                var productSettingsString = Convert.ToString(ModuleSettings["ProductList"]);
                productBvins = JsonConvert.DeserializeObject<Dictionary<int, string>>(productSettingsString) ??
                               new Dictionary<int, string>();
            }
            catch
            {
                productBvins = new Dictionary<int, string>();
            }
            return productBvins;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var products =
                HccApp.CatalogServices.Products.FindManyWithCache(
                    ProductPicker.SelectedProducts.OfType<string>().ToList());
            var sortedProducts = GetSelectedProducts();

            foreach (var p in products)
            {
                sortedProducts.Add(sortedProducts.Keys.DefaultIfEmpty(0).Max() + 1, p);
            }
            SaveItems(sortedProducts);
            LoadItems(sortedProducts);
        }

        protected void rgProducts_OnDeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var bvinsList = GetProductBvins();
            var key = int.Parse(rgProducts.DataKeys[e.RowIndex].ToString(), NumberStyles.Integer);
            bvinsList.Remove(key);
            SaveItems(bvinsList);
            LoadItems(GetProducts(bvinsList));
        }

        protected void rgProducts_OnItemCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Up") || e.CommandName.Equals("Down"))
            {
                var bvinsList = GetProductBvins();
                var key = int.Parse(e.CommandArgument.ToString(), NumberStyles.Integer);

                int key2;
                if (e.CommandName.Equals("Up"))
                    key2 =
                        bvinsList.Where(i => i.Key < key)
                            .DefaultIfEmpty(new KeyValuePair<int, string>(-1, null))
                            .Max(i => i.Key);
                else
                    key2 =
                        bvinsList.Where(i => i.Key > key)
                            .DefaultIfEmpty(new KeyValuePair<int, string>(-1, null))
                            .Min(i => i.Key);
                if (key2 < 0)
                    return;
                var val = bvinsList[key];
                bvinsList[key] = bvinsList[key2];
                bvinsList[key2] = val;
                SaveItems(bvinsList);
                LoadItems(GetProducts(bvinsList));
            }
        }
    }
}