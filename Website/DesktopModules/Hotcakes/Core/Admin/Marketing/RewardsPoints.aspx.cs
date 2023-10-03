#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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

using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing
{
    public partial class RewardsPoints : BaseAdminPage
    {
        #region Properties

        protected string PointsForEachSpent
        {
            get { return string.Format(Localization.GetString("PointsForEachSpent"), string.Format("{0:c}", 1)); }
        }

        protected string PointsForCredit
        {
            get { return string.Format(Localization.GetString("PointsForCredit"), string.Format("{0:c}", 1)); }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = HccApp.CurrentStore.Settings.RewardsPointsName;
            CurrentTab = AdminTabType.Marketing;
            ValidateCurrentUserHasPermission(SystemPermissions.MarketingView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadInfo();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ucMessageBox.ClearMessage();

            if (SaveInfo())
            {
                ucMessageBox.ShowOk(Localization.GetString("ChangesSaved"));
            }
        }

        #endregion

        #region Implementation

        private void LoadInfo()
        {
            var manager = new CustomerPointsManager(HccApp.CurrentRequestContext);

            var pointsIssued = manager.TotalPointsIssuedForStore(HccApp.CurrentStore.Id);
            lblPointsIssued.Text = pointsIssued.ToString();
            lblPointsIssuedValue.Text = manager.DollarCreditForPoints(pointsIssued).ToString("c");

            var pointsReserverd = manager.TotalPointsReservedForStore(HccApp.CurrentStore.Id);
            lblPointsReserved.Text = pointsReserverd.ToString();
            lblPointsReservedValue.Text = manager.DollarCreditForPoints(pointsReserverd).ToString("c");

            RewardsNameField.Text = HccApp.CurrentStore.Settings.RewardsPointsName;
            chkEnableRewardsPoints.Checked = HccApp.CurrentStore.Settings.RewardsPointsEnabled;
            chkUseForUserPrice.Checked = HccApp.CurrentStore.Settings.UseRewardsPointsForUserPrice;
            chkIssuePointsForUserPrice.Checked = HccApp.CurrentStore.Settings.IssuePointsForUserPrice;
            PointsCreditField.Text = HccApp.CurrentStore.Settings.RewardsPointsNeededPerDollarCredit.ToString();
            PointsPerDollarField.Text = HccApp.CurrentStore.Settings.RewardsPointsIssuedPerDollarSpent.ToString();
        }

        private bool SaveInfo()
        {
            HccApp.CurrentStore.Settings.RewardsPointsName = RewardsNameField.Text.Trim();
            HccApp.CurrentStore.Settings.RewardsPointsEnabled = chkEnableRewardsPoints.Checked;
            HccApp.CurrentStore.Settings.UseRewardsPointsForUserPrice = chkUseForUserPrice.Checked;
            HccApp.CurrentStore.Settings.IssuePointsForUserPrice = chkIssuePointsForUserPrice.Checked;
            var pointPerDollar = 1;
            if (int.TryParse(PointsPerDollarField.Text, out pointPerDollar))
            {
                HccApp.CurrentStore.Settings.RewardsPointsIssuedPerDollarSpent = pointPerDollar;
            }
            else
            {
                ucMessageBox.ShowWarning(Localization.GetString("PointValueError"));
                return false;
            }
            var pointsPerCredit = 100;
            if (int.TryParse(PointsCreditField.Text, out pointsPerCredit))
            {
                HccApp.CurrentStore.Settings.RewardsPointsNeededPerDollarCredit = pointsPerCredit;
            }
            else
            {
                ucMessageBox.ShowWarning(Localization.GetString("RedemptionValueError"));
                return false;
            }

            return HccApp.UpdateCurrentStore();
        }

        #endregion
    }
}