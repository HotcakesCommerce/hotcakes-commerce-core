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
using System.Globalization;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Data;
using Hotcakes.Commerce.Membership;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IUserController = Hotcakes.Commerce.Dnn.IUserController;

namespace Hotcakes.Commerce.Tests
{
    [TestClass]
    [DeploymentItem(@"CsvData", "CsvData")]
    [DeploymentItem(@"DbBackup", "DbBackup")]
    public class BaseTest
    {
        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void BaseInitialize(TestContext testContext)
        //{
        //}

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void BaseTestInitialize()
        {
            // Cleanup in-memory database state
            var factory = Factory.Instance as TestHccFactory;
            if (factory != null)
            {
                factory.Connection = null;
                factory.InitFromCsv = true;
            }

            //MemoryStrategyFactory.Clear();

            InitBasicStubs();
        }

        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        public void InitBasicStubs()
        {
            DnnGlobal.SetTestableInstance(GetStubIDnnGlobal());
            DnnUserController.SetTestableInstance(GetStubIUserController(new List<UserInfo>()));
        }

        public static HotcakesApplication CreateHccAppInMemory(bool initFromCsv = true)
        {
            var factory = Factory.Instance as TestHccFactory;
            if (factory != null)
                factory.InitFromCsv = initFromCsv;

            HccRequestContext.Current = new HccRequestContext();
            HccRequestContext.Current.CurrentStore = new Store {Id = 1};

            return HotcakesApplication.Current;
        }

        public static HotcakesApplication CreateHccAppForDb()
        {
            var factory = Factory.Instance as TestHccFactory;
            if (factory != null)
                factory.UseDbConnection = true;

            HccRequestContext.Current = new HccRequestContext();
            HccRequestContext.Current.CurrentStore = new Store {Id = 1};

            return HotcakesApplication.Current;
        }

        #region Fakes factory

        protected IDnnGlobal GetStubIDnnGlobal()
        {
            return new DnnGlobalStub();
        }

        protected IUserController GetStubIUserController(List<UserInfo> users)
        {
            return new UserControllerStub(users);
        }

        internal class DnnGlobalStub : IDnnGlobal
        {
            public int GetPortalId()
            {
                return 1;
            }

            public PortalInfo GetCurrentPortal()
            {
                return new PortalInfo();
            }

            public PortalSettings GetCurrentPortalSettings()
            {
                return new PortalSettings
                {
                    GUID = new Guid("5A767FB5-E1EA-4A41-AEDA-B54F431FA1F0"),
                    PortalName = "localhost.hotcakes"
                };
            }

            public Dictionary<string, Locale> GetLocales()
            {
                return new Dictionary<string, Locale>();
            }

            public Locale GetLocale(string code)
            {
                return new Locale {Code = code, PortalId = 1};
            }

            public CultureInfo GetPageLocale()
            {
                return CultureInfo.InvariantCulture;
            }
        }

        internal class UserControllerStub : IUserController
        {
            private readonly List<UserInfo> _users;

            public UserControllerStub(List<UserInfo> users)
            {
                _users = users;
            }

            public UserInfo GetCurrentUserInfo()
            {
                return new UserInfo();
            }

            public UserInfo BuildUserInfo(string username, string firstname, string lastname, string email,
                string password, int portalId)
            {
                var ui = new UserInfo
                {
                    PortalID = portalId,
                    Username = username,
                    Email = email
                };

                ui.Profile = new UserProfile(ui);
                ui.Membership = new UserMembership(ui);
                ui.Membership.Password = password;
                ui.Membership.Approved = true;
                ui.FirstName = firstname;
                ui.LastName = lastname;
                return ui;
            }

            public CreateUserStatus CreateUser(ref UserInfo ui)
            {
                ui.UserID = _users.Count + 1;
                _users.Add(ui);
                return CreateUserStatus.Success;
            }

            public void UpdateUser(int portalId, UserInfo ui)
            {
                var u1 = _users.First(u => u.PortalID == portalId && u.UserID == ui.UserID);
                u1.Username = ui.Username;
                u1.DisplayName = ui.DisplayName;
            }

            public UserInfo GetUser(int portalId, int userId, bool useCache = true)
            {
                return _users.FirstOrDefault(u => u.UserID == userId && u.PortalID == portalId);
            }

            public List<UserInfo> GetUsers(int portalId, bool includeSuperUsers)
            {
                return _users;
            }

            public List<DnnUser> GetUsersFromDb(int portalId, List<int> userIds)
            {
                return _users.Where(u => u.PortalID == portalId && userIds.Contains(u.UserID))
                    .Select(u => new DnnUser
                    {
                        UserID = u.UserID,
                        Username = u.Username,
                        DisplayName = u.DisplayName,
                        Profile = new List<DnnProfile>
                        {
                            new DnnProfile {PropertyName = "FirstName", PropertyValue = u.FirstName},
                            new DnnProfile {PropertyName = "LastName", PropertyValue = u.LastName}
                        }
                    })
                    .ToList();
            }

            public List<int> FindUserIds(string filter)
            {
                return _users.Select(u => u.UserID).ToList();
            }

            public void AddUserRole(int portalId, int userId, string roleName)
            {
            }

            public void RemoveUserRole(PortalSettings portalSettings, int userId, string roleName)
            {
            }

            public List<RoleInfo> GetRoles()
            {
                return new List<RoleInfo>();
            }

            public bool IsPortalAdmin(UserInfo u)
            {
                return false;
            }

            public void ResetPassword(int userId, string password, string paswordAnswer)
            {
                throw new NotImplementedException();
            }

            public bool IsDnnUsernameExists(string username)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}