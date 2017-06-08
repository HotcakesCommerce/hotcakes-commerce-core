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
using Hotcakes.Commerce.Dnn.Marketing.Qualifications;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing
{
    public partial class Promotions_Edit_Qualification : HccUserControl
    {
        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadQualificationEditor(false);
        }

        #endregion

        public void LoadQualification(Promotion p, IPromotionQualification qualif)
        {
            PromotionId = p.Id;
            QualificationId = qualif.Id;
            mvQualifications.Visible = true;

            switch (qualif.TypeId.ToString().ToUpper())
            {
                case PromotionQualificationBase.TypeIdAnyProduct:
                    mvQualifications.Visible = false;
                    break;
                case PromotionQualificationBase.TypeIdProductBvin:
                    mvQualifications.SetActiveView(viewProductBvin);
                    break;
                case PromotionQualificationBase.TypeIdProductCategory:
                    mvQualifications.SetActiveView(viewProductCategory);
                    break;
                case PromotionQualificationBase.TypeIdProductType:
                    mvQualifications.SetActiveView(viewProductType);
                    break;
                case PromotionQualificationBase.TypeIdOrderHasCoupon:
                    mvQualifications.SetActiveView(viewOrderHasCoupon);
                    break;
                case PromotionQualificationBase.TypeIdAnyOrder:
                    ucMessageBox.ShowInformation("This qualification does not have any configuration options.");
                    mvQualifications.Visible = false;
                    break;
                case PromotionQualificationBase.TypeIdOrderSubTotalIs:
                    mvQualifications.SetActiveView(viewOrderSubTotalIs);
                    break;
                case PromotionQualificationBase.TypeIdOrderHasProducts:
                    mvQualifications.SetActiveView(viewOrderHasProduct);
                    break;
                case PromotionQualificationBase.TypeIdUserIs:
                    mvQualifications.SetActiveView(viewUserId);
                    break;
                case PromotionQualificationBase.TypeIdUserIsInGroup:
                    mvQualifications.SetActiveView(viewUserIsInGroup);
                    break;
                case UserIsInRole.TypeIdString:
                    mvQualifications.SetActiveView(viewUserIsInRole);
                    break;
                case PromotionQualificationBase.TypeIdAnyShippingMethod:
                    ucMessageBox.ShowInformation("Any shipping method does not have any configuration options.");
                    mvQualifications.Visible = false;
                    break;
                case PromotionQualificationBase.TypeIdShippingMethodIs:
                    mvQualifications.SetActiveView(viewShippingMethodIs);
                    break;
                case PromotionQualificationBase.TypeIdLineItemCategory:
                    mvQualifications.SetActiveView(viewLineItemCategory);
                    break;
                case AffiliateApproved.TypeIdString:
                    mvQualifications.SetActiveView(viewAffiliateApproved);
                    break;
                case LineItemIsProduct.TypeIdString:
                    mvQualifications.SetActiveView(viewLineItemIs);
                    break;
                case SumOrCountOfProducts.TypeIdString:
                    mvQualifications.SetActiveView(viewProductsSumCount);
                    break;
                case ProductTypeIs.TypeIdString:
                    mvQualifications.SetActiveView(viewProductTypeIsNot);
                    break;
                case OrderHasNotProducts.TypeIdString:
                    mvQualifications.SetActiveView(viewProductIsNot);
                    break;
                case VendorOrManufacturerIs.TypeIdString:
                    mvQualifications.SetActiveView(viewVendorManufacturer);
                    break;
            }

            LoadQualificationEditor(true);
        }

        public bool SaveQualification()
        {
            var editor = GetCurrentEditor();

            if (editor != null)
            {
                return editor.SaveQualification();
            }

            return false;
        }

        #region Fields

        protected long PromotionId
        {
            get { return (long?) ViewState["PromotionId"] ?? -1; }
            set { ViewState["PromotionId"] = value; }
        }

        protected long QualificationId
        {
            get { return (long?) ViewState["QualificationId"] ?? -1; }
            set { ViewState["QualificationId"] = value; }
        }

        #endregion

        #region Implementation

        private void LoadQualificationEditor(bool loadQualif)
        {
            var editor = GetCurrentEditor();

            if (editor != null)
            {
                editor.Promotion = GetCurrentPromotion();
                editor.Qualification = editor.Promotion.GetQualification(QualificationId);

                if (loadQualif)
                {
                    editor.LoadQualification();
                }
            }
        }

        private BaseQualificationControl GetCurrentEditor()
        {
            var view = mvQualifications.GetActiveView();
            return view != null ? view.Controls.OfType<BaseQualificationControl>().FirstOrDefault() : null;
        }

        private Promotion GetCurrentPromotion()
        {
            return HccApp.MarketingServices.Promotions.Find(PromotionId);
        }

        private IPromotionQualification GetCurrentQualification(Promotion p)
        {
            return p.GetQualification(QualificationId);
        }

        #endregion
    }
}