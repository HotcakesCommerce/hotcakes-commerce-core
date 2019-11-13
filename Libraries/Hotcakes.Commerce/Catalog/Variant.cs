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
    ///     This class is used to manage product variants throughout the application.
    /// </summary>
    [Serializable]
    public class Variant
    {
        public Variant()
        {
            StoreId = 0;
            Bvin = string.Empty;
            ProductId = string.Empty;
            Sku = string.Empty;
            Price = -1;
            Selections = new OptionSelectionList();
        }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID or primary key of the variant.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     The Bvin or unique ID of the product that this variant belongs to.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        ///     When populated, the unique SKU that this variant should be using for inventory &amp; other tracking purposes.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        ///     The price that the product should use when this variant is chosen. Otherwise, 0.00 should be used.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     The Custom Property to add further variant details
        /// </summary>
        public string CustomProperty { get; set; }

        /// <summary>
        ///     These are the options that define the variant.
        /// </summary>
        public OptionSelectionList Selections { get; private set; }

        /// <summary>
        ///     This method returns a unique ID that is comprised of the Bvins, separated by a pipe character.
        /// </summary>
        /// <returns>String - a unique ID using the Bvins of each selection</returns>
        public string UniqueKey()
        {
            return OptionSelection.GenerateUniqueKeyForSelections(Selections);
        }

        /// <summary>
        ///     Allows you to get a list of the names of each option that makes up the variant. Used for displaying the variant in
        ///     administration.
        /// </summary>
        /// <param name="options">List of Option - the options to parse for the returned list</param>
        /// <returns>List of String - a list of all option names in the variant</returns>
        public List<string> SelectionNames(List<Option> options)
        {
            var result = new List<string>();

            foreach (var opt in options)
            {
                var sel = Selections.FindByOptionId(opt.Bvin);
                if (sel != null)
                {
                    var itemBvin = sel.SelectionData;
                    foreach (var oi in opt.Items)
                    {
                        var cleaned = OptionSelection.CleanBvin(oi.Bvin);
                        if (cleaned == itemBvin)
                        {
                            result.Add(oi.Name);
                            break;
                        }
                    }
                }
            }

            return result;
        }


        public VariantDTO ToDto()
        {
            var selDto = Selections.Select(s => s.ToDto()).ToList();

            return new VariantDTO
            {
                Bvin = Bvin,
                ProductId = ProductId,
                Sku = Sku,
                Price = Price,
                Selections = selDto
            };
        }
    }
}