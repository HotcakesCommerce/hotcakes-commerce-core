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
    /// <summary>
    /// Provides access to DNN users, roles, and account operations.
    /// </summary>
    public interface IUserController
    {
        /// <summary>
        /// Gets the authenticated user for the current portal.
        /// </summary>
        /// <returns>The current <see cref="UserInfo"/>, or <see langword="null"/> when the request is anonymous or has no portal.</returns>
        UserInfo GetCurrentUserInfo();

        /// <summary>
        /// Builds a DNN user record without persisting it.
        /// </summary>
        /// <param name="username">The user's login name.</param>
        /// <param name="firstname">The user's first name.</param>
        /// <param name="lastname">The user's last name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's initial password.</param>
        /// <param name="portalId">The portal that owns the user.</param>
        /// <returns>A populated <see cref="UserInfo"/> ready for creation.</returns>
        UserInfo BuildUserInfo(string username, string firstname, string lastname, string email, string password,
            int portalId);

        /// <summary>
        /// Creates a DNN user and maps the DNN result to a Hotcakes status.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The result of the create operation.</returns>
        CreateUserStatus CreateUser(ref UserInfo user);

        /// <summary>
        /// Updates a user in the specified portal.
        /// </summary>
        /// <param name="portalId">The portal that owns the user.</param>
        /// <param name="user">The user to update.</param>
        void UpdateUser(int portalId, UserInfo user);

        /// <summary>
        /// Gets a user by portal and user identifier.
        /// </summary>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="useCache">Whether to use DNN's cached user information.</param>
        /// <returns>The matching <see cref="UserInfo"/>, or <see langword="null"/> when no user exists.</returns>
        UserInfo GetUser(int portalId, int userId, bool useCache = true);

        /// <summary>
        /// Gets users belonging to a portal.
        /// </summary>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="includeSuperUsers">Whether to include superusers in addition to portal users.</param>
        /// <returns>The users found for the requested scope.</returns>
        List<UserInfo> GetUsers(int portalId, bool includeSuperUsers = false);

        /// <summary>
        /// Gets user and profile records directly from the DNN database views.
        /// </summary>
        /// <param name="portalId">The portal identifier used to establish the database context.</param>
        /// <param name="userIds">The user identifiers to retrieve.</param>
        /// <returns>The matching database user records with profiles.</returns>
        List<DnnUser> GetUsersFromDb(int portalId, List<int> userIds);

        /// <summary>
        /// Finds user identifiers whose display name, first name, last name, or email contains the filter.
        /// </summary>
        /// <param name="filter">The text to search for.</param>
        /// <returns>The identifiers of matching users.</returns>
        List<int> FindUserIds(string filter);

        /// <summary>
        /// Adds a user to a role, creating the role when necessary.
        /// </summary>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">The role name.</param>
        void AddUserRole(int portalId, int userId, string roleName);

        /// <summary>
        /// Removes a user from a role when both the role and user exist.
        /// </summary>
        /// <param name="portalSettings">The settings for the portal containing the role.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">The role name.</param>
        void RemoveUserRole(PortalSettings portalSettings, int userId, string roleName);

        /// <summary>
        /// Gets the roles for the current portal.
        /// </summary>
        /// <returns>The roles configured for the current portal.</returns>
        List<RoleInfo> GetRoles();

        /// <summary>
        /// Determines whether a user is a portal administrator or superuser.
        /// </summary>
        /// <param name="u">The user to evaluate.</param>
        /// <returns><see langword="true"/> when the user has portal administrator privileges; otherwise, <see langword="false"/>.</returns>
        bool IsPortalAdmin(UserInfo u);

        /// <summary>
        /// Changes a user's password after validating the current password answer.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The new password.</param>
        /// <param name="paswordAnswer">The answer to the user's password question.</param>
        void ResetPassword(int userId, string password, string paswordAnswer);

        /// <summary>
        /// Determines whether a username is already registered in DNN.
        /// </summary>
        /// <param name="username">The username to find.</param>
        /// <returns><see langword="true"/> when the username exists; otherwise, <see langword="false"/>.</returns>
        bool IsDnnUsernameExists(string username);
    }

    /// <summary>
    /// Provides the DNN user and role operations used by Hotcakes Commerce.
    /// </summary>
    [Serializable]
    public class DnnUserController : ServiceLocator<IUserController, DnnUserController>
    {
        protected override Func<IUserController> GetFactory()
        {
            return () => new UserControllerImpl();
        }

        internal class UserControllerImpl : IUserController
        {
            /// <summary>
            /// Gets the authenticated user for the current portal.
            /// </summary>
            /// <returns>The current <see cref="UserInfo"/>, or <see langword="null"/> when the request is anonymous or has no portal.</returns>
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

            /// <summary>
            /// Gets a user by portal and user identifier.
            /// </summary>
            /// <param name="portalId">The portal identifier.</param>
            /// <param name="userId">The user identifier.</param>
            /// <param name="useCache">Whether to use DNN's cached user information.</param>
            /// <returns>The matching <see cref="UserInfo"/>, or <see langword="null"/> when no user exists.</returns>
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

            /// <summary>
            /// Gets users belonging to a portal.
            /// </summary>
            /// <param name="portalId">The portal identifier.</param>
            /// <param name="includeSuperUsers">Whether to include superusers in addition to portal users.</param>
            /// <returns>The users found for the requested scope.</returns>
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

            /// <summary>
            /// Gets user and profile records directly from the DNN database views.
            /// </summary>
            /// <param name="portalId">The portal identifier used to establish the database context.</param>
            /// <param name="userIds">The user identifiers to retrieve.</param>
            /// <returns>The matching database user records with profiles.</returns>
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

            /// <summary>
            /// Finds user identifiers whose display name, first name, last name, or email contains the filter.
            /// </summary>
            /// <param name="filter">The text to search for.</param>
            /// <returns>The identifiers of matching users.</returns>
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

            /// <summary>
            /// Builds a DNN user record without persisting it.
            /// </summary>
            /// <param name="username">The user's login name.</param>
            /// <param name="firstname">The user's first name.</param>
            /// <param name="lastname">The user's last name.</param>
            /// <param name="email">The user's email address.</param>
            /// <param name="password">The user's initial password.</param>
            /// <param name="portalId">The portal that owns the user.</param>
            /// <returns>A populated <see cref="UserInfo"/> ready for creation.</returns>
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

            /// <summary>
            /// Creates a DNN user and maps the DNN result to a Hotcakes status.
            /// </summary>
            /// <param name="user">The user to create.</param>
            /// <returns>The result of the create operation.</returns>
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

            /// <summary>
            /// Updates a user in the specified portal.
            /// </summary>
            /// <param name="portalId">The portal that owns the user.</param>
            /// <param name="user">The user to update.</param>
            public void UpdateUser(int portalId, UserInfo user)
            {
                UserController.UpdateUser(portalId, user);
            }

            /// <summary>
            /// Adds a user to a role, creating the role when necessary.
            /// </summary>
            /// <param name="portalId">The portal identifier.</param>
            /// <param name="userId">The user identifier.</param>
            /// <param name="roleName">The role name.</param>
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

            /// <summary>
            /// Removes a user from a role when both the role and user exist.
            /// </summary>
            /// <param name="portalSettings">The settings for the portal containing the role.</param>
            /// <param name="userId">The user identifier.</param>
            /// <param name="roleName">The role name.</param>
            public void RemoveUserRole(PortalSettings portalSettings, int userId, string roleName)
            {
                var role = RoleController.Instance.GetRoleByName(portalSettings.PortalId, roleName);
                var user = UserController.Instance.GetUser(portalSettings.PortalId, userId);

                if (role != null && user != null)
                {
                    RoleController.DeleteUserRole(user, role, portalSettings, false);
                }
            }

            /// <summary>
            /// Gets the roles for the current portal.
            /// </summary>
            /// <returns>The roles configured for the current portal.</returns>
            public List<RoleInfo> GetRoles()
            {
                var portalId = DnnGlobal.Instance.GetPortalId();
                return RoleController.Instance.GetRoles(portalId).ToList();
            }

            /// <summary>
            /// Determines whether a user is a portal administrator or superuser.
            /// </summary>
            /// <param name="u">The user to evaluate.</param>
            /// <returns><see langword="true"/> when the user has portal administrator privileges; otherwise, <see langword="false"/>.</returns>
            public bool IsPortalAdmin(UserInfo u)
            {
                var pSett = DnnGlobal.Instance.GetCurrentPortalSettings();
                if (pSett != null)
                    return u.IsSuperUser || u.IsInRole(pSett.AdministratorRoleName);
                return false;
            }

            /// <summary>
            /// Changes a user's password after validating the current password answer.
            /// </summary>
            /// <param name="userId">The user identifier.</param>
            /// <param name="password">The new password.</param>
            /// <param name="paswordAnswer">The answer to the user's password question.</param>
            public void ResetPassword(int userId, string password, string paswordAnswer)
            {
                var ui = GetUser(DnnGlobal.Instance.GetPortalId(), userId);
                UserController.ChangePassword(ui, UserController.GetPassword(ref ui, paswordAnswer), password);
            }

            /// <summary>
            /// Determines whether a username is already registered in DNN.
            /// </summary>
            /// <param name="username">The username to find.</param>
            /// <returns><see langword="true"/> when the username exists; otherwise, <see langword="false"/>.</returns>
            public bool IsDnnUsernameExists(string username)
            {
                var totalCount = 0;
                UserController.GetUsersByUserName(Null.NullInteger, username, 0, 1, ref totalCount, true, false);

                return totalCount > 0;
            }
        }
    }
}
