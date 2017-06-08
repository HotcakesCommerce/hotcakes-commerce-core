using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Hotcakes.Payment.Gateways
{
	[Serializable]
	public class Shift4 : PaymentGateway
	{
		public override string Name
		{
			get { return "Shift4"; }
		}
		public override string Id
		{
			get { return "067FAE9E-3F15-4917-960B-D0B9B4420086"; }
		}

		public Shift4Settings Settings { get; set; }

		public override MethodSettings BaseSettings
		{
			get { return Settings; }
		}

		public Shift4()
		{
			Settings = new Shift4Settings();
		}


		public override void ProcessTransaction(Transaction t)
		{
			try
			{
				switch (t.Action)
				{
					case ActionType.CreditCardCapture:
						Capture(t);
						break;
					case ActionType.CreditCardCharge:
						Charge(t);
						break;
					case ActionType.CreditCardHold:
						Authorize(t);
						break;
					case ActionType.CreditCardRefund:
						Refund(t);
						break;
					case ActionType.CreditCardVoid:
						Void(t);
						break;
					case ActionType.GiftCardBalanceInquiry:
						GiftCardBalanceCheck(t);
						break;
					case ActionType.GiftCardCapture:
						GiftCardCapture(t);
						break;
					case ActionType.GiftCardDecrease:
						GiftCardCharge(t);
						break;
					case ActionType.GiftCardHold:
						GiftCardAuth(t);
						break;
					case ActionType.GiftCardIncrease:
						GiftCardAddValue(t);
						break;
					default:
						t.Result.Succeeded = false;
						t.Result.Messages.Add(new Message("Unsupported action for Shift4 Processor", "HCP", MessageType.Error));
						break;
				}
			}
			catch (Exception ex)
			{
				t.Result.Succeeded = false;
				t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP", MessageType.Error));
				t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
			}
		}

		protected override void Authorize(Transaction transaction)
		{
			bool result = ProcessCard("A", transaction);
			if (!result)
			{
				VoidLastTransaction(transaction);
			}
		}
		protected override void Capture(Transaction transaction)
		{
			bool result = ProcessCard("P", transaction);
			if (!result)
			{
				VoidLastTransaction(transaction);
			}
		}
		protected override void Charge(Transaction transaction)
		{
			bool result = ProcessCard("S", transaction);
			if (!result)
			{
				VoidLastTransaction(transaction);
			}
		}
		protected override void Refund(Transaction transaction)
		{
			bool result = ProcessCard("C", transaction);

			if (!result)
			{
				VoidLastTransaction(transaction);
			}
		}
		protected override void Void(Transaction transaction)
		{
			ProcessCard("V", transaction);
		}

		public bool GiftCardAuth(Transaction transaction)
		{
			bool result = ProcessCard("GA", transaction);

			//If False = result Then
			//    VoidLastTransaction(transaction)
			//End If

			return result;
		}

		public bool GiftCardCapture(Transaction transaction)
		{
			bool result = ProcessCard("GP", transaction);

			//If False = result Then
			//    VoidLastTransaction(transaction)
			//End If

			return result;
		}

		public bool GiftCardCharge(Transaction transaction)
		{
			bool result = ProcessCard("GS", transaction);

			//If False = result Then
			//    VoidLastTransaction(transaction)
			//End If

			return result;
		}
		public bool GiftCardBalanceCheck(Transaction transaction)
		{
			bool result = ProcessCard("BAL", transaction);
			return result;
		}
		public bool GiftCardAddValue(Transaction transaction)
		{
			bool result = ProcessCard("ADDVAL", transaction);

			//If False = result Then
			//    VoidLastTransaction(transaction)
			//End If

			return result;
		}

		private bool ProcessCard(string chargeType, Transaction transaction)
		{
			try
			{
				// Get HTTP Connection Location
				bool utgSecure = Settings.UtgSecure;
				string utgIp = Settings.UtgIp;
				string utgPort = Settings.UtgPort;
				if (string.IsNullOrEmpty(utgIp))
					utgIp = "127.0.0.1";
				if (string.IsNullOrEmpty(utgPort))
					utgPort = "295";

				string protocol = utgSecure ? "https" : "http";
				string serviceUrl = string.Format("{0}://{1}:{2}", protocol, utgIp, utgPort);

				using (WebClient client = new WebClient())
				{
					NameValueCollection parameters = BuildRequestData(transaction, chargeType);

					byte[] responseBytes = client.UploadValues(serviceUrl, "POST", parameters);
					string responseBody = Encoding.UTF8.GetString(responseBytes);

					if (Settings.DebugMode)
						transaction.Result.Messages.Add(new Message(HttpUtility.UrlDecode(responseBody), "DEBUG", MessageType.Information));

					bool result = ParseResponse(transaction, chargeType, responseBody);
					transaction.Result.Succeeded = result;
					return result;
				}
			}
			catch (Exception ex)
			{
				transaction.Result.Succeeded = false;
				transaction.Result.ResponseCodeDescription = "An Unknown Payment Error Occred: " + ex.Message + " " + ex.StackTrace;
				transaction.Result.Messages.Add(new Message(transaction.Result.ResponseCodeDescription, "ex", MessageType.Error));

				return false;
			}
		}

		private NameValueCollection BuildRequestData(Transaction transaction, string chargeType)
		{
			NameValueCollection parameters = new NameValueCollection();
			parameters.Add("STX", "yes");

			parameters.Add("Vendor", Settings.Vendor);
			parameters.Add("APIPassword", Settings.Password);
			parameters.Add("APISerialNumber", Settings.SerialNumber);
			parameters.Add("MerchantID", Settings.MerchantId);

			parameters.Add("Username", Settings.Username);
			string orderNumber = transaction.MerchantInvoiceNumber;

			// SHIFT4 INVOICE NUMBER
			// This is different than order number but may be the same as the Order Number
			// For new transactions like Auth, Charge, Refund a unique time is added to Order Number
			// Limited to 10 digits max by Shift4. Must be numeric only.
			if (NeedsUniqueId(chargeType))
			{
				// Generate a shift4 unique invoice number and save to custom property
				if (orderNumber.Length > 5)
				{
					// Trim off first couple of digits to limit to 5 max
					orderNumber = orderNumber.Substring(orderNumber.Length - 5, 5);
				}
				transaction.SetAdditionalSetting("shift4invoicenumber", orderNumber + CompressCurrentTimeTo5Digits().ToString());

				parameters.Add("invoice", orderNumber + CompressCurrentTimeTo5Digits().ToString());
			}

			parameters.Add("CustomerReference", orderNumber);

			parameters.Add("TaxIndicator", "N");

			var storeShipZip = transaction.Customer.ShipPostalCode;
			if (storeShipZip != null)
				parameters.Add("DestinationZipCode", storeShipZip);

			parameters.Add("primaryamount", Math.Round(transaction.Amount, 2).ToString());

			if (HttpContext.Current != null && HttpContext.Current.Request != null)
				parameters.Add("customeripaddress", HttpContext.Current.Request.UserHostAddress);

			parameters.Add("customeremailaddress", transaction.Customer.Email);

			DateTime now = DateTime.UtcNow;
			parameters.Add("Date", now.ToString("MMDDYY"));
			parameters.Add("Time", now.ToString("HHmmSS"));

			parameters.Add("Notes", "Order Number: " + orderNumber);

			parameters.Add("ApiOptions", "ALLDATA");
			parameters.Add("ReceiptTextColumns", "40");
			parameters.Add("ErrorIndicator", "N");

			string uniqueId = transaction.GetAdditionalSettingAsString("UniqueID");

			switch (chargeType)
			{
				case "S":
				case "GS":
					// Sale / Charge
					parameters.Add("FunctionRequestCode", "1D");
					parameters.Add("SaleFlag", "S");

					AddProductDescriptors(transaction, parameters);
					if (!string.IsNullOrWhiteSpace(uniqueId))
					{
						AddUniqueIdParameters(transaction, parameters);
					}
					else
					{
						if (chargeType == "GS")
							AddGCParameters(transaction, parameters);
						else
							AddCCParameters(transaction, parameters);
					}

					AddAVSParameters(transaction, parameters);

					break;
				case "A":
				case "GA":
					// Pre Auth
					parameters.Add("FunctionRequestCode", "1B");
					parameters.Add("SaleFlag", "S");

					AddProductDescriptors(transaction, parameters);
					if (!string.IsNullOrWhiteSpace(uniqueId))
					{
						AddUniqueIdParameters(transaction, parameters);
					}
					else
					{
						if (chargeType == "GA")
							AddGCParameters(transaction, parameters);
						else
							AddCCParameters(transaction, parameters);
					}
					AddAVSParameters(transaction, parameters);

					break;
				case "P":
				case "GP":
					// Post Auth
					parameters.Add("FunctionRequestCode", "1D");
					parameters.Add("SaleFlag", "S");

					AddProductDescriptors(transaction, parameters);
					if (!string.IsNullOrWhiteSpace(uniqueId))
					{
						AddUniqueIdParameters(transaction, parameters);
					}
					else
					{
						if (chargeType == "GP")
							AddGCParameters(transaction, parameters);
						else
							AddCCParameters(transaction, parameters);
					}
					//parameters.Add("TranID", data.Payment.TransactionReferenceNumber);
					break;
				case "V":
					// Void Transaction
					parameters.Add("FunctionRequestCode", "08");
					//parameters.Add("SaleFlag", "C");
					//parameters.Add("TranID", data.Payment.TransactionReferenceNumber);

					if (!string.IsNullOrWhiteSpace(uniqueId))
					{
						AddUniqueIdParameters(transaction, parameters);
					}
					else
					{
						if (transaction.Card.CardNumber.Length > 4)
						{
							AddCC4Parameters(transaction, parameters);
						}
					}

					break;
				case "C":
					// Credit
					parameters.Add("FunctionRequestCode", "1D");
					parameters.Add("SaleFlag", "C");
					//parameters.Add("TranID", data.Payment.TransactionReferenceNumber);

					AddProductDescriptors(transaction, parameters);

					if (!string.IsNullOrWhiteSpace(uniqueId))
					{
						AddUniqueIdParameters(transaction, parameters);
					}
					else
					{
						AddCCParameters(transaction, parameters);
					}
					break;
				case "ADDVAL":
					// Gift Card Add Value
					parameters.Add("FunctionRequestCode", "1D");
					parameters.Add("SaleFlag", "C");

					if (!string.IsNullOrWhiteSpace(uniqueId))
					{
						AddUniqueIdParameters(transaction, parameters);
					}
					else
					{
						AddGCParameters(transaction, parameters);
					}

					break;
				case "BAL":
					// Gift Card Balance Request
					parameters.Add("FunctionRequestCode", "61");
					parameters.Add("SaleFlag", "S");
					
					AddGCParameters(transaction, parameters);
					break;
			}

			parameters.Add("VERBOSE", "yes");
			parameters.Add("ETX", "yes");

			return parameters;
		}

		private static void AddAVSParameters(Transaction transaction, NameValueCollection parameters)
		{
			parameters.Add("CustomerName", transaction.Customer.FirstName + " " + transaction.Customer.LastName);
			parameters.Add("StreetAddress", transaction.Customer.Street);
			parameters.Add("ZipCode", transaction.Customer.PostalCode);
		}

		private static void AddCCParameters(Transaction transaction, NameValueCollection parameters)
		{
			parameters.Add("CardNumber", transaction.Card.CardNumber);
			parameters.Add("ExpirationMonth", transaction.Card.ExpirationMonthPadded);
			parameters.Add("ExpirationYear", transaction.Card.ExpirationYearTwoDigits);
			parameters.Add("CardType", "");
			parameters.Add("CardEntryMode", "M");
			parameters.Add("CardPresent", "N");

			if (transaction.Card.SecurityCode.Length > 0)
			{
				parameters.Add("CVV2Indicator", "1");
				parameters.Add("CVV2Code", transaction.Card.SecurityCode);
			}
			else
			{
				parameters.Add("CVV2Indicator", "0");
			}
		}

		private static void AddGCParameters(Transaction transaction, NameValueCollection parameters)
		{
			parameters.Add("CardNumber", transaction.GiftCard.CardNumber);
			parameters.Add("IYCCardType", "G");
			parameters.Add("IYCExpirationDate", "0150");
			parameters.Add("CardEntryMode", "M");
			parameters.Add("CardPresent", "N");
		}

		private static void AddCC4Parameters(Transaction transaction, NameValueCollection parameters)
		{
			if (transaction.Card.CardNumber.Length > 4)
			{
				parameters.Add("CardNumber", transaction.Card.CardNumber.Substring(transaction.Card.CardNumber.Length - 4, 4));
				parameters.Add("CardType", "");
				parameters.Add("CardEntryMode", "M");
				parameters.Add("CardPresent", "N");
			}
		}

		private static void AddUniqueIdParameters(Transaction transaction, NameValueCollection parameters)
		{
			string uniqueId = transaction.GetAdditionalSettingAsString("UniqueID");
			parameters.Add("UniqueID", uniqueId);
			parameters.Add("CardType", "");
			parameters.Add("CardEntryMode", "M");
			parameters.Add("CardPresent", "N");
		}
		private void AddProductDescriptors(Transaction transaction, NameValueCollection parameters)
		{
			if (transaction.Items != null)
			{
				int itemCount = transaction.Items.Count;
				var transactionItems = transaction.Items;
				if (itemCount > 4)
				{
					itemCount = 4;
					transactionItems = transaction.Items.OrderByDescending(y => y.LineTotal).ToList();
				}
				for (int i = 0; i < itemCount; i++)
				{
					var description = Hotcakes.Web.Text.TrimToLength(transactionItems[i].Description, 40);
					parameters.Add("ProductDescriptor" + (i + 1), description);
				}
			}
		}


		private bool ParseResponse(Transaction transaction, string chargeType, string sResponse)
		{
			bool result = false;
			// Split response string
			string[] responseLines = sResponse.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			string respOverall = FindLine(1, responseLines);
			if (respOverall != "OK")
			{
				transaction.Result.Messages.Add(new Message("Transaction Failure Occured!", "FAIL", MessageType.Error));
				return false;
			}

			bool HasErrors = FindBool("ErrorIndicator", responseLines);
			string PrimaryErrorCode = FindResponse("PrimaryErrorCode", responseLines);
			string SecondaryErrorCode = FindResponse("SecondaryErrorCode", responseLines);
			string TransactionReference = FindResponse("TranID", responseLines);
			string UniqueIdResponse = FindResponse("UniqueID", responseLines);
			string ResponseCode = FindResponse("Response", responseLines);

			string AVSCode = FindResponse("AVSResult", responseLines);
			string CVVCode = FindResponse("CVV2Result", responseLines);

			// Store Timeout Data to cancel future voids
			bool timeout = IsTimeOutError(PrimaryErrorCode);
			if (timeout)
			{
				transaction.SetAdditionalSetting("shift4timeout", "1");
			}
			else
			{
				transaction.SetAdditionalSetting("shift4timeout", "0");
			}

			// Store Receipt Text
			string ReceiptText = FindResponse("ReceiptText", responseLines);
			string Shift4InvoiceNumber = FindResponse("Invoice", responseLines);
			string Shift4Authorization = FindResponse("Authorization", responseLines);
			string Shift4TranId = FindResponse("TranId", responseLines);

			if (chargeType != "V")
			{
				transaction.SetAdditionalSetting("receipttext", ReceiptText);
				transaction.SetAdditionalSetting("shift4invoicenumber", Shift4InvoiceNumber);
				transaction.SetAdditionalSetting("shift4authorization", Shift4Authorization);
				transaction.SetAdditionalSetting("shift4tranid", Shift4TranId);
				transaction.SetAdditionalSetting("AVS", AVSCode);
			}

			if (!string.IsNullOrWhiteSpace(UniqueIdResponse))
			{
				transaction.SetAdditionalSetting("UniqueID", UniqueIdResponse);
			}

			switch (ResponseCode)
			{
				case "A":
				case "C":
					// Approved
					result = true;
					transaction.Result.ResponseCode = ResponseCode;
					transaction.Result.ReferenceNumber = TransactionReference;
					transaction.Result.ResponseCodeDescription = "Approved";

					break;
				case "D":
				case "f":
				case "R":
				case "X":
					// Declined
					result = false;
					transaction.Result.ResponseCode = ResponseCode;
					if (chargeType == "P" || chargeType == "C" || chargeType == "V")
					{
						// Failed on post auth, capture or void so don't overwrite original transaction id
						// do nothing
					}
					else
					{
						transaction.Result.ReferenceNumber = TransactionReference;
					}

					transaction.Result.ResponseCodeDescription = "Transaction Declined: ";
					if (ResponseCode == "f")
					{
						transaction.Result.ResponseCodeDescription += " AVS or CVV Error! ";
					}
					transaction.Result.Messages.Add(new Message(transaction.Result.ResponseCodeDescription, "MSG", MessageType.Warning));

					break;
				case "e":
					result = false;
					transaction.Result.ResponseCodeDescription = "Processing Error. Please Try Again or Contact Store Owner";
					transaction.Result.Messages.Add(new Message(transaction.Result.ResponseCodeDescription, "ERROR", MessageType.Error));

					if (chargeType == "P" || chargeType == "C" || chargeType == "V")
					{
						// Failed on post auth, capture or void so don't overwrite original transaction id
						// do nothing
					}
					else
					{
						transaction.Result.ReferenceNumber = TransactionReference;
					}

					break;
				case "O":
				case "0":
					// Network Timeout
					result = false;
					transaction.Result.ResponseCodeDescription = "Shift4 Network Timeout Error:";

					// Send true response to customer too
					transaction.Result.Messages.Add(new Message("Network Timeout while Processing. Please Try Again or Contact Store Owner", "FAIL", MessageType.Error));

					break;

				default:
					if (chargeType == "V" && !HasErrors)
					{
						result = true;
						transaction.Result.ResponseCodeDescription = "Transaction Voided.";
					}
					else
					{
						result = false;
						transaction.Result.Messages.Add(new Message("", "ERR", MessageType.Error));
						transaction.Result.ResponseCodeDescription = "Unknown Shift4 Response Code. Unable to complete Transaction.";

						// Send true response to customer too
						transaction.Result.Messages.Add(new Message("Unknown Response Code. Unable to Process Credit Card. Please Contact Store Owner", "ERR", MessageType.Error));
					}
					break;
			}

			// Save Available Balance to Amount for Gift Card Testing
			if (chargeType == "BAL" && !HasErrors)
			{
				string reportedAmount = FindResponse("IYCAvailableBalance", responseLines);

				decimal amount;
				if (decimal.TryParse(reportedAmount, out amount))
					transaction.Amount = amount;
			}


			bool forceVoid = false;

			// AVS
			switch (AVSCode)
			{
				case "A":
					transaction.Result.ResponseCodeDescription += " [AVS - Address (Street) matches, ZIP does not]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address (Street) matches, ZIP does not]", "AVS", MessageType.Error));
					forceVoid = true;
					break;
				case "B":
					transaction.Result.ResponseCodeDescription += " [AVS - Address information not provided for AVS check]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address information not provided for AVS check]", "AVS", MessageType.Error));
					break;
				//forceVoid = true
				case "E":
					transaction.Result.ResponseCodeDescription += " [AVS - Error]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address Verification Error]", "AVS", MessageType.Error));
					forceVoid = true;
					break;
				case "G":
					transaction.Result.ResponseCodeDescription += " [AVS - Card Issuer Does Not Support AVS]";
					transaction.Result.Messages.Add(new Message(" [AVS - Card Issuer Does Not Support Address Verification]", "AVS", MessageType.Error));
					break;
				//forceVoid = true
				case "N":
					transaction.Result.ResponseCodeDescription += " [AVS - No Match on Address (Street) or ZIP]";
					transaction.Result.Messages.Add(new Message(" [AVS - No Match on Address (Street) or ZIP]", "AVS", MessageType.Error));
					forceVoid = true;
					break;
				case "P":
					transaction.Result.ResponseCodeDescription += " [AVS - AVS not applicable for this transaction]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address Verification not Applicable]", "AVS", MessageType.Error));
					break;
				//forceVoid = true
				case "R":
					transaction.Result.ResponseCodeDescription += " [AVS - Retry – System unavailable or timed out]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address Verification Timeout or Unavailable]", "AVS", MessageType.Error));
					forceVoid = true;
					break;
				case "S":
					transaction.Result.ResponseCodeDescription += " [AVS - Service not supported by issuer]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address Verification Service Not Support]", "AVS", MessageType.Error));
					break;
				//forceVoid = true
				case "U":
					transaction.Result.ResponseCodeDescription += " [AVS - Address information is unavailable]";
					transaction.Result.Messages.Add(new Message(" [AVS - Address Couldn't be Verified]", "AVS", MessageType.Error));
					break;
				//forceVoid = true
				case "W":
					transaction.Result.ResponseCodeDescription += " [AVS - 9 digit ZIP matches, Address (Street) does not]";
					break;
				case "X":
					transaction.Result.ResponseCodeDescription += " [AVS - Address (Street) and 9 digit ZIP match]";
					break;
				case "Y":
					transaction.Result.ResponseCodeDescription += " [AVS - Address (Street) and 5 digit ZIP match]";
					break;
				case "Z":
					transaction.Result.ResponseCodeDescription += " [AVS - 5 digit ZIP matches, Address (Street) does not]";
					break;
			}

			// CVV
			switch (CVVCode)
			{
				case "M":
					transaction.Result.ResponseCodeDescription += " [CVV - Match]";
					break;
				case "N":
					transaction.Result.ResponseCodeDescription += " [CVV - No Match]";
					transaction.Result.Messages.Add(new Message(" [CVV - Security Code Incorrect]", "CVV", MessageType.Error));
					forceVoid = true;
					break;
				case "P":
					transaction.Result.ResponseCodeDescription += " [CVV - Not Processed]";
					transaction.Result.Messages.Add(new Message(" [CVV - Security Code Not Processed]", "CVV", MessageType.Error));
					forceVoid = true;
					break;
				case "S":
					transaction.Result.ResponseCodeDescription += " [CVV - Should have been present]";
					transaction.Result.Messages.Add(new Message(" [CVV - Security Code Missing]", "CVV", MessageType.Error));
					forceVoid = true;
					break;
				case "U":
					transaction.Result.ResponseCodeDescription += " [CVV - Issuer unable to process request]";
					transaction.Result.Messages.Add(new Message(" [CVV - Unable to process security code]", "CVV", MessageType.Error));
					forceVoid = true;
					break;
			}


			// Force void request on CVV or AVS failure
			if (chargeType == "A" && result && forceVoid)
			{
				result = false;
				transaction.Result.ResponseCodeDescription += "Requesting transaction void because of failed AVS or CVV";

			}
			return result;
		}


		private bool NeedsUniqueId(string chargeType)
		{
			bool result = false;


			switch (chargeType)
			{
				case "S":
				case "GS":
					// Sale / Charge
					result = true;
					break;
				case "A":
				case "GA":
					// Pre Auth
					result = true;
					break;
				case "P":
				case "GP":
					// Added GP on 2012-10-24 - Marcus
					// Post Auth
					result = false;
					break;
				case "V":
					// Void Transaction
					result = false;
					break;
				case "C":
					// Credit     
					result = true;
					break;
				case "ADDVAL":
					// Gift Card Add Value
					result = true;
					break;
				case "BAL":
					// Gift Card Balance Request
					result = true;
					break;
			}

			return result;
		}

		private bool IsTimeOutError(string errorCode)
		{
			bool result = false;

			switch (errorCode)
			{
				case "1001":
				case "9012":
				case "9018":
				case "9020":
				case "9023":
				case "9033":
				case "9489":
				case "9901":
				case "9902":
				case "9951":
				case "9957":
				case "9960":
				case "9961":
				case "9962":
				case "9964":
				case "9978":
				case "4003":
					result = true;
					break;
			}

			//2:              XXX()
			//For 2XXX, XXX can vary from 000 to 999.   
			if (result == false)
			{
				int code = 0;
				int.TryParse(errorCode, out code);
				if (code >= 2000 & code <= 2999)
				{
					result = true;
				}
			}

			return result;
		}

		private string FindResponse(string paramName, string[] data)
		{
			string result = string.Empty;

			if (data != null)
			{
				foreach (string s in data)
				{
					string[] parts = s.Split('=');
					if (parts.Length > 1)
					{
						if (parts[0].Trim().ToLower() == paramName.Trim().ToLower())
						{
							return parts[1];
						}
					}
				}
			}

			return result;
		}

		private bool FindBool(string paramName, string[] data)
		{
			bool result = false;

			string temp = FindResponse(paramName, data);
			if (temp.ToUpperInvariant() == "Y")
			{
				result = true;
			}

			return result;
		}

		private string FindLine(int lineNumber, string[] data)
		{
			string result = string.Empty;

			if (data != null)
			{
				if (data.Length > 0)
				{
					if (data.Length >= lineNumber)
					{
						result = data[lineNumber - 1];
					}
				}
			}

			return result;
		}

		private void VoidLastTransaction(Transaction transaction)
		{
			string timeout = transaction.GetAdditionalSettingAsString("shift4timeout");
			if (timeout == "1")
				return;

			Transaction voidData = new Transaction();
			voidData.Amount = 0;
			voidData.PreviousTransactionNumber = transaction.Id.ToString();
			voidData.Result.ResponseCodeDescription = string.Empty;

			string uniqueId = transaction.GetAdditionalSettingAsString("UniqueID");
			voidData.SetAdditionalSetting("UniqueID", uniqueId);

			voidData.MerchantInvoiceNumber = transaction.GetAdditionalSettingAsString("shift4invoicenumber");
			if (voidData.MerchantInvoiceNumber.Trim().Length < 1)
			{
				voidData.MerchantInvoiceNumber = transaction.MerchantInvoiceNumber;
			}

			ProcessCard("V", voidData);

			Message m = new Message();
			m.Code = "VOIDDATA";
			m.Description = "Void Transaction Result was " + voidData.Result.Succeeded;
			transaction.Result.Messages.Add(m);
			if (voidData.Result.Messages != null)
			{
				if (voidData.Result.Messages.Count > 0)
				{
					transaction.Result.Messages.AddRange(voidData.Result.Messages);
				}
			}
		}

		private static int CompressCurrentTimeTo5Digits()
		{
			DateTime time = DateTime.UtcNow;

			int result = (time.Hour * 60 * 60) + (time.Minute * 60) + time.Second;

			return result;
		}

	}
}
