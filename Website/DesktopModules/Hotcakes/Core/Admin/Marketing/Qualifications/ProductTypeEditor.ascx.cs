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

            cbIsNot.CheckedChanged += cbIsNot_CheckedChanged;
        }

        private void cbIsNot_CheckedChanged(object sender, EventArgs e)
        {
            if (TypedQualification is ProductTypeIs)
            {
                ((ProductTypeIs) TypedQualification).IsNotMode = cbIsNot.Checked;
            }

            if (TypedQualification is ProductType)
            {
                ((ProductType) TypedQualification).IsNotMode = cbIsNot.Checked;
            }
        }

        protected void btnAddProductType_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            q.AddNewId(lstProductTypes.SelectedValue);
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvProductTypes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            var bvin = (string) e.Keys[0];
            q.RemoveId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        protected void btnDeleteProductType_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            cbIsNot.Visible = IsNotMode;
        }

        #endregion

        #region Helper Methods

        public override void LoadQualification()
        {
            var allTypes = HccApp.CatalogServices.ProductTypes.FindAll();

            if (TypedQualification is ProductTypeIs)
            {
                var pti = (ProductTypeIs) TypedQualification;
                cbIsNot.Checked = pti.IsNotMode;
            }

            if (TypedQualification is ProductType)
            {
                var pti = (ProductType) TypedQualification;
                cbIsNot.Checked = pti.IsNotMode;
            }

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in TypedQualification.CurrentIds())
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;

                var t = allTypes.FirstOrDefault(y => y.Bvin == bvin);
                if (t != null)
                {
                    item.DisplayName = t.ProductTypeName;
                    allTypes.Remove(t);
                }
                displayData.Add(item);
            }

            if (allTypes != null)
            {
                allTypes = allTypes.Where(y => !string.IsNullOrEmpty(y.ProductTypeName)).ToList();
            }

            if (displayData != null)
            {
                displayData = displayData.Where(y => !string.IsNullOrEmpty(y.DisplayName)).ToList();
            }
            lstProductTypes.DataSource = allTypes;
            lstProductTypes.DataValueField = "Bvin";
            lstProductTypes.DataTextField = "ProductTypeName";
            lstProductTypes.DataBind();

            gvProductTypes.DataSource = displayData;
            gvProductTypes.DataBind();
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