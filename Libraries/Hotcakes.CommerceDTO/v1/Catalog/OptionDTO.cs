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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product options in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is Option.</remarks>
    [DataContract]
    [Serializable]
    public class OptionDTO
    {
        public OptionDTO()
        {
            Bvin = string.Empty;
            StoreId = 0;
            OptionType = OptionTypesDTO.Uknown;
            Name = string.Empty;
            NameIsHidden = false;
            IsVariant = false;
            IsShared = false;
            Settings = new List<OptionSettingDTO>();
            TextSettings = new List<OptionSettingDTO>();
            Items = new List<OptionItemDTO>();
        }

        /// <summary>
        ///     The primary key for the option
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     Returns a specific value to define what kind of option this is. This property is accessible through the Processor
        ///     property as well.
        /// </summary>
        [DataMember]
        public OptionTypesDTO OptionType { get; set; }

        /// <summary>
        ///     This is the name of the option as you want it to appear to customers. This is a localizable property.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     If a merchant sets this to true, the viewset should observe the setting and not chose the label.
        /// </summary>
        [DataMember]
        public bool NameIsHidden { get; set; }

        /// <summary>
        ///     If enabled, this property defines that the option can be used as a variants to create a new SKU, price options, and
        ///     more.
        /// </summary>
        [DataMember]
        public bool IsVariant { get; set; }

        /// <summary>
        ///     If true, this property will allow this choice to be used across multiple products.
        /// </summary>
        [DataMember]
        public bool IsShared { get; set; }

        /// <summary>
        ///     Used with the HTML OptionType when this is a Shared choice to save the default option value. All other OptionTypes
        ///     will ignore this value.
        /// </summary>
        [DataMember]
        public List<OptionSettingDTO> Settings { get; set; }

        /// <summary>
        ///     Used with the HTML OptionType when this is not a Shared choice to save the default option value. All other
        ///     OptionTypes will ignore this value.
        /// </summary>
        public List<OptionSettingDTO> TextSettings { get; set; }

        /// <summary>
        ///     Contains a list of items that belong to this option, such as individual sizes when this is a garment size option.
        /// </summary>
        [DataMember]
        public List<OptionItemDTO> Items { get; set; }
    }
}