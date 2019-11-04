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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Peform operation on  hcc_MembershipProductType database table.
    /// </summary>
    public class MembershipProductTypeRepository : HccSimpleRepoBase<hcc_MembershipProductType, MembershipProductType>
    {
        private readonly ProductTypeRepository productTypesRepository;

        public MembershipProductTypeRepository(HccRequestContext c)
            : base(c)
        {
            productTypesRepository = new ProductTypeRepository(c);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public MembershipProductTypeRepository(HccRequestContext c, bool isForMemoryOnly)
            : this(c)
        {
        }

        #endregion

        /// <summary>
        ///     Copy database view model to object model
        /// </summary>
        /// <param name="data"><see cref="hcc_MembershipProductType" /> instance</param>
        /// <param name="model"><see cref="MembershipProductType" /> instance</param>
        protected override void CopyModelToData(hcc_MembershipProductType data, MembershipProductType model)
        {
            data.ProductTypeBvin = DataTypeHelper.BvinToGuid(model.ProductTypeId);
            data.RoleName = model.RoleName;
            data.ExpirationPeriod = model.ExpirationPeriod;
            data.ExpirationPeriodType = (int) model.ExpirationPeriodType;
            data.Notify = model.Notify;
        }

        /// <summary>
        ///     Copy database object model to view model
        /// </summary>
        /// <param name="data"><see cref="hcc_MembershipProductType" /> instance</param>
        /// <param name="model"><see cref="MembershipProductType" /> instance</param>
        protected override void CopyDataToModel(hcc_MembershipProductType data, MembershipProductType model)
        {
            model.ProductTypeId = DataTypeHelper.GuidToBvin(data.ProductTypeBvin);
            model.RoleName = data.RoleName;
            model.ExpirationPeriod = data.ExpirationPeriod;
            model.ExpirationPeriodType = (ExpirationPeriodType) data.ExpirationPeriodType;
            model.Notify = data.Notify;
        }

        /// <summary>
        ///     Get list of the Membership product type.
        /// </summary>
        /// <param name="storeId">Unique store identifier</param>
        /// <returns>Returns list of <see cref="MembershipProductType" /></returns>
        public List<MembershipProductType> GetList(long storeId)
        {
            var productTypes = productTypesRepository.FindAllForStore(storeId);
            var productTypeIds = productTypes.Select(pt => DataTypeHelper.BvinToGuid(pt.Bvin)).ToList();

            var membershipProductTypes =
                FindListPoco(q => { return q.Where(mpt => productTypeIds.Contains(mpt.ProductTypeBvin)); });

            foreach (var membershipProductType in membershipProductTypes)
            {
                var productType = productTypes
                    .Where(pt => pt.Bvin == membershipProductType.ProductTypeId)
                    .FirstOrDefault();

                membershipProductType.StoreId = productType.StoreId;
                membershipProductType.ProductTypeName = productType.ProductTypeName;
            }

            return membershipProductTypes;
        }

        /// <summary>
        ///     Find MembershiptProductType by id
        /// </summary>
        /// <param name="productTypeId">MembershipProductType unique identifier.</param>
        /// <returns>Returns <see cref="MembershipProductType" /> instance</returns>
        public MembershipProductType Find(string productTypeId)
        {
            var productType = productTypesRepository.Find(productTypeId);

            var productTypeGuid = DataTypeHelper.BvinToGuid(productTypeId);
            var membershipProductType = FindFirstPoco(y => y.ProductTypeBvin == productTypeGuid);

            if (membershipProductType != null && productType != null)
            {
                membershipProductType.StoreId = productType.StoreId;
                membershipProductType.ProductTypeName = productType.ProductTypeName;

                return membershipProductType;
            }
            return null;
        }

        /// <summary>
        ///     Create MembershipProductType.
        /// </summary>
        /// <param name="mpt"><see cref="MembershipProductType" /> instance</param>
        /// <returns>Returns true if operation completed successfully</returns>
        public override bool Create(MembershipProductType mpt)
        {
            var pt = new ProductType
            {
                Bvin = Guid.NewGuid().ToString(),
                StoreId = mpt.StoreId,
                ProductTypeName = mpt.ProductTypeName,
                LastUpdated = DateTime.UtcNow,
                IsPermanent = true,
                TemplateName = string.Empty
            };

            var result = productTypesRepository.Create(pt);

            mpt.ProductTypeId = pt.Bvin;
            result &= base.Create(mpt);
            return result;
        }

        /// <summary>
        ///     Update MembershipProductType.
        /// </summary>
        /// <param name="mpt">Instance of  <see cref="MembershipProductType" /></param>
        /// <returns>Returns true if operation completed successfully</returns>
        public bool Update(MembershipProductType mpt)
        {
            var pt = new ProductType
            {
                Bvin = mpt.ProductTypeId,
                StoreId = mpt.StoreId,
                ProductTypeName = mpt.ProductTypeName,
                LastUpdated = DateTime.UtcNow,
                IsPermanent = true,
                TemplateName = string.Empty
            };
            var result = productTypesRepository.Update(pt);
            var productTypeGuid = DataTypeHelper.BvinToGuid(mpt.ProductTypeId);
            result &= base.Update(mpt, y => y.ProductTypeBvin == productTypeGuid);
            return result;
        }

        /// <summary>
        ///     Delete MembershipProductType.
        /// </summary>
        /// <param name="productTypeId">MembershipProductType unique identifier</param>
        /// <returns>Returns true if operation completed successfully</returns>
        public bool Delete(string productTypeId)
        {
            var productTypeGuid = DataTypeHelper.BvinToGuid(productTypeId);
            var result = base.Delete(y => y.ProductTypeBvin == productTypeGuid);
            result &= productTypesRepository.Delete(productTypeId);
            return result;
        }
    }
}