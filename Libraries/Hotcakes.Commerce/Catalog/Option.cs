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
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog.Options;
using Hotcakes.CommerceDTO.v1.Catalog;
using DropDownList = Hotcakes.Commerce.Catalog.Options.DropDownList;
using FileUpload = Hotcakes.Commerce.Catalog.Options.FileUpload;
using RadioButtonList = Hotcakes.Commerce.Catalog.Options.RadioButtonList;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product options in the main application
    /// </summary>
    /// <remarks>The REST API equivalent is OptionDTO.</remarks>
    [Serializable]
    public class Option
    {
        public Option()
        {
            Bvin = string.Empty;
            StoreId = 0;
            Name = string.Empty;
            NameIsHidden = false;
            IsVariant = false;
            IsShared = false;
            Settings = new OptionSettings();
            TextSettings = new OptionSettings();
            Items = new List<OptionItem>();
            Processor = new DropDownList();
        }

        /// <summary>
        ///     This property helps the rest of the application know how to display and format selections of the current option.
        /// </summary>
        public IOptionProcessor Processor { get; set; }

        /// <summary>
        ///     The primary key for the option
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     Returns a specific value to define what kind of option this is. This property is accessible through the Processor
        ///     property as well.
        /// </summary>
        public virtual OptionTypes OptionType
        {
            get { return Processor.GetOptionType(); }
        }

        /// <summary>
        ///     This is the name of the option as you want it to appear to customers. This is a localizable property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     If a merchant sets this to true, the viewset should observe the setting and not chose the label.
        /// </summary>
        public bool NameIsHidden { get; set; }

        /// <summary>
        ///     If enabled, this property defines that the option can be used as a variants to create a new SKU, price options, and
        ///     more.
        /// </summary>
        public bool IsVariant { get; set; }

        /// <summary>
        ///     If true, this property will allow this choice to be used across multiple products.
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        ///     Returns true if this option is supposed to display a swatch on the product view
        /// </summary>
        public bool IsColorSwatch { get; set; }

        /// <summary>
        ///     Used with the HTML OptionType when this is a Shared choice to save the default option value. All other OptionTypes
        ///     will ignore this value.
        /// </summary>
        public OptionSettings Settings { get; set; }

        /// <summary>
        ///     Used with the HTML OptionType when this is not a Shared choice to save the default option value. All other
        ///     OptionTypes will ignore this value.
        /// </summary>
        public OptionSettings TextSettings { get; set; }

        /// <summary>
        ///     Contains a list of items that belong to this option, such as individual sizes when this is a garment size option.
        /// </summary>
        public List<OptionItem> Items { get; set; }

        public bool IsRequired
        {
            get { return Settings.GetBoolSetting("required"); }
            set { Settings.SetBoolSetting("required", value); }
        }

        /// <summary>
        ///     Use this method to define which processor the current Option should be using.
        /// </summary>
        /// <param name="type">OptionType - the type of option the current object should be defined as.</param>
        public void SetProcessor(OptionTypes type)
        {
            switch (type)
            {
                case OptionTypes.CheckBoxes:
                    Processor = new CheckBoxes();
                    break;
                case OptionTypes.DropDownList:
                    Processor = new DropDownList();
                    break;
                case OptionTypes.FileUpload:
                    Processor = new FileUpload();
                    break;
                case OptionTypes.Html:
                    Processor = new Html();
                    break;
                case OptionTypes.RadioButtonList:
                    Processor = new RadioButtonList();
                    break;
                case OptionTypes.TextInput:
                    Processor = new TextInput();
                    break;
            }
        }

        /// <summary>
        ///     This is a shared method to instantiate a new Option, set it's processor and then return the Option back to the
        ///     calling code.
        /// </summary>
        /// <param name="type">OptionType - the type of option the new Option object should be defined as.</param>
        /// <returns>A new instance of Option with defaults set, except for the OptionType.</returns>
        public static Option Factory(OptionTypes type)
        {
            var result = new Option();
            result.SetProcessor(type);
            return result;
        }

        /// <summary>
        ///     Outputs the HTML required to display this Option in the web page. This method uses the Processor property.
        /// </summary>
        /// <returns>An HTNL-friendly string representation of the Option, based upon its respective OptionType.</returns>
        public string Render()
        {
            return Processor.Render(this);
        }

        /// <summary>
        ///     Outputs the HTML required to display this Option in the web page, with the default item selected, when appropriate.
        ///     This method uses the Processor property.
        /// </summary>
        /// <param name="selections">A listing of the default selections that should be made in the output HTML.</param>
        /// <param name="prefix">
        ///     (optional) When specified, this value will be included as a prefix to the Name attribute of the
        ///     rendered items.
        /// </param>
        /// <returns>
        ///     An HTML-friendly string representation of the Option, based upon its respective OptionType, with default
        ///     selections made.
        /// </returns>
        public string RenderWithSelection(OptionSelectionList selections, string prefix = null)
        {
            return Processor.RenderWithSelection(this, selections, prefix);
        }

        /// <summary>
        ///     Creates the respective HtmlControl object of this Option, based upon the specified OptionType.
        /// </summary>
        /// <param name="ph">An instance of the Placeholder control to add the resulting object to.</param>
        /// <param name="prefix">
        ///     (optional) When specified, this value will be included as a prefix to the Id and Name attributes
        ///     of the rendered items.
        /// </param>
        public void RenderAsControl(PlaceHolder ph, string prefix = null)
        {
            Processor.RenderAsControl(this, ph, prefix);
        }

        /// <summary>
        ///     Looks for the respective Option in the supplied Placeholder control, and returns a list of IDs for the selected
        ///     items in the Option.
        /// </summary>
        /// <param name="ph">The Placeholder object to look through for this Option and it's items.</param>
        /// <param name="prefix">
        ///     (optional) When supplied, this value is used to help identify the Option control in the
        ///     placeholder.
        /// </param>
        /// <returns>A listing of the items selected in the Option.</returns>
        public OptionSelection ParseFromPlaceholder(PlaceHolder ph, string prefix = null)
        {
            return Processor.ParseFromPlaceholder(this, ph, prefix);
        }

        /// <summary>
        ///     Allows you to access all of the selected values for Options of the specified form in a single method.
        /// </summary>
        /// <param name="form">An instance of the form from the Request object.</param>
        /// <param name="prefix">
        ///     (optional) When supplied, this value is used to help identify the correct form to pull the values
        ///     from.
        /// </param>
        /// <returns>All of the selected values of the specified form are returned.</returns>
        public OptionSelection ParseFromForm(NameValueCollection form, string prefix = null)
        {
            return Processor.ParseFromForm(this, form, prefix);
        }

        /// <summary>
        ///     Allows you to return a value that contains the Option and a comma delimited list of its items for identifying in
        ///     the description of the Cart/Order. This is primarily called when a product is being converted to a line item for a
        ///     cart/order.
        /// </summary>
        /// <param name="selections">A parsed collection of the selections made on this Option.</param>
        /// <returns>A comma delimited string of the Option items preceded by the option name.</returns>
        public string CartDescription(OptionSelectionList selections)
        {
            return Processor.CartDescription(this, selections);
        }

        /// <summary>
        ///     Allows you to add a new OptionItem to the current Option.
        /// </summary>
        /// <param name="itemName">The name that you want the new OptionItem to have. This is a localizable value.</param>
        /// <returns>If true, the OptionItem was added successfully.</returns>
        public bool AddItem(string itemName)
        {
            var maxSortOrder = Items.Max(i => (int?) i.SortOrder) ?? 0;

            var oi = new OptionItem();
            oi.Name = itemName;
            oi.SortOrder = maxSortOrder + 1;
            return AddItem(oi);
        }

        /// <summary>
        ///     Allows you to add a new OptionItem to the current Option.
        /// </summary>
        /// <param name="item">A pre-populated instance of an OptionItem.</param>
        /// <returns>If true, the OptionItem was added successfully.</returns>
        public bool AddItem(OptionItem item)
        {
            if (item == null) return false;
            item.OptionBvin = Bvin;
            Items.Add(item);
            return true;
        }

        /// <summary>
        ///     When called, this method will return true if the supplied BVIN matches the BVIN of an OptionItem in the Items
        ///     property.
        /// </summary>
        /// <param name="itemBvin">This is the unique ID of the OptionItem you are looking for.</param>
        /// <param name="includeLabels">if set to <c>true</c> [include labels].</param>
        /// <returns>
        ///     If true, the OptionItem is in this Option.
        /// </returns>
        public bool ItemsContains(string itemBvin, bool includeLabels = false)
        {
            // check to see if this option contains a specific item
            foreach (var oi in Items)
            {
                if (!oi.IsLabel || includeLabels)
                {
                    if (oi.Bvin.Replace("-", "") == itemBvin.Replace("-", "")) return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Use this method to create a copy of the current option.
        /// </summary>
        /// <returns>A duplicate of the current Option.</returns>
        public Option Clone()
        {
            var result = Factory(OptionType);

            result.Bvin = string.Empty;
            result.IsShared = IsShared;
            result.IsVariant = IsVariant;
            foreach (var oi in Items)
            {
                result.Items.Add(oi.Clone());
            }
            result.Name = Name;
            result.NameIsHidden = NameIsHidden;
            foreach (var set in Settings)
            {
                result.Settings.AddOrUpdate(set.Key, set.Value);
            }
            foreach (var textSet in TextSettings)
            {
                result.TextSettings.AddOrUpdate(textSet.Key, textSet.Value);
            }
            result.StoreId = StoreId;

            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product option object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OptionDTO</returns>
        public OptionDTO ToDto()
        {
            var dto = new OptionDTO();

            dto.Bvin = Bvin;
            dto.IsShared = IsShared;
            dto.IsVariant = IsVariant;
            dto.Items = new List<OptionItemDTO>();
            foreach (var oi in Items)
            {
                dto.Items.Add(oi.ToDto());
            }
            dto.Name = Name;
            dto.NameIsHidden = NameIsHidden;
            dto.OptionType = (OptionTypesDTO) (int) OptionType;
            dto.Settings = new List<OptionSettingDTO>();
            foreach (var set in Settings)
            {
                var setdto = new OptionSettingDTO();
                setdto.Key = set.Key;
                setdto.Value = set.Value;
                dto.Settings.Add(setdto);
            }
            dto.TextSettings = new List<OptionSettingDTO>();
            foreach (var txtSet in TextSettings)
            {
                var textSetDto = new OptionSettingDTO();
                textSetDto.Key = txtSet.Key;
                textSetDto.Value = txtSet.Value;
                dto.TextSettings.Add(textSetDto);
            }
            dto.StoreId = StoreId;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product option object using an instance of OptionDTO
        /// </summary>
        /// <param name="dto">An instance of the product option from the REST API</param>
        public void FromDto(OptionDTO dto)
        {
            if (dto == null) return;

            Bvin = dto.Bvin ?? string.Empty;
            IsShared = dto.IsShared;
            IsVariant = dto.IsVariant;
            Items.Clear();
            Name = dto.Name ?? string.Empty;
            NameIsHidden = dto.NameIsHidden;
            StoreId = dto.StoreId;

            var typeCode = OptionTypes.DropDownList;
            typeCode = (OptionTypes) dto.OptionType;
            SetProcessor(typeCode);

            foreach (var oi in dto.Items)
            {
                var opt = new OptionItem();
                opt.FromDto(oi);
                AddItem(opt);
            }
            Settings = new OptionSettings();
            foreach (var set in dto.Settings)
            {
                Settings.AddOrUpdate(set.Key, set.Value);
            }
            TextSettings = new OptionSettings();
            foreach (var txtSet in dto.TextSettings)
            {
                TextSettings.AddOrUpdate(txtSet.Key, txtSet.Value);
            }
        }

        #endregion
    }
}