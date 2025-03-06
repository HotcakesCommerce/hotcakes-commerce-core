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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This enumeration defines the possible types that can be used with an Option.
    /// </summary>
    [DataContract]
    [Serializable]
    public enum OptionTypesDTO
    {
        /// <summary>
        ///     This is the default value, but should never be used. If used, it will essentially be ignored in the system.
        /// </summary>
        [EnumMember] Uknown = 0,

        /// <summary>
        ///     When specified, a drop down list is used to render the respective OptionItems. All HTML rules of a drop down list
        ///     are persisted.
        /// </summary>
        [EnumMember] DropDownList = 100,

        /// <summary>
        ///     When specified, a radio button list is used to render the respective OptionItems. All HTML rules of a radio button
        ///     list are persisted.
        /// </summary>
        [EnumMember] RadioButtonList = 200,

        /// <summary>
        ///     When specified, a check box list is used to render the respective OptionItems. All HTML rules of a check box list
        ///     are persisted.
        /// </summary>
        [EnumMember] CheckBoxes = 300,

        /// <summary>
        ///     When specified, the rendered value will use rich text or HTML. No options are displayed since there is only a
        ///     single item (the value).
        /// </summary>
        [EnumMember] Html = 400,

        /// <summary>
        ///     When specified, the rendered value will basically be a label. No options are displayed since there is only a single
        ///     item (the value).
        /// </summary>
        [EnumMember] TextInput = 500,

        /// <summary>
        ///     When specified, the rendered output will display the ability to upload files to the customer. No options are
        ///     displayed since the ability to upload is the option.
        /// </summary>
        [EnumMember] FileUpload = 600
    }
}