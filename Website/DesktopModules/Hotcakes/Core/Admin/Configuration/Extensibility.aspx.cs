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
using System.Linq;
using System.Reflection;
using Hotcakes.Commerce.Dnn.Workflow;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.Integration;
using Telerik.Web.UI;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class Extensibility : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("Extensibility");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnSave.Click += btnSave_Click;
            PageMessageBox = MessageBox;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sett = HccApp.CurrentStore.Settings.Urls;

            sett.WorkflowPluginAssemblyAndType = ddlWorkflowAssemblies.SelectedValue;
            sett.ProductIntegrationAssemblyAndType = ddlProductAssemblies.SelectedValue;
            sett.CartIntegrationAssemblyAndType = ddlCartAssemblies.SelectedValue;
            sett.CheckoutIntegrationAssemblyAndType = ddlCheckoutAssemblies.SelectedValue;

            HccApp.UpdateCurrentStore();

            MessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LocalizeView();

                var sett = HccApp.CurrentStore.Settings.Urls;

                BindAssemblyNames(typeof (DnnWorkflowFactory), ddlWorkflowAssemblies, sett.WorkflowPluginAssemblyAndType);
                BindAssemblyNames(typeof (IProductIntegration), ddlProductAssemblies,
                    sett.ProductIntegrationAssemblyAndType);
                BindAssemblyNames(typeof (ICartIntegration), ddlCartAssemblies, sett.CartIntegrationAssemblyAndType);
                BindAssemblyNames(typeof (ICheckoutIntegration), ddlCheckoutAssemblies,
                    sett.CheckoutIntegrationAssemblyAndType);
            }
        }

        private static void BindAssemblyNames(Type baseType, RadComboBox ddl, string selectedValue)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                    GetLoadableTypes(a).Where(t => baseType.IsAssignableFrom(t) && t != baseType && t.IsPublic)
                )
                .ToList();

            ddl.DataTextField = "FullName";
            ddl.DataValueField = "AssemblyQualifiedName";
            ddl.DataSource = types;
            ddl.AppendDataBoundItems = true;
            ddl.DataBind();

            ddl.SelectedValue = selectedValue;
        }

        public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private void LocalizeView()
        {
            ddlWorkflowAssemblies.Items.Add(new RadComboBoxItem(Localization.GetString("HotcakesDefaultWorkflow"),
                string.Empty));
            ddlProductAssemblies.Items.Add(new RadComboBoxItem(Localization.GetString("NoIntegration"), string.Empty));
            ddlCartAssemblies.Items.Add(new RadComboBoxItem(Localization.GetString("NoIntegration"), string.Empty));
            ddlCheckoutAssemblies.Items.Add(new RadComboBoxItem(Localization.GetString("NoIntegration"), string.Empty));
        }
    }
}