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
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;
using Hotcakes.Shipping.USPostal;
using Hotcakes.Shipping.USPostal.v4;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class ShippingUSPSTester : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("USPSDomesticRatesTester");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LocalizeView();

                LoadServices();
                FromZipField.Text = HccApp.ContactServices.Addresses.FindStoreContactAddress().PostalCode;
            }
        }

        private void LoadServices()
        {
            var uspostal = AvailableServices.FindById(DomesticProvider.ServiceId, HccApp.CurrentStore);
            lstServiceTypes.DataSource = uspostal.ListAllServiceCodes();
            lstServiceTypes.DataTextField = "DisplayName";
            lstServiceTypes.DataValueField = "Code";
            lstServiceTypes.DataBind();
        }

        protected void btnGetRates_Click(object sender, EventArgs e)
        {
            pnlRates.Visible = true;

            var shipment = new Shipment
            {
                DestinationAddress = {PostalCode = ToZipField.Text.Trim()},
                SourceAddress = {PostalCode = FromZipField.Text.Trim()}
            };

            // box
            var item = new Shippable();

            var length = 0m;
            decimal.TryParse(LengthField.Text.Trim(), out length);
            var height = 0m;
            decimal.TryParse(HeightField.Text.Trim(), out height);
            var width = 0m;
            decimal.TryParse(WidthField.Text.Trim(), out width);
            var weightPounds = 0m;
            decimal.TryParse(WeightPoundsField.Text.Trim(), out weightPounds);
            var weightOunces = 0m;
            decimal.TryParse(WeightOuncesField.Text.Trim(), out weightOunces);

            item.BoxLength = length;
            item.BoxHeight = height;
            item.BoxWidth = width;
            item.BoxLengthType = LengthType.Inches;
            item.BoxWeight = weightPounds + Conversions.OuncesToDecimalPounds(weightOunces);
            item.BoxWeightType = WeightType.Pounds;
            item.QuantityOfItemsInBox = 1;

            shipment.Items.Add(item);

            // Global Settings
            var globalSettings = new USPostalServiceGlobalSettings
            {
                UserId = HccApp.CurrentStore.Settings.ShippingUSPostalUserId,
                DiagnosticsMode = true,
                IgnoreDimensions = false
            };

            // Settings
            var settings = new USPostalServiceSettings();

            var code = new ServiceCode
            {
                Code = lstServiceTypes.SelectedItem.Value,
                DisplayName = lstServiceTypes.SelectedItem.Text
            };
            
            var codes = new List<IServiceCode> {code};
            
            settings.ServiceCodeFilter = codes;

            var temp = -1;
            int.TryParse(lstPackagingType.SelectedItem.Value, out temp);

            settings.PackageType = (DomesticPackageType) temp;

            // Provider
            var logger = new TextLogger();
            var provider = new DomesticProvider(globalSettings, logger) {Settings = settings};

            var rates = provider.RateShipment(shipment);

            var sbRates = new StringBuilder();

            sbRates.Append("<ul>");
            foreach (var rate in rates)
            {
                sbRates.Append("<li>");
                sbRates.Append(rate.EstimatedCost.ToString("c"));
                sbRates.Append(" - ");
                sbRates.Append(rate.DisplayName);
                sbRates.Append("</li>");
            }
            sbRates.Append("</ul>");

            litRates.Text = sbRates.ToString();

            var sbMessages = new StringBuilder();

            sbMessages.Append("<ul>");
            foreach (var msg in provider.LatestMessages)
            {
                sbMessages.Append("<li>");
                switch (msg.MessageType)
                {
                    case ShippingServiceMessageType.Diagnostics:
                        sbMessages.Append(Localization.GetString("Diagnostics"));
                        break;
                    case ShippingServiceMessageType.Information:
                        sbMessages.Append(Localization.GetString("Info"));
                        break;
                    case ShippingServiceMessageType.Error:
                        sbMessages.Append(Localization.GetString("Error"));
                        break;
                }
                sbMessages.Append(":");

                sbMessages.Append(HttpUtility.HtmlEncode(string.Concat(msg.Description, " ", msg.Code)));
                sbMessages.Append("</li>");
            }
            sbMessages.Append("</ul>");

            litMessages.Text = sbMessages.ToString();

            litXml.Text = string.Empty;

            while (logger.Messages.Count > 0)
            {
                var tempXml = logger.Messages.Dequeue();
                tempXml = tempXml.Replace("\n", "<br />");
                tempXml = tempXml.Replace("\r", "<br />");
                tempXml = tempXml.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                litXml.Text += "<li>" + HttpUtility.HtmlEncode(tempXml) + "</li>";
            }
        }

        private void LocalizeView()
        {
            if (lstPackagingType.Items.Count == 0)
            {
                var separator = new ListItem
                {
                    Text = "-------------------",
                    Value = "-1"
                };
                separator.Attributes.Add("disabled", "disabled");

                lstPackagingType.Items.Add(new ListItem(Localization.GetString("AutoSelectedPackaging"), "-1"));
                lstPackagingType.Items.Add(separator);
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FirstClassLetter"), "100"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FirstClassLargeEnvelope"), "101"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FirstClassParcel"), "102"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FirstClassPostCard"), "103"));
                lstPackagingType.Items.Add(separator);
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateBox"), "1"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateBoxSmall"), "2"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateBoxMedium"), "3"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateBoxLarge"), "4"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateEnvelope"), "5"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateEnvelopePadded"), "50"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateEnvelopeLegal"), "51"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateEnvelopeSmall"), "52"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateEnvelopeWindow"), "53"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("FlatRateEnvelopeGiftCard"), "53"));
                lstPackagingType.Items.Add(separator);
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("Variable"), "0"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("Rectangular"), "6"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("NonRectangular"), "7"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("RegionalBoxRateA"), "200"));
                lstPackagingType.Items.Add(new ListItem(Localization.GetString("RegionalBoxRateB"), "201"));
            }
        }
    }
}