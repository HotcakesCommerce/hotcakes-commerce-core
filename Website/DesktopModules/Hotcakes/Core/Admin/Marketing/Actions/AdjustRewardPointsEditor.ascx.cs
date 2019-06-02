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

using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Actions
{
    public partial class AdjustRewardPoints : BaseActionControl
    {
        private RewardPointsAjustment TypedAction
        {
            get { return Action as RewardPointsAjustment; }
        }

        public override void LoadAction()
        {
            if (ddlRecipient.Items.Count == 0)
            {
                ddlRecipient.Items.Add(new ListItem(Localization.GetString("Affiliate"), "0"));
                ddlRecipient.Items.Add(new ListItem(Localization.GetString("ReferralAffiliate"), "1"));
            }

            txtRewardPoints.Text = TypedAction.Amount.ToString();
            ddlRecipient.SelectedValue = ((int) TypedAction.Recipient).ToString();
        }

        public override bool SaveAction()
        {
            TypedAction.Amount = txtRewardPoints.Text.ConvertTo<int>();
            TypedAction.Recipient = (RewardPointsAjustment.RecipientType) ddlRecipient.SelectedValue.ConvertTo<int>();

            return UpdatePromotion();
        }
    }
}