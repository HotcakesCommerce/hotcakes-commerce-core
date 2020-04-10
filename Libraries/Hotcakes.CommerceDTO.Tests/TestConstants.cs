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

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     These values/objects must be present in testable site DB
    /// </summary>
    public class TestConstants
    {
        /// <summary>
        ///     DB must contains user:
        ///     - Username: hcctest1
        ///     - Firstname: Test
        ///     - Lastname: HccAccount1
        ///     - Email: hcctest1@test.com
        ///     - Id: 721
        /// </summary>
        public const string TestAccount1Id = "721";

        public const string TestAccount1Email = "hcctest1@test.com";

        public const string TestCategoryName = "HCC-TEST";
        public const string TestCategorySlug = "hcc-test";

        public const string TestProduct1Sku = "HCC-001";
        public const string TestProduct1Slug = "Hcc-ProductVar1";
        public const string TestProductBvin = "fb7f34cc-5434-4631-9144-e8cd167245bb";

        public const string TestOrder1Bvin = "9eb1446b-a004-4e80-9095-75bb1ca441c5";
        public const string TestOrder1Number = "1";
        public const decimal TestOrder1TotalGrand = 5242.99m;
    }
}