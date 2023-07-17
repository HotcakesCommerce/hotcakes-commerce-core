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
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;
using ErrorTypes = Hotcakes.Modules.Core.Admin.AppCode.ErrorTypes;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Default : BaseAdminPage
    {
        #region Fields

        protected int RowCount;

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Products";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PageMessageBox = ucMessageBox;

            ucSimpleProductFilter.GoPressed += SimpleProductFilter_GoPressed;
            ucSimpleProductFilter.FilterChanged += SimpleProductFilter_FilterChanged;
            gvProducts.RowDataBound += gvProducts_RowDataBound;
            gvProducts.RowDeleting += gvProducts_RowDeleting;
            lnkExport.Click += lnkExport_Click;
        }

        private void lnkExport_Click(object sender, EventArgs e)
        {
            var criteria = ucSimpleProductFilter.LoadProductCriteria();
            criteria.DisplayInactiveProducts = true;
            var productsCount = HccApp.CatalogServices.Products.FindCountByCriteria(criteria);

            if (productsCount < 250)
            {
                var products = HccApp.CatalogServices.Products.FindByCriteria(criteria, 1, int.MaxValue, ref RowCount,
                    false);
                var export = new CatalogExport(HccApp);
                export.ExportToExcel(products, "Hotcakes_Products.xlsx", Response);
            }
            else
            {
                var asyncTask = Task.Factory.StartNew(
                    DoExport,
                    new ExportConfiguration
                    {
                        HccRequestContext = HccRequestContext.Current,
                        HttpContext = Context,
                        DnnPortalSettings = PortalSettings.Current,
                        Criteria = criteria
                    });

                ucMessageBox.ShowInformation(Localization.GetString("ExportInProgress"));
            }
        }

        protected void DoExport(object objConfiguration)
        {
            try
            {
                var conf = objConfiguration as ExportConfiguration;

                HccRequestContext.Current = conf.HccRequestContext;
                DnnGlobal.SetPortalSettings(conf.DnnPortalSettings);
                Factory.HttpContext = conf.HttpContext;
                CultureSwitch.SetCulture(HccApp.CurrentStore, conf.DnnPortalSettings);

                var products = HccApp.CatalogServices.Products.FindByCriteria(conf.Criteria, 1, int.MaxValue,
                    ref RowCount, false);

                var export = new CatalogExport(HccApp);
                var fileName = string.Format("Hotcakes_Products_{0}_{1:yyyyMMddhhMMss}.xlsx", HccApp.CurrentCustomerId,
                    DateTime.UtcNow);
                var filePath = DiskStorage.GetStoreDataPhysicalPath(HccApp.CurrentStore.Id, "Exports/" + fileName);
                export.ExportToExcel(products, filePath);

                var pageLink = DiskStorage.GetHccAdminUrl(HccApp, "catalog/default.aspx", false);
                var mailMessage = new MailMessage(conf.DnnPortalSettings.Email, HccApp.CurrentCustomer.Email);
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = Localization.GetFormattedString("ExportProductsMailBody", pageLink);
                mailMessage.Subject = Localization.GetString("ExportProductsMailSubject");
                MailServices.SendMail(mailMessage, HccApp.CurrentStore);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadProducts();
                ucSimpleProductFilter.Focus();

                var locales = Factory.Instance.CreateStoreSettingsProvider().GetLocales();

                if (locales != null && locales.Count > 1)
                {
                    lnkImport.Visible = false;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            LoadExistingExports();
        }

        private void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var prod = e.Row.DataItem as Product;
                var lnkImage = e.Row.FindControl("lnkImage") as HyperLink;
                var lnkEdit = e.Row.FindControl("lnkEdit") as HyperLink;

                var editUrl = string.Format("Products_Edit.aspx?id={0}", prod.Bvin);
                var perfUrl = string.Format("Products_Performance.aspx?id={0}", prod.Bvin);
                lnkImage.NavigateUrl = perfUrl;
                lnkImage.ToolTip = prod.ImageFileSmallAlternateText;
                lnkImage.Style["display"] = "block";
                lnkImage.Style["max-height"] = "80px";
                lnkImage.Style["overflow"] = "hidden";
                lnkEdit.NavigateUrl = editUrl;

                var imageUrl = DiskStorage.ProductImageUrlSmall(((IHccPage) Page).HccApp, prod.Bvin, prod.ImageFileSmall,
                    Page.Request.IsSecureConnection);
                var img = lnkImage.Controls[0] as Image;
                img.ImageUrl = imageUrl;
            }
        }

        private void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var bvin = e.Keys[0] as string;

            if (HccApp.CatalogServices.DestroyProduct(bvin, HccApp.CurrentStore.Id))
            {
                LoadProducts();
            }
            else
            {
                ShowMessage("Unable to delete product. Unknown Error.", ErrorTypes.Warning);
            }
        }

        private void SimpleProductFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void SimpleProductFilter_GoPressed(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void lnkAddSamples_Click(object sender, EventArgs e)
        {
            SampleData.CreateSampleAnalyticsForStoreAsync();
            ShowMessage("Samples Added!", ErrorTypes.Ok);
            LoadProducts();
        }

        protected void btnRemoveSamples_Click(object sender, EventArgs e)
        {
            SampleData.RemoveSampleProductsFromStore();
            ShowMessage("Samples Removed!", ErrorTypes.Ok);
            LoadProducts();
        }

        protected void btnDownloadExportedFile_Command(object sender, CommandEventArgs e)
        {
            var filePath = (string) e.CommandArgument;
            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                Response.Clear();
                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", fileInfo.Name));
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                Response.WriteFile(filePath);
                Response.Flush();

                File.Delete(filePath);

                Response.End();
            }
            else
            {
                ucMessageBox.ShowError(Localization.GetString("FileDoesNotExist"));
            }
        }

        #endregion

        #region Implementation

        private void LoadProducts()
        {
            var criteria = ucSimpleProductFilter.LoadProductCriteria();
            criteria.DisplayInactiveProducts = true;
            var products = HccApp.CatalogServices.Products.FindByCriteria(criteria, ucPager.PageNumber, ucPager.PageSize,
                ref RowCount);
            if (products.Count == 0 && ucPager.PageNumber != 1)
                Response.Redirect("Default.aspx");
            ucPager.SetRowCount(RowCount);

            gvProducts.DataSource = products;
            gvProducts.DataBind();

            // Show sample panel if no products in store
            var allProducts = HccApp.CatalogServices.Products.FindAllCount();
            pnlSamples.Visible = allProducts < 1;

            // show the remove sample data panel only if sample data exists
            btnRemoveSamples.Visible = SampleData.SampleStoreDataExists();
        }

        private void LoadExistingExports()
        {
            var folderPath = DiskStorage.GetStoreDataPhysicalPath(HccApp.CurrentStore.Id, "Exports");
            var fileNamePattern = string.Format("Hotcakes_Products_{0}_*.xlsx", HccApp.CurrentCustomerId);
            var files = PathHelper.ListFiles(folderPath, new[] {fileNamePattern}, new string[] {});
            var exportsExists = files.Count > 0;
            pnlExports.Visible = exportsExists;


            var fileInfos = files.Select(f => new FileInfo(f)).ToList();
            rptExportedFiles.DataSource = fileInfos;
            rptExportedFiles.DataBind();
        }

        protected string GetProductPrice(IDataItemContainer cont)
        {
            var prod = cont.DataItem as Product;
            if (prod.IsGiftCard)
            {
                return "gift card";
            }
            if (prod.IsUserSuppliedPrice)
            {
                return "user price";
            }
            return prod.SitePrice.ToString("C");
        }

        private class ExportConfiguration
        {
            public HccRequestContext HccRequestContext { get; set; }
            public HttpContext HttpContext { get; set; }
            public PortalSettings DnnPortalSettings { get; set; }
            public ProductSearchCriteria Criteria { get; set; }
        }

        #endregion
    }
}