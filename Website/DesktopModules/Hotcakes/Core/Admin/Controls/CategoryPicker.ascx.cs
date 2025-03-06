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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class CategoryPicker : HccUserControl
    {
        #region Properties

        public event EventHandler<UIEventArgs<string>> AddSelectedIds;

        public IList<string> ReservedIds { get; set; }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            lnkAddSelected.Click += lnkAddSelected_Click;
            CategoriesGridView.RowCommand += CategoriesGridView_RowCommand;
            CategoriesGridView.RowDataBound += CategoriesGridView_RowDataBound;
        }

        private void CategoriesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!IsEnabled(e.Row))
                {
                    e.Row.CssClass = "hcDisabled";
                }
            }
        }

        private void CategoriesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Add")
            {
                if (AddSelectedIds != null)
                {
                    var index = Convert.ToInt32(e.CommandArgument);
                    var id = (string) CategoriesGridView.DataKeys[index].Value;
                    AddSelectedIds(this, new UIEventArgs<string> {Items = new List<string> {id}});
                }
            }
        }

        private void lnkAddSelected_Click(object sender, EventArgs e)
        {
            if (AddSelectedIds != null)
            {
                var ids = GetSelectedIds();
                AddSelectedIds(this, new UIEventArgs<string> {Items = ids});
            }
        }

        #endregion

        #region Implementation

        public void BindCategories()
        {
            CategoriesGridView.DataSource = CategoriesHelper.ListFullTreeWithIndents(HccApp.CatalogServices.Categories);
            CategoriesGridView.DataKeyNames = new[] {"value"};
            CategoriesGridView.DataBind();
        }

        private IList<string> GetSelectedIds()
        {
            var ids = new List<string>();

            foreach (GridViewRow row in CategoriesGridView.Rows)
            {
                var chkSelected = (CheckBox) row.FindControl("chkSelected");
                if (chkSelected.Checked)
                {
                    ids.Add((string) CategoriesGridView.DataKeys[row.RowIndex].Value);
                }
            }
            return ids;
        }

        protected bool IsEnabled(IDataItemContainer cont)
        {
            var li = cont.DataItem as ListItem;
            return !ReservedIds.Contains(li.Value);
        }

        #endregion
    }
}