#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
            var action = TypedAction;
            if (action == null) return;

            var products = action.GetQuantities();
            if (products == null || gvProducts?.Rows == null) return;

            for (var i = 0; i < gvProducts.Rows.Count; i++)
            {
                var key = gvProducts.DataKeys?[i]?.Value as string;
                if (string.IsNullOrWhiteSpace(key)) continue;

                var txt = gvProducts.Rows[i].FindControl("txtQuantity") as TextBox;
                if (txt == null) continue;

                // Try to parse the provided value; if invalid, skip and leave existing quantity
                if (int.TryParse(txt.Text, out var parsedQuantity) && parsedQuantity >= 0)
                {
                    products[key] = parsedQuantity;
                }
            }

            action.SaveQuantitiesToSettings(products);
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
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            if (e.Row.DataItem == null) return;

            var item = e.Row.DataItem as FriendlyBvinDisplay;
            if (item == null) return;

            var txtQuantity = e.Row.FindControl("txtQuantity") as TextBox;
            var cvQuantity = e.Row.FindControl("cvCompare") as CompareValidator;
            var rfQuantity = e.Row.FindControl("rvQuantity") as RequiredFieldValidator;

            if (txtQuantity != null)
            {
                txtQuantity.Text = item.Quantity.ToString();
            }

            if (cvQuantity != null)
            {
                cvQuantity.ErrorMessage = Localization.GetString("ValidationMessagePositiveInteger");
            }

            if (rfQuantity != null)
            {
                rfQuantity.ErrorMessage = string.Format("{0} {1}", item.DisplayName,
                    Localization.GetString("ValidationMessageRequired"));
            }
        }

        protected void btnDeleteProduct_OnPreRender(object sender, EventArgs e)
        {
            if (sender is LinkButton link)
            {
                link.Text = Localization.GetString("Delete");
            }
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var action = TypedAction;
            if (action == null) return;
            if (e?.Keys == null || e.Keys.Count == 0) return;

            var bvin = e.Keys[0] as string;
            if (string.IsNullOrWhiteSpace(bvin)) return;

            action.RemoveItemId(bvin);
            UpdatePromotion();
            LoadAction();
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            var action = TypedAction;
            if (action == null) return;
            if (ProductPickerOrderProducts?.SelectedProducts == null) return;

            foreach (var bvin in ProductPickerOrderProducts.SelectedProducts)
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;
                action.AddItemId(bvin, 1);
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

            var action = TypedAction;
            if (action == null)
            {
                gvProducts.DataSource = displayData;
                gvProducts.DataBind();
                return;
            }

            var products = action.GetQuantities() ?? new Dictionary<string, int>();

            foreach (var bvin in products.Keys)
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin,
                    Quantity = products[bvin]
                };

                var p = HccApp?.CatalogServices?.Products?.FindWithCache(item.bvin);

                if (p != null)
                {
                    item.DisplayName = $"[{p.Sku}]{p.ProductName}";
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