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
using Hotcakes.Commerce.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    [TestClass]
    public class CatalogRolesTest : BaseTest
    {
        [TestMethod]
        public void CatalogRoles_CanAddRole()
        {
            var c = new HccRequestContext();
            c.CurrentStore.Id = 2;
            var target = new CatalogRolesRepository(c);

            var expected = new CatalogRole
            {
                RoleName = "NewRole",
                RoleType = CatalogRoleType.ProductRole,
                ReferenceId = new Guid("9113e66b-43fb-4eec-b9d1-84cb9750bccc")
            };

            var res = target.Create(expected);
            var actual = target.FindAllPaged(1, 1).First();

            Assert.AreEqual(1, target.CountOfAll());
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.ReferenceId, actual.ReferenceId);
            Assert.AreEqual(expected.RoleType, actual.RoleType);
        }
    }
}