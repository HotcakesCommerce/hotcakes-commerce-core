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
using System.Text;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce.Dnn.Marketing.Qualifications
{
    [Serializable]
    public class UserIsInRole : PromotionQualificationBase
    {
        public const string TypeIdString = "EF230BB4-C7E1-482F-98EB-AC2706863E85";
        private readonly int _portalId;

        #region Constructor

        public UserIsInRole()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
            _portalId = PortalSettings.Current.PortalId;
        }

        #endregion

        #region Properties

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        #endregion

        #region Public methods

        public List<RoleInfo> GetAllRoles()
        {
            return RoleController.Instance.GetRoles(_portalId).ToList();
        }

        public void AddRole(string roleId)
        {
            var ids = GetRoleIds();
            ids.Add(Convert.ToInt32(roleId));
            SaveRoleIds(ids);
        }

        public void RemoveRole(string roleId)
        {
            var ids = GetRoleIds();
            var id = Convert.ToInt32(roleId);
            ids.Remove(id);
            SaveRoleIds(ids);
        }

        public List<RoleInfo> GetRoles()
        {
            return GetRoleIds().Select(i => GetRoleById(i)).ToList();
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var sb = new StringBuilder();

            sb.Append("When User Is In Role:<ul>");
            var roles = GetRoles();

            foreach (var role in roles)
            {
                sb.AppendFormat("<li>{0}</li>", role.RoleName);
            }

            sb.Append("</ul>");

            return sb.ToString();
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.CurrentCustomer == null) return false;
            if (context.CurrentCustomer.Bvin == string.Empty) return false;

            var customer = context.CurrentCustomer;
            var roles = GetCustomerRoles(customer);
            var roleIds = GetRoleIds();

            foreach (var role in roles)
            {
                if (roleIds.Contains(role.RoleID))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Implementation

        private List<RoleInfo> GetCustomerRoles(CustomerAccount customer)
        {
            var user = DnnUserController.Instance.GetUser(_portalId, Convert.ToInt32(customer.Bvin));
            var roles = RoleController.Instance.GetUserRoles(user, true);
            return roles.OfType<RoleInfo>().ToList();
        }

        private RoleInfo GetRoleById(int roleId)
        {
            return RoleController.Instance.GetRoleById(_portalId, roleId);
        }

        private List<int> GetRoleIds()
        {
            return GetSettingIds("roles");
        }

        private void SaveRoleIds(List<int> roleIds)
        {
            SetSetting("roles", roleIds);
        }

        #endregion
    }
}