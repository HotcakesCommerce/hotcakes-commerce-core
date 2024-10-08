<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.Shipping.UPSFreight.Edit" CodeBehind="Edit.ascx.cs" %>

<h1>
    <asp:Label runat="server" resourcekey="EditShippingMethod"/>
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <label class="hcLabel"><%=Localization.GetString("Name") %><i class="hcLocalizable"></i></label>
        <asp:TextBox ID="NameField" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HighlightColor" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstHighlights" runat="server" ValidationGroup="ShippingMethod"/>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShippingZone" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstZones" runat="server" ValidationGroup="ShippingMethod"/>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AdjustPriceBy" CssClass="hcLabel"/>
        <asp:TextBox ID="AdjustmentTextBox" runat="server" ValidationGroup="ShippingMethod" />
        <asp:RequiredFieldValidator ID="rfvAdjustmentTextBox" runat="server" ControlToValidate="AdjustmentTextBox" CssClass="hcFormError" Display="Dynamic" ValidationGroup="ShippingMethod"/>
        <asp:CustomValidator ID="cvAdjustmentTextBox" runat="server" ControlToValidate="AdjustmentTextBox" CssClass="hcFormError" Display="Dynamic" OnServerValidate="cvAdjustmentTextBox_ServerValidate" ValidationGroup="ShippingMethod"/>
    </div>

    <div class="hcFormItemHor">
        <label class="hcLabel"></label>
        <asp:DropDownList ID="AdjustmentDropDownList" runat="server" ValidationGroup="ShippingMethod"/>
    </div>
    <asp:Panel ID="pnlFilter" runat="server" CssClass="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AvailableServices" CssClass="hcLabel"/>
        <asp:RadioButtonList ID="ShippingTypesRadioButtonList" runat="server" ValidationGroup="ShippingMethod"/>
    </asp:Panel>
   <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PayerShipmentBillingOption" CssClass="hcLabel"/>
        <asp:DropDownList ID="PayerShipmentBillingOptionDropDownList" runat="server" ValidationGroup="ShippingMethod"/>
    </div>
     <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HandlineOneUnitType" CssClass="hcLabel"/>
        <asp:DropDownList ID="HandlineOneUnitTypeDropDownList" runat="server" ValidationGroup="ShippingMethod"/>
    </div>
</div>

<h2>
    <asp:Label runat="server" resourcekey="UPSSettings"/>
</h2>
<div class="hcForm">
    <%--<div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="RegistrationStatus" CssClass="hcLabel"/>
        <asp:HyperLink ID="lnkRegister" Target="_blank" runat="server" NavigateUrl="~/DesktopModules/Hotcakes/Core/Admin/Configuration/ShippingUpsLicense.aspx"/>
    </div>--%>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AccountNumber" CssClass="hcLabel"/>
        <asp:TextBox ID="AccountNumberField" runat="server" ValidationGroup="ShippingMethod" />
        <asp:RequiredFieldValidator runat="server" ID="AccountNumberRequired" Text="* Account Number required" CssClass="hc-validation" ValidationGroup="ShippingMethod" ControlToValidate="AccountNumberField"></asp:RequiredFieldValidator>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AccessKey" CssClass="hcLabel"/>
        <asp:TextBox ID="AccessKeyField" runat="server" ValidationGroup="ShippingMethod" />
        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" Text="* Access Key required" CssClass="hc-validation" ValidationGroup="ShippingMethod" ControlToValidate="AccessKeyField"></asp:RequiredFieldValidator>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Username" CssClass="hcLabel"/>
        <asp:TextBox ID="UserNameField" runat="server" ValidationGroup="ShippingMethod" />
        <asp:RequiredFieldValidator runat="server" ID="UsernameRequired" Text="* Username required" CssClass="hc-validation" ValidationGroup="ShippingMethod" ControlToValidate="UserNameField"></asp:RequiredFieldValidator>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Password" CssClass="hcLabel"/>
        <asp:TextBox ID="PasswordField" runat="server" ValidationGroup="ShippingMethod" />
        <asp:RequiredFieldValidator runat="server" ID="PasswordRequired" Text="* Password required" CssClass="hc-validation" ValidationGroup="ShippingMethod" ControlToValidate="PasswordField"></asp:RequiredFieldValidator>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ForceResidentialAddresses" CssClass="hcLabel"/>
        <asp:CheckBox ID="ResidentialAddressCheckBox" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DontUseDimensions" CssClass="hcLabel"/>
        <asp:CheckBox ID="SkipDimensionsCheckBox" runat="server" ValidationGroup="ShippingMethod" />
    </div>
   <%-- <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PickupType" CssClass="hcLabel"/>
        <asp:RadioButtonList ID="PickupTypeRadioButtonList" runat="server" ValidationGroup="ShippingMethod"/>
    </div>--%>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DefaultService" CssClass="hcLabel"/>
        <asp:DropDownList runat="server" ID="DefaultServiceField" ValidationGroup="ShippingMethod"/>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DefaultPackaging" CssClass="hcLabel"/>
        <asp:DropDownList runat="server" ID="DefaultPackagingField" ValidationGroup="ShippingMethod"/>
    </div>
     <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DefaultFreightClass" CssClass="hcLabel"/>
        <asp:DropDownList runat="server" ID="DefaultFreightClassField" ValidationGroup="ShippingMethod"/>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DiagnosticsMode" CssClass="hcLabel"/>
        <asp:CheckBox ID="chkDiagnostics" runat="server" ValidationGroup="ShippingMethod" />
    </div>
</div>
<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" CssClass="hcPrimaryAction" OnClick="btnSave_Click" ValidationGroup="ShippingMethod" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CausesValidation="false" runat="server" CssClass="hcSecondaryAction" OnClick="btnCancel_Click" ValidationGroup="ShippingMethod" />
    </li>
</ul>