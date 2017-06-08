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
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class ContactAbandonedCartUsers : HccUserControl
    {
        protected DateTime StartDate
        {
            get { return ViewState["StartDate"].ConvertTo<DateTime>(); }
            set { ViewState["StartDate"] = value; }
        }

        protected DateTime EndDate
        {
            get { return ViewState["EndDate"].ConvertTo<DateTime>(); }
            set { ViewState["EndDate"] = value; }
        }

        protected string ProductId
        {
            get { return (string) ViewState["ProductId"]; }
            set { ViewState["ProductId"] = value; }
        }

        protected bool ShowDialog
        {
            get { return ViewState["ShowDialog"].ConvertTo(false); }
            set { ViewState["ShowDialog"] = value; }
        }

        public void ShowContactDialog(DateTime startDate, DateTime endDate, string productId)
        {
            StartDate = startDate;
            EndDate = endDate;
            ProductId = productId;

            ShowDialog = true;
            mvScreens.ActiveViewIndex = 0;
            txtCustomMessage.Text = string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ShowPaymentDialog();
        }

        private void ShowPaymentDialog()
        {
            pnlContactAbandonedCartUsers.Visible = ShowDialog;

            if (ShowDialog)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hcContactAbandonedCartUsers",
                    "hcContactAbandonedCartUsers();", true);
            }
        }

        protected void btnSendEmails_Click(object sender, EventArgs e)
        {
            var userIds = HccApp.OrderServices.Orders.FindAbandonedCartsUsers(StartDate, EndDate, ProductId);
            var users = HccApp.MembershipServices.Customers.FindMany(userIds);


            var task = Task.Factory.StartNew(hccContext =>
            {
                HccRequestContext.Current = hccContext as HccRequestContext;
                SendEmails(users, HccRequestContext.Current);
            },
                HccRequestContext.Current);

            BindEmailedList(users);

            mvScreens.ActiveViewIndex = 1;
        }

        protected void btnCancelClose_Click(object sender, EventArgs e)
        {
            ShowDialog = false;
        }

        private void SendEmails(List<CustomerAccount> users, HccRequestContext hccContext)
        {
            HccRequestContext.Current = hccContext;
            var contentService = Factory.CreateService<ContentService>(hccContext);
            var template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.ContactAbandonedCartUsers);
            var customMessage = txtCustomMessage.Text.Trim();
            var customMessageTag = new Replaceable("[[CustomMessage]]", customMessage);
            template = template.ReplaceTagsInTemplate(hccContext, customMessageTag);
            foreach (var user in users)
            {
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var mailMessage = template.ConvertToMailMessage(user.Email);
                    MailServices.SendMail(mailMessage, hccContext.CurrentStore);
                }
            }
        }

        private void BindEmailedList(List<CustomerAccount> customers)
        {
            rpEmailedCustomers.DataSource = customers;
            rpEmailedCustomers.DataBind();
        }

        protected void btnDownloadContacts_Click(object sender, EventArgs e)
        {
            var userIds = HccApp.OrderServices.Orders.FindAbandonedCartsUsers(StartDate, EndDate, ProductId);
            var users = HccApp.MembershipServices.Customers.FindMany(userIds);

            var response = HttpContext.Current.Response;

            var filename = "EmailedUsers.csv";
            CsvWriter.InitHttpResponse(response, filename);

            using (var csv = new CsvWriter(response.OutputStream))
            {
                csv.WriteLine(Localization.GetString("FirstName"), Localization.GetString("LastName"),
                    Localization.GetString("Email"));

                foreach (var user in users)
                {
                    csv.WriteLine(user.FirstName, user.LastName, user.Email);
                }
            }

            response.Flush();
            response.Close();
        }
    }
}