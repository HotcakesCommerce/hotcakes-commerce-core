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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class GiftCards : BaseAdminPage
    {
        #region Properties

        private long? GiftCardId
        {
            get { return (long?) ViewState["GiftCardId"]; }
            set { ViewState["GiftCardId"] = value; }
        }

        #endregion

        #region Event handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("GiftCards");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvGiftCards.RowDeleting += gvGiftCards_RowDeleting;
            gvGiftCards.RowEditing += gvGiftCards_RowEditing;
            gvGiftCards.RowCommand += gvGiftCards_RowCommand;

            lnkSave.Click += lnkSave_Click;
            lnkCancel.Click += lnkCancel_Click;
            btnFind.Click += (s, a) =>
            {
                ucPager.ResetPageNumber();
                LoadGiftCards();
            };
            ucDateRangePicker.RangeTypeChanged += (s, a) =>
            {
                ucPager.ResetPageNumber();
                LoadGiftCards();
            };
            
            ucPager.PageChanged += (s, a) => { LoadGiftCards(); };
            lnkExportToExcel.Click += lnkExportToExcel_Click;
        }

        protected void gvGiftCards_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GoToOrder")
            {
                var o = HccApp.OrderServices.Orders.FindByOrderNumber(e.CommandArgument.ToString());

                Response.Redirect("../Orders/ViewOrder.aspx?id=" + o.bvin);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadGiftCards();
                LocalizeView();
            }
        }

        private void gvGiftCards_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var id = (long) gvGiftCards.DataKeys[e.NewEditIndex].Value;
            LoadGiftCardEditor(id);
            e.Cancel = true;
        }

        private void gvGiftCards_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) e.Keys[0];
            HccApp.CatalogServices.GiftCards.Delete(id);
            LoadGiftCards();
        }

        private void lnkSave_Click(object sender, EventArgs e)
        {
            var card = HccApp.CatalogServices.GiftCards.Find(GiftCardId.Value);
            card.ExpirationDateUtc = DateHelper.ConvertStoreTimeToUtc(HccApp, DateTime.Parse(dpExpiration.Text.Trim()));
            card.Amount = Convert.ToDecimal(txtAmount.Text);
            card.Enabled = cbEnabled.Checked;

            HccApp.CatalogServices.GiftCards.Update(card);
            ShowEditor(false);
            LoadGiftCards();
        }

        private void lnkCancel_Click(object sender, EventArgs e)
        {
            ShowEditor(false);
        }

        private void lnkExportToExcel_Click(object sender, EventArgs e)
        {
            var criteria = GetCriteria();
            var rowCount = 0;
            var items = HccApp.CatalogServices.GiftCards.FindAllWithFilter(criteria, 1, int.MaxValue, ref rowCount);
            var giftCardExport = new GiftCardExport(HccApp);
            giftCardExport.ExportToExcel(Response, "Hotcakes_GiftCards.xlsx", items);
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void gvGiftCards_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("IssueDate");
                e.Row.Cells[1].Text = Localization.GetString("Recipient");
                e.Row.Cells[2].Text = Localization.GetString("CardNumber");
                e.Row.Cells[3].Text = Localization.GetString("Amount");
                e.Row.Cells[4].Text = Localization.GetString("Balance");
                e.Row.Cells[5].Text = Localization.GetString("Enabled");
                e.Row.Cells[6].Text = Localization.GetString("OrderNumber");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var chkBox = e.Row.Cells[5].Controls[0] as CheckBox;
                chkBox.Enabled = true;
                chkBox.Attributes.Add("onclick", "return false;");
            }
        }

        #endregion

        #region Implementation

        private void LoadGiftCards()
        {
            var lineItemId = Request.QueryString["lineitem"].ConvertToNullable<long>();
            List<GiftCard> items = null;

            if (!lineItemId.HasValue)
            {
                var criteria = GetCriteria();
                var rowCount = 0;
                items = HccApp.CatalogServices.GiftCards.FindAllWithFilter(criteria, ucPager.PageNumber,
                    ucPager.PageSize, ref rowCount);
                ucPager.SetRowCount(rowCount);
            }
            else
            {
                items = HccApp.CatalogServices.GiftCards.FindByLineItem(lineItemId.Value);
                ucPager.SetRowCount(items.Count);
                divFilterPanel.Visible = false;
                pnlFilteredByLineItem.Visible = true;
            }

            foreach (var item in items)
            {
                item.IssueDateUtc = DateHelper.ConvertUtcToStoreTime(HccApp, item.IssueDateUtc);
                item.ExpirationDateUtc = DateHelper.ConvertUtcToStoreTime(HccApp, item.ExpirationDateUtc);
                item.UsedAmount = item.Amount - item.UsedAmount; // replace "used amount" to "balance"
            }

            gvGiftCards.DataSource = items;
            gvGiftCards.DataBind();

            lblNoGiftcards.Visible = items.Count == 0;
        }

        private GiftCardSearchCriteria GetCriteria()
        {
            var criteria = new GiftCardSearchCriteria
            {
                SearchText = txtSearchText.Text.Trim(),
                ShowDisabled = cbShowDisabled.Checked,
                ShowExpired = cbShowExpired.Checked,
                StartDateUtc = ucDateRangePicker.GetStartDateUtc(HccApp),
                EndDateUtc = ucDateRangePicker.GetEndDateUtc(HccApp),
                CompareAmount = ucAmountComare.GetCompareCriteria(),
                CompareBalance = ucBalanceComare.GetCompareCriteria()
            };
            return criteria;
        }

        private void ShowEditor(bool show)
        {
            pnlEditGiftCard.Visible = show;

            if (show)
            {
                ClientScript.RegisterStartupScript(GetType(), "hcEditGiftCardDialog", "hcEditGiftCardDialog();", true);
            }
        }

        private void LoadGiftCardEditor(long id)
        {
            var card = HccApp.CatalogServices.GiftCards.Find(id);

            cbEnabled.Checked = card.Enabled;
            lblCardNumber.Text = card.CardNumber;
            lblIssueDate.Text = DateHelper.ConvertUtcToStoreTime(HccApp, card.IssueDateUtc).ToShortDateString();
            lblRecipientEmail.Text = card.RecipientEmail + "&nbsp;";
            //NOTE: hack for jqueryui dialog, otherwise max-height calculates wrong
            lblRecipientName.Text = card.RecipientName + "&nbsp;";

            dpExpiration.Text = DateHelper.ConvertUtcToStoreTime(HccApp, card.ExpirationDateUtc).ToString("MM/dd/yyyy");
            txtAmount.Text = Money.FormatCurrency(card.Amount);
            lblUsedAmount.Text = Money.FormatCurrency(card.UsedAmount);
            lblOrderNumber.Text = card.OrderNumber;

            GiftCardId = id;
            ShowEditor(true);
        }

        private void LocalizeView()
        {
            rfvAmount.ErrorMessage = Localization.GetString("rfvAmount.ErrorMessage");
            cvAmount.ErrorMessage = Localization.GetString("cvAmount.ErrorMessage");
        }

        #endregion
    }
}