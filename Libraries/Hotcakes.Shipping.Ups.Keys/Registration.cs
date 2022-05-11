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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public class Registration
    {
        private const string _DeveloperKey = "EB74C27E60949E4C";
        private bool _RequestSuggestedUsername;
        private string _XmlLicenseAcceptRequest = string.Empty;
        private string _XmlLicenseAcceptResponse = string.Empty;
        private string _XmlLicenseRequest = string.Empty;
        private string _XmlLicenseResponse = string.Empty;

        private string _XmlRegisterRequest = string.Empty;
        private string _XmlRegisterResponse = string.Empty;
        private string sAccountNumber = string.Empty;
        private string sAddress1 = string.Empty;
        private string sAddress2 = string.Empty;
        private string sAddress3 = string.Empty;
        private string sCity = string.Empty;
        private string sCompany = string.Empty;
        private string sContactMe = "no";
        private string sCountry = string.Empty;
        private string sEmail = string.Empty;
        private string sErrorCode = string.Empty;

        // Variables    
        private string sErrorMessage = string.Empty;
        private string sLicense = string.Empty;
        private string sLicenseNumber = string.Empty;
        private string sName = string.Empty;
        private string sPassword = string.Empty;
        private string sPhone = string.Empty;
        private string sPhoneExt = string.Empty;
        private string sState = string.Empty;
        private string sSuggestedUsername = string.Empty;
        private string sTitle = string.Empty;
        private string sURL = string.Empty;

        private string sUsername = string.Empty;
        private string sZip = string.Empty;

        // Properties
        public string Username
        {
            get { return sUsername; }
            set { sUsername = value; }
        }

        public string Password
        {
            get { return sPassword; }
            set { sPassword = value; }
        }

        public string Name
        {
            get { return sName; }
            set { sName = value; }
        }

        public string Company
        {
            get { return sCompany; }
            set { sCompany = value; }
        }

        public string Title
        {
            get { return sTitle; }
            set { sTitle = value; }
        }

        public string Address1
        {
            get { return sAddress1; }
            set { sAddress1 = value; }
        }

        public string Address2
        {
            get { return sAddress2; }
            set { sAddress2 = value; }
        }

        public string Address3
        {
            get { return sAddress3; }
            set { sAddress3 = value; }
        }

        public string City
        {
            get { return sCity; }
            set { sCity = value; }
        }

        public string State
        {
            get { return sState; }
            set { sState = value; }
        }

        public string Zip
        {
            get { return sZip; }
            set { sZip = value; }
        }

        public string Country
        {
            get { return sCountry; }
            set { sCountry = value; }
        }

        public string Phone
        {
            get { return sPhone; }
            set { sPhone = CleanPhoneNumber(value); }
        }

        public string PhoneExt
        {
            get { return sPhoneExt; }
            set { sPhoneExt = value; }
        }

        public string Email
        {
            get { return sEmail; }
            set { sEmail = value; }
        }

        public string ErrorMessage
        {
            get { return sErrorMessage; }
        }

        public string ErrorCode
        {
            get { return sErrorCode; }
        }

        public string SuggestedUsername
        {
            get { return sSuggestedUsername; }
        }

        public string License
        {
            get { return sLicense; }
            set { sLicense = value; }
        }

        public string URL
        {
            get { return sURL; }
            set { sURL = value; }
        }

        public string LicenseNumber
        {
            get { return sLicenseNumber; }
            set { sLicenseNumber = value; }
        }

        public string ContactMe
        {
            get { return sContactMe; }
            set { sContactMe = value; }
        }

        public string AccountNumber
        {
            get { return sAccountNumber; }
            set { sAccountNumber = value; }
        }

        public bool RequestSuggestedUsername
        {
            get { return _RequestSuggestedUsername; }
            set { _RequestSuggestedUsername = value; }
        }

        public string XmlRegisterRequest
        {
            get { return _XmlRegisterRequest; }
        }

        public string XmlRegisterResponse
        {
            get { return _XmlRegisterResponse; }
        }

        public string XmlLicenseRequest
        {
            get { return _XmlLicenseRequest; }
        }

        public string XmlLicenseResponse
        {
            get { return _XmlLicenseResponse; }
        }

        public string XmlLicenseAcceptRequest
        {
            get { return _XmlLicenseAcceptRequest; }
        }

        public string XmlLicenseAcceptResponse
        {
            get { return _XmlLicenseAcceptResponse; }
        }

        private string CleanPhoneNumber(string sNumber)
        {
            var input = sNumber.Trim();
            input = Regex.Replace(input, @"[^0-9]", string.Empty);
            return input;
        }

        private string BuildRegisterRequest()
        {
            var sXML = string.Empty;

            var strWriter = new StringWriter();
            var xw = new XmlTextWriter(strWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 3
            };

            xw.WriteStartDocument();

            //--------------------------------------------            
            // Registration Request
            xw.WriteStartElement("RegistrationRequest");

            //--------------------------------------------            
            // Request
            xw.WriteStartElement("Request");
            xw.WriteStartElement("TransactionReference");
            xw.WriteElementString("CustomerContext", "Hotcakes Registration Request");
            xw.WriteElementString("XcpiVersion", "1.0001");
            xw.WriteEndElement();
            xw.WriteElementString("RequestAction", "Register");
            if (_RequestSuggestedUsername)
            {
                xw.WriteElementString("RequestOption", "suggest");
            }
            xw.WriteEndElement();
            // End Request
            //--------------------------------------------            

            xw.WriteElementString("UserId", sUsername);
            xw.WriteElementString("Password", sPassword);

            //--------------------------------------------            
            // Registration Information
            xw.WriteStartElement("RegistrationInformation");
            xw.WriteElementString("UserName", sName);
            xw.WriteElementString("CompanyName", sCompany);
            xw.WriteElementString("Title", sTitle);

            //--------------------------------------------
            // Address 
            xw.WriteStartElement("Address");
            xw.WriteElementString("AddressLine1", sAddress1);
            if (!string.IsNullOrEmpty(sAddress2))
            {
                xw.WriteElementString("AddressLine2", sAddress2);
            }
            if (!string.IsNullOrEmpty(sAddress3))
            {
                xw.WriteElementString("AddressLine3", sAddress3);
            }
            xw.WriteElementString("City", sCity);
            xw.WriteElementString("StateProvinceCode", sState);
            if (!string.IsNullOrEmpty(sZip))
            {
                xw.WriteElementString("PostalCode", sZip);
            }
            xw.WriteElementString("CountryCode", sCountry);
            xw.WriteEndElement();
            // End Address
            //--------------------------------------------

            if (sAccountNumber.Length > 1)
            {
                xw.WriteStartElement("ShipperAccount");
                xw.WriteElementString("AccountName", "Company UPS Account");
                xw.WriteElementString("ShipperNumber", sAccountNumber);
                xw.WriteElementString("PickupPostalCode", sZip);
                xw.WriteElementString("PickupCountryCode", sCountry);
                xw.WriteEndElement();
            }

            xw.WriteElementString("PhoneNumber", sPhone);
            xw.WriteElementString("EMailAddress", sEmail);
            xw.WriteEndElement();
            // End Registration Information
            //--------------------------------------------

            xw.WriteEndElement();
            // End Registration Request
            //--------------------------------------------

            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();

            sXML = strWriter.GetStringBuilder().ToString();

            xw = null;

            return sXML;
        }

        private string BuildLicenseRequest()
        {
            var sXML = string.Empty;

            var strWriter = new StringWriter();
            var xw = new XmlTextWriter(strWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 3
            };

            xw.WriteStartDocument();

            //--------------------------------------------            
            // Agreement Request
            xw.WriteStartElement("AccessLicenseAgreementRequest");

            //--------------------------------------------            
            // Request
            xw.WriteStartElement("Request");
            xw.WriteStartElement("TransactionReference");
            xw.WriteElementString("CustomerContext", "Hotcakes License Request");
            xw.WriteElementString("XcpiVersion", "1.0001");
            xw.WriteEndElement();
            xw.WriteElementString("RequestAction", "AccessLicense");
            xw.WriteElementString("RequestOption", "AllTools");
            xw.WriteEndElement();
            // End Request
            //--------------------------------------------            

            xw.WriteElementString("DeveloperLicenseNumber", _DeveloperKey);

            //--------------------------------------------            
            // Access License Profile
            xw.WriteStartElement("AccessLicenseProfile");
            xw.WriteElementString("CountryCode", !string.IsNullOrEmpty(sCountry) ? sCountry : "US");
            xw.WriteElementString("LanguageCode", "EN");
            xw.WriteEndElement();
            // Access License Profile
            //--------------------------------------------

            //--------------------------------------------
            // Tool
            xw.WriteStartElement("OnLineTool");
            xw.WriteElementString("ToolID", "RateXML");
            xw.WriteElementString("ToolVersion", "1.0");
            xw.WriteEndElement();
            // End Tool
            //--------------------------------------------

            //--------------------------------------------
            // Tool
            xw.WriteStartElement("OnLineTool");
            xw.WriteElementString("ToolID", "TrackXML");
            xw.WriteElementString("ToolVersion", "1.0");
            xw.WriteEndElement();
            // End Tool
            //--------------------------------------------

            xw.WriteEndElement();
            // End Agreement Request
            //--------------------------------------------

            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();

            sXML = strWriter.GetStringBuilder().ToString();

            xw = null;

            return sXML;
        }

        private string BuildLicenseAccept()
        {
            var sXML = string.Empty;

            var strWriter = new StringWriter();
            var xw = new XmlTextWriter(strWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 3
            };

            xw.WriteStartDocument();

            //--------------------------------------------            
            // Agreement Request
            xw.WriteStartElement("AccessLicenseRequest");

            //--------------------------------------------            
            // Request
            xw.WriteStartElement("Request");
            xw.WriteStartElement("TransactionReference");
            xw.WriteElementString("CustomerContext", "License Request");
            xw.WriteElementString("XcpiVersion", "1.0001");
            xw.WriteEndElement();
            xw.WriteElementString("RequestAction", "AccessLicense");
            xw.WriteElementString("RequestOption", "AllTools");
            xw.WriteEndElement();
            // End Request
            //--------------------------------------------

            xw.WriteElementString("CompanyName", sCompany);

            //--------------------------------------------
            // Address
            xw.WriteStartElement("Address");
            xw.WriteElementString("AddressLine1", sAddress1);
            if (!string.IsNullOrEmpty(sAddress2))
            {
                xw.WriteElementString("Addressline2", sAddress2);
            }
            if (!string.IsNullOrEmpty(sAddress3))
            {
                xw.WriteElementString("Addressline3", sAddress3);
            }
            if (!string.IsNullOrEmpty(sCity))
            {
                xw.WriteElementString("City", sCity);
            }
            xw.WriteElementString("StateProvinceCode", sState);
            if (!string.IsNullOrEmpty(sZip))
            {
                xw.WriteElementString("PostalCode", sZip);
            }

            xw.WriteElementString("CountryCode", sCountry);
            xw.WriteEndElement();
            // End Address
            //--------------------------------------------

            //--------------------------------------------
            // Primary Contact
            xw.WriteStartElement("PrimaryContact");
            xw.WriteElementString("Name", sName);
            xw.WriteElementString("Title", sTitle);
            xw.WriteElementString("EMailAddress", sEmail);
            xw.WriteElementString("PhoneNumber", sPhone);
            xw.WriteEndElement();
            // End Primary Contact
            //--------------------------------------------

            xw.WriteElementString("CompanyURL", sURL);
            if (!string.IsNullOrEmpty(sAccountNumber))
            {
                xw.WriteElementString("ShipperNumber", sAccountNumber);
            }
            xw.WriteElementString("DeveloperLicenseNumber", _DeveloperKey);

            //--------------------------------------------            
            // Access License Profile
            xw.WriteStartElement("AccessLicenseProfile");
            if (!string.IsNullOrEmpty(sCountry))
            {
                xw.WriteElementString("CountryCode", sCountry);
            }
            else
            {
                xw.WriteElementString("CountryCode", "US");
            }
            xw.WriteElementString("LanguageCode", "EN");

            xw.WriteElementString("AccessLicenseText", sLicense);

            xw.WriteEndElement();
            // Access License Profile
            //--------------------------------------------

            //--------------------------------------------
            // Tool
            xw.WriteStartElement("OnLineTool");
            xw.WriteElementString("ToolID", "RateXML");
            xw.WriteElementString("ToolVersion", "1.0");
            xw.WriteEndElement();
            // End Tool
            //--------------------------------------------

            //--------------------------------------------
            // Tool
            xw.WriteStartElement("OnLineTool");
            xw.WriteElementString("ToolID", "TrackXML");
            xw.WriteElementString("ToolVersion", "1.0");
            xw.WriteEndElement();
            // End Tool
            //--------------------------------------------

            //--------------------------------------------
            // Client Software Profile
            xw.WriteStartElement("ClientSoftwareProfile");
            xw.WriteElementString("SoftwareInstaller", sContactMe);
            xw.WriteElementString("SoftwareProductName", "Hotcakes");
            xw.WriteElementString("SoftwareProvider", "Hotcakes Commerce, LLC");
            xw.WriteElementString("SoftwareVersionNumber", "1.0");
            xw.WriteEndElement();
            // End Client Software Profile
            //--------------------------------------------

            xw.WriteEndElement();
            // End Agreement Request
            //--------------------------------------------

            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();

            sXML = strWriter.GetStringBuilder().ToString();

            xw = null;

            return sXML;
        }

        public bool Register(string serverUrl)
        {
            var bRet = true;

            sErrorMessage = string.Empty;
            sErrorCode = string.Empty;
            sSuggestedUsername = string.Empty;

            var sURL = serverUrl;
            sURL += "Register";
            var sXML = BuildRegisterRequest();

            // Save Request
            _XmlRegisterRequest = sXML;

            var sResponse = string.Empty;

            try
            {
                sResponse = ReadHtmlPage_POST(sURL, sXML);
                // Save Response
                _XmlRegisterResponse = sResponse;
            }
            catch (Exception Ex)
            {
                throw new ArgumentException("Error Calling HTTP POST: " + Ex.Message);
            }

            XmlDocument xDoc;
            XmlNodeList NodeList;
            var sStatusCode = "-1";

            try
            {
                xDoc = new XmlDocument();
                xDoc.LoadXml(sResponse);

                if (xDoc.DocumentElement.Name == "RegistrationResponse")
                {
                    XmlNode n;
                    var i = 0;
                    XmlNode nTag;

                    NodeList = xDoc.GetElementsByTagName("RegistrationResponse");
                    n = NodeList.Item(0);
                    for (i = 0; i <= n.ChildNodes.Count - 1; i++)
                    {
                        nTag = n.ChildNodes.Item(i);
                        switch (nTag.Name)
                        {
                            case "Response":
                                var iRes = 0;
                                XmlNode nRes;
                                for (iRes = 0; iRes <= nTag.ChildNodes.Count - 1; iRes++)
                                {
                                    nRes = nTag.ChildNodes[iRes];
                                    switch (nRes.Name)
                                    {
                                        case "ResponseStatusCode":
                                            sStatusCode = nRes.FirstChild.Value;
                                            break;
                                        case "ResponseStatusDescription":
                                            // Not Used
                                            break;
                                        case "Error":
                                            var iErr = 0;
                                            XmlNode nErr;
                                            for (iErr = 0; iErr <= nRes.ChildNodes.Count - 1; iErr++)
                                            {
                                                nErr = nRes.ChildNodes[iErr];
                                                switch (nErr.Name)
                                                {
                                                    case "ErrorCode":
                                                        sErrorCode = nErr.FirstChild.Value;
                                                        break;
                                                    case "ErrorDescription":
                                                        sErrorMessage = nErr.FirstChild.Value;
                                                        break;
                                                    case "ErrorSeverity":
                                                        // Not Used
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "UserId":
                                sSuggestedUsername = nTag.FirstChild.Value;
                                break;
                        }
                    }
                }
                else
                {
                    bRet = false;
                    sErrorMessage = "Couldn't find valid XML response from server.";
                }
            }
            catch (Exception Exx)
            {
                throw new ArgumentException("Error Reading Response: " + Exx.Message);
            }

            // Process Response
            if (sStatusCode == "1")
            {
                bRet = true;
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        public bool GetLicense(string serverUrl)
        {
            var bRet = true;

            sErrorMessage = string.Empty;
            sErrorCode = string.Empty;
            sLicense = string.Empty;

            var sURL = serverUrl;
            sURL += "License";

            var sXML = BuildLicenseRequest();

            // Save Request
            _XmlLicenseRequest = sXML;

            var sResponse = string.Empty;

            try
            {
                sResponse = ReadHtmlPage_POST(sURL, sXML);
                // Save Response
                _XmlLicenseResponse = sResponse;
            }
            catch (Exception Ex)
            {
                throw new ArgumentException("Error Calling HTTP POST: " + Ex.Message);
            }

            XmlDocument xDoc;
            XmlNodeList NodeList;
            var sStatusCode = "-1";

            try
            {
                xDoc = new XmlDocument();
                xDoc.LoadXml(sResponse);

                if (xDoc.DocumentElement.Name == "AccessLicenseAgreementResponse")
                {
                    XmlNode n;
                    var i = 0;
                    XmlNode nTag;

                    NodeList = xDoc.GetElementsByTagName("AccessLicenseAgreementResponse");
                    n = NodeList.Item(0);
                    for (i = 0; i <= n.ChildNodes.Count - 1; i++)
                    {
                        nTag = n.ChildNodes.Item(i);
                        switch (nTag.Name)
                        {
                            case "Response":
                                var iRes = 0;
                                XmlNode nRes;
                                for (iRes = 0; iRes <= nTag.ChildNodes.Count - 1; iRes++)
                                {
                                    nRes = nTag.ChildNodes[iRes];
                                    switch (nRes.Name)
                                    {
                                        case "ResponseStatusCode":
                                            sStatusCode = nRes.FirstChild.Value;
                                            break;
                                        case "ResponseStatusDescription":
                                            // Not Used
                                            break;
                                        case "Error":
                                            var iErr = 0;
                                            XmlNode nErr;
                                            for (iErr = 0; iErr <= nRes.ChildNodes.Count - 1; iErr++)
                                            {
                                                nErr = nRes.ChildNodes[iErr];
                                                switch (nErr.Name)
                                                {
                                                    case "ErrorCode":
                                                        sErrorCode = nErr.FirstChild.Value;
                                                        break;
                                                    case "ErrorDescription":
                                                        sErrorMessage = nErr.FirstChild.Value;
                                                        break;
                                                    case "ErrorSeverity":
                                                        // Not Used
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "AccessLicenseText":
                                sLicense = nTag.FirstChild.Value;
                                break;
                        }
                    }
                }
                else
                {
                    bRet = false;
                    sErrorMessage = "Couldn't find valid XML response from server.";
                }
            }

            catch (Exception Exx)
            {
                throw new ArgumentException("Error Reading Response: " + Exx.Message);
            }

            // Process Response
            if (sStatusCode == "1")
            {
                bRet = true;
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        public bool AcceptLicense(string serverUrl)
        {
            var bRet = true;
            var sResponse = string.Empty;
            sErrorMessage = string.Empty;
            sErrorCode = string.Empty;

            var sURL = serverUrl;
            sURL += "License";

            var sXML = BuildLicenseAccept();

            // Save Request
            _XmlLicenseAcceptRequest = sXML;

            try
            {
                sResponse = ReadHtmlPage_POST(sURL, sXML);
                // Save Response
                _XmlLicenseAcceptResponse = sResponse;
            }
            catch (Exception Ex)
            {
                throw new ArgumentException("Error Calling HTTP POST: " + Ex.Message);
            }

            XmlDocument xDoc;
            XmlNodeList NodeList;
            var sStatusCode = "-1";

            try
            {
                xDoc = new XmlDocument();
                xDoc.LoadXml(sResponse);

                if (xDoc.DocumentElement.Name == "AccessLicenseResponse")
                {
                    XmlNode n;
                    var i = 0;
                    XmlNode nTag;

                    NodeList = xDoc.GetElementsByTagName("AccessLicenseResponse");
                    n = NodeList.Item(0);
                    for (i = 0; i <= n.ChildNodes.Count - 1; i++)
                    {
                        nTag = n.ChildNodes.Item(i);
                        switch (nTag.Name)
                        {
                            case "Response":
                                var iRes = 0;
                                XmlNode nRes;
                                for (iRes = 0; iRes <= nTag.ChildNodes.Count - 1; iRes++)
                                {
                                    nRes = nTag.ChildNodes[iRes];
                                    switch (nRes.Name)
                                    {
                                        case "ResponseStatusCode":
                                            sStatusCode = nRes.FirstChild.Value;
                                            break;
                                        case "ResponseStatusDescription":
                                            // Not Used
                                            break;
                                        case "Error":
                                            var iErr = 0;
                                            XmlNode nErr;
                                            for (iErr = 0; iErr <= nRes.ChildNodes.Count - 1; iErr++)
                                            {
                                                nErr = nRes.ChildNodes[iErr];
                                                switch (nErr.Name)
                                                {
                                                    case "ErrorCode":
                                                        sErrorCode = nErr.FirstChild.Value;
                                                        break;
                                                    case "ErrorDescription":
                                                        sErrorMessage = nErr.FirstChild.Value;
                                                        break;
                                                    case "ErrorSeverity":
                                                        // Not Used
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "AccessLicenseNumber":
                                sLicenseNumber = nTag.FirstChild.Value;
                                break;
                        }
                    }
                }
                else
                {
                    bRet = false;
                    sErrorMessage = "Couldn't find valid XML response from server.";
                }
            }
            catch (Exception Exx)
            {
                throw new ArgumentException("Error Reading Response: " + Exx.Message);
            }

            // Process Response
            if (sStatusCode == "1")
            {
                bRet = true;
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        private string ReadHtmlPage_POST(string sURL, string sPostData)
        {
            HttpWebResponse objResponse;
            HttpWebRequest objRequest;
            var result = string.Empty;
            byte[] bytes;

            // Encode Post Stream
            try
            {
                bytes = Encoding.Default.GetBytes(sPostData);
            }
            catch (Exception Ex1)
            {
                throw new ArgumentException("Setup Bytes Exception: " + Ex1.Message);
            }

            // Create Request
            try
            {
                objRequest = (HttpWebRequest)WebRequest.Create(sURL);
                objRequest.Method = "POST";
                objRequest.ContentLength = bytes.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";
            }
            catch (Exception E)
            {
                throw new ArgumentException("Error Creating Web Request: " + E.Message);
            }

            // Dump Post Data to Request Stream
            try
            {
                var OutStream = objRequest.GetRequestStream();
                OutStream.Write(bytes, 0, bytes.Length);
                OutStream.Close();
            }
            catch (Exception Ex)
            {
                throw new ArgumentException("Error Posting Data: " + Ex.Message + " " + Ex.Source + " " + Ex.StackTrace);
            }
            finally
            {
            }

            // Read Response
            try
            {
                objResponse = (HttpWebResponse)objRequest.GetResponse();
                var sr = new StreamReader(objResponse.GetResponseStream(), Encoding.Default, true);
                result += sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception Exx)
            {
                throw new ArgumentException("Stream Reader Error: " + Exx.Message + " " + Exx.Source);
            }

            return result;
        }
    }
}