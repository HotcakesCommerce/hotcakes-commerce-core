<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.AddressEditor" CodeBehind="AddressEditor.ascx.cs" %>
<div class="hcForm">
    <div class="hcFormItem">
        <label class="hcLabel"><%=Localization.GetString("Country") %></label>
        <asp:DropDownList ID="lstCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCountry_SelectedIndexChanged"/>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("FirstName") %></label>
        <asp:TextBox ID="firstNameField" TabIndex="103" runat="server" />
        <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="firstNameField" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("LastName") %></label>
        <asp:TextBox ID="lastNameField" runat="server" />
        <asp:RequiredFieldValidator ID="valLastName" runat="server" ControlToValidate="lastNameField" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem" id="CompanyNameRow" runat="server">
        <label class="hcLabel"><%=Localization.GetString("Company") %></label>
        <asp:TextBox ID="CompanyField" runat="server" Width="100%" />
        <asp:RequiredFieldValidator ID="valCompany" runat="server" ControlToValidate="CompanyField" Enabled="False" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem">
        <label class="hcLabel"><%=Localization.GetString("Address") %></label>
        <asp:TextBox ID="address1Field" runat="server" Width="100%" />
        <asp:RequiredFieldValidator ID="valAddress" runat="server" ControlToValidate="address1Field" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem" id="divAddressLine2" runat="server"  >
        <asp:TextBox ID="address2Field" runat="server" Width="100%" />
    </div>
    <div class="hcFormItem" id="divAddressLine3" runat="server">
        <asp:TextBox ID="address3Field" runat="server" />
    </div>
    <div class="hcFormItem">
        <label class="hcLabel"><%=Localization.GetString("City") %></label>
        <asp:TextBox ID="cityField" runat="server" Width="100%"/>
        <asp:RequiredFieldValidator ID="valCity" runat="server" ControlToValidate="cityField" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("State") %></label>
        <asp:DropDownList ID="lstRegions" runat="server" CssClass="hcDropdownAlign" />
		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="regionField" runat="server" Visible="False"/>
        <asp:RequiredFieldValidator ID="valRegion" runat="server" ControlToValidate="lstRegions" Enabled="False" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("ZipCode") %></label>
        <asp:TextBox ID="postalCodeField" runat="server" />
        <asp:RequiredFieldValidator ID="valPostalCode" runat="server" ControlToValidate="postalCodeField" CssClass="hcFormError" />
    </div>
    <div class="hcFormItem" id="PhoneRow" runat="server">
        <label class="hcLabel"><%=Localization.GetString("PhoneNumber") %></label>
        <asp:TextBox ID="PhoneNumberField" runat="server" Width="100%" />
        <asp:RequiredFieldValidator ID="valPhone" runat="server" ControlToValidate="PhoneNumberField" Enabled="False" CssClass="hcFormError" />
    </div>
</div>
<asp:HiddenField ID="AddressBvin" runat="server" />
<asp:HiddenField ID="AddressTypeField" runat="server" />
<asp:HiddenField ID="StoreId" runat="server" />

<%if (CreateValidationInputs)
  {%>
<script type="text/javascript">
    (function ($) {
        var AddressValidationInputs = createAddressValidationInputs();
        AddressValidationInputs.init(
            "<%=FormSelector %>",
            "<%=ErrorMessageSelector %>",
            "#<%=lstCountry.ClientID %>",
            "#<%=firstNameField.ClientID %>",
            "#<%=lastNameField.ClientID %>",
            "#<%=CompanyField.ClientID %>",
            "#<%=address1Field.ClientID %>",
            "#<%=address2Field.ClientID %>",
            "#<%=cityField.ClientID %>",
            "#<%=lstRegions.ClientID %>",
            "#<%=postalCodeField.ClientID %>",
            "#<%=PhoneNumberField.ClientID %>"
        );
    })(jQuery);
</script>
<%}%>
