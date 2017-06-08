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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
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
            var allCats = HccApp.CatalogServices.Categories.FindAll();
            var available = CategoriesHelper.ListFullTreeWithIndents(allCats, true);

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in TypedAction.GetCategories())
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;

                var t = available.FirstOrDefault(y => y.Value.ToLowerInvariant() == bvin.ToLowerInvariant());
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
                lstLineItemAdjustType.Items.Add(new ListItem(Localization.GetString("Amount"), "0"));
                lstLineItemAdjustType.Items.Add(new ListItem(Localization.GetString("Percent"), "1"));
            }

            LineItemAdjustAmountField.Text = TypedAction.Amount.ToString();
            lstLineItemAdjustType.SelectedValue = TypedAction.AdjustmentType == AmountTypes.Percent ? "1" : "0";

            LoadCategories();
        }

        public override bool SaveAction()
        {
            TypedAction.Amount = LineItemAdjustAmountField.Text.ConvertTo(TypedAction.Amount);
            TypedAction.AdjustmentType = lstLineItemAdjustType.SelectedValue == "1"
                ? AmountTypes.Percent
                : AmountTypes.MonetaryAmount;

            if (TypedAction.AdjustmentType == AmountTypes.MonetaryAmount)
            {
                TypedAction.Amount = Money.RoundCurrency(TypedAction.Amount);
            }

            return UpdatePromotion();
        }

        #endregion

        #region Event Handlers

        protected void btnAddProductCategory_Click(object sender, EventArgs e)
        {
            var q = TypedAction;
            q.AddCategoryId(lstProductCategories.SelectedValue);
            UpdatePromotion();
            LoadAction();
        }

        protected void gvProductCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedAction;
            var bvin = (string) e.Keys[0];
            q.RemoveCategoryId(bvin);
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