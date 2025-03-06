#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Text;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing
{
    public partial class Promotions : BaseAdminPage
    {
        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("Promotions");
            CurrentTab = AdminTabType.Marketing;
            ValidateCurrentUserHasPermission(SystemPermissions.MarketingView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            lnkMigrate.Click += lnkMigrate_Click;
            chkShowDisabled.CheckedChanged += (s, a) => ResetAllPageNumbers();
            btnGo.Click += (s, a) => ResetAllPageNumbers();
        }

        private void lnkMigrate_Click(object sender, EventArgs e)
        {
            ucMessageBox.ClearMessage();
            lnkMigrate.Visible = false;
            ucMessageBox.ShowOk(Localization.GetString("MigrateSuccess"));
        }

        private void ResetAllPageNumbers()
        {
            ucSalesList.ResetPageNumber();
            ucOffersOrderItems.ResetPageNumber();
            ucOffersOrderSubTotal.ResetPageNumber();
            ucOffersFreeItem.ResetPageNumber();
            ucOffersOrderShipping.ResetPageNumber();
            ucAffiliatePromotions.ResetPageNumber();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                InitialBindData();
            }
            else
            {
                SessionManager.AdminPromotionShowDisabled = chkShowDisabled.Checked;
                SessionManager.AdminPromotionKeywords = txtKeywords.Text;
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var predefinedPromo = (PreDefinedPromotion) Convert.ToInt32(lstNewType.SelectedValue);
            var promo = HccApp.MarketingServices.GetPredefinedPromotion(predefinedPromo);
            HccApp.MarketingServices.Promotions.Create(promo);

            Response.Redirect(GetEditUrl(promo.Id));
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ucSalesList.LoadPromotions(txtKeywords.Text, chkShowDisabled.Checked);
            ucOffersOrderItems.LoadPromotions(txtKeywords.Text, chkShowDisabled.Checked);
            ucOffersOrderSubTotal.LoadPromotions(txtKeywords.Text, chkShowDisabled.Checked);
            ucOffersOrderShipping.LoadPromotions(txtKeywords.Text, chkShowDisabled.Checked);
            ucAffiliatePromotions.LoadPromotions(txtKeywords.Text, chkShowDisabled.Checked);
            ucOffersFreeItem.LoadPromotions(txtKeywords.Text, chkShowDisabled.Checked);

            if (!chkShowDisabled.Checked)
            {
                ucMessageBox.ShowInformation(Localization.GetString("PromotionsOrder"));
            }
        }

        #endregion

        #region Implementation

        private void InitialBindData()
        {
            chkShowDisabled.Checked = SessionManager.AdminPromotionShowDisabled;
            txtKeywords.Text = SessionManager.AdminPromotionKeywords;
            txtKeywords.Focus();

            lstNewType.Items.Add(new ListItem(Localization.GetString("CustomSale"), "0"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("CustomOfferForItems"), "1"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("CustomOfferForFreeItems"), "5"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("CustomOfferForOrder"), "2"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("CustomOfferForShipping"), "3"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("AffiliatePromotion"), "4"));

            var lstSeparator = new ListItem("--------------", string.Empty);
            lstSeparator.Enabled = false;
            lstNewType.Items.Add(lstSeparator);

            lstNewType.Items.Add(new ListItem(Localization.GetString("SaleStoreWide"), "10"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("SaleProducts"), "11"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("SaleCategories"), "12"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("SaleProductTypes"), "13"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("SaleByPriceGroup"), "14"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("SaleByUser"), "15"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("OfferWithCoupon"), "16"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("OfferByPriceGroup"), "18"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("OfferByUser"), "17"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("OfferFreeShipping"), "19"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("OfferShippingDiscount"), "20"));
            lstNewType.Items.Add(new ListItem(Localization.GetString("OfferFreeShippingCategory"), "21"));
        }

        private string GetEditUrl(long id)
        {
            return string.Concat("Promotions_edit.aspx?id=", id);
        }

        #endregion
    }
}