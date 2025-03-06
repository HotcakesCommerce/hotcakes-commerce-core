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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.SetupWizard
{
    public partial class Step2Payment : HccPart
    {
        #region Properties

        private string EditorPaymentMethodId
        {
            get { return ViewState["PaymentMethodId"] as string; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ViewState.Remove("PaymentMethodId");
                else
                    ViewState["PaymentMethodId"] = value;
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            LoadPaymentMethods();
            btnCloseDialog.Click += btnCloseDialog_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load editors to fire initiated events
            LoadPaymentEditor();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ShowDialog();
        }

        private void btnCloseDialog_Click(object sender, EventArgs e)
        {
            ClosePaymentEditor();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveChanges();
                //Show next view
                NotifyFinishedEditing();
            }
        }

        protected void btnLater_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing("EXIT");
        }

        #endregion

        #region Implementation

        protected void PaymentMethodsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var method = (PaymentMethod) e.Row.DataItem;

                var chkEnabled = e.Row.FindControl("chkEnabled") as CheckBox;
                var lblMethodName = e.Row.FindControl("lblMethodName") as Label;
                var btnEdit = e.Row.FindControl("btnEdit") as LinkButton;

                var contains = HccApp.CurrentStore.Settings.PaymentMethodsEnabled.Contains(method.MethodId);
                chkEnabled.Checked = contains;

                lblMethodName.Text = LocalizationUtils.GetPaymentMethodFriendlyName(method.MethodName);

                var editor = HccPartController.LoadPaymentMethodEditor(method.MethodName, Page) as HccPart;
                var editorExists = editor != null;
                btnEdit.Visible = editorExists;
            }
        }

        protected void PaymentMethodsGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            EditorPaymentMethodId = PaymentMethodsGrid.DataKeys[e.NewEditIndex].Value.ToString();

            LoadPaymentEditor();
        }

        private void LoadPaymentMethods()
        {
            PaymentMethodsGrid.DataSource = PaymentMethods.AvailableMethods();
            PaymentMethodsGrid.DataBind();
        }

        private void SaveChanges()
        {
            var newList = new List<string>();
            for (var i = 0; i < PaymentMethodsGrid.Rows.Count; i++)
            {
                var chkEnabled = PaymentMethodsGrid.Rows[i].FindControl("chkEnabled") as CheckBox;
                if (chkEnabled != null && chkEnabled.Checked)
                {
                    newList.Add(PaymentMethodsGrid.DataKeys[i].Value.ToString());
                }
            }

            HccApp.CurrentStore.Settings.PaymentMethodsEnabled = newList;
            HccApp.UpdateCurrentStore();
        }

        private void ShowDialog()
        {
            if (!string.IsNullOrEmpty(EditorPaymentMethodId))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcEditPaymentDialog",
                    "hcEditPaymentDialog();", true);
            }
        }

        #endregion

        #region Editor Payment Method

        private void LoadPaymentEditor()
        {
            if (!string.IsNullOrEmpty(EditorPaymentMethodId))
            {
                methodEditor.LoadEditor(EditorPaymentMethodId);
            }
        }

        private void ClosePaymentEditor()
        {
            EditorPaymentMethodId = null;
            methodEditor.RemoveEditor();
        }

        protected void methodEditor_EditingComplete(object sender, HccPartEventArgs e)
        {
            ClosePaymentEditor();
        }

        #endregion
    }
}