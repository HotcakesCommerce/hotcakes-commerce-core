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
using System.Web.UI;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class InventoryNotices : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                EmailReportToTextBox.Text = HccApp.CurrentStore.Settings.MailServer.EmailForGeneral;
                LowStockHoursTextBox.Text = WebAppSettings.InventoryLowHours.ToString();
                LinePrefixTextBox.Text = WebAppSettings.InventoryLowReportLinePrefix;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Inventory Settings";
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            if (Save())
            {
                MessageBox1.ShowOk("Settings saved successfully.");
            }
        }

        private bool Save()
        {
            var result = false;
            HccApp.CurrentStore.Settings.MailServer.EmailForGeneral = EmailReportToTextBox.Text;
            
            result = true;

            return result;
        }

        protected void SendLowStockReportImageButton_Click(object sender, ImageClickEventArgs e)
        {
            MessageBox1.ClearMessage();
            if (EmailReportToTextBox.Text.Length > 0)
            {
                if (ProductInventory.EmailLowStockReport(EmailReportToTextBox.Text,
                    HccApp.CurrentStore.Settings.FriendlyName, HccApp))
                {
                    MessageBox1.ShowOk("Report sent!");
                }
                else
                {
                    MessageBox1.ShowWarning("Report failed to send.");
                }
            }
            else
            {
                MessageBox1.ShowWarning("You must enter an email address to send the report!");
            }
        }
    }
}