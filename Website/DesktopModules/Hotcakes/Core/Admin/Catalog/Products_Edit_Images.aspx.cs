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
using System.IO;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Products_Edit_Images : BaseProductPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            PageTitle = Localization.GetString("AdditionalImages");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAdd.Click += btnAdd_Click;
            gvAditionalImages.RowDataBound += gvAditionalImages_RowDataBound;
            gvAditionalImages.RowDeleting += gvAditionalImages_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadItems();
            }
        }

        protected void gvAditionalImages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var imgImage = e.Row.FindControl("imgImage") as Image;
                var litFileName = e.Row.FindControl("litFileName") as Literal;

                var productImage = e.Row.DataItem as ProductImage;

                imgImage.ImageUrl = DiskStorage.ProductAdditionalImageUrlTiny(HccApp,
                    productImage.ProductId,
                    productImage.Bvin,
                    productImage.FileName,
                    HccApp.IsCurrentRequestSecure());
                imgImage.AlternateText = productImage.AlternateText;
                litFileName.Text = productImage.FileName;

                e.Row.Attributes["id"] = productImage.Bvin;
            }
        }

        protected void gvAditionalImages_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var imageId = (string) gvAditionalImages.DataKeys[e.RowIndex].Value;
            HccApp.CatalogServices.ProductImageDelete(imageId);

            LoadItems();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (fuImageUpload.HasFile)
                {
                    var fileName = Path.GetFileNameWithoutExtension(fuImageUpload.FileName);
                    var ext = Path.GetExtension(fuImageUpload.FileName);

                    if (DiskStorage.ValidateImageType(ext))
                    {
                        fileName = Text.CleanFileName(fileName);

                        var img = new ProductImage();
                        img.Bvin = Guid.NewGuid().ToString();

                        if (DiskStorage.UploadAdditionalProductImage(HccApp.CurrentStore.Id,
                            ProductId,
                            img.Bvin,
                            fuImageUpload.PostedFile))
                        {
                            img.AlternateText = fileName + ext;
                            img.FileName = fileName + ext;
                            img.Caption = string.Empty;
                            img.ProductId = ProductId;
                            if (HccApp.CatalogServices.ProductImageCreate(img))
                            {
                                ucMessageBox.ShowOk(string.Format(Localization.GetString("NewImageAdded"), DateTime.Now));
                            }
                            else
                            {
                                ucMessageBox.ShowError(Localization.GetString("ImageSaveError"));
                            }
                        }
                        else
                        {
                            ucMessageBox.ShowError(Localization.GetString("ImageSaveError"));
                        }

                        LoadItems();
                    }
                    else
                    {
                        ucMessageBox.ShowError(Localization.GetString("ImageFileTypeError"));
                    }
                }
                else
                {
                    ucMessageBox.ShowError(Localization.GetString("NoFileFound"));
                }
            }
        }

        private void LoadItems()
        {
            var aditionalImages = HccApp.CatalogServices.ProductImages.FindByProductId(ProductId);

            gvAditionalImages.DataSource = aditionalImages;
            gvAditionalImages.DataBind();
        }
    }
}