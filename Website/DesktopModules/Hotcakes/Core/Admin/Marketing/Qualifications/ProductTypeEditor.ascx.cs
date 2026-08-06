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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class ProductTypeEditor : BaseQualificationControl
    {
        #region Properties

        private PromotionIdQualificationBase TypedQualification
        {
            get { return Qualification as PromotionIdQualificationBase; }
        }

        public bool IsNotMode { get; set; }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (cbIsNot != null) cbIsNot.CheckedChanged += cbIsNot_CheckedChanged;
        }

        private void cbIsNot_CheckedChanged(object sender, EventArgs e)
        {
            if (TypedQualification is ProductTypeIs pti)
            {
                pti.IsNotMode = cbIsNot?.Checked ?? pti.IsNotMode;
            }
            else if (TypedQualification is ProductType pt)
            {
                pt.IsNotMode = cbIsNot?.Checked ?? pt.IsNotMode;
            }
        }

        protected void btnAddProductType_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            if (q == null || lstProductTypes == null) return;

            var selected = lstProductTypes.SelectedValue;
            if (string.IsNullOrWhiteSpace(selected)) return;

            q.AddNewId(selected.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvProductTypes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null) return;
            if (e?.Keys == null || e.Keys.Count == 0) return;

            var keyObj = e.Keys[0];
            if (keyObj == null) return;

            var bvin = keyObj as string ?? keyObj.ToString();
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveId(bvin.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        protected void btnDeleteProductType_OnPreRender(object sender, EventArgs e)
        {
            if (sender is LinkButton link)
            {
                link.Text = Localization.GetString("Delete");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (cbIsNot != null) cbIsNot.Visible = IsNotMode;
        }

        #endregion

        #region Helper Methods

        public override void LoadQualification()
        {
            var q = TypedQualification;
            if (q == null)
            {
                if (lstProductTypes != null) lstProductTypes.Items.Clear();
                if (gvProductTypes != null) gvProductTypes.DataBind();
                return;
            }

            var allTypes = HccApp.CatalogServices.ProductTypes.FindAll();

            if (q is ProductTypeIs pti)
            {
                if (cbIsNot != null) cbIsNot.Checked = pti.IsNotMode;
            }

            if (q is ProductType pt)
            {
                if (cbIsNot != null) cbIsNot.Checked = pt.IsNotMode;
            }

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var rawBvin in (q.CurrentIds() ?? Enumerable.Empty<string>()))
            {
                if (string.IsNullOrWhiteSpace(rawBvin)) continue;
                var bvin = rawBvin.Trim();

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                var match = allTypes.FirstOrDefault(y =>
                    string.Equals((y.Bvin ?? string.Empty).Trim(), bvin, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    item.DisplayName = match.ProductTypeName ?? bvin;
                    allTypes.Remove(match);
                }

                displayData.Add(item);
            }

            // Filter out empty product type names for selection list
            var availableTypes = allTypes
                .Where(y => !string.IsNullOrEmpty(y.ProductTypeName))
                .ToList();

            if (lstProductTypes != null)
            {
                lstProductTypes.DataSource = availableTypes;
                lstProductTypes.DataValueField = "Bvin";
                lstProductTypes.DataTextField = "ProductTypeName";
                lstProductTypes.DataBind();
            }

            if (gvProductTypes != null)
            {
                gvProductTypes.DataSource = displayData.Where(y => !string.IsNullOrEmpty(y.DisplayName)).ToList();
                gvProductTypes.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            if (TypedQualification is ProductTypeIs)
            {
                ((ProductTypeIs) TypedQualification).IsNotMode = cbIsNot.Checked;
            }

            if (TypedQualification is ProductType)
            {
                ((ProductType) TypedQualification).IsNotMode = cbIsNot.Checked;
            }

            return UpdatePromotion();
        }

        #endregion
    }
}