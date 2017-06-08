#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
            var items =
                HccApp.MarketingServices.Promotions.FindAllWithFilter(Mode, keywords, showDisabled, ucPager.PageNumber,
                    ucPager.PageSize, ref RowCount);

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
                e.Row.Cells[1].Text = Localization.GetString("Name");
                e.Row.Cells[2].Text = Localization.GetString("Status");
                e.Row.Cells[3].Text = Localization.GetString("Enabled");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var p = e.Row.DataItem as Promotion;
                e.Row.Attributes["id"] = p.Id.ToString();

                var chkBox = e.Row.Cells[3].Controls[0] as CheckBox;
                chkBox.Enabled = true;
                chkBox.Attributes.Add("onclick", "return false;");
            }
        }

        private void gvPromotions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) e.Keys[0];
            HccApp.MarketingServices.Promotions.Delete(id);
        }

        protected void lnkDelete_OnPreRender(object sender, EventArgs e)
        {
            var linkButton = (LinkButton) sender;
            linkButton.OnClientClick = string.Concat("return hcConfirm(event, '",
                Localization.GetString("ConfirmDelete.Text"), "');");
        }

        private void gvPromotions_PreRender(object sender, EventArgs e)
        {
            // We need calculate rowoffset to correct working of sorting functionality
            gvPromotions.Attributes["data-rowoffset"] = (ucPager.PageSize*(ucPager.PageNumber - 1)).ToString();
        }

        #endregion

        #region Implementation

        protected string GetStatus(IDataItemContainer cont)
        {
            var p = cont.DataItem as Promotion;
            return p.GetStatus().ToString();
        }

        protected string GetEditUrl(IDataItemContainer cont)
        {
            var p = cont.DataItem as Promotion;
            return GetEditUrl(p.Id);
        }

        private string GetEditUrl(long id)
        {
            return string.Concat("Promotions_edit.aspx?id=", id);
        }

        #endregion
    }
}