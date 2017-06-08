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
using System.Threading;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using Hotcakes.Commerce.Dnn.Data;
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce.Dnn
{
    public interface IUserController
    {
        UserInfo GetCurrentUserInfo();

        UserInfo BuildUserInfo(string username, string firstname, string lastname, string email, string password,
            int portalId);

        CreateUserStatus CreateUser(ref UserInfo user);
        void UpdateUser(int portalId, UserInfo user);
        UserInfo GetUser(int portalId, int userId, bool useCache = true);
        List<UserInfo> GetUsers(int portalId, bool includeSuperUsers = false);
        List<DnnUser> GetUsersFromDb(int portalId, List<int> userIds);
        List<int> FindUserIds(string filter);
        void AddUserRole(int portalId, int userId, string roleName);
        void RemoveUserRole(PortalSettings portalSettings, int userId, string roleName);
        List<RoleInfo> GetRoles();
        bool IsPortalAdmin(UserInfo u);
        void ResetPassword(int userId, string password, string paswordAnswer);
        bool IsDnnUsernameExists(string username);
    }

    [Serializable]
    public class DnnUserController : ServiceLocator<IUserController, DnnUserController>
    {
        protected override Func<IUserController> GetFactory()
        {
            return () => new UserControllerImpl();
        }

        internal class UserControllerImpl : IUserController
        {
            public UserInfo GetCurrentUserInfo()
            {
                var portalId = DnnGlobal.Instance.GetPortalId();
                var identity = Thread.CurrentPrincipal.Identity;

                if (portalId != Null.NullInteger && identity.IsAuthenticated)
                {
                    return UserController.GetCachedUser(portalId, identity.Name);
                }

                return null;
            }

            public UserInfo GetUser(int portalId, int userId, bool useCache = true)
            {
                if (useCache)
                {
                    return UserController.GetUserById(portalId, userId);
                }
                // Copied from DNN. Turns out there is no other way to ensure that we don't use cached user info
                var effectivePortalId = PortalController.GetEffectivePortalId(portalId);
                var user = MembershipProvider.Instance().GetUser(effectivePortalId, userId);
                if (user != null)
                    user.PortalID = portalId;
                return user;
            }

            public List<UserInfo> GetUsers(int portalId, bool includeSuperUsers = false)
            {
                var users = UserController.GetUsers(portalId).OfType<UserInfo>().ToList();
                if (includeSuperUsers)
                {
                    users.AddRange(
                        UserController.GetUsers(Null.NullInteger).OfType<UserInfo>().Where(u => u.IsSuperUser).ToList());
                }

                return users;
            }

            public List<DnnUser> GetUsersFromDb(int portalId, List<int> userIds)
            {
                using (var dnnDb = DnnDbContext.Instantiate())
                {
                    var usersAndProfile = dnnDb.vw_Users
                        .Where(u => userIds.Contains(u.UserID))
                        .Join(dnnDb.vw_Profile, u => u.UserID, p => p.UserID, (u, p) => new {User = u, Profile = p})
                        .GroupBy(up => up.User.UserID)
                        .ToList();

                    return usersAndProfile.Select(up =>
                    {
                        var u = up.First().User;
                        u.Profile = up.Select(upi => upi.Profile).ToList();
                        return u;
                    }).ToList();
                }
            }

            public List<int> FindUserIds(string filter)
            {
                using (var dnnDb = DnnDbContext.Instantiate())
                {
                    return dnnDb.vw_Users
                        .Where(u => u.DisplayName.Contains(filter)
                                    || u.FirstName.Contains(filter)
                                    || u.LastName.Contains(filter)
                                    || u.Email.Contains(filter))
                        .Select(u => u.UserID)
                        .ToList();
                }
            }

            public UserInfo BuildUserInfo(string username, string firstname, string lastname, string email,
                string password, int portalId)
            {
                var ui = new UserInfo
                {
                    PortalID = portalId,
                    Username = username,
                    FirstName = firstname,
                    LastName = lastname,
                    Email = email
                };

                var sett = DnnGlobal.Instance.GetCurrentPortalSettings();
                if (sett != null)
                {
                    ui.Profile.PreferredLocale = sett.DefaultLanguage;
                }
                ui.Membership.Password = password;
                ui.Membership.Approved = true;
                return ui;
            }

            public CreateUserStatus CreateUser(ref UserInfo user)
            {
                var cuStatus = CreateUserStatus.None;
                var dnnStatus = UserController.CreateUser(ref user);

                switch (dnnStatus)
                {
                    case UserCreateStatus.Success:
                        cuStatus = CreateUserStatus.Success;
                        break;

                    case UserCreateStatus.DuplicateEmail:
                        cuStatus = CreateUserStatus.DuplicateEmail;
                        break;

                    case UserCreateStatus.DuplicateUserName:
                    case UserCreateStatus.UserAlreadyRegistered:
                    case UserCreateStatus.UsernameAlreadyExists:
                        cuStatus = CreateUserStatus.DuplicateUsername;
                        break;
                    case UserCreateStatus.InvalidPassword:
                        cuStatus = CreateUserStatus.InvalidPassword;
                        break;
                    default:
                        cuStatus = CreateUserStatus.UpdateFailed;
                        break;
                }

                return cuStatus;
            }

            public void UpdateUser(int portalId, UserInfo user)
            {
                UserController.UpdateUser(portalId, user);
            }

            public void AddUserRole(int portalId, int userId, string roleName)
            {
                var ctl = new RoleController();
                var role = ctl.GetRoleByName(portalId, roleName);

                if (role == null)
                {
                    role = new RoleInfo
                    {
                        PortalID = portalId,
                        RoleGroupID = Null.NullInteger,
                        RoleName = roleName
                    };
                    role.RoleID = ctl.AddRole(role);
                }

                if (ctl.GetUserRole(portalId, userId, role.RoleID) == null)
                {
                    ctl.AddUserRole(portalId, userId, role.RoleID, Null.NullDate, Null.NullDate);
                }
            }

            public void RemoveUserRole(PortalSettings portalSettings, int userId, string roleName)
            {
                var role = RoleController.Instance.GetRoleByName(portalSettings.PortalId, roleName);
                var user = UserController.Instance.GetUser(portalSettings.PortalId, userId);

                if (role != null && user != null)
                {
                    RoleController.DeleteUserRole(user, role, portalSettings, false);
                }
            }

            public List<RoleInfo> GetRoles()
            {
                var portalId = DnnGlobal.Instance.GetPortalId();
                return RoleController.Instance.GetRoles(portalId).ToList();
            }

            public bool IsPortalAdmin(UserInfo u)
            {
                var pSett = DnnGlobal.Instance.GetCurrentPortalSettings();
                if (pSett != null)
                    return u.IsSuperUser || u.IsInRole(pSett.AdministratorRoleName);
                return false;
            }

            public void ResetPassword(int userId, string password, string paswordAnswer)
            {
                var ui = GetUser(DnnGlobal.Instance.GetPortalId(), userId);
                UserController.ChangePassword(ui, UserController.GetPassword(ref ui, paswordAnswer), password);
            }

            public bool IsDnnUsernameExists(string username)
            {
                var totalCount = 0;
                UserController.GetUsersByUserName(Null.NullInteger, username, 0, 1, ref totalCount, true, false);

                return totalCount > 0;
            }
        }
    }
}