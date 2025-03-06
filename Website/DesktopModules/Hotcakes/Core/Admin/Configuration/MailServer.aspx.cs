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
using System.Net.Mail;
using System.Web.UI;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class MailServer : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Mail Server Settings";
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var valueToSet = "0";
            pnlMain.Visible = false;

            if (HccApp.CurrentStore.Settings.MailServer.UseCustomMailServer)
            {
                valueToSet = "1";
                pnlMain.Visible = true;
            }


            if (lstMailServerChoice.Items.FindByValue(valueToSet) != null)
            {
                lstMailServerChoice.ClearSelection();
                lstMailServerChoice.Items.FindByValue(valueToSet).Selected = true;
            }

            MailServerField.Text = HccApp.CurrentStore.Settings.MailServer.HostAddress;
            chkMailServerAuthentication.Checked = HccApp.CurrentStore.Settings.MailServer.UseAuthentication;
            UsernameField.Text = HccApp.CurrentStore.Settings.MailServer.Username;
            PasswordField.Text = "****************";

            chkSSL.Checked = HccApp.CurrentStore.Settings.MailServer.UseSsl;
            SmtpPortField.Text = HccApp.CurrentStore.Settings.MailServer.Port;
        }


        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            msg.ShowOk("Settings saved successfully.");
        }

        private void SaveData()
        {
            HccApp.CurrentStore.Settings.MailServer.HostAddress = MailServerField.Text.Trim();
            HccApp.CurrentStore.Settings.MailServer.UseAuthentication = chkMailServerAuthentication.Checked;
            HccApp.CurrentStore.Settings.MailServer.Username = UsernameField.Text.Trim();
            if (PasswordField.Text.Trim().Length > 0)
            {
                if (PasswordField.Text != "****************")
                {
                    HccApp.CurrentStore.Settings.MailServer.Password = PasswordField.Text.Trim();
                    PasswordField.Text = "****************";
                }
            }

            HccApp.CurrentStore.Settings.MailServer.UseSsl = chkSSL.Checked;
            HccApp.CurrentStore.Settings.MailServer.Port = SmtpPortField.Text.Trim();
            if (lstMailServerChoice.SelectedItem.Value == "1")
            {
                HccApp.CurrentStore.Settings.MailServer.UseCustomMailServer = true;
            }
            else
            {
                HccApp.CurrentStore.Settings.MailServer.UseCustomMailServer = false;
            }
            HccApp.UpdateCurrentStore();
        }

        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnSendTest_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();

            msg.ClearMessage();

            var m = new MailMessage("testemail@hotcakescommerce.com", TestToField.Text.Trim())
            {
                Subject = "Mail Server Test Message",
                Body = "Your mail server appears to be correctly configured!",
                IsBodyHtml = false
            };

            if (MailServices.SendMail(m, HccApp.CurrentStore))
            {
                msg.ShowOk("Test Message Sent");
            }
            else
            {
                msg.ShowError("Test Failed. Please check your settings and try again.");
            }

            m = null;
        }

        protected void lstMailServerChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMailServerChoice.SelectedItem.Value == "0")
            {
                pnlMain.Visible = false;
            }
            else
            {
                pnlMain.Visible = true;
            }
        }
    }
}