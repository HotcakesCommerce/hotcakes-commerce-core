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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class OrderHasProductEditor : BaseQualificationControl
    {
        private PromotionIdQualificationBase TypedQualification
        {
            get { return Qualification as PromotionIdQualificationBase; }
        }

        public bool IsNotMode { get; set; }

        protected void gvOrderProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            var bvin = (string) e.Keys[0];
            q.RemoveId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        protected void btnAddOrderProduct_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            foreach (var bvin in ProductPickerOrderProducts.SelectedProducts)
            {
                q.AddNewId(bvin);
            }
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            if (IsNotMode)
            {
                pnlHasHeader.Visible = pnlHas.Visible = false;
                pnlHasNotHeader.Visible = pnlHasNot.Visible = true;
            }
            else
            {
                var ohp = (OrderHasProducts) TypedQualification;

                pnlHas.Visible = true;
                pnlHasNot.Visible = false;

                if (lstOrderProductSetMode.Items.Count == 0)
                {
                    lstOrderProductSetMode.Items.Add(new ListItem(Localization.GetString("Any"), "1"));
                    lstOrderProductSetMode.Items.Add(new ListItem(Localization.GetString("All"), "0"));
                }

                var typedQty = 1;
                int.TryParse(ohp.Quantity.ToString(), out typedQty);

                OrderProductQuantityField.Text = typedQty == 0 ? "1" : ohp.Quantity.ToString();
            }

            ProductPickerOrderProducts.LoadSearch();
            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in TypedQualification.CurrentIds())
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;

                var p = HccApp.CatalogServices.Products.FindWithCache(item.bvin);
                if (p != null)
                {
                    item.DisplayName = "[" + p.Sku + "] " + p.ProductName;
                }
                displayData.Add(item);
            }

            gvOrderProducts.DataSource = displayData;
            gvOrderProducts.DataBind();
        }

        public override bool SaveQualification()
        {
            if (!IsNotMode)
            {
                var ohp = (OrderHasProducts) TypedQualification;

                var qty1 = ohp.Quantity;
                var parsedqty1 = 1;
                if (int.TryParse(OrderProductQuantityField.Text, out parsedqty1))
                {
                    qty1 = parsedqty1;
                }
                var setmode = ohp.SetMode;
                var parsedsetmode = 1;
                if (int.TryParse(lstOrderProductSetMode.SelectedValue, out parsedsetmode))
                {
                    setmode = (QualificationSetMode) parsedsetmode;
                }
                ohp.Quantity = qty1;
                ohp.SetMode = setmode;
            }

            return UpdatePromotion();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            pnlHasHeader.Visible = pnlHas.Visible = !IsNotMode;
            pnlHasNotHeader.Visible = pnlHasNot.Visible = IsNotMode;
        }

        protected void btnDeleteOrderProduct_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}