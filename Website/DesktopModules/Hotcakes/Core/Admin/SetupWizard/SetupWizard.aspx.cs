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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.SetupWizard
{
    public partial class SetupWizard : BaseAdminPage, IHccPage
    {
        private int? _step;

        public int Step
        {
            get
            {
                if (_step == null)
                {
                    var step = Request.QueryString["Step"];
                    var iStep = 0;
                    int.TryParse(Request.QueryString["Step"], out iStep);
                    _step = iStep;
                }
                return _step.Value;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            CurrentTab = AdminTabType.SetupWizard;

            base.OnPreInit(e);

            if (HccApp.CurrentStore == null && !HccApp.MembershipServices.IsSuperUserLoggedIn())
            {
                Response.Redirect(HccApp.MembershipServices.GetLoginPagePath());
            }
            else if (HccApp.CurrentStore != null)
            {
                // TFS 33658
                // if the store is already configured, ensure the user has permissions still
                ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
            }

            if (HccApp.CurrentStore == null && Step != 0)
            {
                Response.Redirect(WizardPagePath);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                MultiView.ActiveViewIndex = Step;

                hlStep1.NavigateUrl = StepUrl(1);
                hlStep2.NavigateUrl = StepUrl(2);
                hlStep3.NavigateUrl = StepUrl(3);
                hlStep4.NavigateUrl = StepUrl(4);
            }
            else
            {
                LoadCurrentView();
            }
        }

        protected void MultiView_ActiveViewChanged(object sender, EventArgs e)
        {
            LoadCurrentView();
        }

        private void LoadCurrentView()
        {
            switch (MultiView.ActiveViewIndex)
            {
                case 4:
                    LoadStep4();
                    break;
                case 3:
                    LoadStep3();
                    break;
                case 2:
                    LoadStep2();
                    break;
                case 1:
                    LoadStep1();
                    break;
                default:
                    LoadStep0();
                    break;
            }
        }

        private void LoadStep0()
        {
            var control = LoadControl("Step0Dashboard.ascx") as HccPart;
            control.ID = "Step0Dashboard1";
            control.EditingComplete += Step0_Completed;
            phStep0.Controls.Clear();
            phStep0.Controls.Add(control);
            pnlNavigation.Visible = false;
        }

        private void LoadStep1()
        {
            var control = LoadControl("Step1General.ascx") as HccPart;
            control.ID = "Step1General1";
            control.EditingComplete += Step1_Completed;
            phStep1.Controls.Clear();
            phStep1.Controls.Add(control);
            pnlNavigation.Visible = true;
        }

        private void LoadStep2()
        {
            var control = LoadControl("Step2Payment.ascx") as HccPart;
            control.ID = "Step2Payment1";
            control.EditingComplete += Step2_Completed;
            phStep2.Controls.Clear();
            phStep2.Controls.Add(control);
            pnlNavigation.Visible = true;
        }

        private void LoadStep3()
        {
            var control = LoadControl("Step3Shipping.ascx") as HccPart;
            control.ID = "Step3Shipping1";
            control.EditingComplete += Step3_Completed;
            phStep3.Controls.Clear();
            phStep3.Controls.Add(control);
            pnlNavigation.Visible = true;
        }

        private void LoadStep4()
        {
            var control = LoadControl("Step4Taxes.ascx") as HccPart;
            control.ID = "Step4Taxes1";
            control.EditingComplete += Step4_Completed;
            phStep4.Controls.Clear();
            phStep4.Controls.Add(control);
            pnlNavigation.Visible = true;
        }

        protected void OpenStep(int stepIndex)
        {
            Response.Redirect(StepUrl(stepIndex));
        }

        protected string StepUrl(int stepIndex)
        {
            return string.Format(WizardPageStepPath, stepIndex);
        }

        private void Step0_Completed(object sender, HccPartEventArgs e)
        {
            if ("EXIT".Equals(e.Info, StringComparison.OrdinalIgnoreCase))
                ExitWizard();
            else
                OpenStep(1);
        }

        private void Step1_Completed(object sender, HccPartEventArgs e)
        {
            if ("EXIT".Equals(e.Info, StringComparison.OrdinalIgnoreCase))
                ExitWizard();
            else
                OpenStep(2);
        }

        private void Step2_Completed(object sender, HccPartEventArgs e)
        {
            if ("EXIT".Equals(e.Info, StringComparison.OrdinalIgnoreCase))
                ExitWizard();
            else
                OpenStep(3);
        }

        private void Step3_Completed(object sender, HccPartEventArgs e)
        {
            if ("EXIT".Equals(e.Info, StringComparison.OrdinalIgnoreCase))
                ExitWizard();
            else
                OpenStep(4);
        }

        private void Step4_Completed(object sender, HccPartEventArgs e)
        {
            ExitWizard();
        }

        private void ExitWizard()
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/default.aspx");
        }

        protected string GetProgressItemClass(int step)
        {
            if (MultiView.ActiveViewIndex == step)
                return "hcProgressActive";
            if (MultiView.ActiveViewIndex < step)
                return "hcProgressDisabled";

            return string.Empty;
        }
    }
}