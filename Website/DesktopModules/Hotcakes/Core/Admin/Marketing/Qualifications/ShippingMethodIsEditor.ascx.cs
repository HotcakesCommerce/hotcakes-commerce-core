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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class ShippingMethodIsEditor : BaseQualificationControl
    {
        private ShippingMethodIs TypedQualification
        {
            get { return Qualification as ShippingMethodIs; }
        }


        protected void btnAddShippingMethodIs_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            q.AddItemId(lstShippingMethodIs.SelectedValue.ToUpperInvariant());
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvShippingMethodIs_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null) return;
            var bvin = (string) e.Keys[0];
            q.RemoveItemId(bvin.ToUpperInvariant());
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            var available = HccApp.OrderServices.ShippingMethods.FindAll(HccApp.CurrentStore.Id);

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var itemid in TypedQualification.ItemIds())
            {
                var item = new FriendlyBvinDisplay
                {
                    bvin = itemid,
                    DisplayName = itemid
                };

                var t = available.FirstOrDefault(y => y.Bvin.ToUpperInvariant() == itemid.ToUpperInvariant());
                if (t != null)
                {
                    item.DisplayName = t.Name;
                    available.Remove(t);
                }
                displayData.Add(item);
            }

            lstShippingMethodIs.Items.Clear();
            lstShippingMethodIs.DataSource = available;
            lstShippingMethodIs.DataTextField = "Name";
            lstShippingMethodIs.DataValueField = "Bvin";
            lstShippingMethodIs.DataBind();

            gvShippingMethodIs.DataSource = displayData;
            gvShippingMethodIs.DataBind();
        }

        public override bool SaveQualification()
        {
            return UpdatePromotion();
        }

        protected void btnDeleteShippingMethodIs_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}