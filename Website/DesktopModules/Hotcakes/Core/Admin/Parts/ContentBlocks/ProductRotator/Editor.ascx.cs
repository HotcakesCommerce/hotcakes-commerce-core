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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ProductRotator
{
    partial class Editor : HccContentBlockPart
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvProducts.RowDataBound += gvProducts_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
                LoadItems(b);
            }
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var imgProduct = e.Row.FindControl("imgProduct") as Image;
                var litProductName = e.Row.FindControl("litProductName") as Literal;

                var li = e.Row.DataItem as ContentBlockSettingListItem;
                var product = HccApp.CatalogServices.Products.FindWithCache(li.Setting1);
                if (product != null)
                {
                    imgProduct.ImageUrl = DiskStorage.ProductImageUrlSmall(HccApp, product.Bvin, product.ImageFileSmall,
                        Page.Request.IsSecureConnection);
                    litProductName.Text = product.ProductName;
                }

                e.Row.Attributes["id"] = li.Id;
            }
        }

        private void LoadItems(ContentBlock b)
        {
            gvProducts.DataSource = b.Lists.FindList("Products");
            gvProducts.DataBind();
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnAddProducts_Click(object sender, EventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);

            foreach (var prodBvin in ucProductPicker.SelectedProducts)
            {
                if (!b.Lists.Items.Any(si => si.Setting1 == prodBvin && si.ListName == "Products"))
                {
                    var sett = new ContentBlockSettingListItem
                    {
                        Setting1 = prodBvin,
                        ListName = "Products"
                    };
                    b.Lists.AddItem(sett);
                    HccApp.ContentServices.Columns.UpdateBlock(b);
                }
            }

            LoadItems(b);
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            var bvin = (string) gvProducts.DataKeys[e.RowIndex].Value;
            b.Lists.RemoveItem(bvin);
            HccApp.ContentServices.Columns.UpdateBlock(b);
            LoadItems(b);
        }
    }
}