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
    ///     This is the primary class used for option selection in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is OptionSelectionDTO.</remarks>
    [Serializable]
    public class OptionSelection
    {
        private string _OptionBvin = string.Empty;
        private string _SelectionData = string.Empty;

        public OptionSelection()
        {
        }

        public OptionSelection(string optionBvin, string selectionData)
        {
            _OptionBvin = CleanBvin(optionBvin);
            _SelectionData = CleanBvin(selectionData);
        }

        /// <summary>
        ///     The unique ID or primary key of the option.
        /// </summary>
        public string OptionBvin
        {
            get { return _OptionBvin; }
            set { _OptionBvin = CleanBvin(value); }
        }

        /// <summary>
        ///     The options or choices selected for the product.
        /// </summary>
        public string SelectionData
        {
            get { return _SelectionData; }
            set 
			{
				Guid bvin;
				if (Guid.TryParse(value, out bvin))
				{
					_SelectionData = CleanBvin(value);
				}
				else
				{
					_SelectionData = value; 
				}
			}
        }

        /// <summary>
        ///     Removes the hyphens from the Bvin (GUID).
        /// </summary>
        /// <param name="input">String - the original GUID or Bvin</param>
        /// <returns>String - te GUID without the hypens</returns>
        public static string CleanBvin(string input)
        {
            return input.Replace("-", string.Empty);
        }

        /// <summary>
        ///     Allows you to generate a unique key for the OptionSelection using the given choices.
        /// </summary>
        /// <param name="selections">The chosen selections for the product.</param>
        /// <returns>String - a unique key generated from the given selections</returns>
        public static string GenerateUniqueKeyForSelections(List<OptionSelection> selections)
        {
            var result = string.Empty;

            if (selections == null) return result;
            if (selections.Count < 1) return result;

            var sorted = selections.OrderBy(y => y.OptionBvin);
            foreach (var s in sorted)
            {
                result += s.OptionBvin + "-" + s.SelectionData + "|";
            }

            return result;
        }

        /// <summary>
        ///     Checks to see if a list of selection data contains a selection that isn't a valid variant in a list of options.
        /// </summary>
        /// <param name="options">The available options for the product.</param>
        /// <param name="selections">The options that have been selected.</param>
        /// <returns>If true, the selected options all match that available choices.</returns>
        public static bool ContainsInvalidSelectionForOptions(OptionList options, List<OptionSelection> selections)
        {
            var result = false;

            foreach (var sel in selections)
            {
                if (!options.ContainsVariantSelection(sel))
                {
                    return true;
                }
            }

            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current open selection object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OptionSelectionDTO</returns>
        public OptionSelectionDTO ToDto()
        {
            var dto = new OptionSelectionDTO();

            dto.OptionBvin = OptionBvin;
            dto.SelectionData = SelectionData;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current option selection object using a OptionSelectionDTO instance
        /// </summary>
        /// <param name="dto">An instance of the option selection from the REST API</param>
        public void FromDto(OptionSelectionDTO dto)
        {
            if (dto == null) return;

            OptionBvin = dto.OptionBvin ?? string.Empty;
            SelectionData = dto.SelectionData ?? string.Empty;
        }

        #endregion
    }
}
