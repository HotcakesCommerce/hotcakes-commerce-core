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
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class MembershipTypeEdit : HccPart
    {
        public MembershipProductType Model { get; set; }

        public LinkButton CancelButton
        {
            get { return btnCancel; }
        }

        private string TypeId
        {
            get { return ViewState["ProductTypeId"] as string; }
            set { ViewState["ProductTypeId"] = value; }
        }

        public event EventHandler SaveData;

        protected override void OnInit(EventArgs e)
        {
            btnAdd.Click += btnAdd_Click;
            DataBinding += MembershipTypeEdit_DataBinding;
            base.OnInit(e);

            rfvProductTypeName.ValidationGroup = ID;
            rfvExpiration.ValidationGroup = ID;
            btnAdd.ValidationGroup = ID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
            }
        }

        private void MembershipTypeEdit_DataBinding(object sender, EventArgs e)
        {
            BindControl();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {


                Model = new MembershipProductType
                {
                    ProductTypeId = TypeId,
                    StoreId = HccApp.CurrentStore.Id,
                    ProductTypeName = txtProductTypeName.Text.Trim(),
                    RoleName = ddlMembershipRole.SelectedValue,
                    ExpirationPeriod = int.Parse(txtExpirationNum.Text.Trim()),
                    ExpirationPeriodType = (ExpirationPeriodType)Convert.ToInt32(ddlPeriodType.SelectedValue),
                    Notify = chkNotify.Checked
                };

                if (SaveData != null)
                    SaveData(this, EventArgs.Empty);
            }
        }

        #region Implementation

        private void BindControl()
        {
            var roles = RoleController.Instance
                .GetRoles(PortalSettings.Current.PortalId)
                .OfType<RoleInfo>()
                .ToList();

            ddlMembershipRole.DataSource = roles;
            ddlMembershipRole.DataTextField = "RoleName";
            ddlMembershipRole.DataValueField = "RoleName";
            ddlMembershipRole.DataBind();

            if (Model != null)
            {
                TypeId = Model.ProductTypeId;
                txtProductTypeName.Text = Model.ProductTypeName;
                txtExpirationNum.Text = Model.ExpirationPeriod.ToString();

                if (roles.Any(r => r.RoleName == Model.RoleName))
                {
                    ddlMembershipRole.SelectedValue = Model.RoleName;
                }
                else if (!string.IsNullOrEmpty(Model.RoleName))
                {
                    cvRoleName.IsValid = false;
                }
                ddlPeriodType.SelectedValue = ((int) Model.ExpirationPeriodType).ToString();

                chkNotify.Checked = Model.Notify;

                btnAdd.Text = string.IsNullOrEmpty(TypeId) ? "Add" : "Update";
            }
        }

        #endregion
    }
}