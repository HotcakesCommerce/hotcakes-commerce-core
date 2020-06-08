#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Users_Edit : BaseCustomerPage
    {
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = Localization.GetString("EditCustomer");
            InitNavMenu(ucNavMenu);

            revPassword.ErrorMessage = Localization.GetFormattedString("revPassword.ErrorMessage",
                WebAppSettings.PasswordMinimumLength);
            revPassword.ValidationExpression = string.Concat(".{", WebAppSettings.PasswordMinimumLength, ",50}");

            dgOrders.RowCommand += dgOrders_RowCommand;
        }

        private void dgOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var bvin = e.CommandArgument.ToString();
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Orders/ViewOrder.aspx?id=" + bvin);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindPricingGroups();
                if (!string.IsNullOrWhiteSpace(CustomerId))
                {
                    pnlPassword.Visible = false;
                    pnlConfirmPassword.Visible = false;
                    blkUserProfile.Visible = true;
                    lnkDnnUserProfile.NavigateUrl = HccUrlBuilder.RouteHccUrl(HccRoute.EditUserProfile,
                        new {userId = CustomerId});
                    colCustomerHistory.Visible = true;

                    if (!string.IsNullOrEmpty(ReturnUrl) && ReturnUrl == "Y")
                    {
                        lnkBacktoAbandonedCartsReport.Visible = true;
                        lnkBacktoAbandonedCartsReport.NavigateUrl =
                            "~/DesktopModules/Hotcakes/Core/Admin/reports/AbandonedCarts/view.aspx";
                    }
                    else
                    {
                        lnkBacktoAbandonedCartsReport.Visible = false;
                    }

                    LoadUser();
                    LoadOrders();
                    LoadSearchResults();
                    LoadWishList();
                }
            }
        }

        protected void WhishList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var product = e.Item.DataItem as Product;
                var aEditLink = e.Item.FindControl("aEditLink") as HtmlAnchor;
                aEditLink.HRef =
                    string.Format("~/DesktopModules/Hotcakes/Core/Admin/Catalog/Products_Performance.aspx?id={0}",
                        product.Bvin);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Save())
                {
                    Response.Redirect("default.aspx");
                }
            }
        }

        protected void dgOrders_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("OrderNumber");
                e.Row.Cells[1].Text = Localization.GetString("Total");
            }
        }

        protected void DetailsButton_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("ViewOrder");
        }

        protected void dgSearchHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("QueryPhrase");
                e.Row.Cells[1].Text = Localization.GetString("DateSearched");
            }
        }

        #endregion

        #region Implementation

        private void LoadUser()
        {
            var u = HccApp.MembershipServices.Customers.Find(CustomerId);

            if (u != null)
            {
                UsernameField.Text = u.Username;
                UsernameField.Enabled = false;
                EmailField.Text = u.Email;
                FirstNameField.Text = u.FirstName;
                LastNameField.Text = u.LastName;
                chkTaxExempt.Checked = u.TaxExempt;
                txtTaxExemptionNumber.Text = u.TaxExemptionNumber;
                CommentField.Text = u.Notes;
                PricingGroupDropDownList.SelectedValue = u.PricingGroupId;
            }
        }

        private void BindPricingGroups()
        {
            var priceGroups = HccApp.ContactServices.PriceGroups.FindAll();

            if (priceGroups != null && priceGroups.Count > 0)
            {
                PricingGroupDropDownList.Items.Clear();
                PricingGroupDropDownList.DataSource = priceGroups;
                PricingGroupDropDownList.DataTextField = "Name";
                PricingGroupDropDownList.DataValueField = "bvin";
                PricingGroupDropDownList.DataBind();

                PricingGroupDropDownList.Items.Insert(0, new ListItem(Localization.GetString("None"), string.Empty));

                divPriceGroup.Visible = true;
            }
            else
            {
                divPriceGroup.Visible = false;
            }
        }

        private void LoadOrders()
        {
            var totalCount = 0;
            var dtOrders = HccApp.OrderServices.Orders.FindByUserId(CustomerId, 1, 100, ref totalCount);

            if (dtOrders != null)
            {
                if (dtOrders.Count < 100)
                {
                    lblItems.Text = string.Format(Localization.GetString("ResultsFound"), dtOrders.Count);
                }
                else
                {
                    lblItems.Text = string.Format(Localization.GetString("FirstOfTotalOrders"), dtOrders.Count,
                        totalCount);
                }

                dgOrders.DataSource = dtOrders;
                dgOrders.DataBind();
            }
            else
            {
                lblItems.Text = Localization.GetString("NoOrdersFound");
            }
        }

        private void LoadSearchResults()
        {
            var totalCount = 0;

            var sr = HccApp.MetricsSerices.SearchQueries.FindByShopperId(CustomerId, 1, 50, ref totalCount);

            if (sr != null)
            {
                sr.ForEach(i => { i.LastUpdated = DateHelper.ConvertUtcToStoreTime(HccApp, i.LastUpdated); });

                dgSearchHistory.DataSource = sr;
                dgSearchHistory.DataBind();
            }
        }

        private void LoadWishList()
        {
            var w = HccApp.CatalogServices.WishListItems.FindByCustomerIdPaged(CustomerId, 1, 100);
            var products = new List<Product>();

            foreach (var item in w)
            {
                var n = HccApp.CatalogServices.Products.FindWithCache(item.ProductId);

                n.ImageFileSmall = DiskStorage.ProductImageUrlSmall(
                    HccApp,
                    n.Bvin,
                    n.ImageFileSmall,
                    Request.IsSecureConnection);
                products.Add(n);
            }

            if (products.Count > 0)
            {
                WishList.Visible = true;
                WishList.DataSource = products;
                WishList.DataBind();
            }
            else
            {
                lblNoWishListItems.Visible = true;
            }
        }

        private bool Save()
        {
            var result = false;

            var account = HccApp.MembershipServices.Customers.Find(CustomerId);

            if (account == null)
                account = new CustomerAccount();

            var oldEmailAddress = account.Email.Trim().ToLower();
            var newEmailAddress = EmailField.Text.Trim().ToLower();
            var emailChanged = oldEmailAddress != newEmailAddress;
            var isNewUser = string.IsNullOrEmpty(CustomerId);

            account.Notes = CommentField.Text.Trim();
            account.FirstName = FirstNameField.Text.Trim();
            account.LastName = LastNameField.Text.Trim();
            account.TaxExempt = chkTaxExempt.Checked;
            account.TaxExemptionNumber = txtTaxExemptionNumber.Text.Trim();
            account.PricingGroupId = PricingGroupDropDownList.SelectedValue;

            var usrStatus = CreateUserStatus.None;

            if (isNewUser) // Create NEW customer account
            {
                if (!MembershipUtils.CheckPasswordComplexity(Membership.Provider, PasswordField.Text.Trim()))
                {
                    ucMessageBox.ShowError(string.Format(Localization.GetString("revPassword.ErrorMessage"),
                        WebAppSettings.PasswordMinimumLength));
                    return false;
                }

                account.Email = EmailField.Text.Trim();
                account.Username = UsernameField.Text;
                result = HccApp.MembershipServices.CreateCustomer(account, out usrStatus, PasswordField.Text.Trim());
            }
            else // Update EXIST customer account
            {
                result = HccApp.MembershipServices.UpdateCustomer(account, out usrStatus);

                // Send email changed notification
                if (result && emailChanged)
                {
                    if (HccApp.MembershipServices.UpdateCustomerEmail(account, newEmailAddress))
                    {
                        HccApp.CurrentRequestContext.IntegrationEvents.CustomerAccountEmailChanged(oldEmailAddress,
                            account.Email);
                    }
                }
            }

            if (!result)
            {
                HandleCreationError(usrStatus);
            }

            return result;
        }

        private void HandleCreationError(CreateUserStatus usrStatus)
        {
            string message = null;
            switch (usrStatus)
            {
                case CreateUserStatus.DuplicateUsername:
                    message = Localization.GetString("UsernameExists");
                    break;
                case CreateUserStatus.DuplicateEmail:
                    message = Localization.GetString("EmailExists");
                    break;
                default:
                    message = Localization.GetString("UserCreateError");
                    break;
            }

            ucMessageBox.ShowError(message);
        }

        #endregion
    }
}