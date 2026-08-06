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
using System.Linq;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing
{
    public partial class Promotions_Edit_Actions : HccUserControl
    {
        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadActionEditor(false);
        }

        #endregion

        #region Fields

        protected long PromotionId
        {
            get { return (long?) ViewState["PromotionId"] ?? -1; }
            set { ViewState["PromotionId"] = value; }
        }

        protected long ActionId
        {
            get { return (long?) ViewState["ActionId"] ?? -1; }
            set { ViewState["ActionId"] = value; }
        }

        #endregion

        #region Public Methods

        public void LoadAction(Promotion prom, IPromotionAction action)
        {
            if (prom == null || action == null)
            {
                if (mvActions != null) mvActions.Visible = false;
                return;
            }

            PromotionId = prom.Id;
            ActionId = action.Id;

            var typeId = action.TypeId.ToString().ToUpperInvariant();

            if (mvActions == null)
            {
                return;
            }

            switch (typeId)
            {
                case ProductPriceAdjustment.TypeIdString:
                    mvActions.SetActiveView(viewAdjustProductPrice);
                    break;
                case OrderTotalAdjustment.TypeIdString:
                    mvActions.SetActiveView(viewAdjustOrderTotal);
                    break;
                case LineItemAdjustment.TypeIdString:
                    mvActions.SetActiveView(viewAdjustLineItem);
                    break;
                case OrderShippingAdjustment.TypeIdString:
                    mvActions.SetActiveView(viewOrderShippingAdjustment);
                    break;
                case LineItemFreeShipping.TypeIdString:
                    mvActions.SetActiveView(viewLineItemFreeShipping);
                    break;
                case RewardPointsAjustment.TypeIdString:
                    mvActions.SetActiveView(viewAdjustRewardPoints);
                    break;
                case ReceiveFreeProductAdjustment.TypeIdString:
                    mvActions.SetActiveView(viewReceiveFreeProduct);
                    break;
                case CategoryDiscountAdjustment.TypeIdString:
                    mvActions.SetActiveView(viewCategoryDiscount);
                    break;
                default:
                    // If type is unknown, hide editor
                    mvActions.Visible = false;
                    break;
            }

            LoadActionEditor(true);
        }

        public bool SaveAction()
        {
            var editor = GetCurrentActionEditor();

            if (editor != null)
            {
                return editor.SaveAction();
            }

            return false;
        }

        #endregion

        #region Implementation

        private void LoadActionEditor(bool loadAction)
        {
            var editor = GetCurrentActionEditor();

            if (editor == null)
            {
                return;
            }

            var promotion = GetCurrentPromotion();
            editor.Promotion = promotion;

            editor.Action = promotion != null ? promotion.GetAction(ActionId) : null;

            if (loadAction && editor.Action != null)
            {
                editor.LoadAction();
            }
        }

        private BaseActionControl GetCurrentActionEditor()
        {
            if (mvActions == null)
            {
                return null;
            }

            var view = mvActions.GetActiveView();
            return view != null ? view.Controls.OfType<BaseActionControl>().FirstOrDefault() : null;
        }

        private Promotion GetCurrentPromotion()
        {
            return HccApp.MarketingServices.Promotions.Find(PromotionId);
        }

        #endregion
    }
}