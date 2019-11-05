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
using System.Globalization;
using System.Threading;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class PriceGroups : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LocalizeView();

                BindGrids();
            }
        }

        protected void BindGrids()
        {
            PricingGroupsGridView.DataSource = HccApp.ContactServices.PriceGroups.FindAll();
            PricingGroupsGridView.DataBind();
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PriceGroups");
            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.PeopleView);
        }

        protected void SaveImageButton_Click(object sender, EventArgs e)
        {
            var isValidate = true;
            var isRounded = false;
            var roundedAdjustmentAmountFileds = "<br/>";
            foreach (GridViewRow row in PricingGroupsGridView.Rows)
            {
                var key = (string) PricingGroupsGridView.DataKeys[row.RowIndex].Value;

                var pricingGroup = HccApp.ContactServices.PriceGroups.Find(key);

                var NameTextBox = (TextBox) row.FindControl("NameTextBox");
                var PricingTypeDropDownList = (DropDownList) row.FindControl("PricingTypeDropDownList");
                var AdjustmentAmountTextBox = (TextBox) row.FindControl("AdjustmentAmountTextBox");

                var needToUpdate = false;

                if (pricingGroup.Name != NameTextBox.Text)
                {
                    pricingGroup.Name = NameTextBox.Text;
                    needToUpdate = true;
                }

                if ((int) pricingGroup.PricingType != int.Parse(PricingTypeDropDownList.SelectedValue))
                {
                    pricingGroup.PricingType = (PricingTypes) int.Parse(PricingTypeDropDownList.SelectedValue);
                    needToUpdate = true;
                }

                decimal adjustmentAmount = 0;

                if (decimal.TryParse(AdjustmentAmountTextBox.Text, NumberStyles.Currency,
                    Thread.CurrentThread.CurrentUICulture, out adjustmentAmount))
                {
                    if (pricingGroup.AdjustmentAmount != adjustmentAmount)
                    {
                        pricingGroup.AdjustmentAmount = adjustmentAmount;

                        if (pricingGroup.PricingType == PricingTypes.AmountAboveCost ||
                            pricingGroup.PricingType == PricingTypes.AmountOffListPrice ||
                            pricingGroup.PricingType == PricingTypes.AmountOffSitePrice)
                        {
                            isRounded = true;
                            pricingGroup.AdjustmentAmount = Money.RoundCurrency(pricingGroup.AdjustmentAmount);
                            AdjustmentAmountTextBox.Text =
                                Money.RoundCurrency(pricingGroup.AdjustmentAmount).ToString("N");
                            roundedAdjustmentAmountFileds += NameTextBox.Text + "<br/>";
                        }

                        needToUpdate = true;
                    }
                }
                else
                {
                    isValidate = false;
                    MessageBox1.ShowError(string.Format(Localization.GetString("AdjustmentAmountError"),
                        NameTextBox.Text));
                    break;
                }

                if (needToUpdate)
                {
                    HccApp.ContactServices.PriceGroups.Update(pricingGroup);
                }
            }

            if (!isValidate)
            {
                return;
            }

            if (isRounded)
            {
                MessageBox1.ShowOk(string.Format(Localization.GetString("SettingsSuccessfulWithCurrencyRounded"),
                    roundedAdjustmentAmountFileds));
            }
            else
            {
                MessageBox1.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
        }

        protected void PricingGroupsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    var pricingGroup = (PriceGroup) e.Row.DataItem;
                    ((TextBox) e.Row.FindControl("NameTextBox")).Text = pricingGroup.Name;
                    ((DropDownList) e.Row.FindControl("PricingTypeDropDownList")).SelectedValue =
                        ((int) pricingGroup.PricingType).ToString();
                    ((TextBox) e.Row.FindControl("AdjustmentAmountTextBox")).Text =
                        pricingGroup.AdjustmentAmount.ToString("N");

                    if (pricingGroup.PricingType == PricingTypes.AmountAboveCost ||
                        pricingGroup.PricingType == PricingTypes.AmountOffListPrice ||
                        pricingGroup.PricingType == PricingTypes.AmountOffSitePrice)
                        ((CompareValidator) e.Row.FindControl("cValidator")).Enabled = true;
                    else
                        ((CompareValidator) e.Row.FindControl("cValidator")).Enabled = false;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Name");
                e.Row.Cells[1].Text = Localization.GetString("PricingType");
                e.Row.Cells[2].Text = Localization.GetString("AdjustmentAmount");
            }
        }

        protected void AddNewImageButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var pricingGroup = new PriceGroup {Name = txtPriceGroup.Text};

                if (HccApp.ContactServices.PriceGroups.Create(pricingGroup))
                {
                    MessageBox1.ShowOk(Localization.GetString("NewPriceGroupAdded"));
                }
                else
                {
                    MessageBox1.ShowError(Localization.GetString("NewPriceGroupError"));
                }
            }

            BindGrids();
        }

        protected void CancelImageButton_Click(object sender, EventArgs e)
        {
            BindGrids();

            MessageBox1.ShowOk(Localization.GetString("CancelMessage"));
        }

        protected void PricingGroupsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var key = (string) PricingGroupsGridView.DataKeys[e.RowIndex].Value;

            if (HccApp.ContactServices.PriceGroups.Delete(key))
            {
                MessageBox1.ShowOk(Localization.GetString("PriceGroupDeleted"));
            }
            else
            {
                MessageBox1.ShowError(Localization.GetString("DeleteError"));
            }

            BindGrids();
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("DeleteConfirm"));
        }

        private void LocalizeView()
        {
            rfvDisplayName.ErrorMessage = Localization.GetString("rfvDisplayName.ErrorMessage");
        }
    }
}