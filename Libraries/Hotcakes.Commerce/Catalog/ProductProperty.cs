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
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of ProductProperties in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is ProductPropertyDTO.</remarks>
    [Serializable]
    public class ProductProperty
    {
        public ProductProperty()
        {
            Id = 0;
            StoreId = 0;
            PropertyName = string.Empty;
            DisplayName = string.Empty;
            DisplayOnSite = true;
            DisplayToDropShipper = false;
            TypeCode = ProductPropertyType.TextField;
            DefaultValue = string.Empty;
            CultureCode = "en-US";
            Choices = new List<ProductPropertyChoice>();
            LastUpdatedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     The unique ID or identifier/primary key of the product property.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The globally used system name for the product property.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        ///     The name used on localized sites to recognize the product property in the native or chosen language.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     A setting used to determine whether or not this product property is displayed on the site to customers.
        /// </summary>
        public bool DisplayOnSite { get; set; }

        /// <summary>
        ///     A setting used to determine whether or not this product property is displayed to drop shippers.
        /// </summary>
        public bool DisplayToDropShipper { get; set; }

        /// <summary>
        ///     A setting used to determine whether or not this product property is displayed on the search results facets.
        /// </summary>
        public bool DisplayOnSearch { get; set; }

        /// <summary>
        ///     The type of property that this product property is, which helps determine how it's displayed to merchants and
        ///     customers.
        /// </summary>
        public ProductPropertyType TypeCode { get; set; }

        /// <summary>
        ///     This is the default value that will first be used in products, until the product has been updated to have another
        ///     value.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        ///     If the site is multi-lingual, this value will be used instead of the DefaultValue property.
        /// </summary>
        public string DefaultLocalizableValue { get; set; }

        /// <summary>
        ///     This property reflects the language that this product property has been saved for.
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product property was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     This property is used to determine whether or not the currenty property supports localization.
        /// </summary>
        /// <remarks>When true, the DefaultLocalizableValue will be used over the DefaultValue property.</remarks>
        public bool IsLocalizable { get; set; }

        /// <summary>
        ///     A collection of choices that belong to this product property.
        /// </summary>
        /// <remarks>This property only applies when the TypeCode property is MultipleChoiceField (2).</remarks>
        public List<ProductPropertyChoice> Choices { get; set; }

        /// <summary>
        ///     A human-readable version of the TypeCode property for displaying to merchants.
        /// </summary>
        /// <remarks>This property is not currently used anywere in the application.</remarks>
        public string FriendlyTypeName
        {
            get
            {
                var result = "Unknown";
                switch (TypeCode)
                {
                    case ProductPropertyType.CurrencyField:
                        result = "Currency";
                        break;
                    case ProductPropertyType.DateField:
                        result = "Date";
                        break;
                    case ProductPropertyType.MultipleChoiceField:
                        result = "Multiple Choice";
                        break;
                    case ProductPropertyType.TextField:
                        result = "Text Block";
                        break;
                    case ProductPropertyType.HyperLink:
                        result = "Hyperlink";
                        break;
                    default:
                        result = "Unknown";
                        break;
                }
                return result;
            }
        }

        /// <summary>
        ///     A human-readable version of the TypeCode property for displaying to merchants.
        /// </summary>
        /// <remarks>This property is not currently used anywere in the application.</remarks>
        public string TypeCodeDisplayName
        {
            get
            {
                var result = "Unknown";
                switch (TypeCode)
                {
                    case ProductPropertyType.CurrencyField:
                        result = "Currency Field";
                        break;
                    case ProductPropertyType.DateField:
                        result = "Date Field";
                        break;
                    case ProductPropertyType.HyperLink:
                        result = "Hyperlink";
                        break;
                    case ProductPropertyType.MultipleChoiceField:
                        result = "Multiple Choice";
                        break;
                    case ProductPropertyType.None:
                        result = "Unknown";
                        break;
                    case ProductPropertyType.TextField:
                        result = "Text Field";
                        break;
                    default:
                        result = "Unknown";
                        break;
                }
                return result;
            }
        }

        /// <summary>
        ///     This method returns the value for the given ChoiceName in the parameter. The choice must exist in the Choices
        ///     property to return the expected value.
        /// </summary>
        /// <param name="choiceName">The name of the choice to find a value for.</param>
        /// <returns>If the choice exists, the respective value will be returned. Otherwise, an empty string will be returned.</returns>
        public string GetChoiceValue(string choiceName)
        {
            var choice = Choices.FirstOrDefault(c => c.ChoiceName == choiceName);
            return choice != null ? choice.Id.ToString() : string.Empty;
        }

        /// <summary>
        ///     This method returns the name for the given ChoiceValue in the parameter. The choice must exist in the Choices
        ///     property to return the expected name.
        /// </summary>
        /// <param name="choiceVal">The value of the choice to find the name of.</param>
        /// <returns>If the choice exists, the respective name will be returned. Otherwise, an empty string will be returned.</returns>
        public string GetChoiceName(long choiceVal)
        {
            var choice = Choices.FirstOrDefault(c => c.Id == choiceVal);
            return choice != null ? choice.ChoiceName : string.Empty;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product property object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductPropertyDTO</returns>
        public ProductPropertyDTO ToDto()
        {
            var dto = new ProductPropertyDTO();

            foreach (var c in Choices)
            {
                dto.Choices.Add(c.ToDto());
            }
            dto.CultureCode = CultureCode;
            dto.DefaultValue = DefaultValue;
            dto.DisplayName = DisplayName;
            dto.DisplayOnSite = DisplayOnSite;
            dto.DisplayToDropShipper = DisplayToDropShipper;
            dto.Id = Id;
            dto.PropertyName = PropertyName;
            dto.StoreId = StoreId;
            dto.TypeCode = (ProductPropertyTypeDTO) (int) TypeCode;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product property object using a ProductPropertyDTO instance
        /// </summary>
        /// <param name="dto">An instance of the ProductProperty from the REST API</param>
        public void FromDto(ProductPropertyDTO dto)
        {
            if (dto == null) return;

            Choices.Clear();
            if (dto.Choices != null)
            {
                foreach (var c in dto.Choices)
                {
                    var pc = new ProductPropertyChoice();
                    pc.FromDto(c);
                    Choices.Add(pc);
                }
            }
            CultureCode = dto.CultureCode;
            DefaultValue = dto.DefaultValue;
            DisplayName = dto.DisplayName;
            DisplayOnSite = dto.DisplayOnSite;
            DisplayToDropShipper = dto.DisplayToDropShipper;
            Id = dto.Id;
            PropertyName = dto.PropertyName;
            StoreId = dto.StoreId;
            TypeCode = (ProductPropertyType) (int) dto.TypeCode;
            LastUpdatedUtc = dto.LastUpdatedUtc;
        }

        #endregion
    }
}