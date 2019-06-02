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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Web.Validation;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     You'll be using the CheckoutAddressViewModel for anything related to an address at
    ///     the checkout, and beyond.  Another example might be when confirming an address
    ///     after using an offsite payment option.
    /// </summary>
    [Serializable]
    public class CheckoutAddressViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public CheckoutAddressViewModel()
        {
            Addresses = new List<Address>();
            Address = new Address();
            Prefix = string.Empty;
            TabIndex = 0;
            Countries = new List<Country>();
            ShowPhone = true;
            Violations = new List<RuleViolation>();
            ErrorCssClass = "input-validation-error";
        }

        /// <summary>
        ///     This shows already saved addresses for the currently logged in customer's
        ///     address book.
        /// </summary>
        public List<Address> Addresses { get; set; }

        /// <summary>
        ///     Indicates a specific selected address from the address drop down list or the
        ///     new address entered by the customer on the form.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        ///     Prefix (e.g., Shipping, Billing) used to create the id of the all controls of the form.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        ///     Tab index related to each control. This will be used as starting index then all child controls will have the
        ///     incremental value for the index.
        /// </summary>
        public int TabIndex { get; set; }

        /// <summary>
        ///     List of enabled countries shown to the customer.
        /// </summary>
        public List<Country> Countries { get; set; }

        /// <summary>
        ///     If true, indicates that the form needs to show the telephone input control on the view.
        /// </summary>
        public bool ShowPhone { get; set; }

        /// <summary>
        ///     Contains list of the different validation errors on the form.
        /// </summary>
        public List<RuleViolation> Violations { get; set; }

        /// <summary>
        ///     CSS class name used to display form errors to the customer.
        /// </summary>
        public string ErrorCssClass { get; set; }

        /// <summary>
        ///     Check if error exist for specific control
        /// </summary>
        /// <param name="nameWithoutPrefix">Control id without prefix</param>
        /// <returns>Error string if any</returns>
        public string IsErr(string nameWithoutPrefix)
        {
            var result = string.Empty;

            if (Violations != null)
            {
                var v = Violations.FirstOrDefault(y => y.ControlName == Prefix + nameWithoutPrefix);
                if (v != null)
                {
                    return ErrorCssClass;
                }
            }
            return result;
        }
    }
}