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
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin
{
    public partial class HostAdmin : BaseAdminPage
    {
        public override bool ForceWizardRedirect
        {
            get { return false; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("HostAdministration");
            CurrentTab = AdminTabType.HostAdmin;
            if (!HccApp.MembershipServices.IsSuperUserLoggedIn())
                Response.Redirect(HccApp.MembershipServices.GetLoginPagePath());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var _scriptMan = ScriptManager.GetCurrent(this);
            _scriptMan.AsyncPostBackTimeout = 36000;

            btnUninstall.OnClientClick =
                WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("UninstallConfirmation"));
            btnClearStoreData.OnClientClick =
                WebUtils.JsConfirmMessageWithDisabledCheck(Localization.GetJsEncodedString("ClearStoreDataConfirmation"));
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            var isSampleAnalyticsExist = SampleData.SampleAnalyticsExists();

            phSampleData.Visible = isSampleAnalyticsExist;
            btnCreateSampleData.Visible = !isSampleAnalyticsExist;
            btnRemoveSampleData.Visible = isSampleAnalyticsExist;
        }

        protected void btnClearCache_Click(object sender, EventArgs e)
        {
            CacheManager.ClearAll();
            ucMessageBox.ShowOk(string.Format(Localization.GetString("CacheCleared"), DateTime.Now));
        }

        protected void btnUninstall_Click(object sender, EventArgs e)
        {
            HotcakesController.Uninstall(chkDeleteModuleFiles.Checked, chkDeleteStoreFiles.Checked);

            Response.Redirect(HccUrlBuilder.RouteHccUrl(HccRoute.Home));
        }

        protected void btnClearStoreData_Click(object sender, EventArgs e)
        {
            if (chkDeleteCustomers.Checked)
            {
                HccApp.MembershipServices.DestroyCurrentPortalCustomers();
            }
            if (chkDeleteOrders.Checked)
            {
                HccApp.OrderServices.RemoveAllOrders(HccApp.CurrentStore.Id);
            }
            if (chkDeleteAnalytics.Checked)
            {
                HccApp.AnalyticsService.DeleteEventsByStoreID(HccApp.CurrentStore.Id);
            }
            ucMessageBox.ShowOk(string.Format(Localization.GetString("StoreDataDeletedMsg"), DateTime.Now));
        }

        protected void btnCreateSampleData_Click(object sender, EventArgs e)
        {
            var task = Task.Factory.StartNew(objConfiguration =>
            {
                var conf = objConfiguration as AnalyticsGenerationConfiguration;

                HccRequestContext.Current = conf.HccRequestContext;
                Factory.HttpContext = conf.HttpContext;

                var products = HccApp.CatalogServices.Products.FindAllPaged(1, int.MaxValue);
                SampleData.CreateSampleAnalyticsForStore(products);
            },
                new AnalyticsGenerationConfiguration
                {
                    HccRequestContext = HccRequestContext.Current,
                    HttpContext = Context
                });
        }

        protected void btnRemoveSampleData_Click(object sender, EventArgs e)
        {
            SampleData.RemoveSampleAnalyticsForStore();
        }

        private class AnalyticsGenerationConfiguration
        {
            public HccRequestContext HccRequestContext { get; set; }
            public HttpContext HttpContext { get; set; }
        }
    }
}