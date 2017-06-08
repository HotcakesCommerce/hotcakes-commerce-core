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
using System.Linq;

namespace Hotcakes.Commerce.Dnn.Data
{
    [Serializable]
    public partial class DnnUser
    {
        public List<DnnProfile> Profile { get; set; }

        public string this[string propertyName]
        {
            get
            {
                var property = Profile.FirstOrDefault(p => p.PropertyName == propertyName);
                return property != null ? property.PropertyValue : null;
            }
        }

        public string ProfileCountry
        {
            get { return this["Country"]; }
        }

        public string ProfileCompany
        {
            get { return this["Company"]; }
        }

        public string ProfileStreet
        {
            get { return this["Street"]; }
        }

        public string ProfileCity
        {
            get { return this["City"]; }
        }

        public string ProfileRegion
        {
            get { return this["Region"]; }
        }

        public string ProfilePostalCode
        {
            get { return this["PostalCode"]; }
        }

        public string ProfileTelephone
        {
            get { return this["Telephone"]; }
        }
    }
}