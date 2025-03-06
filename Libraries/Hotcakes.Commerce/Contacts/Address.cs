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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Hotcakes.Commerce.Globalization;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.Web.Geography;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    ///     This is the main object that is used for all address management in the main application
    /// </summary>
    /// <remarks>The main REST API equivalent to this is the AddressDTO object</remarks>
    [Serializable]
    public class Address : IAddress
    {
        private readonly XmlWriterSettings _hccXmlWriterSettings = new XmlWriterSettings();

        private string _postalCode = string.Empty;

        public Address()
        {
            Init();
        }

        // removed since resharper says it is not being used
        //private XmlReaderSettings _hccXmlReaderSettings = new XmlReaderSettings();

        /// <summary>
        ///     This is the unique ID of the address.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the address was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     A user-defined value to name this address for easy identification in the user interface.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     The first name of the recipient at this address.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The middle iniital of the recipient at this address.
        /// </summary>
        public string MiddleInitial { get; set; }

        /// <summary>
        ///     The last name or surname of the recipient at this address.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     If this is not a residential address, a company name should be specified.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        ///     The first line of the street address, such as 123 Main Street.
        /// </summary>
        /// <remarks>This property is mapped to the Street property.</remarks>
        public string Line1 { get; set; }

        /// <summary>
        ///     The second line of the street address, such as Suite 100.
        /// </summary>
        /// <remarks>This property is mapped to the Street2 property.</remarks>
        public string Line2 { get; set; }

        /// <summary>
        ///     The third line of the street address. Usually used for non-US or military addresses.
        /// </summary>
        public string Line3 { get; set; }

        /// <summary>
        ///     If RegionData is not null, this property will contain the RegionData.SystemName. Otherwise, this will be an empty
        ///     string.
        /// </summary>
        public string RegionSystemName
        {
            get { return RegionData != null ? RegionData.SystemName : string.Empty; }
        }

        /// <summary>
        ///     If RegionData is not null, this property will contain the RegionData.DisplayName. Otherwise, this will be an empty
        ///     string.
        /// </summary>
        public string RegionDisplayName
        {
            get { return RegionData != null ? RegionData.DisplayName : string.Empty; }
        }

        /// <summary>
        ///     If CountryData is not null, this property will contain the CountryData.SystemName. Otherwise, this will be an empty
        ///     string.
        /// </summary>
        public string CountrySystemName
        {
            get { return CountryData != null ? CountryData.SystemName : string.Empty; }
        }

        /// <summary>
        ///     If CountryData is not null, this property will contain the CountryData.DisplayName. Otherwise, this will be an
        ///     empty string.
        /// </summary>
        public string CountryDisplayName
        {
            get { return CountryData != null ? CountryData.DisplayName : string.Empty; }
        }

        /// <summary>
        ///     A telephone number for the address.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     A fax or facsimile number for the address.
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        ///     A website URL for the address. Primarily used for vendors and manufacturers.
        /// </summary>
        public string WebSiteUrl { get; set; }

        /// <summary>
        ///     Addresses are mapped back to a CMS user record, even for vendors and manufacturers. This field is the user ID.
        /// </summary>
        public string UserBvin { get; set; }

        //public bool Residential {get;set;}

        /// <summary>
        ///     Allows you to specify if the address belongs to the merchant or not.
        /// </summary>
        public AddressTypes AddressType { get; set; }

        /// <summary>
        ///     The name of the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     This should contain a valid ID of the Region, if applicable.
        /// </summary>
        public string RegionBvin { get; set; }

        /// <summary>
        ///     Contains a localization-friendly mapping of the region information and it's respective country.
        /// </summary>
        public IRegion RegionData
        {
            get
            {
                try
                {
                    var countryRepo = Factory.CreateRepo<CountryRepository>();
                    if (countryRepo == null)
                        return null;

                    var country = countryRepo.Find(CountryBvin);
                    if (country == null || country.Regions == null)
                        return null;

                    return country.Regions.FirstOrDefault(r => r.Abbreviation == RegionBvin);
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        ///     Contains the zip or postal code of the address, as necessary.
        /// </summary>
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        /// <summary>
        ///     This is the unique ID or bvin of the country related to this address.
        /// </summary>
        public string CountryBvin { get; set; }

        /// <summary>
        ///     A populated instance of a localized country object matching this address.
        /// </summary>
        public ICountry CountryData
        {
            get
            {
                var countryRepo = Factory.CreateRepo<CountryRepository>();
                return countryRepo.Find(CountryBvin);
            }
        }

        private void Init()
        {
            StoreId = 0;
            NickName = string.Empty;
            FirstName = string.Empty;
            MiddleInitial = string.Empty;
            LastName = string.Empty;
            Company = string.Empty;
            Line1 = string.Empty;
            Line2 = string.Empty;
            Line3 = string.Empty;
            City = string.Empty;
            RegionBvin = string.Empty;
            PostalCode = string.Empty;
            //this.CountryBvin = Country.UnitedStatesCountryBvin;
            CountryBvin = string.Empty;
            Phone = string.Empty;
            Fax = string.Empty;
            WebSiteUrl = string.Empty;
            UserBvin = string.Empty;
            AddressType = AddressTypes.General;
            LastUpdatedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Allows you to populate this address from an XML data source.
        /// </summary>
        /// <param name="xr">A loaded instance of an XmlReader containing a serialized address object.</param>
        /// <returns>Returns true is the XML is successfully parsed.</returns>
        public bool FromXml(ref XmlReader xr)
        {
            var results = false;

            try
            {
                while (xr.Read())
                {
                    if (xr.IsStartElement())
                    {
                        if (!xr.IsEmptyElement)
                        {
                            switch (xr.Name)
                            {
                                case "Bvin":
                                    xr.Read();
                                    Bvin = xr.ReadString();
                                    break;
                                case "NickName":
                                    xr.Read();
                                    NickName = xr.ReadString();
                                    break;
                                case "FirstName":
                                    xr.Read();
                                    FirstName = xr.ReadString();
                                    break;
                                case "MiddleInitial":
                                    xr.Read();
                                    MiddleInitial = xr.ReadString();
                                    break;
                                case "LastName":
                                    xr.Read();
                                    LastName = xr.ReadString();
                                    break;
                                case "Company":
                                    xr.Read();
                                    Company = xr.ReadString();
                                    break;
                                case "Line1":
                                    xr.Read();
                                    Line1 = xr.ReadString();
                                    break;
                                case "Line2":
                                    xr.Read();
                                    Line2 = xr.ReadString();
                                    break;
                                case "Line3":
                                    xr.Read();
                                    Line3 = xr.ReadString();
                                    break;
                                case "City":
                                    xr.Read();
                                    City = xr.ReadString();
                                    break;
                                case "RegionBvin":
                                    xr.Read();
                                    RegionBvin = xr.ReadString();
                                    break;
                                case "PostalCode":
                                    xr.Read();
                                    _postalCode = xr.ReadString();
                                    break;
                                case "CountryBvin":
                                    xr.Read();
                                    CountryBvin = xr.ReadString();
                                    break;
                                case "Phone":
                                    xr.Read();
                                    Phone = xr.ReadString();
                                    break;
                                case "Fax":
                                    xr.Read();
                                    Fax = xr.ReadString();
                                    break;
                                case "WebSiteUrl":
                                    xr.Read();
                                    WebSiteUrl = xr.ReadString();
                                    break;
                                case "LastUpdated":
                                    xr.Read();
                                    LastUpdatedUtc = DateTime.Parse(xr.ReadString());
                                    break;
                                case "UserBvin":
                                    xr.Read();
                                    UserBvin = xr.ReadString();
                                    break;
                                //case "Residential":
                                //    xr.Read();
                                //    Residential = bool.Parse(xr.ReadString());
                                //    break;
                                case "AddressType":
                                    xr.Read();
                                    var tempType = int.Parse(xr.ReadString());
                                    AddressType = (AddressTypes) tempType;
                                    break;
                            }
                        }
                    }
                }

                results = true;
            }
            catch (XmlException ex)
            {
                EventLog.LogEvent(ex);
                results = false;
            }

            return results;
        }

        /// <summary>
        ///     Allows you to pass in an XmlWriter to emit the current address as XML.
        /// </summary>
        /// <param name="xw">A byreference instance of an XmlWriter.</param>
        public void ToXmlWriter(ref XmlWriter xw)
        {
            if (xw != null)
            {
                xw.WriteStartElement("Address");

                xw.WriteElementString("Bvin", Bvin);
                xw.WriteElementString("NickName", NickName);
                xw.WriteElementString("FirstName", FirstName);
                xw.WriteElementString("MiddleInitial", MiddleInitial);
                xw.WriteElementString("LastName", LastName);
                xw.WriteElementString("Company", Company);
                xw.WriteElementString("Line1", Line1);
                xw.WriteElementString("Line2", Line2);
                xw.WriteElementString("Line3", Line3);
                xw.WriteElementString("City", City);
                xw.WriteElementString("RegionName", RegionSystemName);
                xw.WriteElementString("RegionBvin", RegionBvin);
                xw.WriteElementString("PostalCode", _postalCode);
                xw.WriteElementString("CountryName", CountrySystemName);
                xw.WriteElementString("CountryBvin", CountryBvin);
                xw.WriteElementString("Phone", Phone);
                xw.WriteElementString("Fax", Fax);
                xw.WriteElementString("WebSiteUrl", WebSiteUrl);
                xw.WriteElementString("UserBvin", UserBvin);
                //xw.WriteStartElement("Residential");
                //xw.WriteValue(_Residential);
                //xw.WriteEndElement();
                xw.WriteStartElement("LastUpdated");
                xw.WriteValue(LastUpdatedUtc);
                xw.WriteEndElement();
                xw.WriteElementString("AddressType", ((int) AddressType).ToString());
                xw.WriteEndElement();
            }
        }

        /// <summary>
        ///     Allows you to generate an HTML representation of the current Address object.
        /// </summary>
        /// <returns>An HTML representation of the current Address object.</returns>
        public string ToHtmlString()
        {
            var sb = new StringBuilder();
            if (NickName.Trim().Length > 0)
            {
                sb.Append("<em>" + NickName + "</em><br />");
            }
            if (LastName.Length > 0 || FirstName.Length > 0)
            {
                sb.Append(FirstName);
                if (MiddleInitial.Trim().Length > 0)
                {
                    sb.Append(" " + MiddleInitial);
                }
                sb.Append(" " + LastName + "<br />");
                if (Company.Trim().Length > 0)
                {
                    sb.Append(Company + "<br />");
                }
            }

            sb.Append(GetLinesHtml());

            return sb.ToString();
        }

        /// <summary>
        ///     Returns a List-based representation of the address, including all of the common addres properties, as specified in
        ///     the parameters.
        /// </summary>
        /// <param name="appendNameAndCompany">It true, the full name and company name will be included in the list.</param>
        /// <param name="appendPhones">If true, the phone, fax, and website URL will be included in the list.</param>
        /// <returns>List - an array of the address information in the current object.</returns>
        public List<string> GetLines(bool appendNameAndCompany = true, bool appendPhones = true)
        {
            var lines = new List<string>();

            if (appendNameAndCompany)
            {
                lines.Add(
                    LastName +
                    (string.IsNullOrWhiteSpace(MiddleInitial) ? "" : " " + MiddleInitial.Trim()) +
                    (string.IsNullOrWhiteSpace(FirstName) ? "" : " " + FirstName.Trim())
                    );
                lines.Add(Company);
            }

            lines.Add(Line1);
            lines.Add(Line2);
            lines.Add(Line3);

            var cityLine = City;

            if (RegionData != null)
            {
                cityLine = string.Concat(cityLine, ", ", RegionData.Abbreviation, " ", _postalCode);
            }
            else
            {
                if (!string.IsNullOrEmpty(_postalCode))
                {
                    cityLine = string.Concat(cityLine, ", ", _postalCode);
                }
            }

            if (cityLine.Trim() != ",") lines.Add(cityLine);

            if (CountryData != null && !string.IsNullOrEmpty(CountryData.DisplayName))
            {
                lines.Add(CountryData.DisplayName);
            }
            if (appendPhones)
            {
                lines.Add(Phone);

                if (!string.IsNullOrWhiteSpace(Fax))
                {
                    lines.Add("Fax: " + Fax);
                }

                lines.Add(WebSiteUrl);
            }

            // Remove empty lines
            var i = 0;
            while (i < lines.Count)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    lines.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            return lines;
        }

        /// <summary>
        ///     Returns an HTML-based representation of the address, including all of the common addres properties, as specified in
        ///     the parameters.
        /// </summary>
        /// <param name="appendNameAndCompany">It true, the full name and company name will be included in the list.</param>
        /// <param name="appendPhones">If true, the phone, fax, and website URL will be included in the list.</param>
        /// <returns>String - The address information in the current object, with each line specified by a BR tag.</returns>
        public string GetLinesHtml(bool appendNameAndCompany = false, bool appendPhones = true)
        {
            var sb = new StringBuilder();

            foreach (var line in GetLines(appendNameAndCompany, appendPhones))
            {
                sb.Append(line);
                sb.AppendLine("<br />");
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Allows you to compare another address object to determine if the two addresses are the same.
        /// </summary>
        /// <param name="a2">Another address object.</param>
        /// <returns>If true, the current address matches the address in the parameter.</returns>
        public bool IsEqualTo(Address a2)
        {
            if (a2 == null)
            {
                return false;
            }

            var result = true;

            if (string.Compare(NickName.Trim(), a2.NickName.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(FirstName.Trim(), a2.FirstName.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(MiddleInitial.Trim(), a2.MiddleInitial.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(LastName.Trim(), a2.LastName.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(Company.Trim(), a2.Company.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(Line1.Trim(), a2.Line1.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(Line2.Trim(), a2.Line2.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(Line3.Trim(), a2.Line3.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(RegionBvin.Trim(), a2.RegionBvin.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(City.Trim(), a2.City.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(PostalCode.Trim(), a2.PostalCode.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(CountryBvin.Trim(), a2.CountryBvin.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(Phone.Trim(), a2.Phone.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(Fax.Trim(), a2.Fax.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            if (string.Compare(WebSiteUrl.Trim(), a2.WebSiteUrl.Trim(), true, CultureInfo.InvariantCulture) != 0)
            {
                result = false;
            }
            //if (this.Residential != a2.Residential) {
            //    result = false;
            //}

            return result;
        }

        /// <summary>
        ///     Used to copy the current address object to another address object.
        /// </summary>
        /// <param name="destinationAddress">An instance of Address where you want the address to be copied to</param>
        /// <returns>If the copy can execute successfully, true will be returned.</returns>
        public bool CopyTo(Address destinationAddress)
        {
            var result = true;

            try
            {
                destinationAddress.Bvin = Bvin;
                destinationAddress.NickName = NickName;
                destinationAddress.FirstName = FirstName;
                destinationAddress.MiddleInitial = MiddleInitial;
                destinationAddress.LastName = LastName;
                destinationAddress.Company = Company;
                destinationAddress.Line1 = Line1;
                destinationAddress.Line2 = Line2;
                destinationAddress.Line3 = Line3;
                destinationAddress.City = City;
                destinationAddress.RegionBvin = RegionBvin;
                destinationAddress.PostalCode = PostalCode;
                destinationAddress.CountryBvin = CountryBvin;
                destinationAddress.Phone = Phone;
                destinationAddress.Fax = Fax;
                destinationAddress.WebSiteUrl = WebSiteUrl;
                //destinationAddress.Residential = this.Residential;                
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        ///     Allows you to return a serialized version of this address in XML format.
        /// </summary>
        /// <param name="omitDeclaration">This value is not used in this method.</param>
        /// <returns>A string representation of the Address object in XML format.</returns>
        public virtual string ToXml(bool omitDeclaration)
        {
            var response = string.Empty;
            var sb = new StringBuilder();
            _hccXmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
            var xw = XmlWriter.Create(sb, _hccXmlWriterSettings);
            ToXmlWriter(ref xw);
            xw.Flush();
            xw.Close();
            response = sb.ToString();
            return response;
        }

        /// <summary>
        ///     Used to deserialize an XML string to form a new Address object.
        /// </summary>
        /// <param name="x">String - the XML used to create an Address object.</param>
        /// <returns>If successful, true will be returned.</returns>
        public virtual bool FromXmlString(string x)
        {
            var sw = new StringReader(x);
            var xr = XmlReader.Create(sw);
            var result = FromXml(ref xr);
            sw.Dispose();
            xr.Close();
            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current address object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of AddressDTO</returns>
        public AddressDTO ToDto()
        {
            var dto = new AddressDTO();

            dto.Bvin = Bvin;
            dto.StoreId = StoreId;
            dto.NickName = NickName ?? string.Empty;
            dto.FirstName = FirstName ?? string.Empty;
            dto.MiddleInitial = MiddleInitial ?? string.Empty;
            dto.LastName = LastName ?? string.Empty;
            dto.Company = Company ?? string.Empty;
            dto.Line1 = Line1 ?? string.Empty;
            dto.Line2 = Line2 ?? string.Empty;
            dto.Line3 = Line3 ?? string.Empty;
            dto.City = City ?? string.Empty;
            dto.RegionName = RegionSystemName ?? string.Empty;
            dto.RegionBvin = RegionBvin ?? string.Empty;
            dto.PostalCode = PostalCode ?? string.Empty;
            dto.CountryName = CountrySystemName ?? string.Empty;
            dto.CountryBvin = CountryBvin ?? string.Empty;
            dto.Phone = Phone ?? string.Empty;
            dto.Fax = Fax ?? string.Empty;
            dto.WebSiteUrl = WebSiteUrl ?? string.Empty;
            dto.UserBvin = UserBvin ?? string.Empty;
            dto.AddressType = (AddressTypesDTO) (int) AddressType;
            dto.LastUpdatedUtc = LastUpdatedUtc;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current address object using an AddressDTO instance
        /// </summary>
        /// <param name="dto">An instance of the Address from the REST API</param>
        public void FromDto(AddressDTO dto)
        {
            Bvin = dto.Bvin;
            StoreId = dto.StoreId;
            NickName = dto.NickName ?? string.Empty;
            FirstName = dto.FirstName ?? string.Empty;
            MiddleInitial = dto.MiddleInitial ?? string.Empty;
            LastName = dto.LastName ?? string.Empty;
            Company = dto.Company ?? string.Empty;
            Line1 = dto.Line1 ?? string.Empty;
            Line2 = dto.Line2 ?? string.Empty;
            Line3 = dto.Line3 ?? string.Empty;
            City = dto.City ?? string.Empty;
            RegionBvin = dto.RegionBvin ?? string.Empty;
            PostalCode = dto.PostalCode ?? string.Empty;
            CountryBvin = dto.CountryBvin ?? string.Empty;
            Phone = dto.Phone ?? string.Empty;
            Fax = dto.Fax ?? string.Empty;
            WebSiteUrl = dto.WebSiteUrl ?? string.Empty;
            UserBvin = dto.UserBvin ?? string.Empty;
            AddressType = (AddressTypes) (int) dto.AddressType;
            LastUpdatedUtc = dto.LastUpdatedUtc;
        }

        #endregion

        #region IAddress Members

        /// <summary>
        ///     The first line of the street address, such as 123 Main Street.
        /// </summary>
        /// <remarks>This property is mapped to the Line1 property.</remarks>
        public string Street
        {
            get { return Line1; }
            set { Line1 = value; }
        }

        /// <summary>
        ///     The second line of the street address, such as Suite 100.
        /// </summary>
        /// <remarks>This property is mapped to the Line2 property.</remarks>
        public string Street2
        {
            get { return Line2; }
            set { Line2 = value; }
        }

        #endregion
    }
}