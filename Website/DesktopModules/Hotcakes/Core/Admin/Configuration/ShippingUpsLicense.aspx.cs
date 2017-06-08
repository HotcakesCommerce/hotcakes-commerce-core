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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping.Ups;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class ShippingUpsLicense : BaseAdminPage
    {
        private TextLogger _logger;

        protected TextLogger ExceptionLogger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new TextLogger();
                }

                return _logger;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("UPSOnlineToolsLicense");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                try
                {
                    LocalizeView();

                    LoadCountries();

                    var contactAddress = HccApp.ContactServices.Addresses.FindStoreContactAddress();

                    var country = HccApp.GlobalizationServices.Countries.Find(contactAddress.CountryBvin);

                    if (country != null)
                    {
                        inCountry.SelectedValue = country.IsoCode;
                    }

                    LoadStates();

                    inAddress1.Text = contactAddress.Line1;
                    inAddress2.Text = contactAddress.Line2;
                    inCity.Text = contactAddress.City;

                    var region = country.FindRegion(contactAddress.RegionBvin);

                    if (region != null)
                    {
                        inState.SelectedValue = region.Abbreviation;
                    }

                    inEmail.Text = HccApp.CurrentStore.Settings.MailServer.EmailForGeneral;
                    inPhone.Text = contactAddress.Phone;
                    inZip.Text = contactAddress.PostalCode;
                }
                catch (Exception Ex)
                {
                    ExceptionLogger.LogException(Ex);

                    msg.ShowException(Ex);
                }

                // Get License
                try
                {
                    var UPSReg = new Registration();
                    if (UPSReg.GetLicense(UPSService.UPSLIVESERVER))
                    {
                        lblLicense.Text = Server.HtmlEncode(UPSReg.License);
                        lblLicense.Text = lblLicense.Text.Replace(((char) 10).ToString(), "&nbsp;<br/>");
                    }
                    else
                    {
                        msg.ShowError(string.Concat(Localization.GetString("LicenseError"), ": ", UPSReg.ErrorMessage));
                    }
                }
                catch (Exception Exx)
                {
                    ExceptionLogger.LogException(Exx);
                    msg.ShowWarning(string.Concat(Exx.Message, Localization.GetString("LicenseError")));
                }
            }
        }

        protected void inCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStates();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Shipping_Methods.aspx", true);
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            if (rbContactYes.Checked == false & rbContactNo.Checked == false)
            {
                msg.ShowWarning(Localization.GetString("ContactWarning"));
            }
            else
            {
                try
                {
                    var UPSReg = new Registration();

                    UPSReg.Address1 = inAddress1.Text.Trim();
                    if (inAddress2.Text.Trim().Length > 0)
                    {
                        UPSReg.Address2 = inAddress2.Text.Trim();
                    }
                    if (inAddress3.Text.Trim().Length > 0)
                    {
                        UPSReg.Address3 = inAddress3.Text.Trim();
                    }
                    UPSReg.City = inCity.Text.Trim();
                    UPSReg.Company = inCompany.Text.Trim();
                    UPSReg.Title = inTitle.Text.Trim();
                    UPSReg.Country = inCountry.SelectedValue;
                    UPSReg.Email = inEmail.Text.Trim();
                    UPSReg.Name = inName.Text.Trim();
                    UPSReg.URL = inURL.Text.Trim();
                    UPSReg.Phone = inPhone.Text.Trim();
                    if (inCountry.SelectedValue == "US" || inCountry.SelectedValue == "CA")
                    {
                        UPSReg.State = inState.SelectedValue;
                        UPSReg.Zip = inZip.Text.Trim();
                    }
                    if (rbContactYes.Checked)
                    {
                        UPSReg.ContactMe = "yes";
                    }
                    else
                    {
                        UPSReg.ContactMe = "no";
                    }
                    UPSReg.AccountNumber = inUPSAccountNumber.Text.Trim();

                    var sTempLicense = lblLicense.Text;
                    sTempLicense = sTempLicense.Replace("&nbsp;<br/>", ((char) 10).ToString());
                    sTempLicense = Server.HtmlDecode(sTempLicense);
                    
                    UPSReg.License = sTempLicense;


                    if (UPSReg.AcceptLicense(UPSService.UPSLIVESERVER))
                    {
                        HccApp.CurrentStore.Settings.ShippingUpsLicense = UPSReg.LicenseNumber;

                        // Complete Registration process here...

                        var tempUsername = "hcc";
                        if (inPhone.Text.Trim().Length > 3)
                        {
                            tempUsername += inPhone.Text.Trim().Substring(inPhone.Text.Trim().Length - 4, 4);
                        }
                        UPSReg.Password = PasswordGenerator.GeneratePassword(10);
                        if (UPSReg.Password.Length > 10)
                        {
                            UPSReg.Password = UPSReg.Password.Substring(0, 10);
                        }

                        var RegistrationComplete = false;
                        var MaxRegistrationAttempts = 10;
                        var CurrentAttempts = 0;


                        while (true)
                        {
                            CurrentAttempts += 1;
                            if (RegistrationComplete | CurrentAttempts > MaxRegistrationAttempts)
                            {
                                break;
                            }
                            UPSReg.Username = tempUsername;

                            UPSReg.RequestSuggestedUsername = true;
                            if (UPSReg.Register(UPSService.UPSLIVESERVER))
                            {
                                // Got Suggested Username
                                UPSReg.Username = UPSReg.SuggestedUsername;

                                // Now attempt actual registration
                                UPSReg.RequestSuggestedUsername = false;
                                if (UPSReg.Register(UPSService.UPSLIVESERVER))
                                {
                                    HccApp.CurrentStore.Settings.ShippingUpsUsername = UPSReg.Username;
                                    HccApp.CurrentStore.Settings.ShippingUpsPassword = UPSReg.Password;
                                    HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
                                    RegistrationComplete = true;
                                    break;
                                }
                            }

                            UPSReg.RequestSuggestedUsername = false;
                        }

                        if (RegistrationComplete)
                        {
                            Response.Redirect("ShippingUpsThanks.aspx");
                        }
                        else
                        {
                            msg.ShowError(Localization.GetString("RegistrationIncompleteError"));
                        }
                    }
                    else
                    {
                        msg.ShowError(string.Concat(UPSReg.ErrorMessage, "<br/>", Localization.GetString("ErrorCode"),
                            ":", UPSReg.ErrorCode));
                    }
                }
                catch (Exception Ex)
                {
                    ExceptionLogger.LogException(Ex);
                    msg.ShowException(Ex);
                }
            }

            HccApp.UpdateCurrentStore();
        }

        private void LoadCountries()
        {
            inCountry.DataSource = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            inCountry.DataTextField = "DisplayName";
            inCountry.DataValueField = "IsoCode";
            inCountry.DataBind();
        }

        private void LoadStates()
        {
            try
            {
                var isoCode = inCountry.SelectedValue;
                var selectedCountry = HccApp.GlobalizationServices.Countries.FindByISOCode(isoCode);

                inState.DataSource = selectedCountry.Regions;
                inState.DataTextField = "DisplayName";
                inState.DataValueField = "Abbreviation";
                inState.DataBind();

                var li = new ListItem(Localization.GetString("NotSelected"), string.Empty);
                inState.Items.Insert(0, li);
            }
            catch (Exception Ex)
            {
                ExceptionLogger.LogException(Ex);
                msg.ShowException(Ex);
            }
        }

        private void LocalizeView()
        {
            rfvPhoneNumber.ErrorMessage = Localization.GetString("rfvPhoneNumber.ErrorMessage");
            rfvEmail.ErrorMessage = Localization.GetString("rfvEmail.ErrorMessage");
        }
    }
}