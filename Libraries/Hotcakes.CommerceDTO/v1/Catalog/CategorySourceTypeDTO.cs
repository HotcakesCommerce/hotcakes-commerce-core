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

using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     Specifies how a category obtains its content or destination.
    /// </summary>
    [DataContract]
    public enum CategorySourceTypeDTO
    {
        /// <summary>
        ///     The category is managed manually.
        /// </summary>
        [EnumMember] Manual = 0,
        /// <summary>
        ///     The category contents are determined by rules.
        /// </summary>
        [EnumMember] ByRules = 1,
        /// <summary>
        ///     The category points to a custom link.
        /// </summary>
        [EnumMember] CustomLink = 2,
        /// <summary>
        ///     The category points to a custom page.
        /// </summary>
        [EnumMember] CustomPage = 3,
        /// <summary>
        ///     The category provides a drill-down destination.
        /// </summary>
        [EnumMember] DrillDown = 4,
        /// <summary>
        ///     The category points to a flexible page.
        /// </summary>
        [EnumMember] FlexPage = 5
    }
}
