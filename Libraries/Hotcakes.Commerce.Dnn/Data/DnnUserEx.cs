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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Commerce.Dnn.Data
{
    /// <summary>
    /// Adds convenient accessors for DNN user profile data to the generated user entity.
    /// </summary>
    [Serializable]
    public partial class DnnUser
    {
        /// <summary>
        /// Gets or sets the profile properties associated with the user.
        /// </summary>
        public List<DnnProfile> Profile { get; set; }

        /// <summary>
        /// Gets the profile value for a property name.
        /// </summary>
        /// <param name="propertyName">The DNN profile property name.</param>
        /// <returns>The matching profile value, or <see langword="null"/> when no matching property exists.</returns>
        public string this[string propertyName]
        {
            get
            {
                var property = Profile.FirstOrDefault(p => p.PropertyName == propertyName);
                return property != null ? property.PropertyValue : null;
            }
        }

        /// <summary>
        /// Gets the user's country profile value.
        /// </summary>
        public string ProfileCountry
        {
            get { return this["Country"]; }
        }

        /// <summary>
        /// Gets the user's company profile value.
        /// </summary>
        public string ProfileCompany
        {
            get { return this["Company"]; }
        }

        /// <summary>
        /// Gets the user's street profile value.
        /// </summary>
        public string ProfileStreet
        {
            get { return this["Street"]; }
        }

        /// <summary>
        /// Gets the user's city profile value.
        /// </summary>
        public string ProfileCity
        {
            get { return this["City"]; }
        }

        /// <summary>
        /// Gets the user's region profile value.
        /// </summary>
        public string ProfileRegion
        {
            get { return this["Region"]; }
        }

        /// <summary>
        /// Gets the user's postal-code profile value.
        /// </summary>
        public string ProfilePostalCode
        {
            get { return this["PostalCode"]; }
        }

        /// <summary>
        /// Gets the user's telephone profile value.
        /// </summary>
        public string ProfileTelephone
        {
            get { return this["Telephone"]; }
        }
    }
}
