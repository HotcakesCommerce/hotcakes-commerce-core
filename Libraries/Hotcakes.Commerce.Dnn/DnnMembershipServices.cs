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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Common.Dnn;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnMembershipServices : MembershipServices, IMembershipServices
    {
        public DnnMembershipServices(HccRequestContext context)
            : base(context)
        {
            Customers = new DnnCustomerAccountRepository(Context);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public DnnMembershipServices(HccRequestContext c, bool isForMemoryOnly)
            : this(c)
        {
        }

        #endregion

        public override bool LoginUser(string username, string password, out string errorMessage, out string userId)
        {
            var psett = PortalSettings.Current;
            errorMessage = "";
            userId = null;

            var loginStatus = UserLoginStatus.LOGIN_FAILURE;
            var request = Context.RoutingContext.HttpContext.Request;
            var uInfo = UserController.UserLogin(psett.PortalId, username, password, string.Empty, psett.PortalName,
                request.UserHostAddress, ref loginStatus, true);

            if (loginStatus == UserLoginStatus.LOGIN_SUCCESS || loginStatus == UserLoginStatus.LOGIN_SUPERUSER)
            {
                userId = uInfo.UserID.ToString();
                return true;
            }
            switch (loginStatus)
            {
                case UserLoginStatus.LOGIN_FAILURE:
                    errorMessage = "Password is incorrect.";
                    break;
                case UserLoginStatus.LOGIN_USERLOCKEDOUT:
                    errorMessage = "User is locked out.";
                    break;
                case UserLoginStatus.LOGIN_USERNOTAPPROVED:
                    errorMessage = "User is not approved.";
                    break;
                default:
                    errorMessage = "Unknown login error. Contact administrator for assistance.";
                    break;
            }

            return false;
        }

        public override bool IsUserLoggedIn()
        {
            var user = DnnUserController.Instance.GetCurrentUserInfo();
            return user != null;
        }

        public override bool IsSuperUserLoggedIn()
        {
            var usr = DnnUserController.Instance.GetCurrentUserInfo();
            return usr != null && usr.IsSuperUser;
        }

        public override string GetCurrentUserId()
        {
            var ui = DnnUserController.Instance.GetCurrentUserInfo();

            return ui == null ? string.Empty : ui.UserID.ToString();
        }

        public override bool HasCurrentUserPermission(string perm, HotcakesApplication app)
        {
            var usr = DnnUserController.Instance.GetCurrentUserInfo();
            if (usr != null)
            {
                var roleName = RolePermissionsMatrix.GetRoleNameByPermission(app, perm);
                if (!string.IsNullOrEmpty(roleName))
                    return usr.IsInRole(roleName);
                return usr.IsPortalAdmin();
            }

            return false;
        }

        public override string GetLoginPagePath()
        {
            return Globals.NavigateURL(PortalSettings.Current.LoginTabId, "Login",
                "returnurl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));
        }

        public override bool CreateCustomer(CustomerAccount u, out CreateUserStatus status, string clearPassword)
        {
            status = CreateUserStatus.None;
            var dnnStatus = UserCreateStatus.UnexpectedError;

            if (!string.IsNullOrEmpty(clearPassword))
            {
                u.Password = clearPassword;
            }

            DnnCustomerAccountRepository.CreateDnnAccount(u, PortalSettings.Current.PortalId, ref dnnStatus);

            switch (dnnStatus)
            {
                case UserCreateStatus.Success:
                    status = CreateUserStatus.Success;
                    break;
                case UserCreateStatus.DuplicateEmail:
                    status = CreateUserStatus.DuplicateEmail;
                    break;
                case UserCreateStatus.DuplicateUserName:
                case UserCreateStatus.UsernameAlreadyExists:
                    status = CreateUserStatus.DuplicateUsername;
                    break;
                case UserCreateStatus.InvalidPassword:
                    status = CreateUserStatus.InvalidPassword;
                    break;
                case UserCreateStatus.UserRejected:
                    status = CreateUserStatus.UserRejected;
                    break;
                default:
                    status = CreateUserStatus.UpdateFailed;
                    break;
            }

            return status == CreateUserStatus.Success;
        }

        public override List<string> GetAllRoles()
        {
            return DnnUserController.Instance.GetRoles().Select(r => r.RoleName).ToList();
        }

        public override Dictionary<string, string> GetUserSettings(string userId, bool getCached = true)
        {
            return DnnCustomerAccountRepository.GetUserSettings(userId, getCached);
        }

        public override void UpdateUserSettings(string userId, Dictionary<string, string> settings,
            bool getCached = true)
        {
            DnnCustomerAccountRepository.UpdateUserSettings(userId, settings, getCached);
        }

        public override void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan)
        {
            var portalId = PortalSettings.Current.PortalId;
            var userInfo = UserController.GetUserById(portalId, Convert.ToInt32(userId));

            var _roleController = new RoleController();
            var roleInfo = _roleController.GetRoleByName(portalId, membershipType.RoleName);
            var lastExpirationTime = TimeSpan.Zero;
            if (userInfo.IsInRole(membershipType.RoleName))
            {
                var userRole = _roleController.GetUserRole(portalId, userInfo.UserID, roleInfo.RoleID);
                var unusedPeriod = userRole.ExpiryDate - DateTime.Now;

                if (lastExpirationTime < unusedPeriod)
                {
                    lastExpirationTime = unusedPeriod;
                }

                RoleController.DeleteUserRole(userInfo, roleInfo, PortalSettings.Current, false);
            }

            // DNN does verification against "DateTime.Now" and doesn't use UTC.
            RoleController.AddUserRole(userInfo, roleInfo, PortalSettings.Current,
                RoleStatus.Approved,
                DateTime.Now, // effective date
                DateTime.Now + lastExpirationTime + timeSpan, // expiration date
                membershipType.Notify, // notify user
                false); // is owner
        }
    }
}