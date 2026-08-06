#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing
{
    public partial class Promotions_List : HccUserControl
    {
        #region Fields

        protected int RowCount;

        #endregion

        #region Properties

        public PromotionType Mode { get; set; }

        #endregion

        #region Public methods

        public void LoadPromotions(string keywords, bool showDisabled)
        {
            var items = HccApp.MarketingServices.Promotions.FindAllWithFilter(Mode, keywords, showDisabled, ucPager.PageNumber,
                    ucPager.PageSize, ref RowCount) ?? Enumerable.Empty<Promotion>();

            ucPager.SetRowCount(RowCount);

            gvPromotions.Columns[0].Visible = string.IsNullOrEmpty(keywords) && !showDisabled;
            gvPromotions.DataSource = items;
            gvPromotions.DataBind();

            Visible = items.Any();
        }

        #endregion

        public void ResetPageNumber()
        {
            ucPager.ResetPageNumber();
        }

        #region Event handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvPromotions.RowDeleting += gvPromotions_RowDeleting;
            gvPromotions.PreRender += gvPromotions_PreRender;
        }

        protected void gvPromotions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (e.Row.Cells.Count > 1) e.Row.Cells[1].Text = Localization.GetString("Name");
                if (e.Row.Cells.Count > 2) e.Row.Cells[2].Text = Localization.GetString("Status");
                if (e.Row.Cells.Count > 3) e.Row.Cells[3].Text = Localization.GetString("Enabled");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var p = e.Row.DataItem as Promotion;
                if (p != null)
                {
                    e.Row.Attributes["id"] = p.Id.ToString();
                }

                CheckBox chkBox = null;
                if (e.Row.Cells.Count > 3)
                {
                    chkBox = e.Row.Cells[3].Controls.OfType<CheckBox>().FirstOrDefault();
                }

                if (chkBox != null)
                {
                    chkBox.Enabled = true;
                    chkBox.Attributes.Add("onclick", "return false;");
                }
            }
        }

        private void gvPromotions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.Keys != null && e.Keys.Count > 0 && e.Keys[0] != null)
            {
                var id = (long)e.Keys[0];
                HccApp.MarketingServices.Promotions.Delete(id);
            }
        }

        protected void lnkDelete_OnPreRender(object sender, EventArgs e)
        {
            var linkButton = (LinkButton)sender;
            linkButton.OnClientClick = string.Concat("return hcConfirm(event, '",
                Localization.GetString("ConfirmDelete.Text"), "');");
        }

        private void gvPromotions_PreRender(object sender, EventArgs e)
        {
            // We need calculate rowoffset to correct working of sorting functionality
            var pageSize = ucPager.PageSize < 1 ? 10 : ucPager.PageSize;
            var pageNumber = ucPager.PageNumber < 1 ? 1 : ucPager.PageNumber;
            gvPromotions.Attributes["data-rowoffset"] = (pageSize * (pageNumber - 1)).ToString();
        }

        #endregion

        #region Implementation

        protected string GetStatus(IDataItemContainer cont)
        {
            var p = cont.DataItem as Promotion;
            return p != null ? p.GetStatus().ToString() : string.Empty;
        }

        protected string GetEditUrl(IDataItemContainer cont)
        {
            var p = cont.DataItem as Promotion;
            return p != null ? GetEditUrl(p.Id) : GetEditUrl(0);
        }

        private string GetEditUrl(long id)
        {
            return string.Concat("Promotions_edit.aspx?id=", id);
        }

        #endregion
    }
}