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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AuthorizeNet;
using AuthorizeNet.APICore;

namespace Hotcakes.Commerce.Tests
{
    public class SubscriptionGateway2
    {
        private readonly HttpXmlUtility2 _gateway;

        public SubscriptionGateway2(string apiLogin, string transactionKey, ServiceMode mode)
        {
            if (mode == ServiceMode.Live)
            {
                _gateway = new HttpXmlUtility2(ServiceMode.Live, apiLogin, transactionKey);
                return;
            }
            _gateway = new HttpXmlUtility2(ServiceMode.Test, apiLogin, transactionKey);
        }

        public SubscriptionDetail GetSubscriptionById(int subId)
        {
            return
                GetSubscriptionList(ARBGetSubscriptionListSearchTypeEnum.subscriptionActive)
                    .FirstOrDefault(s => s.id == subId);
        }

        public List<SubscriptionDetail> GetSubscriptionList(ARBGetSubscriptionListSearchTypeEnum seacrhType)
        {
            return GetSubscriptionList(
                seacrhType,
                new ARBGetSubscriptionListSorting
                {
                    orderBy = ARBGetSubscriptionListOrderFieldEnum.id,
                    orderDescending = true
                },
                new Paging {limit = 1000, offset = 1});
        }

        public List<SubscriptionDetail> GetSubscriptionList(ARBGetSubscriptionListSearchTypeEnum searchType,
            ARBGetSubscriptionListSorting sorting, Paging paging)
        {
            var request = new ARBGetSubscriptionListRequest
            {
                paging = paging,
                searchType = searchType,
                sorting = sorting
            };
            var response = (ARBGetSubscriptionListResponse) _gateway.Send(request);

            return response.subscriptionDetails.ToList();
        }

        public class HttpXmlUtility2
        {
            public const string TEST_URL = "https://apitest.authorize.net/xml/v1/request.api";
            public const string URL = "https://api2.authorize.net/xml/v1/request.api";
            private readonly string _apiLogin = string.Empty;
            private readonly string _serviceUrl = "https://apitest.authorize.net/xml/v1/request.api";
            private readonly string _transactionKey = string.Empty;
            private XmlDocument _xmlDoc;

            public HttpXmlUtility2(ServiceMode mode, string apiLogin, string transactionKey)
            {
                if (mode == ServiceMode.Live)
                {
                    _serviceUrl = "https://api2.authorize.net/xml/v1/request.api";
                }
                _apiLogin = apiLogin;
                _transactionKey = transactionKey;
            }

            private void AuthenticateRequest(ANetApiRequest request)
            {
                request.merchantAuthentication = new merchantAuthenticationType();
                request.merchantAuthentication.name = _apiLogin;
                request.merchantAuthentication.Item = _transactionKey;
                request.merchantAuthentication.ItemElementName = ItemChoiceType.transactionKey;
            }

            public ANetApiResponse Send(ANetApiRequest apiRequest)
            {
                AuthenticateRequest(apiRequest);
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(_serviceUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.KeepAlive = true;
                var type = apiRequest.GetType();
                var xmlSerializer = new XmlSerializer(type);
                XmlWriter xmlWriter = new XmlTextWriter(httpWebRequest.GetRequestStream(), Encoding.UTF8);
                xmlSerializer.Serialize(xmlWriter, apiRequest);
                xmlWriter.Close();
                var response = httpWebRequest.GetResponse();
                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(XmlReader.Create(response.GetResponseStream()));
                var aNetApiResponse = DecideResponse(_xmlDoc);
                CheckForErrors(aNetApiResponse);
                return aNetApiResponse;
            }

            private string Serialize(object apiRequest)
            {
                var result = "";
                using (var memoryStream = new MemoryStream())
                {
                    var xmlSerializer = new XmlSerializer(apiRequest.GetType());
                    var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                    xmlSerializer.Serialize(xmlTextWriter, apiRequest);
                    xmlTextWriter.Close();
                    result = Encoding.UTF8.GetString(memoryStream.GetBuffer());
                }
                return result;
            }

            private void CheckForErrors(ANetApiResponse response)
            {
                if (response.GetType() == typeof (createCustomerProfileTransactionResponse))
                {
                    var createCustomerProfileTransactionResponse = (createCustomerProfileTransactionResponse) response;
                    createCustomerProfileTransactionResponse.directResponse =
                        _xmlDoc.ChildNodes[1].ChildNodes[1].InnerText;
                    response = createCustomerProfileTransactionResponse;
                    return;
                }
                if (response.messages.message.Length > 0 && response.messages.resultCode == messageTypeEnum.Error)
                {
                    var stringBuilder = new StringBuilder();
                    for (var i = 0; i < response.messages.message.Length; i++)
                    {
                        stringBuilder.AppendFormat("Error processing request: {0} - {1}",
                            response.messages.message[i].code, response.messages.message[i].text);
                    }
                    throw new InvalidOperationException(stringBuilder.ToString());
                }
            }

            private ANetApiResponse DecideResponse(XmlDocument xmldoc)
            {
                ANetApiResponse result = null;
                try
                {
                    var textReader = new StringReader(xmldoc.DocumentElement.OuterXml);
                    string name;
                    switch (name = xmldoc.DocumentElement.Name)
                    {
                        case "getSettledBatchListResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getSettledBatchListResponse));
                            result = (getSettledBatchListResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getTransactionDetailsResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getTransactionDetailsResponse));
                            result = (getTransactionDetailsResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getTransactionListResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getTransactionListResponse));
                            result = (getTransactionListResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getUnsettledTransactionListResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getUnsettledTransactionListResponse));
                            result = (getUnsettledTransactionListResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getBatchStatisticsResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getBatchStatisticsResponse));
                            result = (getBatchStatisticsResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "createCustomerPaymentProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (createCustomerPaymentProfileResponse));
                            result = (createCustomerPaymentProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "createCustomerProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (createCustomerProfileResponse));
                            result = (createCustomerProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "createCustomerProfileTransactionResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (createCustomerProfileTransactionResponse));
                            result = (createCustomerProfileTransactionResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "createCustomerShippingAddressResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (createCustomerShippingAddressResponse));
                            result = (createCustomerShippingAddressResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "deleteCustomerPaymentProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (deleteCustomerPaymentProfileResponse));
                            result = (deleteCustomerPaymentProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "deleteCustomerProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (deleteCustomerProfileResponse));
                            result = (deleteCustomerProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "deleteCustomerShippingAddressResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (deleteCustomerShippingAddressResponse));
                            result = (deleteCustomerShippingAddressResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getCustomerPaymentProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getCustomerPaymentProfileResponse));
                            result = (getCustomerPaymentProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getCustomerProfileIdsResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getCustomerProfileIdsResponse));
                            result = (getCustomerProfileIdsResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getCustomerProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getCustomerProfileResponse));
                            result = (getCustomerProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "getCustomerShippingAddressResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (getCustomerShippingAddressResponse));
                            result = (getCustomerShippingAddressResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "isAliveResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (isAliveResponse));
                            result = (isAliveResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "updateCustomerPaymentProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (updateCustomerPaymentProfileResponse));
                            result = (updateCustomerPaymentProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "updateCustomerProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (updateCustomerProfileResponse));
                            result = (updateCustomerProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "updateCustomerShippingAddressResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (updateCustomerShippingAddressResponse));
                            result = (updateCustomerShippingAddressResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "validateCustomerPaymentProfileResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (validateCustomerPaymentProfileResponse));
                            result = (validateCustomerPaymentProfileResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "ARBCreateSubscriptionResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (ARBCreateSubscriptionResponse));
                            result = (ARBCreateSubscriptionResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "ARBUpdateSubscriptionResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (ARBUpdateSubscriptionResponse));
                            result = (ARBUpdateSubscriptionResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "ARBCancelSubscriptionResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (ARBCancelSubscriptionResponse));
                            result = (ARBCancelSubscriptionResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "ARBGetSubscriptionStatusResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (ARBGetSubscriptionStatusResponse));
                            result = (ARBGetSubscriptionStatusResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "ARBGetSubscriptionListResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (ARBGetSubscriptionListResponse));
                            result = (ARBGetSubscriptionListResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                        case "ErrorResponse":
                        {
                            var xmlSerializer = new XmlSerializer(typeof (ANetApiResponse));
                            result = (ANetApiResponse) xmlSerializer.Deserialize(textReader);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType() + ": " + ex.Message);
                }
                return result;
            }
        }
    }
}