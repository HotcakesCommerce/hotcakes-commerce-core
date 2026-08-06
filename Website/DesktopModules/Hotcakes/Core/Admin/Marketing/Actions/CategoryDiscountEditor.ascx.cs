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
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Actions
{
    public partial class CategoryDiscountEditor : BaseActionControl
    {
        #region Properties

        private CategoryDiscountAdjustment TypedAction
        {
            get { return Action as CategoryDiscountAdjustment; }
        }

        #endregion

        #region Helper Methods

        private void LoadCategories()
        {
            var allCats = HccApp?.CatalogServices?.Categories?.FindAll() ?? new List<CategorySnapshot>();
            var available = CategoriesHelper.ListFullTreeWithIndents(allCats, true) ?? new Collection<ListItem>();

            var displayData = new List<FriendlyBvinDisplay>();

            var categories = TypedAction?.GetCategories() ?? new List<string>();
            foreach (var bvin in categories)
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var normalizedBvin = bvin.Trim();
                var item = new FriendlyBvinDisplay
                {
                    bvin = normalizedBvin,
                    DisplayName = normalizedBvin
                };

                var t = available.FirstOrDefault(y => string.Equals(y.Value, normalizedBvin, StringComparison.OrdinalIgnoreCase));
                if (t != null)
                {
                    item.DisplayName = t.Text;
                    available.Remove(t);
                }
                displayData.Add(item);
            }

            lstProductCategories.Items.Clear();
            foreach (var li in available)
            {
                lstProductCategories.Items.Add(li);
            }

            gvProductCategories.DataSource = displayData;
            gvProductCategories.DataBind();
        }

        #endregion

        #region Overrides

        public override void LoadAction()
        {
            if (lstLineItemAdjustType.Items.Count == 0)
            {
                lstLineItemAdjustType.Items.Add(new ListItem(Localization.GetString("Amount"), ((int)AmountTypes.MonetaryAmount).ToString()));
                lstLineItemAdjustType.Items.Add(new ListItem(Localization.GetString("Percent"), ((int)AmountTypes.Percent).ToString()));
            }

            var action = TypedAction;
            if (action == null)
            {
                LineItemAdjustAmountField.Text = string.Empty;
                lstLineItemAdjustType.SelectedIndex = 0;
                LoadCategories();
                return;
            }

            LineItemAdjustAmountField.Text = action.Amount.ToString();
            lstLineItemAdjustType.SelectedValue = action.AdjustmentType == AmountTypes.Percent
                ? ((int)AmountTypes.Percent).ToString()
                : ((int)AmountTypes.MonetaryAmount).ToString();

            LoadCategories();
        }

        public override bool SaveAction()
        {
            var action = TypedAction;
            if (action == null) return UpdatePromotion();

            action.Amount = LineItemAdjustAmountField.Text.ConvertTo(action.Amount);

            action.AdjustmentType = lstLineItemAdjustType.SelectedValue == ((int)AmountTypes.Percent).ToString()
                ? AmountTypes.Percent
                : AmountTypes.MonetaryAmount;

            if (action.AdjustmentType == AmountTypes.MonetaryAmount)
            {
                action.Amount = Money.RoundCurrency(action.Amount);
            }

            return UpdatePromotion();
        }

        #endregion

        #region Event Handlers

        protected void btnAddProductCategory_Click(object sender, EventArgs e)
        {
            var action = TypedAction;
            var selected = lstProductCategories?.SelectedValue?.Trim();
            if (action == null || string.IsNullOrWhiteSpace(selected)) return;

            action.AddCategoryId(selected);
            UpdatePromotion();
            LoadAction();
        }

        protected void gvProductCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var action = TypedAction;
            if (action == null || e?.Keys == null || e.Keys.Count == 0) return;

            var bvin = e.Keys[0] as string;
            if (string.IsNullOrWhiteSpace(bvin)) return;

            action.RemoveCategoryId(bvin.Trim());
            UpdatePromotion();
            LoadAction();
        }

        protected void btnDeleteProductCategory_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }

        #endregion
    }
}