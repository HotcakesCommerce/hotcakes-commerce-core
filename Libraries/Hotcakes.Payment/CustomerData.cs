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
using Hotcakes.Web.Geography;

namespace Hotcakes.Payment
{
    [Serializable]
    public class CustomerData
    {
        public CustomerData()
        {
            UserId = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Company = string.Empty;
            Street = string.Empty;
            City = string.Empty;
            RegionName = string.Empty;
            RegionBvin = string.Empty;
            PostalCode = string.Empty;
            CountryName = string.Empty;
            CountryBvin = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            IpAddress = "0.0.0.0";

            ShipFirstName = string.Empty;
            ShipLastName = string.Empty;
            ShipCompany = string.Empty;
            ShipStreet = string.Empty;
            ShipCity = string.Empty;
            ShipRegionName = string.Empty;
            ShipRegionBvin = string.Empty;
            ShipPostalCode = string.Empty;
            ShipCountryName = string.Empty;
            ShipCountryBvin = string.Empty;
            ShipPhone = string.Empty;
        }

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string RegionName { get; set; }
        public string RegionBvin { get; set; }
        public string PostalCode { get; set; }
        public string CountryName { get; set; }
        public string CountryBvin { get; set; }
        public ICountry CountryData { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IpAddress { get; set; }

        public string ShipFirstName { get; set; }
        public string ShipLastName { get; set; }
        public string ShipCompany { get; set; }
        public string ShipStreet { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegionName { get; set; }
        public string ShipRegionBvin { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountryName { get; set; }
        public string ShipCountryBvin { get; set; }
        public ICountry ShipCountryData { get; set; }
        public string ShipPhone { get; set; }
    }
}