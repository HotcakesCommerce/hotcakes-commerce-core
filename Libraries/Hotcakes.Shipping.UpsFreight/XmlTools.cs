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
using System.Collections;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Hotcakes.Shipping.UpsFreight
{
    [Serializable]
    public sealed class XmlTools
    {
        private XmlTools()
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     builds XML access key for UPS requests
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public static string BuildAccessKey(UPSFreightSettings settings)
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
            xw.WriteStartElement("AccessRequest");

            xw.WriteElementString("AccessLicenseNumber", settings.License);
            xw.WriteElementString("UserId", settings.UserID);
            xw.WriteElementString("Password", settings.Password);

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

        public static string ReadHtmlPage_POST(string sURL, string sPostData)
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
                objRequest = (HttpWebRequest) WebRequest.Create(sURL);
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
                objResponse = (HttpWebResponse) objRequest.GetResponse();
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

        public static string CleanPhoneNumber(string sNumber)
        {
            var input = sNumber.Trim();
            input = Regex.Replace(input, @"[^0-9]", string.Empty);
            return input;
        }

        public static string TrimToLength(string input, int maxLength)
        {
            if (input == null)
            {
                return string.Empty;
            }
            if (input.Length < 1)
            {
                return input;
            }
            if (maxLength < 0)
            {
                maxLength = input.Length;
            }

            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength);
            }
            return input;
        }


        #region " Conversion Methods "

        public static string ConvertCurrencyCodeToString(CurrencyCode c)
        {
            var result = "USD";

            switch (c)
            {
                case CurrencyCode.AustalianDollar:
                    result = "AUD";
                    break;
                case CurrencyCode.Baht:
                    result = "THB";
                    break;
                case CurrencyCode.BritishPounds:
                    result = "GBP";
                    break;
                case CurrencyCode.CanadianDollar:
                    result = "CAD";
                    break;
                case CurrencyCode.DenmarkKrone:
                    result = "DKK";
                    break;
                case CurrencyCode.Drachma:
                    result = "GRD";
                    break;
                case CurrencyCode.Euro:
                    result = "EUR";
                    break;
                case CurrencyCode.HongKongDollar:
                    result = "HKD";
                    break;
                case CurrencyCode.NewZealandDollar:
                    result = "NZD";
                    break;
                case CurrencyCode.NorwayKrone:
                    result = "NOK";
                    break;
                case CurrencyCode.Peso:
                    result = "MXN";
                    break;
                case CurrencyCode.Ringgit:
                    result = "MYR";
                    break;
                case CurrencyCode.SingaporeDollar:
                    result = "SGD";
                    break;
                case CurrencyCode.SwedishKrona:
                    result = "SEK";
                    break;
                case CurrencyCode.SwissFranc:
                    result = "CHF";
                    break;
                case CurrencyCode.TaiwanDollar:
                    result = "TWD";
                    break;
                case CurrencyCode.UsDollar:
                    result = "USD";
                    break;
                default:
                    result = "USD";
                    break;
            }

            return result;
        }

        public static CurrencyCode ConvertStringToCurrencyCode(string s)
        {
            var result = CurrencyCode.UsDollar;

            switch (s.ToUpper())
            {
                case "AUD":
                    result = CurrencyCode.AustalianDollar;
                    break;
                case "THB":
                    result = CurrencyCode.Baht;
                    break;
                case "GBP":
                    result = CurrencyCode.BritishPounds;
                    break;
                case "CAD":
                    result = CurrencyCode.CanadianDollar;
                    break;
                case "DKK":
                    result = CurrencyCode.DenmarkKrone;
                    break;
                case "GRD":
                    result = CurrencyCode.Drachma;
                    break;
                case "EUR":
                    result = CurrencyCode.Euro;
                    break;
                case "HKD":
                    result = CurrencyCode.HongKongDollar;
                    break;
                case "NZD":
                    result = CurrencyCode.NewZealandDollar;
                    break;
                case "NOK":
                    result = CurrencyCode.NorwayKrone;
                    break;
                case "MXN":
                    result = CurrencyCode.Peso;
                    break;
                case "MYR":
                    result = CurrencyCode.Ringgit;
                    break;
                case "SGD":
                    result = CurrencyCode.SingaporeDollar;
                    break;
                case "SEK":
                    result = CurrencyCode.SwedishKrona;
                    break;
                case "CHF":
                    result = CurrencyCode.SwissFranc;
                    break;
                case "TWD":
                    result = CurrencyCode.TaiwanDollar;
                    break;
                case "USD":
                    result = CurrencyCode.UsDollar;
                    break;
                default:
                    result = CurrencyCode.UsDollar;
                    break;
            }

            return result;
        }

        public static UnitsType ConvertStringToUnits(string s)
        {
            var result = UnitsType.Imperial;

            switch (s.ToUpper())
            {
                case "LBS":
                    result = UnitsType.Imperial;
                    break;
                case "IN":
                    result = UnitsType.Imperial;
                    break;
                case "KGS":
                    result = UnitsType.Metric;
                    break;
                case "CM":
                    result = UnitsType.Metric;
                    break;
                default:
                    result = UnitsType.Imperial;
                    break;
            }

            return result;
        }


        #endregion

        #region " Write Entity Methods "

        public static bool WriteShipperXml(ref XmlTextWriter xw, ref Entity e)
        {
            return WriteEntity(ref xw, ref e, EntityType.Shipper);
        }

        public static bool WriteShipToXml(ref XmlTextWriter xw, ref Entity e)
        {
            return WriteEntity(ref xw, ref e, EntityType.ShipTo);
        }

        public static bool WriteShipFromXml(ref XmlTextWriter xw, ref Entity e)
        {
            return WriteEntity(ref xw, ref e, EntityType.ShipFrom);
        }

        private static bool WriteEntity(ref XmlTextWriter xw, ref Entity e, EntityType et)
        {
            var result = true;
            
            //--------------------------------------------
            // Start Opening Tag
            switch (et)
            {
                case EntityType.ShipFrom:
                    xw.WriteStartElement("ShipFrom");
                    xw.WriteElementString("CompanyName", TrimToLength(e.CompanyOrContactName, 35));
                    break;
                case EntityType.Shipper:
                    xw.WriteStartElement("Shipper");
                    xw.WriteElementString("Name", TrimToLength(e.CompanyOrContactName, 35));
                    xw.WriteElementString("ShipperNumber", e.AccountNumber);
                    if (e.TaxIDNumber != string.Empty)
                    {
                        xw.WriteElementString("TaxIdentificationNumber", e.TaxIDNumber);
                    }
                    break;
                case EntityType.ShipTo:
                    xw.WriteStartElement("ShipTo");
                    xw.WriteElementString("CompanyName", TrimToLength(e.CompanyOrContactName, 35));
                    if (e.TaxIDNumber != string.Empty)
                    {
                        xw.WriteElementString("TaxIdentificationNumber", e.TaxIDNumber);
                    }
                    break;
            }

            if (e.AttentionName != string.Empty)
            {
                xw.WriteElementString("AttentionName", TrimToLength(e.AttentionName, 35));
            }

            if (e.PhoneNumber != string.Empty)
            {
                xw.WriteElementString("PhoneNumber", e.PhoneNumber);
            }

            if (e.FaxNumber != string.Empty)
            {
                xw.WriteElementString("FaxNumber", e.FaxNumber);
            }

            //-------------------------------------------
            // Start Address
            xw.WriteStartElement("Address");
            xw.WriteElementString("AddressLine1", TrimToLength(e.AddressLine1, 35));
            if (e.AddressLine2 != string.Empty)
            {
                xw.WriteElementString("AddressLine2", TrimToLength(e.AddressLine2, 35));
            }
            if (e.AddressLine3 != string.Empty)
            {
                xw.WriteElementString("AddressLine3", TrimToLength(e.AddressLine3, 35));
            }
            xw.WriteElementString("City", e.City);
            if (e.StateProvinceCode != string.Empty)
            {
                xw.WriteElementString("StateProvinceCode", TrimToLength(e.StateProvinceCode, 5));
            }
            if (e.PostalCode != string.Empty)
            {
                xw.WriteElementString("PostalCode", TrimToLength(e.PostalCode, 10));
            }
            xw.WriteElementString("CountryCode", TrimToLength(e.CountryCode, 2));

            if (et == EntityType.ShipTo)
            {
                if (e.ResidentialAddress)
                {
                    xw.WriteElementString("ResidentialAddress", "");
                }
            }

            xw.WriteEndElement();
            // End Address
            //-------------------------------------------

            xw.WriteEndElement();
            // End Opening Tag
            //--------------------------------------------

            return result;
        }

        #endregion

        #region " XPath Parsing Methods "

        public static string XPathToString(ref XmlDocument xdoc, string xpath)
        {
            var result = string.Empty;

            var node = xdoc.SelectSingleNode(xpath);
            if (node != null)
            {
                result = node.InnerText;
            }

            return result;
        }

        public static decimal XPathToInteger(ref XmlDocument xdoc, string xpath)
        {
            var result = 0;

            var node = xdoc.SelectSingleNode(xpath);
            if (node != null)
            {
                result = int.Parse(node.InnerText);
            }

            return result;
        }

        public static decimal XPathToDecimal(ref XmlDocument xdoc, string xpath)
        {
            decimal result = 0;

            var node = xdoc.SelectSingleNode(xpath);
            if (node != null)
            {
                result = decimal.Parse(node.InnerText);
            }

            return result;
        }

        public static CurrencyCode XPathToCurrencyCode(ref XmlDocument xdoc, string xpath)
        {
            var result = CurrencyCode.UsDollar;

            var node = xdoc.SelectSingleNode(xpath);
            if (node != null)
            {
                result = ConvertStringToCurrencyCode(node.InnerText);
            }

            return result;
        }

        public static UnitsType XPathToUnits(ref XmlDocument xdoc, string xpath)
        {
            var result = UnitsType.Imperial;

            var node = xdoc.SelectSingleNode(xpath);
            if (node != null)
            {
                result = ConvertStringToUnits(node.InnerText);
            }

            return result;
        }

      

        #endregion


        #region "Soap Methods"
        public static string SOAPSerializer(Object objToSoap)
        {
            IFormatter formatter;
            MemoryStream memStream = null;
            string strObject = "";
            try
            {
                memStream = new MemoryStream();
                formatter = new SoapFormatter();

                formatter.Serialize(memStream, objToSoap);
                strObject =
                   Encoding.ASCII.GetString(memStream.GetBuffer());

                //Check for the null terminator character
                int index = strObject.IndexOf("\0");
                if (index > 0)
                {
                    strObject = strObject.Substring(0, index);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (memStream != null) memStream.Close();
            }
            return strObject;
        }

        public static string BuildSoapRequest(UPSFreightSettings settings, string bodyElement)
        {

            StringBuilder xml = new StringBuilder();

            xml.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" ");
            xml.Append(@"xmlns:v1=""http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0"" ");
            xml.Append(@"xmlns:v11=""http://www.ups.com/XMLSchema/XOLTWS/FreightRate/v1.0"" ");
            xml.Append(@"xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" ");
            xml.Append(@"xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" ");
            xml.Append(@"xmlns:v12=""http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0"">");

            xml.Append("<soapenv:Header>");
            xml.Append("<v1:UPSSecurity>");
            xml.Append("<v1:UsernameToken>");
            xml.Append("<v1:Username>Tusharupendoventures</v1:Username>");
            xml.Append("<v1:Password>pendo@123</v1:Password>");
            xml.Append("</v1:UsernameToken>");
            xml.Append("<v1:ServiceAccessToken>");
            xml.Append("<v1:AccessLicenseNumber>4A24R5</v1:AccessLicenseNumber>");
            xml.Append("</v1:ServiceAccessToken>");
            xml.Append("</v1:UPSSecurity>");
            xml.Append("</soapenv:Header>");
            xml.Append("<soapenv:Body>");
            xml.Append(bodyElement);
            xml.Append("</soapenv:Body>");
            xml.Append("</soapenv:Envelope>");

            return xml.ToString();
        }

        public static string SOAPWebRequest_POST(string apiURL,string soapData)
        {
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(soapData);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(apiURL);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

           // System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    return soapResult;
                    //return webRequest;
                }
            }
        }

        public static object ParseSoapResponse(string response)
        {
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.LoadXml(response);
            
            if (responseDoc.GetElementsByTagName("freightRate:FreightRateResponse") != null && responseDoc.GetElementsByTagName("freightRate:FreightRateResponse").Count > 0)
            {
                XmlNode node = responseDoc.GetElementsByTagName("freightRate:FreightRateResponse").Item(0);

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(FreightRateResponse));
                FreightRateResponse freightRateResponse = (FreightRateResponse)serializer.Deserialize(new StringReader(node.OuterXml.ToString()));
                return freightRateResponse;
            }
            else
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Envelope));
                Envelope errorResponse = (Envelope)serializer.Deserialize(new StringReader(responseDoc.OuterXml));
                return errorResponse;
            }
        }

        #endregion
    }
}