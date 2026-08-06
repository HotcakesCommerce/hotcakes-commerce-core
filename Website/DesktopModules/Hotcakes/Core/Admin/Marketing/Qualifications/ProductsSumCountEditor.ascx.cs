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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class ProductsSumCountEditor : BaseQualificationControl
    {
        private SumOrCountOfProducts TypedQualification
        {
            get { return Qualification as SumOrCountOfProducts; }
        }

        protected void btnAddLineItemCategory_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            if (q == null || lstLineItemCategories == null) return;

            var selected = lstLineItemCategories.SelectedValue;
            if (string.IsNullOrWhiteSpace(selected)) return;

            q.AddCategoryId(selected.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvLineItemCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null || e?.Keys == null || e.Keys.Count == 0) return;

            var keyObj = e.Keys[0];
            if (keyObj == null) return;

            var bvin = keyObj as string ?? keyObj.ToString();
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveCategoryId(bvin.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            if (TypedQualification == null) return;

            if (ddlCalcMode != null && ddlCalcMode.Items.Count == 0)
            {
                ddlCalcMode.Items.Add(new ListItem(Localization.GetString("TotalPrice"), "0"));
                ddlCalcMode.Items.Add(new ListItem(Localization.GetString("TotalCount"), "1"));
            }

            var allCats = HccApp.CatalogServices.Categories.FindAll() ?? new List<CategorySnapshot>();
            var available = CategoriesHelper.ListFullTreeWithIndents(allCats, true) ?? new Collection<ListItem>();

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in TypedQualification.CategoryIds() ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                var t = available.FirstOrDefault(y => string.Equals(y.Value ?? string.Empty, bvin, StringComparison.OrdinalIgnoreCase));
                if (t != null)
                {
                    item.DisplayName = t.Text;
                    available.Remove(t);
                }
                displayData.Add(item);
            }

            if (lstLineItemCategories != null)
            {
                lstLineItemCategories.Items.Clear();
                foreach (var li in available)
                {
                    lstLineItemCategories.Items.Add(li);
                }
            }

            if (ddlCalcMode != null)
            {
                var modeInt = (int)TypedQualification.CalculationMode;
                var modeString = modeInt.ToString(CultureInfo.InvariantCulture);
                if (ddlCalcMode.Items.FindByValue(modeString) != null)
                {
                    ddlCalcMode.SelectedValue = modeString;
                }
                else if (ddlCalcMode.Items.Count > 0)
                {
                    ddlCalcMode.SelectedIndex = 0;
                }
            }

            if (txtSumOrCount != null)
            {
                txtSumOrCount.Text = TypedQualification.SumAmount.ToString(CultureInfo.CurrentCulture);
            }

            if (gvLineItemCategories != null)
            {
                gvLineItemCategories.DataSource = displayData;
                gvLineItemCategories.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            var q = TypedQualification;
            if (q == null) return false;

            if (ddlCalcMode != null)
            {
                if (int.TryParse(ddlCalcMode.SelectedValue, out var modeInt) &&
                    Enum.IsDefined(typeof(SumOrCountMode), modeInt))
                {
                    q.CalculationMode = (SumOrCountMode)modeInt;
                }
            }

            if (txtSumOrCount != null)
            {
                if (decimal.TryParse(txtSumOrCount.Text?.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var val))
                {
                    q.SumAmount = val;
                }
                else
                {
                    q.SumAmount = 0m;
                }
            }

            return UpdatePromotion();
        }

        protected void btnDeleteLineItemCategory_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}