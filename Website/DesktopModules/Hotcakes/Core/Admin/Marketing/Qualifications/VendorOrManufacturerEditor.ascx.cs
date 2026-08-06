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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class VendorOrManufacturerEditor : BaseQualificationControl
    {
        #region Properties

        private VendorOrManufacturerIs TypedQualification
        {
            get { return Qualification as VendorOrManufacturerIs; }
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
            if (TypedQualification != null && cbIsNot != null)
            {
                TypedQualification.IsNotMode = cbIsNot.Checked;
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            if (q == null) return;

            if (lstItems == null) return;

            q.IsNotMode = cbIsNot?.Checked ?? q.IsNotMode;
            var selected = lstItems.SelectedValue;
            if (!string.IsNullOrWhiteSpace(selected))
            {
                q.AddNewId(selected);
                UpdatePromotion();
                LoadQualification();
            }
        }

        protected void gvItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null) return;
            if (e.Keys == null || e.Keys.Count == 0) return;

            var bvin = e.Keys[0] as string;
            if (string.IsNullOrEmpty(bvin)) return;

            q.RemoveId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        protected void btnDeleteItem_OnPreRender(object sender, EventArgs e)
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
            if (q == null) return;

            var vendors = HccApp?.ContactServices?.Vendors?.FindAll() ?? new List<VendorManufacturer>();
            var manufacturers = HccApp?.ContactServices?.Manufacturers?.FindAll() ?? new List<VendorManufacturer>();

            var displayData = new List<FriendlyBvinDisplay>();

            if (cbIsNot != null) cbIsNot.Checked = q.IsNotMode;

            foreach (var bvin in q.CurrentIds())
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                // Trim once and use ordinal ignore-case comparison
                var idTrim = bvin.Trim();

                var v = vendors.FirstOrDefault(y => string.Equals(y.Bvin?.Trim(), idTrim, StringComparison.OrdinalIgnoreCase));
                if (v != null)
                {
                    item.DisplayName = v.DisplayName;
                    // remove to avoid duplicate entries in selection list
                    vendors.Remove(v);
                }
                else
                {
                    var m = manufacturers.FirstOrDefault(y => string.Equals(y.Bvin?.Trim(), idTrim, StringComparison.OrdinalIgnoreCase));
                    if (m != null)
                    {
                        item.DisplayName = m.DisplayName;
                        manufacturers.Remove(m);
                    }
                }

                displayData.Add(item);
            }

            const string sepText = "---------------------------------------";

            var list = new List<VendorManufacturer>();
            list.AddRange(vendors);
            list.Add(new VendorManufacturer { DisplayName = sepText, Bvin = string.Empty });
            list.AddRange(manufacturers);

            if (lstItems != null)
            {
                lstItems.DataSource = list;
                lstItems.DataValueField = "Bvin";
                lstItems.DataTextField = "DisplayName";
                lstItems.DataBind();

                var sepitem = lstItems.Items.FindByText(sepText);
                if (sepitem != null) sepitem.Attributes.Add("disabled", "disabled");
            }

            if (gvItems != null)
            {
                gvItems.DataSource = displayData;
                gvItems.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            var q = TypedQualification;
            if (q == null) return false;

            q.IsNotMode = cbIsNot?.Checked ?? q.IsNotMode;

            return UpdatePromotion();
        }

        #endregion
    }
}