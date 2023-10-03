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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Dnn.Marketing.Qualifications;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing
{
    public partial class Promotions_Edit : BaseAdminPage
    {
        private const string DATEFORMAT = "MM/dd/yyyy";

        #region Fields

        private PromotionFactory _promFactory;
        private Promotion _currPromotion;

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            CurrentTab = AdminTabType.Marketing;
            ValidateCurrentUserHasPermission(SystemPermissions.MarketingView);
            _promFactory = Factory.Instance.CreatePromotionFactory();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvQualifications.RowEditing += gvQualifications_RowEditing;
            gvQualifications.RowDeleting += gvQualifications_RowDeleting;

            gvActions.RowEditing += gvActions_RowEditing;
            gvActions.RowDeleting += gvActions_RowDeleting;

            btnCloseQualificationEditor.Click += btnCloseQualificationEditor_Click;
            btnCloseActionEditor.Click += btnCloseActionEditor_Click;
            _currPromotion = GetCurrentPromotion();
            PageTitle = Localization.GetString("EditPromotion_" + _currPromotion.Mode);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                lnkBack.NavigateUrl = BackUrl();
                PopulateLists(_currPromotion.Mode);
                gvActions.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            chkDoNotCombine.Visible = _currPromotion.Mode != PromotionType.Affiliate;

            base.OnPreRender(e);
        }

        private void gvActions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) e.Keys[0];
            DeleteAction(id);
        }

        private void gvActions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var id = (long) gvActions.DataKeys[e.NewEditIndex].Value;
            ShowActionEditor(id, 1050);
        }

        protected void gvActions_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Actions");
            }
        }

        private void gvQualifications_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) e.Keys[0];
            DeleteQualification(id);
        }

        private void gvQualifications_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var id = (long) gvQualifications.DataKeys[e.NewEditIndex].Value;
            ShowQualificationEditor(id);
        }

        protected void gvQualifications_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Qualifications");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ucMessageBox.ClearMessage();

            if (IsValid && Save())
            {
                Response.Redirect(BackUrl());
            }
        }

        protected void btnNewQualification_Click(object sender, EventArgs e)
        {
            var p = _currPromotion;
            if (p == null) return;

            var newid = lstNewQualification.SelectedValue;
            var pq = _promFactory.CreateQualification(new Guid(newid));
            p.AddQualification(pq);

            HccApp.MarketingServices.Promotions.Update(p);

            if (pq.HasOptions)
            {
                ShowQualificationEditor(pq.Id);
            }
        }

        protected void btnNewAction_Click(object sender, EventArgs e)
        {
            var p = _currPromotion;
            if (p == null) return;

            var newid = lstNewAction.SelectedValue;
            var pa = _promFactory.CreateAction(new Guid(newid));
            p.AddAction(pa);

            HccApp.MarketingServices.Promotions.Update(p);
            ShowActionEditor(pa.Id, 1050);
        }

        protected void btnSaveQualification_Click(object sender, EventArgs e)
        {
            if (SaveQualificationEditor())
            {
                pnlEditQualification.Visible = false;
            }
        }

        protected void btnSaveAction_Click(object sender, EventArgs e)
        {
            if (SaveActionEditor())
            {
                pnlEditAction.Visible = false;
            }
        }

        protected void btnCloseQualificationEditor_Click(object sender, EventArgs e)
        {
            CloseDialogs();
        }

        protected void btnCloseActionEditor_Click(object sender, EventArgs e)
        {
            CloseDialogs();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            LoadPromotion();
        }

        protected void lnkDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = string.Concat("return hcConfirm(event, '", Localization.GetString("ConfirmDelete"),
                "');");
        }

        protected void lnkEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        #endregion

        #region Implementation

        // Loaders
        private void LoadPromotion()
        {
            var p = _currPromotion = GetCurrentPromotion();
            if (p != null)
            {
                chkEnabled.Checked = p.IsEnabled;
                chkDoNotCombine.Checked = p.DoNotCombine;
                txtName.Text = p.Name;
                txtCustomerDescription.Text = string.IsNullOrEmpty(p.CustomerDescription)
                    ? p.Name + " Description "
                    : p.CustomerDescription;
                radDateStart.Text = DateHelper.ConvertUtcToStoreTime(HccApp, p.StartDateUtc).ToString(DATEFORMAT);
                radDateEnd.Text = DateHelper.ConvertUtcToStoreTime(HccApp, p.EndDateUtc).ToString(DATEFORMAT);

                gvQualifications.DataSource = p.Qualifications;
                gvQualifications.DataBind();

                gvActions.DataSource = p.Actions;
                gvActions.DataBind();
            }
        }

        protected string GetQualificationDescription(IDataItemContainer cont)
        {
            var q = cont.DataItem as IPromotionQualification;
            var desc = q.FriendlyDescription(HccApp);

            if (q.ProcessingCost == RelativeProcessingCost.Higher ||
                q.ProcessingCost == RelativeProcessingCost.Highest)
            {
                desc += Localization.GetString("SlowMessage.Text");
            }

            return desc;
        }

        protected string GetActionDescription(IDataItemContainer cont)
        {
            var a = cont.DataItem as IPromotionAction;
            return a.FriendlyDescription(HccApp);
        }

        protected bool HasQualificationOptions(IDataItemContainer cont)
        {
            var q = cont.DataItem as IPromotionQualification;
            return q.HasOptions;
        }

        // Qualifiers and Action Methods
        private void PopulateLists(PromotionType mode)
        {
            lstNewQualification.Items.Clear();
            lstNewAction.Items.Clear();

            switch (mode)
            {
                case PromotionType.Sale:
                    // sale
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("AnyProduct"),
                        PromotionQualificationBase.TypeIdAnyProduct));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenProductIs"),
                        PromotionQualificationBase.TypeIdProductBvin));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenProductCategoryIs"),
                        PromotionQualificationBase.TypeIdProductCategory));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenProductTypeIs"),
                        PromotionQualificationBase.TypeIdProductType));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenUserIs"),
                        PromotionQualificationBase.TypeIdUserIs));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenUserIsInRole"),
                        UserIsInRole.TypeIdString));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenUserPriceGroupIs"),
                        PromotionQualificationBase.TypeIdUserIsInGroup));
                    lstNewAction.Items.Add(new ListItem(Localization.GetString("AdjustProductPrice"),
                        ProductPriceAdjustment.TypeIdString));
                    break;
                case PromotionType.OfferForLineItems:
                case PromotionType.OfferForOrder:
                case PromotionType.OfferForShipping:
                case PromotionType.OfferForFreeItems:
                    // offer
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("AnyOrder"),
                        PromotionQualificationBase.TypeIdAnyOrder));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("OrderHasCouponCode"),
                        PromotionQualificationBase.TypeIdOrderHasCoupon));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenOrderHasProducts"),
                        PromotionQualificationBase.TypeIdOrderHasProducts));
                    lstNewQualification.Items.Add(
                        new ListItem(Localization.GetString("WhenOrderDoesNotHaveProducts"),
                            OrderHasNotProducts.TypeIdString));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenOrderTotal"),
                        PromotionQualificationBase.TypeIdOrderSubTotalIs));

                    if (mode != PromotionType.OfferForFreeItems)
                    {
                        lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenUserIs"),
                            PromotionQualificationBase.TypeIdUserIs));
                        lstNewQualification.Items.Add(new ListItem(
                            Localization.GetString("WhenUserPriceGroupIs"),
                            PromotionQualificationBase.TypeIdUserIsInGroup));
                        lstNewQualification.Items.Add(new ListItem(Localization.GetString("AnyShippingMethod"),
                            PromotionQualificationBase.TypeIdAnyShippingMethod));
                        lstNewQualification.Items.Add(new ListItem(Localization.GetString("ShippingMethodIs"),
                            PromotionQualificationBase.TypeIdShippingMethodIs));
                    }

                    lstNewQualification.Items.Add(new ListItem(
                        Localization.GetString("WhenItemCategoryIsIsNot"),
                        PromotionQualificationBase.TypeIdLineItemCategory));
                    lstNewQualification.Items.Add(new ListItem(
                        Localization.GetString("WhenVendorManufacturerIs"), VendorOrManufacturerIs.TypeIdString));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenLineItemIs"),
                        LineItemIsProduct.TypeIdString));
                    lstNewQualification.Items.Add(new ListItem(Localization.GetString("WhenProductTypeIs"),
                        ProductTypeIs.TypeIdString));

                    if (mode == PromotionType.OfferForLineItems)
                    {
                        lstNewAction.Items.Add(new ListItem(Localization.GetString("AdjustQualifyingItems"),
                            LineItemAdjustment.TypeIdString));
                        lstNewAction.Items.Add(
                            new ListItem(Localization.GetString("MakeQualifyingItemsFreeShipping"),
                                LineItemFreeShipping.TypeIdString));
                        lstNewAction.Items.Add(new ListItem(Localization.GetString("DiscountCategory"),
                            CategoryDiscountAdjustment.TypeIdString));
                    }
                    else if (mode == PromotionType.OfferForOrder)
                    {
                        lstNewQualification.Items.Add(new ListItem(
                            Localization.GetString("SumOrCountOfProducts"), SumOrCountOfProducts.TypeIdString));
                        lstNewAction.Items.Add(new ListItem(Localization.GetString("AdjustOrderTotal"),
                            OrderTotalAdjustment.TypeIdString));
                    }
                    else if (mode == PromotionType.OfferForShipping)
                    {
                        lstNewAction.Items.Add(new ListItem(Localization.GetString("AdjustShippingBy"),
                            OrderShippingAdjustment.TypeIdString));
                    }
                    else if (mode == PromotionType.OfferForFreeItems)
                    {
                        lstNewAction.Items.Add(new ListItem(Localization.GetString("ReceiveFreeProduct"),
                            ReceiveFreeProductAdjustment.TypeIdString));
                    }
                    break;
                case PromotionType.Affiliate:
                    lstNewQualification.Items.Add(new ListItem(
                        Localization.GetString("WhenAffiliateIsApproved"), AffiliateApproved.TypeIdString));
                    lstNewAction.Items.Add(new ListItem(Localization.GetString("IssueRewardPoints"),
                        RewardPointsAjustment.TypeIdString));
                    break;
                default:
                    break;
            }
        }

        // Save        
        private bool Save()
        {
            var result = false;

            var p = _currPromotion;
            if (p == null) return false;

            p.IsEnabled = chkEnabled.Checked;
            p.DoNotCombine = chkDoNotCombine.Checked;
            p.Name = txtName.Text.Trim();
            p.CustomerDescription = txtCustomerDescription.Text.Trim();
            p.StartDateUtc = ConvertStartDateToUtc(DateTime.Parse(radDateStart.Text.Trim()));
            p.EndDateUtc = ConvertStartDateToUtc(DateTime.Parse(radDateEnd.Text.Trim()));

            result = HccApp.MarketingServices.Promotions.Update(p);

            if (result == false)
            {
                ucMessageBox.ShowWarning(Localization.GetString("SavePromotionError.Text"));
            }

            return result;
        }

        private void DeleteAction(long id)
        {
            var p = _currPromotion;
            if (p != null && p.RemoveAction(id))
            {
                HccApp.MarketingServices.Promotions.Update(p);
            }
        }

        private void DeleteQualification(long id)
        {
            var p = _currPromotion;
            if (p != null && p.RemoveQualification(id))
            {
                HccApp.MarketingServices.Promotions.Update(p);
            }
        }

        private void ShowQualificationEditor(long id)
        {
            var qualif = _currPromotion.GetQualification(id);
            Promotions_Edit_Qualification1.LoadQualification(_currPromotion, qualif);
            pnlEditQualification.Visible = true;
            pnlEditAction.Visible = false;

            ScriptManager.RegisterStartupScript(Page, GetType(), "hcShowQualificationDialog",
                "hcShowQualificationDialog();", true);
        }

        private bool SaveQualificationEditor()
        {
            return Promotions_Edit_Qualification1.SaveQualification();
        }

        private void ShowActionEditor(long id, int width = 500)
        {
            var action = _currPromotion.GetAction(id);
            Promotions_Edit_Actions1.LoadAction(_currPromotion, action);
            pnlEditAction.Visible = true;
            pnlEditQualification.Visible = false;

            ScriptManager.RegisterStartupScript(Page, GetType(), "hcShowActionDialog",
                "hcShowActionDialog(" + width + ");", true);
        }

        private bool SaveActionEditor()
        {
            return Promotions_Edit_Actions1.SaveAction();
        }

        private void CloseDialogs()
        {
            pnlEditQualification.Visible = false;
            pnlEditAction.Visible = false;
        }

        private string BackUrl()
        {
            return "promotions.aspx?page=" + Request.QueryString["page"];
        }

        private Promotion GetCurrentPromotion()
        {
            var pid = Request.QueryString["id"].ConvertTo<long>(0);
            return HccApp.MarketingServices.Promotions.Find(pid);
        }

        #endregion
    }
}