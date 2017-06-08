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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductVolumeDiscounts : BaseProductPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ProductVolumeDiscounts");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(ProductId))
                {
                    Response.Redirect(DefaultCatalogPage);
                }
                else
                {
                    BindGridViews();
                }
            }
        }

        protected void BindGridViews()
        {
            LocalizationUtils.LocalizeGridView(VolumeDiscountsGridView, Localization);

            VolumeDiscountsGridView.DataSource = HccApp.CatalogServices.VolumeDiscounts.FindByProductId(ProductId);
            VolumeDiscountsGridView.DataBind();
        }

        protected void btnNewLevel_Click(object sender, EventArgs e)
        {
            var quantity = 0;
            var amount = 0m;
            var hasError = false;

            if (!int.TryParse(txtQuantity.Text, out quantity))
            {
                ucMessageBox.ShowError(Localization.GetString("QuantityError"));
                hasError = true;
            }

            if (!decimal.TryParse(txtPrice.Text, out amount))
            {
                ucMessageBox.ShowError(Localization.GetString("PriceError"));
                hasError = true;
            }

            if (!hasError)
            {
                var volumeDiscounts =
                    HccApp.CatalogServices.VolumeDiscounts.FindByProductId(ProductId);
                ProductVolumeDiscount volumeDiscount = null;
                foreach (var item in volumeDiscounts)
                {
                    if (item.Qty == quantity)
                    {
                        volumeDiscount = item;
                    }
                }
                if (volumeDiscount == null)
                {
                    volumeDiscount = new ProductVolumeDiscount();
                }

                volumeDiscount.DiscountType = ProductVolumeDiscountType.Amount;
                volumeDiscount.Amount = Money.RoundCurrency(amount);
                volumeDiscount.Qty = quantity;
                volumeDiscount.ProductId = ProductId;

                var result = false;
                if (string.IsNullOrEmpty(volumeDiscount.Bvin))
                {
                    result = HccApp.CatalogServices.VolumeDiscounts.Create(volumeDiscount);
                }
                else
                {
                    result = HccApp.CatalogServices.VolumeDiscounts.Update(volumeDiscount);
                }
                if (result)
                {
                    ucMessageBox.ShowOk(Localization.GetString("VolumeDiscountAdded"));
                    txtQuantity.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                }
                else
                {
                    ucMessageBox.ShowError(Localization.GetString("AddError"));
                }
                BindGridViews();
            }
        }

        protected void VolumeDiscountsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (
                HccApp.CatalogServices.VolumeDiscounts.Delete(
                    VolumeDiscountsGridView.DataKeys[e.RowIndex].Value.ToString()))
            {
                ucMessageBox.ShowOk(Localization.GetString("VolumeDiscountDeleted"));
            }
            else
            {
                ucMessageBox.ShowOk(Localization.GetString("DeleteError"));
            }
            BindGridViews();
        }
    }
}