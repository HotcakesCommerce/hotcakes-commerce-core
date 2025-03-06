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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    /// <summary>
    ///     This class manages all of the primary aspects of managing a single product in the store
    /// </summary>
    partial class Products_Edit : BaseProductPage
    {
        #region Fields

        private const string ProductsUrl = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/Default.aspx";
        private const string ProductEditUrl = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/Products_Edit.aspx";

        protected Dictionary<long, string> _productTypeProperties = new Dictionary<long, string>();

        private string LastProductType
        {
            get { return ViewState["LastProductType"] as string ?? string.Empty; }
            set { ViewState["LastProductType"] = value; }
        }

        private bool IsNew
        {
            get { return ProductNavigator.IsNew; }
        }

        #endregion

        #region Event handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetDefaultValues();
            rbBehaviour.SelectedIndexChanged += rbBehaviour_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
           
            PageMessageBox = ucMessageBox;

            // TODO: troubleshoot taxonomy to see why it's not saving and loading
            divTaxonomyBlock.Visible = false;

            if (!Page.IsPostBack)
            {
                PopulateTemplates();
                PopulateManufacturers();
                PopulateVendors();
                PopulateProductTypes();
                PopulateColumns();
                PopulateTaxes();
                SkuField.Focus();

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

                if (!string.IsNullOrEmpty(ProductId))
                {
                    LoadProduct();
                    if (Request.QueryString["u"] == "1")
                    {
                        ucMessageBox.ShowOk(Localization.GetString("ProductUpdateSuccess"));
                    }
                }
                var props = HccApp.CatalogServices.ProductPropertiesFindForType(lstProductType.SelectedValue);
                foreach (var prop in props)
                {
                    var propertyValue = HccApp.CatalogServices.ProductPropertyValues.GetPropertyValue(ProductId, prop,
                        true);
                    _productTypeProperties.Add(prop.Id, propertyValue);
                }
                GenerateProductTypePropertyFields();
                UrlsAssociated1.ObjectId = ProductId;
                UrlsAssociated1.LoadUrls();
            }
            else
            {
                //this is a postback
                var props = HccApp.CatalogServices.ProductPropertiesFindForType(lstProductType.SelectedValue);
                foreach (var prop in props)
                {
                    var propertyValue = Request["ProductTypeProperty" + prop.Id];
                    _productTypeProperties.Add(prop.Id, propertyValue);
                }

                CheckIfProductTypePropertyChanged();
                GenerateProductTypePropertyFields();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsNew)
            {
                lnkViewInStore.Enabled = false;
                lnkViewInStore.CssClass += " hcDisabled";
                lnkClone.Enabled = false;
                lnkClone.CssClass += " hcDisabled";
                btnDelete.Enabled = false;
                btnDelete.OnClientClick = string.Empty;
                btnDelete.CssClass += " hcDisabled";
            }
        }


        /// <summary>
        ///     Handles the SelectedIndexChanged event of the rbBehaviour control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void rbBehaviour_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isBundle = rbBehaviour.SelectedValue == "B";
            var isGC = rbBehaviour.SelectedValue == "GC";
            ddlShipType.Enabled = !isBundle;
            ProductNavigator.IsBundle = isBundle;
            pnlPricing.Visible = !isGC;

            var p = HccApp.CatalogServices.Products.Find(ProductId);
            if (p != null)
            {
                if (p.IsBundle != isBundle)
                {
                    p.IsBundle = isBundle;
                    if (!isBundle)
                    {
                        // get rid of orphaned bundle data
                        HccApp.CatalogServices.BundledProducts.DeleteAllForProduct(p.Bvin);
                    }

                    // update the product
                    HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                Response.Redirect(ProductsUrl);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                Response.Redirect(ProductEditUrl + "?id=" + ProductId + "&u=1");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ProductsUrl);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!HccApp.CatalogServices.DestroyProduct(ProductId, HccApp.CurrentStore.Id))
            {
                ucMessageBox.ShowWarning(Localization.GetString("ProductDeleteFailure"));
            }
            else
            {
                Response.Redirect(ProductsUrl);
            }
        }

        protected void ProductTypeCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var props = HccApp.CatalogServices.ProductPropertiesFindForType(lstProductType.SelectedValue);
            foreach (var prop in props)
            {
                if (_productTypeProperties.ContainsKey(prop.Id))
                    continue;
                switch (prop.TypeCode)
                {
                    case ProductPropertyType.CurrencyField:
                        decimal temp;
                        if (!decimal.TryParse(_productTypeProperties[prop.Id], out temp))
                        {
                            args.IsValid = false;
                            ProductTypeCustomValidator.ErrorMessage = string.Format(Localization.GetString("ProductTypeCustomValidator.ErrorMessage1"), prop.DisplayName);
                            return;
                        }
                        break;
                    case ProductPropertyType.DateField:
                        DateTime temp2;
                        if (
                            !DateTime.TryParse(_productTypeProperties[prop.Id], CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out temp2))
                        {
                            args.IsValid = false;
                            ProductTypeCustomValidator.ErrorMessage = string.Format(Localization.GetString("ProductTypeCustomValidator.ErrorMessage2"), prop.DisplayName);
                            return;
                        }
                        break;
                    case ProductPropertyType.MultipleChoiceField:
                        break;
                }
            }
        }

        #endregion

        #region Implementation

        protected bool Save()
        {
            var culture = Thread.CurrentThread.CurrentUICulture;
            var result = false;

            if (Page.IsValid)
            {
                var p = HccApp.CatalogServices.Products.Find(ProductId);
                if (p == null)
                    p = new Product();

                p.Status = chkActive.Checked ? ProductStatus.Active : ProductStatus.Disabled;
                p.IsSearchable = chkSearchable.Checked;
                p.AllowUpcharge = chkAllowUpcharge.Checked;

                p.UpchargeAmount = UpchargeAmountField.Text.ConvertTo(p.UpchargeAmount);

                p.UpchargeUnit = lstUpchargeUnitType.SelectedValue;

                if (p.UpchargeUnit == "0")
                {
                    p.UpchargeAmount = Money.RoundCurrency(p.UpchargeAmount);
                }
                
                var isBundle = rbBehaviour.SelectedValue == "B";
                var isGC = rbBehaviour.SelectedValue == "GC";
                p.IsBundle = isBundle;

                if (isGC && p.IsGiftCard != isGC && !string.IsNullOrEmpty(p.Bvin))
                {
                    HccApp.CatalogServices.BundledProducts.DeleteByBundleProductId(p.Bvin);
                }
                p.IsGiftCard = isGC;

                p.TemplateName = ddlTemplateList.SelectedValue;
                p.Featured = chkFeatured.Checked;

                if (string.Compare(p.Sku.Trim(), SkuField.Text.Trim(), true) != 0)
                {
                    //sku changed, so do a sku check
                    var prodGuid = DataTypeHelper.BvinToNullableGuid(p.Bvin);
                    if (HccApp.CatalogServices.Products.IsSkuExist(SkuField.Text.Trim(), prodGuid))
                    {
                        ucMessageBox.ShowError(Localization.GetString("SkuExists"));
                        return false;
                    }
                }

                p.Sku = SkuField.Text.Trim();

                p.ProductName = txtProductName.Text.Trim();
                p.ProductTypeId = lstProductType.SelectedValue ?? string.Empty;

                p.IsUserSuppliedPrice = chkUserPrice.Checked;
                p.HideQty = chkHideQty.Checked;
                p.UserSuppliedPriceLabel = txtUserPriceLabel.Text;

                if (p.IsUserSuppliedPrice || isGC)
                {
                    p.ListPrice = 0;
                    p.SiteCost = 0;
                    p.SitePrice = 0;
                    p.SitePriceOverrideText = string.Empty;
                }
                else
                {
                    decimal listPrice;
                    if (decimal.TryParse(ListPriceField.Text, NumberStyles.Currency, culture, out listPrice))
                    {
                        p.ListPrice = Money.RoundCurrency(listPrice);
                    }
                    decimal cost;
                    if (decimal.TryParse(CostField.Text, NumberStyles.Currency, Thread.CurrentThread.CurrentUICulture,
                        out cost))
                    {
                        p.SiteCost = Money.RoundCurrency(cost);
                    }
                    decimal price;
                    if (decimal.TryParse(SitePriceField.Text, NumberStyles.Currency,
                        Thread.CurrentThread.CurrentUICulture, out price))
                    {
                        p.SitePrice = Money.RoundCurrency(price);
                    }
                    p.SitePriceOverrideText = PriceOverrideTextBox.Text.Trim();
                }
                p.LongDescription = LongDescriptionField.Text.Trim();
                p.ManufacturerId = lstManufacturers.SelectedValue ?? string.Empty;
                p.VendorId = lstVendors.SelectedValue ?? string.Empty;


                if (string.IsNullOrEmpty(SmallImageAlternateTextField.Text))
                {
                    p.ImageFileSmallAlternateText = p.ProductName + " " + p.Sku;
                }
                else
                {
                    p.ImageFileSmallAlternateText = SmallImageAlternateTextField.Text;
                }
                if (string.IsNullOrEmpty(MediumImageAlternateTextField.Text))
                {
                    p.ImageFileMediumAlternateText = p.ProductName + " " + p.Sku;
                }
                else
                {
                    p.ImageFileMediumAlternateText = MediumImageAlternateTextField.Text;
                }

                p.PreContentColumnId = ddlPreContentColumn.SelectedValue;
                p.PostContentColumnId = ddlPostContentColumn.SelectedValue;

                var oldUrl = p.UrlSlug;

                // no entry, generate one
                if (p.UrlSlug.Trim().Length < 1)
                {
                    p.UrlSlug = Text.Slugify(p.ProductName, false);
                }
                else
                {
                    p.UrlSlug = Text.Slugify(txtRewriteUrl.Text, false);
                }

                if (UrlRewriter.IsProductSlugInUse(p.UrlSlug, p.Bvin, HccApp))
                {
                    ucMessageBox.ShowWarning(Localization.GetString("UrlExists"));
                    return false;
                }

                p.Keywords = txtKeywords.Text.Trim();
                p.ProductTypeId = lstProductType.SelectedValue;
                if (!string.IsNullOrEmpty(lstTaxClasses.SelectedValue))
                    p.TaxSchedule = long.Parse(lstTaxClasses.SelectedValue);
                else
                    p.TaxSchedule = -1;

                p.MetaTitle = txtMetaTitle.Text.Trim();
                p.MetaDescription = txtMetaDescription.Text.Trim();
                p.MetaKeywords = txtMetaKeywords.Text.Trim();

                decimal weight = 0;
                decimal.TryParse(txtWeight.Text, NumberStyles.Float, Thread.CurrentThread.CurrentUICulture, out weight);
                p.ShippingDetails.Weight = weight;

                decimal length = 0;
                decimal.TryParse(txtLength.Text, NumberStyles.Float, Thread.CurrentThread.CurrentUICulture, out length);
                p.ShippingDetails.Length = length;

                decimal width = 0;
                decimal.TryParse(txtWidth.Text, NumberStyles.Float, Thread.CurrentThread.CurrentUICulture, out width);
                p.ShippingDetails.Width = width;

                decimal height = 0;
                decimal.TryParse(txtHeight.Text, NumberStyles.Float, Thread.CurrentThread.CurrentUICulture, out height);
                p.ShippingDetails.Height = height;

                p.ShippingDetails.ExtraShipFee =
                    Money.RoundCurrency(decimal.Parse(ExtraShipFeeField.Text, NumberStyles.Currency, culture));
                p.ShippingMode = (ShippingMode)int.Parse(ddlShipType.SelectedValue);
                p.ShippingDetails.IsNonShipping = chkNonShipping.Checked;
                p.ShippingDetails.ShipSeparately = chkShipSeparately.Checked;

                p.ShippingCharge = (ShippingChargeType)int.Parse(lstShippingCharge.SelectedValue);

                p.MinimumQty = int.Parse(txtMinimumQty.Text, NumberStyles.Integer, Thread.CurrentThread.CurrentUICulture);

                p.TaxExempt = TaxExemptField.Checked;

                p.GiftWrapAllowed = false;

                if (!string.IsNullOrWhiteSpace(rblAllowReviews.SelectedValue))
                    p.AllowReviews = bool.Parse(rblAllowReviews.SelectedValue);
                else
                    p.AllowReviews = null;
                p.SwatchPath = swatchpathfield.Text;

                if (ucImageUploadLarge.HasFile)
                {
                    var fileName = Text.CleanFileName(Path.GetFileName(ucImageUploadLarge.FileName));
                    p.ImageFileMedium = fileName;
                    p.ImageFileSmall = fileName;
                }

                if (string.IsNullOrEmpty(p.Bvin))
                {
                    result = HccApp.CatalogServices.ProductsCreateWithInventory(p, true);
                }
                else
                {
                    result = HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);
                }

                if (result)
                {
                    // Create taxonomy tags
                    var taxonomyTags = txtTaxonomyTags.Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    HccApp.SocialService.UpdateProductTaxonomy(p, taxonomyTags);

                    // Save images
                    UploadImage(p);

                    // Store propety values
                    var props = HccApp.CatalogServices.ProductPropertiesFindForType(lstProductType.SelectedValue);
                    foreach (var prop in props)
                    {
                        if (_productTypeProperties.ContainsKey(prop.Id))
                        {
                            HccApp.CatalogServices.ProductPropertyValues.SetPropertyValue(p.Bvin, prop,
                                _productTypeProperties[prop.Id]);
                        }
                    }

                    // Update bvin field so that next save will call updated instead of create
                    ProductId = p.Bvin;

                    if (!string.IsNullOrEmpty(oldUrl) && oldUrl != p.UrlSlug)
                    {
                        HccApp.ContentServices.CustomUrls.Register301(oldUrl,
                            p.UrlSlug,
                            p.Bvin, CustomUrlType.Product, HccApp.CurrentRequestContext, HccApp);
                        UrlsAssociated1.LoadUrls();
                    }
                }
                else
                {
                    ucMessageBox.ShowError(Localization.GetString("SaveProductFailure"));
                }
            }
            return result;
        }

        protected void SetDefaultValues()
        {
            decimal val = 0;
            chkUserPrice.Checked = false;
            ListPriceField.Text = val.ToString("c");
            CostField.Text = val.ToString("c");
            SitePriceField.Text = val.ToString("c");
            txtWeight.Text = val.ToString("N");
            txtLength.Text = val.ToString("N");
            txtWidth.Text = val.ToString("N");
            txtHeight.Text = val.ToString("N");
            ExtraShipFeeField.Text = val.ToString("c");
            LongDescriptionField.EditorHeight = WebAppSettings.ProductLongDescriptionEditorHeight;

            lstShippingCharge.SelectedIndex = 1;
        }

        private void PopulateTemplates()
        {
            ddlTemplateList.Items.Clear();
            ddlTemplateList.Items.Add(new ListItem(Localization.GetString("NotSet"), string.Empty));
            ddlTemplateList.AppendDataBoundItems = true;
            ddlTemplateList.DataSource = DnnPathHelper.GetViewNames("Products");
            ddlTemplateList.DataBind();
        }

        private void PopulateManufacturers()
        {
            lstManufacturers.DataSource = HccApp.ContactServices.Manufacturers.FindAll();
            lstManufacturers.DataTextField = "DisplayName";
            lstManufacturers.DataValueField = "Bvin";
            lstManufacturers.DataBind();
            lstManufacturers.Items.Insert(0, new ListItem(Localization.GetString("NoManufacturer"), string.Empty));
        }

        private void PopulateVendors()
        {
            lstVendors.DataSource = HccApp.ContactServices.Vendors.FindAll();
            lstVendors.DataTextField = "DisplayName";
            lstVendors.DataValueField = "Bvin";
            lstVendors.DataBind();
            lstVendors.Items.Insert(0, new ListItem(Localization.GetString("NoVendor"), string.Empty));
        }

        private void PopulateTaxes()
        {
            lstTaxClasses.DataSource = HccApp.OrderServices.TaxSchedules.FindAll(HccApp.CurrentStore.Id);
            lstTaxClasses.DataTextField = "Name";
            lstTaxClasses.DataValueField = "Id";
            lstTaxClasses.DataBind();
            lstTaxClasses.Items.Insert(0, new ListItem(Localization.GetString("NotSet"), string.Empty));
        }

        private void PopulateProductTypes()
        {
            lstProductType.Items.Clear();
            lstProductType.Items.Add(new ListItem(Localization.GetString("ProductTypeDefault"), string.Empty));
            lstProductType.AppendDataBoundItems = true;
            lstProductType.DataSource = HccApp.CatalogServices.ProductTypes.FindAll();
            lstProductType.DataTextField = "ProductTypeName";
            lstProductType.DataValueField = "bvin";
            lstProductType.DataBind();
        }

        private void PopulateColumns()
        {
            var columns = HccApp.ContentServices.Columns.FindAll();
            ddlPreContentColumn.Items.Add(new ListItem(Localization.GetString("NotSet"), string.Empty));
            ddlPostContentColumn.Items.Add(new ListItem(Localization.GetString("NotSet"), string.Empty));
            foreach (var col in columns)
            {
                ddlPreContentColumn.Items.Add(new ListItem(col.DisplayName, col.Bvin));
                ddlPostContentColumn.Items.Add(new ListItem(col.DisplayName, col.Bvin));
            }
        }

        private void LoadProduct()
        {
            var p = HccApp.CatalogServices.Products.Find(ProductId);
            if (p != null)
            {
                chkActive.Checked = p.Status == ProductStatus.Active;
                chkSearchable.Checked = p.IsSearchable;
                chkAllowUpcharge.Checked = p.AllowUpcharge;

                if (p.IsBundle)
                {
                    rbBehaviour.SelectedValue = "B";
                }
                else if (p.IsGiftCard)
                {
                    rbBehaviour.SelectedValue = "GC";
                    pnlPricing.Visible = false;
                }

                if (ddlTemplateList.Items.FindByValue(p.TemplateName) != null)
                {
                    ddlTemplateList.ClearSelection();
                    ddlTemplateList.Items.FindByValue(p.TemplateName).Selected = true;
                }

                chkFeatured.Checked = p.Featured;

                SkuField.Text = p.Sku;
                txtProductName.Text = p.ProductName;

                chkUserPrice.Checked = p.IsUserSuppliedPrice;
                chkHideQty.Checked = p.HideQty;
                txtUserPriceLabel.Text = p.UserSuppliedPriceLabel;

                if (!p.IsUserSuppliedPrice)
                {
                    ListPriceField.Enabled = true;
                    CostField.Enabled = true;
                    SitePriceField.Enabled = true;
                    ListPriceField.Text = p.ListPrice.ToString("C");
                    CostField.Text = p.SiteCost.ToString("C");
                    SitePriceField.Text = p.SitePrice.ToString("C");
                    PriceOverrideTextBox.Text = p.SitePriceOverrideText;
                }
                else
                {
                    ListPriceField.Enabled = false;
                    CostField.Enabled = false;
                    SitePriceField.Enabled = false;
                    PriceOverrideTextBox.Enabled = false;
                    ListPriceField.Text = string.Empty;
                    CostField.Text = string.Empty;
                    SitePriceField.Text = string.Empty;
                    PriceOverrideTextBox.Text = string.Empty;
                    rfvCostField.Enabled = false;
                    rfvListPrice.Enabled = false;
                    rfvSitePrice.Enabled = false;
                }

                LongDescriptionField.Text = p.LongDescription;

                TaxExemptField.Checked = p.TaxExempt;
                if (lstTaxClasses.Items.FindByValue(p.TaxSchedule.ToString()) != null)
                {
                    lstTaxClasses.ClearSelection();
                    lstTaxClasses.Items.FindByValue(p.TaxSchedule.ToString()).Selected = true;
                }
                if (lstManufacturers.Items.FindByValue(p.ManufacturerId) != null)
                {
                    lstManufacturers.ClearSelection();
                    lstManufacturers.Items.FindByValue(p.ManufacturerId).Selected = true;
                }
                if (lstVendors.Items.FindByValue(p.VendorId) != null)
                {
                    lstVendors.ClearSelection();
                    lstVendors.Items.FindByValue(p.VendorId).Selected = true;
                }
                if (lstUpchargeUnitType.Items.Count == 0)
                {
                    lstUpchargeUnitType.Items.Add(new ListItem(UpchargeAmountTypesDTO.Amount.ToString(), ((int)UpchargeAmountTypesDTO.Amount).ToString()));
                    lstUpchargeUnitType.Items.Add(new ListItem(UpchargeAmountTypesDTO.Percent.ToString(), ((int)UpchargeAmountTypesDTO.Percent).ToString())); 
                }

                UpchargeAmountField.Text = p.UpchargeAmount % 1 == 0 ? ((int)p.UpchargeAmount).ToString() : p.UpchargeAmount.ToString("0.##");

                lstUpchargeUnitType.SelectedValue = Enum.TryParse<AmountTypes>(p.UpchargeUnit, out var upchargeType) ? ((int)upchargeType).ToString() : "0";

                LoadImagePreview(p);
                if (string.IsNullOrEmpty(p.ImageFileSmallAlternateText))
                {
                    p.ImageFileSmallAlternateText = p.ProductName + " " + p.Sku;
                }
                SmallImageAlternateTextField.Text = p.ImageFileSmallAlternateText;
                if (string.IsNullOrEmpty(p.ImageFileMediumAlternateText))
                {
                    p.ImageFileMediumAlternateText = p.ProductName + " " + p.Sku;
                }
                MediumImageAlternateTextField.Text = p.ImageFileMediumAlternateText;

                if (lstProductType.Items.FindByValue(p.ProductTypeId) != null)
                {
                    lstProductType.ClearSelection();
                    lstProductType.Items.FindByValue(p.ProductTypeId).Selected = true;
                }
                // Added this line to stop errors on immediate load and save - Marcus
                LastProductType = p.ProductTypeId;

                if (!string.IsNullOrWhiteSpace(p.PreContentColumnId))
                {
                    if (ddlPreContentColumn.Items.FindByValue(p.PreContentColumnId) != null)
                    {
                        ddlPreContentColumn.Items.FindByValue(p.PreContentColumnId).Selected = true;
                    }
                }
                if (!string.IsNullOrWhiteSpace(p.PostContentColumnId))
                {
                    if (ddlPostContentColumn.Items.FindByValue(p.PostContentColumnId) != null)
                    {
                        ddlPostContentColumn.Items.FindByValue(p.PostContentColumnId).Selected = true;
                    }
                }

                txtKeywords.Text = p.Keywords;
                txtMetaTitle.Text = p.MetaTitle;
                txtMetaDescription.Text = p.MetaDescription;
                txtMetaKeywords.Text = p.MetaKeywords;

                txtWeight.Text = Math.Round(p.ShippingDetails.Weight, 3).ToString();
                txtLength.Text = Math.Round(p.ShippingDetails.Length, 3).ToString();
                txtWidth.Text = Math.Round(p.ShippingDetails.Width, 3).ToString();
                txtHeight.Text = Math.Round(p.ShippingDetails.Height, 3).ToString();

                ExtraShipFeeField.Text = p.ShippingDetails.ExtraShipFee.ToString("C");
                if (ddlShipType.Items.FindByValue(((int)p.ShippingMode).ToString()) != null)
                {
                    ddlShipType.ClearSelection();
                    ddlShipType.Items.FindByValue(((int)p.ShippingMode).ToString()).Selected = true;
                }
                if (lstShippingCharge.Items.FindByValue(((int)p.ShippingCharge).ToString()) != null)
                {
                    lstShippingCharge.ClearSelection();
                    lstShippingCharge.Items.FindByValue(((int)p.ShippingCharge).ToString()).Selected = true;
                }
                chkNonShipping.Checked = p.ShippingDetails.IsNonShipping;
                chkShipSeparately.Checked = p.ShippingDetails.ShipSeparately;

                txtMinimumQty.Text = Math.Round((decimal)p.MinimumQty, 0).ToString();

                txtRewriteUrl.Text = p.UrlSlug;

                lnkViewInStore.NavigateUrl = UrlRewriter.BuildUrlForProduct(p);

                lnkClone.NavigateUrl = "ProductClone.aspx?id=" + p.Bvin;

                if (p.AllowReviews.HasValue)
                    rblAllowReviews.SelectedValue = p.AllowReviews.ToString();
                else
                    rblAllowReviews.SelectedValue = string.Empty;

                swatchpathfield.Text = p.SwatchPath;

                txtTaxonomyTags.Text = string.Join(",", HccApp.SocialService.GetTaxonomyTerms(p));
            }
        }

        private void LoadImagePreview(Product p)
        {
            ucImageUploadLarge.ImageUrl = DiskStorage.ProductImageUrlMedium(HccApp, p.Bvin, p.ImageFileMedium,
                HccApp.IsCurrentRequestSecure());
            imgPreviewSmall.ImageUrl = DiskStorage.ProductImageUrlSmall(HccApp, p.Bvin, p.ImageFileSmall,
                HccApp.IsCurrentRequestSecure());
        }

        private void UploadImage(Product p)
        {
            if (ucImageUploadLarge.HasFile)
            {
                var fileName = Text.CleanFileName(Path.GetFileName(ucImageUploadLarge.FileName));

                if (
                    !DiskStorage.CopyProductImage(HccApp.CurrentStore.Id, p.Bvin, ucImageUploadLarge.TempImagePath,
                        fileName))
                {
                    ucMessageBox.ShowError(Localization.GetString("ImageFileTypeError"));
                }
            }
        }

        protected void CheckIfProductTypePropertyChanged()
        {
            if (lstProductType.SelectedValue != LastProductType)
            {
                _productTypeProperties.Clear();
                LastProductType = lstProductType.SelectedValue;
            }
        }

        protected void GenerateProductTypePropertyFields()
        {
            litProductTypeProperties.Text = string.Empty;
            var productTypeBvin = lstProductType.SelectedValue;
            if (!string.IsNullOrWhiteSpace(productTypeBvin))
            {
                var props = HccApp.CatalogServices.ProductPropertiesFindForType(productTypeBvin);
                var sb = new StringBuilder();
                foreach (var item in props)
                {
                    sb.Append("<div class=\"hcFormItem\">");
                    sb.Append("<label class=\"hcLabel\">");
                    sb.Append(item.DisplayName);
                    if (item.IsLocalizable)
                        sb.Append("<i class=\"hcLocalizable\"></i>");
                    sb.Append("</label>");

                    string propertyValue = null;
                    if (_productTypeProperties.ContainsKey(item.Id))
                        propertyValue = _productTypeProperties[item.Id];
                    var controlString = GeneratePropertyControl(item, propertyValue);
                    sb.Append(controlString);

                    sb.Append("</div>");
                }
                litProductTypeProperties.Text = sb.ToString();
            }
        }

        private string GeneratePropertyControl(ProductProperty item, string propertyValue)
        {
            using (var sw = new StringWriter())
            {
                using (var writer = new HtmlTextWriter(sw))
                {
                    if (item.TypeCode == ProductPropertyType.CurrencyField)
                    {
                        var input = new HtmlInputText();
                        input.ID = "ProductTypeProperty" + item.Id;
                        if (propertyValue != null)
                        {
                            input.Value = propertyValue;
                        }
                        else
                        {
                            input.Value = item.DefaultValue;
                        }
                        input.RenderControl(writer);
                    }
                    else if (item.TypeCode == ProductPropertyType.DateField)
                    {
                        var input = new HtmlInputText();
                        input.ID = "ProductTypeProperty" + item.Id;
                        if (propertyValue != null)
                        {
                            input.Value = propertyValue;
                        }
                        else
                        {
                            input.Value = item.DefaultValue;
                        }
                        input.RenderControl(writer);
                    }
                    else if (item.TypeCode == ProductPropertyType.HyperLink)
                    {
                        var input = new HtmlInputText();
                        input.ID = "ProductTypeProperty" + item.Id;
                        if (propertyValue != null)
                        {
                            input.Value = propertyValue;
                        }
                        else
                        {
                            input.Value = item.DefaultValue;
                        }
                        input.RenderControl(writer);
                    }
                    else if (item.TypeCode == ProductPropertyType.MultipleChoiceField)
                    {
                        var input = new HtmlSelect();
                        input.ID = "ProductTypeProperty" + item.Id;
                        foreach (var choice in item.Choices)
                        {
                            var li = new ListItem(choice.DisplayName, choice.Id.ToString());
                            input.Items.Add(li);
                        }

                        if (propertyValue != null)
                        {
                            input.Value = propertyValue;
                        }
                        else
                        {
                            input.Value = item.DefaultValue;
                        }
                        input.RenderControl(writer);
                    }
                    else if (item.TypeCode == ProductPropertyType.TextField)
                    {
                        var input = new HtmlTextArea();
                        input.ID = "ProductTypeProperty" + item.Id;
                        var defaultValue = item.IsLocalizable ? item.DefaultLocalizableValue : item.DefaultValue;
                        if (propertyValue != null)
                        {
                            input.Value = propertyValue;
                        }
                        else
                        {
                            input.Value = defaultValue;
                        }
                        input.Rows = 5;
                        input.Cols = 40;
                        input.RenderControl(writer);
                    }
                    else if (item.TypeCode == ProductPropertyType.FileUpload)
                    {
                        //TODO:
                    }

                    writer.Flush();
                    return sw.ToString();
                }
            }
        }

        #endregion
    }
}