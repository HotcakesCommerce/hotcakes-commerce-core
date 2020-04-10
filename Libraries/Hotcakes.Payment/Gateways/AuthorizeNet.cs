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
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Hotcakes.Web;

namespace Hotcakes.Payment.Gateways
{
	[Serializable]
	public class AuthorizeNet : PaymentGateway
	{
		private const string LiveUrl = "https://secure2.authorize.net/gateway/transact.dll";
		private const string DeveloperUrl = "https://test.authorize.net/gateway/transact.dll";

        public AuthorizeNet()
        {
            Settings = new AuthorizeNetSettings();
        }

		public override string Name
		{
			get { return "Authorize.Net"; }
		}

		public override string Id
		{
			get { return "828F3F70-EF01-4db6-A385-C5467CF91587"; }
		}

		public AuthorizeNetSettings Settings { get; set; }

		public override MethodSettings BaseSettings
		{
			get { return Settings; }
		}

		public override void ProcessTransaction(Transaction t)
		{
			try
			{
                var apiUrl = LiveUrl;
				if (Settings.DeveloperMode)
					apiUrl = DeveloperUrl;

                using (var client = new WebClient())
				{
                    var parameters = BuildRequestData(t);

					if (Settings.DebugMode)
					{
						t.Result.Messages.Add(new Message(Url.BuldQueryString(parameters), "DEBUG", MessageType.Information));
					}

                    var responseBytes = client.UploadValues(apiUrl, "POST", parameters);
                    var responseBody = Encoding.UTF8.GetString(responseBytes);

					if (Settings.DebugMode)
					{
						t.Result.Messages.Add(new Message(HttpUtility.UrlDecode(responseBody), "DEBUG", MessageType.Information));
					}

					ProcessResponse(t, responseBody);
				}
			}
			catch (Exception ex)
			{
				t.Result.Succeeded = false;
				t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP", MessageType.Error));
				t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
			}
		}

		#region Private Methods

		private NameValueCollection BuildRequestData(Transaction t)
		{
            var parameters = new NameValueCollection();

            /*
             * Added to get support from Authorize.net
             * https://developer.authorize.net/api/solution_id/
             */
            if (Settings.DeveloperMode)
            {
                //parameters.Add("x_solution_id", "AAA100302"); // sandbox
                parameters.Add("x_solution_id", "AAA100303"); // sandbox
                //parameters.Add("x_solution_id", "AAA100304"); // sandbox
            }
            else
            {
                parameters.Add("x_solution_id", "AAA175312"); // production
            }

            parameters.Add("x_version", "3.1");
			parameters.Add("x_login", Settings.MerchantLoginId);
			parameters.Add("x_tran_key", Settings.TransactionKey);
			parameters.Add("x_Amount", t.Amount.ToString(CultureInfo.InvariantCulture));
			parameters.Add("x_Cust_ID", Regex.Replace(t.Customer.Email, @"[^0-9a-zA-Z]+", string.Empty));
			parameters.Add("x_Description", t.MerchantDescription);
			parameters.Add("x_invoice_num", t.MerchantInvoiceNumber);
			parameters.Add("x_Email_Customer", WriteBool(Settings.SendEmailToCustomer));
			parameters.Add("x_delim_data", WriteBool(true));
			parameters.Add("x_ADC_URL", WriteBool(false));
			parameters.Add("x_delim_char", ",");
			parameters.Add("x_relay_response", WriteBool(false));

			parameters.Add("x_Email", t.Customer.Email);


			parameters.Add("x_First_Name", t.Customer.FirstName);
			parameters.Add("x_Last_Name", t.Customer.LastName);
			parameters.Add("x_Company", t.Customer.Company);
			parameters.Add("x_Address", t.Customer.Street);
			parameters.Add("x_City", t.Customer.City);
            var country = t.Customer.CountryData;
			if (country != null)
			{
				parameters.Add("x_Country", country.IsoNumeric);
			}
			else
			{
				parameters.Add("x_Country", t.Customer.CountryName);
			}
			// TODO: Add code to make sure we've got the correct state format
			if (!string.IsNullOrWhiteSpace(t.Customer.RegionName))
			{
				parameters.Add("x_State", t.Customer.RegionName);
			}

			parameters.Add("x_Zip", t.Customer.PostalCode);
			parameters.Add("x_Phone", t.Customer.Phone);

			// only add the shipping attributes if there is at least one shippable line item
			if (t.Items != null && t.Items.Any(li => !li.IsNonShipping))
			{
				parameters.Add("x_Ship_To_First_Name", t.Customer.ShipFirstName);
				parameters.Add("x_Ship_To_Last_Name", t.Customer.ShipLastName);
				parameters.Add("x_Ship_To_Company", t.Customer.ShipCompany);
				parameters.Add("x_Ship_To_Address", t.Customer.ShipStreet);
				parameters.Add("x_Ship_To_City", t.Customer.ShipCity);

				var shipcountry = t.Customer.ShipCountryData;
                parameters.Add("x_Ship_To_Country",
                    shipcountry != null ? shipcountry.IsoNumeric : t.Customer.ShipCountryName);

				// TODO: Add code to make sure we've got the correct state format
				if (!string.IsNullOrWhiteSpace(t.Customer.ShipRegionName))
				{
					parameters.Add("x_Ship_To_State", t.Customer.ShipRegionName);
				}

				parameters.Add("x_Ship_To_Zip", t.Customer.ShipPostalCode);
				parameters.Add("x_Ship_To_Phone", t.Customer.ShipPhone);
			}

			parameters.Add("x_Method", "CC");

			if (Settings.TestMode)
			{
				parameters.Add("x_test_request", WriteBool(true));
			}

			switch (t.Action)
			{
				case ActionType.CreditCardCharge:
					// Charge
					parameters.Add("x_Type", "AUTH_CAPTURE");
					parameters.Add("x_customer_ip", t.Customer.IpAddress);
					break;
				case ActionType.CreditCardHold:
					// Authorize
					parameters.Add("x_Type", "AUTH_ONLY");
					parameters.Add("x_customer_ip", t.Customer.IpAddress);
					break;
				case ActionType.CreditCardCapture:
					// Capture, Post Authorize
					parameters.Add("x_Type", "PRIOR_AUTH_CAPTURE");
					parameters.Add("x_trans_id", t.PreviousTransactionNumber);
					break;
				case ActionType.CreditCardVoid:
					// Void
					parameters.Add("x_Type", "VOID");
					parameters.Add("x_trans_id", t.PreviousTransactionNumber);
					break;
				case ActionType.CreditCardRefund:
					// Refund, Credit
					parameters.Add("x_Type", "CREDIT");
					parameters.Add("x_trans_id", t.PreviousTransactionNumber);
					break;
			}

			// Add Card Number, CVV Code and Expiration Date
			parameters.Add("x_Card_Num", t.Card.CardNumber);
			if (!string.IsNullOrEmpty(t.Card.SecurityCode))
			{
				parameters.Add("x_Card_Code", t.Card.SecurityCode);
			}
			string expDate = t.Card.ExpirationMonthPadded + t.Card.ExpirationYearTwoDigits;
			parameters.Add("x_Exp_Date", expDate);

			//Add Duplicate window (if not passed default is 2 min - 120 seconds). By submitting it, it will return original transaction id which doesnt come if ignored.
			//https://support.authorize.net/authkb/index?page=content&id=A425
			parameters.Add("x_duplicate_window", "120");

			return parameters;
		}

		private void ProcessResponse(Transaction t, string responseBody)
		{
			// Split response string
            var output = responseBody.Split(',');

			if (output.Length < 7)
			{
				t.Result.Succeeded = false;
			}
			else
			{
				//
				// the output parameters are described in the Authorize.Net documentation here:
				// http://www.authorize.net/support/merchant/wwhelp/wwhimpl/common/html/wwhelp.htm#context=Merchant_Integration_Guide&file=4_TransResponse.html
				//
				string responseCode = output[0];                // 1. response code
				string responseSubCode = output[1];             // 2. response sub code 
				string resonseReasonCode = output[2];           // 3. resonse reason code
				string responseDescription = output[3];         // 4. response reason
				string responseAuthCode = output[4];            // 5. authorization code
				string responseAVSCode = output[5];             // 6. AVS response
				string responseReferenceCode = output[6];       // 7. transaction id
				string responseSecurityCode = string.Empty;
				if (output.Length > 38)
				{
                    responseSecurityCode = output[38]; // 39. card code response
				}

				t.Result.AvsCode = ParseAvsCode(responseAVSCode);
				t.Result.CvvCode = ParseSecurityCode(responseSecurityCode);


				// Trim off Extra Quotes on response codes
				responseCode = responseCode.Trim('"');

				// Save result information to payment data object 
				t.Result.ResponseCode = responseCode;
				t.Result.ResponseCodeDescription = responseDescription;
				t.Result.ReferenceNumber = responseReferenceCode;

				switch (responseCode)
				{
					case "1":
						// Approved
						t.Result.Succeeded = true;
						break;
					case "2":
						// Declined
						t.Result.Succeeded = false;
						t.Result.Messages.Add(new Message("Declined: " + responseDescription, responseCode, MessageType.Warning));
						break;
					case "3":
						// UNKNOWN
						switch (resonseReasonCode)
						{
							case  "11":
								//Duplicate transaction : A transaction with identical amount and credit card information was submitted two minutes prior.
								//We can mark current transaction success to finish payment workflow considering order will either success/fail base on original transaction's status
								t.Action = ActionType.CreditCardIgnored;
								t.Result.Succeeded = true;
								t.Result.Messages.Add(new Message("Authorize.Net Error: " + responseDescription , responseCode, MessageType.Error));
								break;
							default:
								t.Result.Succeeded = false;
								t.Result.Messages.Add(new Message("Authorize.Net Error: " + responseDescription, responseCode, MessageType.Error));
								break;
						}
						break;
				}
			}
		}

		private string WriteBool(bool input)
		{
			return input ? "TRUE" : "FALSE";
		}

		private AvsResponseType ParseAvsCode(string code)
		{
            var result = AvsResponseType.Unavailable;

			switch (code.ToUpper())
			{
				case "A":
					result = AvsResponseType.PartialMatchAddress;
					break;
				case "B":
					result = AvsResponseType.Unavailable;
					break;
				case "E":
					result = AvsResponseType.Error;
					break;
				case "G":
					result = AvsResponseType.Unavailable;
					break;
				case "N":
					result = AvsResponseType.NoMatch;
					break;
				case "P":
					result = AvsResponseType.Unavailable;
					break;
				case "R":
					result = AvsResponseType.Unavailable;
					break;
				case "S":
					result = AvsResponseType.Unavailable;
					break;
				case "U":
					result = AvsResponseType.Unavailable;
					break;
				case "W":
					result = AvsResponseType.PartialMatchPostalCode;
					break;
				case "X":
					result = AvsResponseType.FullMatch;
					break;
				case "Y":
					result = AvsResponseType.FullMatch;
					break;
				case "Z":
					result = AvsResponseType.PartialMatchPostalCode;
					break;
			}

			return result;
		}

		private CvnResponseType ParseSecurityCode(string code)
		{
            var result = CvnResponseType.Unavailable;

			switch (code.ToUpper())
			{
				case "M":
					result = CvnResponseType.Match;
					break;
				case "N":
					result = CvnResponseType.NoMatch;
					break;
				case "P":
					result = CvnResponseType.Unavailable;
					break;
				case "S":
					result = CvnResponseType.Error;
					break;
				case "U":
					result = CvnResponseType.Unavailable;
					break;
			}
			return result;
		}

		#endregion
	}
}
