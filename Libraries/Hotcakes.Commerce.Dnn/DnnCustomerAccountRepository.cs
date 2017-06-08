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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Dnn.Data;
using Hotcakes.Commerce.Membership;
using Hotcakes.Common.Dnn;
using Hotcakes.Web;
using Json = Hotcakes.Web.Json;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnCustomerAccountRepository : ICustomerAccountRepository
    {
        #region Constructor

        public DnnCustomerAccountRepository(HccRequestContext c)
        {
            Context = c;
        }

        #endregion

        #region Properties

        protected HccRequestContext Context;

        private int PortalId
        {
            get { return DnnGlobal.Instance.GetPortalId(); }
        }

        #endregion

        #region Public methods

        public CustomerAccount Find(string bvin, bool useCache = true)
        {
            var userId = bvin.ConvertTo(Null.NullInteger);
            var uInfo = DnnUserController.Instance.GetUser(PortalId, userId, useCache);

            if (uInfo != null)
            {
                return UserInfoToCustomer(uInfo);
            }

            return null;
        }

        public CustomerAccount FindForAllStores(string bvin)
        {
            return Find(bvin);
        }

        public bool Create(CustomerAccount cust)
        {
            var status = UserCreateStatus.UnexpectedError;
            CreateDnnAccount(cust, PortalId, ref status);
            return status == UserCreateStatus.Success;
        }

        public bool Update(CustomerAccount cust)
        {
            try
            {
                var userId = cust.Bvin.ConvertTo(Null.NullInteger);
                var uInfo = DnnUserController.Instance.GetUser(PortalId, userId);

                if (uInfo != null)
                {
                    UpdateUserInfo(uInfo, cust);
                    DnnUserController.Instance.UpdateUser(PortalId, uInfo);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Delete(string bvin)
        {
            var userId = bvin.ConvertTo(Null.NullInteger);
            var uInfo = UserController.GetUserById(PortalId, userId);
            return uInfo == null ? false : UserController.DeleteUser(ref uInfo, false, false);
        }

        public void DeleteAllForPortal()
        {
            UserController.DeleteUsers(PortalId, false, false);
            UserController.RemoveDeletedUsers(PortalId);
        }

        public List<CustomerAccount> FindAll()
        {
            var users = UserController.GetUsers(PortalId).OfType<UserInfo>();
            if (users != null)
            {
                return users
                    .Select(u => UserInfoToCustomer(u))
                    .ToList();
            }

            return new List<CustomerAccount>();
        }

        public List<CustomerAccount> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var pageIndex = GetPageIndex(pageNumber);
            var users = UserController.GetUsers(PortalId, pageIndex, pageSize, ref totalCount).OfType<UserInfo>();
            if (users != null)
            {
                return users
                    .Select(u => UserInfoToCustomer(u))
                    .ToList();
            }

            return new List<CustomerAccount>();
        }

        public List<CustomerAccount> FindAllPaged(int pageNumber, int pageSize)
        {
            var totalCount = 0;
            return FindAllPaged(pageNumber, pageSize, ref totalCount);
        }

        public List<CustomerAccount> FindByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var totalCount = 0;
            return UserController.GetUsersByEmail(PortalId, email, 0, 1, ref totalCount)
                .OfType<UserInfo>()
                .Select(ui => UserInfoToCustomer(ui))
                .ToList();
        }

        public CustomerAccount FindByUsername(string username)
        {
            var totalCount = 0;
            var user =
                UserController.GetUsersByUserName(PortalId, username, 0, 1, ref totalCount)
                    .OfType<UserInfo>()
                    .FirstOrDefault();
            return user == null ? null : UserInfoToCustomer(user);
        }

        public List<CustomerAccount> FindMany(List<string> ids)
        {
            var users = DnnUserController.Instance.GetUsers(PortalId, true)
                .Where(u => ids.Contains(u.UserID.ToString()))
                .ToList();

            if (users != null)
            {
                return users
                    .Select(u => UserInfoToCustomer(u))
                    .ToList();
            }

            return new List<CustomerAccount>();
        }

        public List<CustomerAccount> FindByFilter(string filter, int pageNumber, int pageSize, ref int totalCount)
        {
            var pageIndex = GetPageIndex(pageNumber);
            IEnumerable<UserInfo> users = null;
            if (string.IsNullOrEmpty(filter))
            {
                users = UserController.GetUsers(PortalId, pageIndex, pageSize, ref totalCount)
                    .OfType<UserInfo>();
            }
            else
            {
                using (var dnnDb = DnnDbContext.Instantiate())
                {
                    var userIds = dnnDb.vw_Users
                        .Where(u =>
                            u.Email.Contains(filter) ||
                            (u.Username != null && u.Username.Contains(filter)) ||
                            (u.FirstName != null && u.FirstName.Contains(filter)) ||
                            (u.LastName != null && u.LastName.Contains(filter)) ||
                            (u.DisplayName != null && u.DisplayName.Contains(filter)))
                        .OrderBy(u => u.Username)
                        .Select(u => u.UserID)
                        .ToList();

                    userIds.AddRange(dnnDb.vw_Profile
                        .Where(p => p.PropertyValue.Contains(filter) ||
                                    p.PropertyValue.Contains(filter))
                        .OrderBy(u => u.UserID)
                        .Select(u => u.UserID)
                        .ToList());

                    userIds = userIds.Distinct().ToList();
                    totalCount = userIds.Count();
                    var filteredIds = userIds.Skip(pageIndex*pageSize).Take(pageSize);
                    var usersInfo = new List<UserInfo>();
                    foreach (var id in filteredIds)
                    {
                        usersInfo.Add(UserController.GetUserById(PortalId, id));
                    }
                    users = usersInfo;
                }
            }
            return users.Select(UserInfoToCustomer).ToList();
        }

        public int CountOfAll()
        {
            var users = UserController.GetUsers(PortalId);
            return users == null ? 0 : users.Count;
        }

        public void DestroyAllForStore(long storeId)
        {
            // We won't delete store users since we cann't be sure
            // if those users are portal users or were created only for store access
            Factory.CreateEventLogger().LogMessage("We are not deleting store users");
        }

        #endregion

        #region Implementation

        protected CustomerAccount UserInfoToCustomer(UserInfo uInfo)
        {
            if (uInfo == null)
                throw new ArgumentNullException("uInfo");

            var addressBook = uInfo.Profile.GetPropertyValue("HccAddressBook") ?? "";
            var billingAddress = uInfo.Profile.GetPropertyValue("HccBillingAddress") ?? "";
            var shippingAddress = uInfo.Profile.GetPropertyValue("HccShippingAddress") ?? "";
            var taxExemptString = uInfo.Profile.GetPropertyValue("HccTaxExempt");
            var taxExempt = string.IsNullOrEmpty(taxExemptString) ? false : bool.Parse(taxExemptString);
            var taxExemptionNumber = uInfo.Profile.GetPropertyValue("HccTaxExemptionNumber") ?? "";
            var notes = uInfo.Profile.GetPropertyValue("HccNotes") ?? "";
            var pricingGroupId = uInfo.Profile.GetPropertyValue("HccPricingGroupId") ?? "";

            return new CustomerAccount
            {
                Bvin = uInfo.UserID.ToString(),
                StoreId = Context.CurrentStore != null ? Context.CurrentStore.Id : 0,
                Username = uInfo.Username,
                Email = uInfo.Email,
                FirstName = uInfo.FirstName,
                LastName = uInfo.LastName,
                Password = string.Empty,
                TaxExempt = taxExempt,
                TaxExemptionNumber = taxExemptionNumber,
                Notes = notes,
                PricingGroupId = pricingGroupId,
                Locked = !uInfo.Membership.Approved,
                LockedUntilUtc = DateTime.UtcNow, // TODO: Remove from CustomerAccount
                FailedLoginCount = 0, // TODO: Remove from CustomerAccount

                LastUpdatedUtc = uInfo.LastModifiedOnDate.ToUniversalTime(),
                CreationDateUtc = uInfo.CreatedOnDate.ToUniversalTime(),
                LastLoginDateUtc = uInfo.Membership.LastLoginDate.ToUniversalTime(),
                BillingAddress = Json.ObjectFromJson<Address>(billingAddress) ?? new Address(),
                ShippingAddress = Json.ObjectFromJson<Address>(shippingAddress) ?? new Address(),
                Addresses = AddressList.FromJson(addressBook)
            };
        }

        internal static bool CreateDnnAccount(CustomerAccount cust, int portalId, ref UserCreateStatus status)
        {
            if (cust == null)
                throw new ArgumentNullException("cust");

            var ui = new UserInfo();
            if (string.IsNullOrWhiteSpace(cust.Username))
            {
                cust.Username = BuildUsername(cust);
            }

            ui.InitRequiredProperties(
                cust.Username,
                cust.FirstName,
                cust.LastName,
                cust.Email,
                cust.Password,
                portalId,
                true);

            UpdateUserInfo(ui, cust);
            status = UserController.CreateUser(ref ui);
            cust.Bvin = ui.UserID.ToString();

            return status == UserCreateStatus.Success;
        }

        internal static void UpdateUserInfo(UserInfo uInfo, CustomerAccount cust)
        {
            if (uInfo == null)
                throw new ArgumentNullException("uInfo");

            uInfo.Email = cust.Email;
            uInfo.FirstName = cust.FirstName;
            uInfo.LastName = cust.LastName;
            uInfo.Membership.Approved = !cust.Locked;

            var profile = uInfo.Profile;
            var portalId = uInfo.IsSuperUser ? Null.NullInteger : DnnGlobal.Instance.GetPortalId();

            profile.EnsureProfileProperty("HccAddressBook", "Hotcakes", portalId, false);
            profile.EnsureProfileProperty("HccBillingAddress", "Hotcakes", portalId, false);
            profile.EnsureProfileProperty("HccShippingAddress", "Hotcakes", portalId, false);
            profile.EnsureProfileProperty("HccTaxExempt", "Hotcakes", portalId, false);
            profile.EnsureProfileProperty("HccTaxExemptionNumber", "Hotcakes", portalId, false);
            profile.EnsureProfileProperty("HccNotes", "Hotcakes", portalId, false);
            profile.EnsureProfileProperty("HccPricingGroupId", "Hotcakes", portalId, false);

            profile.SetProfileProperty("HccAddressBook", cust.Addresses.ToJson());
            profile.SetProfileProperty("HccBillingAddress", cust.BillingAddress.ObjectToJson());
            profile.SetProfileProperty("HccShippingAddress", cust.ShippingAddress.ObjectToJson());
            profile.SetProfileProperty("HccTaxExempt", cust.TaxExempt.ToString());
            profile.SetProfileProperty("HccTaxExemptionNumber", cust.TaxExemptionNumber);
            profile.SetProfileProperty("HccNotes", cust.Notes);
            profile.SetProfileProperty("HccPricingGroupId", cust.PricingGroupId);
        }

        internal static string BuildUsername(CustomerAccount cust)
        {
            var userNameBase = string.IsNullOrEmpty(cust.FirstName) ? string.Empty : cust.FirstName.Substring(0, 1).ToLower();
            userNameBase += cust.LastName ?? "user";

            var userName = userNameBase;
            var suffix = 1;

            while (DnnUserController.Instance.IsDnnUsernameExists(userName))
            {
                userName = userNameBase + suffix;
                suffix++;
            }

            return userName;
        }

        private static int GetPageIndex(int pageNumber)
        {
            pageNumber--;
            return pageNumber >= 0 ? pageNumber : 0;
        }

        internal static Dictionary<string, string> GetUserSettings(string userId, bool getCached = true)
        {
            var id = userId.ConvertTo(Null.NullInteger);
            var portalId = DnnGlobal.Instance.GetPortalId();
            var uInfo = DnnUserController.Instance.GetUser(portalId, id, getCached);

            if (uInfo != null)
            {
                return uInfo.Profile.ProfileProperties
                    .GetByCategory("Hotcakes")
                    .OfType<ProfilePropertyDefinition>()
                    .ToDictionary(p => p.PropertyName, p => p.PropertyValue);
            }

            return new Dictionary<string, string>();
        }

        internal static void UpdateUserSettings(string userId, Dictionary<string, string> settings, bool isCached = true)
        {
            var id = userId.ConvertTo(Null.NullInteger);
            var portalId = DnnGlobal.Instance.GetPortalId();
            var uInfo = DnnUserController.Instance.GetUser(portalId, id, isCached);

            if (uInfo != null)
            {
                if (uInfo.IsSuperUser)
                {
                    portalId = Null.NullInteger;
                }
                foreach (var key in settings.Keys)
                {
                    var value = settings[key];
                    uInfo.Profile.EnsureProfileProperty(key, "Hotcakes", portalId, false);
                    uInfo.Profile.SetProfileProperty(key, value);
                }

                DnnUserController.Instance.UpdateUser(portalId, uInfo);
            }
        }

        #endregion
    }
}