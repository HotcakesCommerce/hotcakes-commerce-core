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
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.CategoryRotator
{
    partial class Editor : HccContentBlockPart
    {
        private ContentBlock _block;

        private ContentBlock Block
        {
            get
            {
                return _block ??
                       (_block = HccApp.ContentServices.Columns.FindBlock(BlockId));
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ucCategoryPicker.AddSelectedIds += ucCategoryPicker_AddSelectedIds;
            gvCategories.RowDataBound += gvCategories_RowDataBound;
        }

        private void gvCategories_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var bvin = (string) gvCategories.DataKeys[e.Row.DataItemIndex].Value;
                var settings = Block.Lists.FindList("Categories");
                var item = settings.FirstOrDefault(s => s.Setting1 == bvin);
                e.Row.Attributes["id"] = item.Id;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void ucCategoryPicker_AddSelectedIds(object sender, UIEventArgs<string> e)
        {
            var block = HccApp.ContentServices.Columns.FindBlock(BlockId);

            if (block != null)
            {
                var settings = block.Lists.FindList("Categories");

                foreach (var category in e.Items)
                {
                    var add = true;
                    foreach (var item in settings)
                    {
                        if (item.Setting1 == category)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        var c = new ContentBlockSettingListItem
                        {
                            Setting1 = category,
                            ListName = "Categories"
                        };
                        block.Lists.AddItem(c);
                        HccApp.ContentServices.Columns.UpdateBlock(block);
                    }
                }

                BindCategoryGridView(block);
            }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            if (b != null)
            {
                b.BaseSettings.SetBoolSetting("ShowInOrder", chkShowInOrder.Checked);
                HccApp.ContentServices.Columns.UpdateBlock(b);
            }
            NotifyFinishedEditing();
        }

        protected void gvCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            var settings = b.Lists.FindList("Categories");
            foreach (var item in settings)
            {
                if (item.Setting1 == (string) gvCategories.DataKeys[e.RowIndex].Value)
                {
                    b.Lists.RemoveItem(item.Id);
                }
            }
            HccApp.ContentServices.Columns.UpdateBlock(b);
            BindCategoryGridView(b);
        }

        private void BindCategoryGridView(ContentBlock b)
        {
            var settings = b.Lists.FindList("Categories");
            var categories = new Collection<Category>();
            foreach (var item in settings)
            {
                var category = HccApp.CatalogServices.Categories.Find(item.Setting1);
                if (category != null && category.Bvin != string.Empty)
                {
                    categories.Add(category);
                }
            }
            gvCategories.DataSource = categories;
            gvCategories.DataBind();

            ucCategoryPicker.ReservedIds = categories.Select(c => c.Bvin).ToList();
            ucCategoryPicker.BindCategories();
        }

        private void LoadData()
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            if (b != null)
            {
                chkShowInOrder.Checked = b.BaseSettings.GetBoolSetting("ShowInOrder");
            }
            BindCategoryGridView(b);
        }
    }
}