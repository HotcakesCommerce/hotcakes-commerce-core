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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Actions
{
    public partial class ReceiveFreeProduct : BaseActionControl
    {
        #region Properties

        private ReceiveFreeProductAdjustment TypedAction
        {
            get { return Action as ReceiveFreeProductAdjustment; }
        }

        #endregion

        #region Helper Methods

        private void UpdateQuantities()
        {
            var products = TypedAction.GetQuantities();

            for (var i = 0; i < gvProducts.Rows.Count; i++)
            {
                var id = (string) gvProducts.DataKeys[i].Value;

                if (products.ContainsKey(id))
                {
                    var txt = (TextBox) gvProducts.Rows[i].FindControl("txtQuantity");

                    if (string.IsNullOrEmpty(txt.Text))
                    {
                        return;
                    }

                    var quantity = int.Parse(txt.Text);
                    products[id] = quantity;
                }
            }

            TypedAction.SaveQuantitiesToSettings(products);
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gvProducts.RowDataBound += gvProducts_RowDataBound;

            gvProducts.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
        }

        private void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = (FriendlyBvinDisplay) e.Row.DataItem;
                var txtQuantity = (TextBox) e.Row.FindControl("txtQuantity");
                var cvQuantity = (CompareValidator) e.Row.FindControl("cvCompare");
                var rfQuantity = (RequiredFieldValidator) e.Row.FindControl("rvQuantity");

                txtQuantity.Text = item.Quantity.ToString();
                cvQuantity.ErrorMessage = Localization.GetString("ValidationMessagePositiveInteger");
                rfQuantity.ErrorMessage = string.Format("{0} {1}", item.DisplayName,
                    Localization.GetString("ValidationMessageRequired"));
            }
        }

        protected void btnDeleteProduct_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedAction;
            var bvin = (string) e.Keys[0];
            q.RemoveItemId(bvin);
            UpdatePromotion();
            LoadAction();
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            var q = TypedAction;

            foreach (var bvin in ProductPickerOrderProducts.SelectedProducts)
            {
                q.AddItemId(bvin, 1);
            }

            UpdatePromotion();
            LoadAction();
        }

        #endregion

        #region Overrides

        public override void LoadAction()
        {
            ProductPickerOrderProducts.LoadSearch();
            var displayData = new List<FriendlyBvinDisplay>();
            var products = TypedAction.GetQuantities();

            foreach (var bvin in products.Keys)
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;
                item.Quantity = products[bvin];

                var p = HccApp.CatalogServices.Products.FindWithCache(item.bvin);

                if (p != null)
                {
                    item.DisplayName = string.Format("[{0}]{1}", p.Sku, p.ProductName);
                }

                displayData.Add(item);
            }

            gvProducts.DataSource = displayData;
            gvProducts.DataBind();
        }

        public override bool SaveAction()
        {
            var item = gvProducts.Rows[0].FindControl("txtQuantity");

            UpdateQuantities();

            return UpdatePromotion();
        }

        #endregion
    }
}