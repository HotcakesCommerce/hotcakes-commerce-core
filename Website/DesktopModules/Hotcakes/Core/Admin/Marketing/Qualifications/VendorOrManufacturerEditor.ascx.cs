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

            cbIsNot.CheckedChanged += cbIsNot_CheckedChanged;
        }

        private void cbIsNot_CheckedChanged(object sender, EventArgs e)
        {
            TypedQualification.IsNotMode = cbIsNot.Checked;
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            q.IsNotMode = cbIsNot.Checked;
            q.AddNewId(lstItems.SelectedValue);
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            var bvin = (string) e.Keys[0];
            q.RemoveId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        protected void btnDeleteItem_OnPreRender(object sender, EventArgs e)
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
            var vendors = HccApp.ContactServices.Vendors.FindAll();
            var manufacturers = HccApp.ContactServices.Manufacturers.FindAll();

            var displayData = new List<FriendlyBvinDisplay>();

            cbIsNot.Checked = TypedQualification.IsNotMode;

            foreach (var bvin in TypedQualification.CurrentIds())
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;

                var t =
                    vendors
                        .FirstOrDefault(y => y.Bvin.Trim().ToLowerInvariant() == bvin.Trim().ToLowerInvariant());

                if (t != null)
                {
                    item.DisplayName = t.DisplayName;
                    vendors.Remove(t);
                }

                if (t == null)
                {
                    t =
                        manufacturers
                            .FirstOrDefault(y => y.Bvin.Trim().ToLowerInvariant() == bvin.Trim().ToLowerInvariant());

                    if (t != null)
                    {
                        item.DisplayName = t.DisplayName;
                        manufacturers.Remove(t);
                    }
                }

                displayData.Add(item);
            }

            var sepText = "---------------------------------------";

            var list = new List<VendorManufacturer>();
            list.AddRange(vendors);
            list.Add(new VendorManufacturer { DisplayName = sepText, Bvin = string.Empty });
            list.AddRange(manufacturers);

            lstItems.DataSource = list;
            lstItems.DataValueField = "Bvin";
            lstItems.DataTextField = "DisplayName";
            lstItems.DataBind();

            var sepitem = lstItems.Items.FindByText(sepText);

            if (sepitem != null)
            {
                sepitem.Attributes.Add("disabled", "disabled");
            }

            gvItems.DataSource = displayData;
            gvItems.DataBind();
        }

        public override bool SaveQualification()
        {
            TypedQualification.IsNotMode = cbIsNot.Checked;

            return UpdatePromotion();
        }

        #endregion
    }
}