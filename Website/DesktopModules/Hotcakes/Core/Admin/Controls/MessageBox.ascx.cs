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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Validation;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class MessageBox : HccUserControl, IMessageBox
    {
        private bool? _addValidationSummaries;

        private bool? _useDefaultValidationGroup;

        public bool AddValidationSummaries
        {
            get
            {
                if (_addValidationSummaries.HasValue)
                    return _addValidationSummaries.Value;
                return true;
            }
            set { _addValidationSummaries = value; }
        }

        public bool UseDefaultValidationGroup
        {
            get
            {
                if (_useDefaultValidationGroup.HasValue)
                    return _useDefaultValidationGroup.Value;
                return true;
            }
            set { _useDefaultValidationGroup = value; }
        }

        public string ValidationGroups { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (AddValidationSummaries)
            {
                if (UseDefaultValidationGroup)
                    AddValidationSummary(string.Empty);

                if (!string.IsNullOrEmpty(ValidationGroups))
                {
                    var valGroups = ValidationGroups.Split(',');
                    foreach (var valGroup in valGroups)
                    {
                        if (!string.IsNullOrEmpty(valGroup))
                            AddValidationSummary(valGroup);
                    }
                }
            }
        }

        private void AddValidationSummary(string validationGroup)
        {
            var summary = new ValidationSummary();
            summary.ValidationGroup = validationGroup;
            summary.DisplayMode = ValidationSummaryDisplayMode.List;
            summary.CssClass = "hcFormMessage hcFormValidationSummary";

            phValidation.Controls.Add(summary);
        }

        public void RenderViolations(List<RuleViolation> violations)
        {
            if (violations == null) return;

            foreach (var v in violations)
            {
                ShowWarning(v.ErrorMessage);
            }
        }

        #region IMessageBox

        public void ClearMessage()
        {
            litMain.Text = string.Empty;
        }

        public void ShowOk(string msg)
        {
            ShowMessage(msg, DisplayMessageType.Success);
        }

        public void ShowError(string msg)
        {
            ShowMessage(msg, DisplayMessageType.Error);
        }

        public void ShowInformation(string msg)
        {
            ShowMessage(msg, DisplayMessageType.Information);
        }

        public void ShowWarning(string msg)
        {
            ShowMessage(msg, DisplayMessageType.Warning);
        }

        public void ShowException(Exception ex)
        {
            var msg = ex.Message + "<br />" + ex.Source + "<br />" + ex.StackTrace;
            ShowMessage(msg, DisplayMessageType.Exception);
        }

        public void ShowMessage(string msg, DisplayMessageType msgType)
        {
            litMain.Text += "<div class=\"";
            switch (msgType)
            {
                case DisplayMessageType.Error:
                    litMain.Text += "hcFormMessage hcFormError";
                    break;
                case DisplayMessageType.Exception:
                    litMain.Text += "hcFormMessage hcFormError";
                    break;
                case DisplayMessageType.Information:
                    litMain.Text += "hcFormMessage hcFormInfo";
                    break;
                case DisplayMessageType.Success:
                    litMain.Text += "hcFormMessage hcFormSuccess";
                    break;
                case DisplayMessageType.Warning:
                    litMain.Text += "hcFormMessage hcFormWarning";
                    break;
            }

            litMain.Text += "\">" + msg + "</div>";
        }

        #endregion
    }
}