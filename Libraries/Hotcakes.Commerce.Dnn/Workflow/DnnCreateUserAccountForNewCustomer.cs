#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.BusinessRules.OrderTasks;
using Hotcakes.Commerce.Membership;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Dnn.Workflow
{
    /// <summary>
    /// Creates a DNN user account for a new Hotcakes customer during order processing.
    /// </summary>
    [Serializable]
    public class DnnCreateUserAccountForNewCustomer : CreateUserAccountForNewCustomer
    {
        private static readonly ILogger _Logger = new SupressLogger();
        private const string GUEST_ROLENAME = "GuestCheckout";

        protected override bool RequiresUniqueEmail
        {
            get { return MembershipProviderConfig.RequiresUniqueEmail; }
        }

        /// <summary>
        /// Executes the customer-account creation task.
        /// </summary>
        /// <param name="context">The order task context containing the customer and order.</param>
        /// <returns><see langword="true"/> when execution succeeds; otherwise, <see langword="false"/>.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            return base.Execute(context);
        }

        protected override void CreateAccount(OrderTaskContext context, CustomerAccount n)
        {
            try
            {
                if (context.HccApp.MembershipServices.CreateCustomer(n, n.Password))
                {
                    if (string.IsNullOrEmpty(context.HccApp.CurrentCustomerId))
                        LoginNewUser(context, n);

                    // Update Addresses for Customer
                    context.Order.BillingAddress.CopyTo(n.BillingAddress);
                    context.Order.ShippingAddress.CopyTo(n.ShippingAddress);
                    context.HccApp.MembershipServices.UpdateCustomer(n);

                    if (_isCreateGuestAccount)
                    {
                        context.Order.CustomProperties.Add("hcc", "allowpasswordreset", "1");

                        AssignToGuestRole(n);
                    }

                    // Email Password to Customer
                    EmailPasswordToCustomer(context, n);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                EventLog.LogEvent(ex);
            }
        }

        private void AssignToGuestRole(CustomerAccount n)
        {
            try
            {
                var roleCtl = new RoleController();
                var portalId = PortalSettings.Current.PortalId;
                var ui = UserController.GetUserById(portalId, Convert.ToInt32(n.Bvin));

                var gRole = roleCtl.GetRoleByName(portalId, GUEST_ROLENAME);

                if (gRole == null)
                {
                    gRole = new RoleInfo();

                    gRole.PortalID = portalId;
                    gRole.RoleName = GUEST_ROLENAME;
                    gRole.IsPublic = false;

                    //Null.NullInteger is giving error in Evoq. Needs to be -1
                    gRole.RoleGroupID = -1; //Null.NullInteger; Default value as per DNN EditRole.ascx.cs BindGroups()
                    gRole.Status = RoleStatus.Approved;

                    //Need Auto assignment (Not Null in DNN Role table)
                    gRole.AutoAssignment = false;

                    //Need Security Mode (Not Null in DNN Role table)
                    gRole.SecurityMode = SecurityMode.SecurityRole;

                    //Need IsSystemRole (Not Null in DNN Role table)
                    gRole.IsSystemRole = false;

                    //Even if this property are Nullable in DNN Role Table , DNN Store Procedure: AddRole() expects them as non nullable.
                    // Set them with default values. Reference DNN: EditRoles.ascx.cs (Line 382: OnUpdateClick())
                    gRole.Description = string.Empty;
                    gRole.ServiceFee = 0; //Required value 0 if BillingFrequency is 'N'
                    gRole.BillingPeriod = -1;
                    gRole.BillingFrequency = "N"; //Default in EditRole.ascx.cs

                    gRole.TrialPeriod = -1;
                    gRole.TrialFrequency = "N"; //Default in EditRole.ascx.cs
                    gRole.TrialFee = 0;

                    gRole.IconFile = string.Empty;
                    gRole.RSVPCode = string.Empty;

                    gRole.RoleID = roleCtl.AddRole(gRole);
                }

                roleCtl.AddUserRole(portalId, ui.UserID, gRole.RoleID, Null.NullDate, Null.NullDate);
            }
            catch (Exception ex)
            {
                LogError(ex);
                EventLog.LogEvent(ex);
            }
        }

        private static void LoginNewUser(OrderTaskContext context, CustomerAccount n)
        {
            try
            {
                var errorMessage = string.Empty;
                var userId = string.Empty;

                context.HccApp.MembershipServices.LoginUser(n.Username, n.Password, out errorMessage, out userId);
            }
            catch (Exception ex)
            {
                LogError(ex);
                EventLog.LogEvent(ex);
            }
        }

        /// <summary>
        /// Rolls back this task.
        /// </summary>
        /// <param name="context">The order task context for the operation.</param>
        /// <returns><see langword="true"/> because this task has no rollback work.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the stable identifier for this workflow task.
        /// </summary>
        /// <returns>The task identifier.</returns>
        public override string TaskId()
        {
            return "39950016-1AF4-4BF3-9BAC-9D40A27C87E5";
        }

        /// <summary>
        /// Gets the display name for this workflow task.
        /// </summary>
        /// <returns>The task name.</returns>
        public override string TaskName()
        {
            return "Create DNN User Account for New Customer";
        }

        /// <summary>
        /// Gets the display name for this workflow step.
        /// </summary>
        /// <returns>The step name.</returns>
        public override string StepName()
        {
            var result = string.Empty;
            result = "Create DNN User Account for New Customer";
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        /// <summary>
        /// Creates a new instance of this workflow task.
        /// </summary>
        /// <returns>A new <see cref="DnnCreateUserAccountForNewCustomer"/> instance.</returns>
        public override Task Clone()
        {
            return new DnnCreateUserAccountForNewCustomer();
        }

        private static void LogError(Exception ex)
        {
            if (ex != null)
            {
                _Logger.LogException(ex);
                if (ex.InnerException != null)
                {
                    LogError(ex.InnerException);
                }
            }
        }
    }
}
