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

using System;
using System.Web.UI;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Shipping_EditMethod : BaseAdminPage
    {
        private ShippingMethod m;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("EditShippingMethod");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Request.QueryString["id"] != null)
            {
                BlockIDField.Value = Request.QueryString["id"];
            }

            m = HccApp.OrderServices.ShippingMethods.Find(BlockIDField.Value);

            LoadEditor();
        }

        private void LoadEditor()
        {
            var p = AvailableServices.FindById(m.ShippingProviderId, HccApp.CurrentStore);

            var editor = HccPartController.LoadShippingEditor(p.Name, this) as HccShippingPart;

            if (editor != null)
            {
                editor.ShippingMethod = m;
                phEditor.Controls.Add(editor);
                editor.EditingComplete += editor_EditingComplete;
            }
            else
            {
                phEditor.Controls.Add(new LiteralControl(Localization.GetString("EditorLoadError")));
            }
        }

        protected void editor_EditingComplete(object sender, HccPartEventArgs e)
        {
            if (e.Info.ToUpper() == "CANCELED")
            {
                if (Request.QueryString["doc"] == "1")
                {
                    HccApp.OrderServices.ShippingMethods.Delete(m.Bvin);
                }
            }
            else
            {
                if (e.Info != string.Empty)
                {
                    m.Name = e.Info;
                    HccApp.OrderServices.ShippingMethods.Update(m);
                }
            }
            Response.Redirect("Shipping_Methods.aspx");
        }
    }
}