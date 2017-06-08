<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.Shipping.FedEx.Edit" CodeBehind="Edit.ascx.cs" %>
<%@ Register Src="../../../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="hcc" %>

<h1>
    <asp:Label runat="server" resourcekey="EditShippingMethod" />
</h1>

<div class="hcForm" style="width: 75%;">
    <div class="hcFormItemHor">
        <label class="hcLabel"><%=Localization.GetString("Name") %><i class="hcLocalizable"></i></label>
        <asp:TextBox ID="NameField" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HighlightColor" CssClass="hcLabel" />
        <asp:DropDownList ID="lstHighlights" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShippingZone" CssClass="hcLabel" />
        <asp:DropDownList ID="lstZones" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Service" CssClass="hcLabel" />
        <asp:DropDownList ID="lstServiceCode" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Packaging" CssClass="hcLabel" />
        <asp:DropDownList ID="lstPackaging" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="UseNegotiatedRates" CssClass="hcLabel" />
        <asp:CheckBox ID="chkNegotiatedRates" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
</div>

<h2>
    <asp:Label runat="server" resourcekey="Adjustments" />
</h2>
<div class="hcForm" style="width: 75%;">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AdjustPriceBy" CssClass="hcLabel" />
        <asp:TextBox ID="AdjustmentTextBox" runat="server" ValidationGroup="EditShippingMethod" />
        <asp:RequiredFieldValidator ID="rfvAdjustmentTextBox" runat="server" ControlToValidate="AdjustmentTextBox" Display="Dynamic" CssClass="hcFormError" ValidationGroup="EditShippingMethod" />
        <asp:CustomValidator ID="cvAdjustmentTextBox" runat="server" ControlToValidate="AdjustmentTextBox" Display="Dynamic" CssClass="hcFormError" ValidationGroup="EditShippingMethod" OnServerValidate="cvAdjustmentTextBox_ServerValidate" />
    </div>
    <div class="hcFormItemHor">
        <label class="hcLabel"></label>
        <asp:DropDownList ID="AdjustmentDropDownList" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
</div>

<h2>
    <asp:Label runat="server" resourcekey="GlobalSettings" />
</h2>
<div class="hcForm" style="width: 75%;">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="FedExSiteMessage" />: <a href="https://www.fedex.com/wpor/web/jsp/commonTC.jsp" target="_blank">https://www.fedex.com/wpor/web/jsp/commonTC.jsp</a>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Key" CssClass="hcLabel" />
        <asp:TextBox ID="KeyField" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Password" CssClass="hcLabel" />
        <asp:TextBox ID="PasswordField" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AccountNumber" CssClass="hcLabel" />
        <asp:TextBox ID="AccountNumberField" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="MeterNumber" CssClass="hcLabel" />
        <asp:TextBox ID="MeterNumberField" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DefaultPackaging" CssClass="hcLabel" />
        <asp:DropDownList ID="lstDefaultPackaging" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DropOffType" CssClass="hcLabel" />
        <asp:DropDownList ID="lstDropOffType" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ForceResidentialRates" CssClass="hcLabel" />
        <asp:CheckBox ID="chkResidential" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="UseDevelopmentServiceUrl" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDevelopmentUrl" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DiagnosticsMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDiagnostics" runat="server" ValidationGroup="EditShippingMethod" />
    </div>
</div>
<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" ValidationGroup="EditShippingMethod" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" OnClick="btnCancel_Click" ValidationGroup="EditShippingMethod" />
    </li>
</ul>


<h2>
    <asp:Label runat="server" resourcekey="TestRates" />
</h2>
<div class="hcForm">
    <div class="hcColumnLeft">
        <h3>
            <asp:Label runat="server" resourcekey="Source" />
        </h3>
        <div class="hcFormItem">
            <hcc:AddressEditor ID="SourceAddress" TabOrderOffSet="100" RequireFirstName="false" RequireLastName="false" RequireAddress="false" RequireCity="false" RequireCompany="false" RequirePostalCode="false" RequireRegion="false" runat="server" />
        </div>
    </div>
    <div class="hcColumnRight">
        <h3>
            <asp:Label runat="server" resourcekey="Destination" />
        </h3>
        <div class="hcFormItem">
            <hcc:AddressEditor ID="DestinationAddress" TabOrderOffSet="200" RequireFirstName="false" RequireLastName="false" RequireAddress="false" RequireCity="false" RequireCompany="false" RequirePostalCode="false" RequireRegion="false" runat="server" />
        </div>
    </div>
</div>
<div class="hcForm" style="width: 50%;">
    <div class="hcFormItem">
        <asp:Label runat="server" resourcekey="Package" CssClass="hcLabel" />
        <asp:DropDownList ID="lstServicesTest" runat="server" ValidationGroup="TextRates" />
    </div>
    <div class="hcFormItem">
        <asp:Label runat="server" resourcekey="Length" CssClass="hcLabel" />
        <asp:TextBox ID="TestLength" runat="server" value="1" ValidationGroup="TextRates" />
    </div>
    <div class="hcFormItem">
        <asp:Label runat="server" resourcekey="Width" CssClass="hcLabel" />
        <asp:TextBox ID="TestWidth" runat="server" value="1" ValidationGroup="TextRates" />
    </div>
    <div class="hcFormItem">
        <asp:Label runat="server" resourcekey="Height" CssClass="hcLabel" />
        <asp:TextBox ID="TestHeight" runat="server" value="1" ValidationGroup="TextRates" />
    </div>
    <div class="hcFormItem">
        <asp:Label runat="server" resourcekey="Weight" CssClass="hcLabel" />
        <asp:TextBox ID="TestWeight" runat="server" value="1" ValidationGroup="TextRates" />
    </div>
    <div class="hcFormItem">
        <asp:Literal ID="litTestOuput" runat="server" />
    </div>
</div>
<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnTest" resourcekey="btnTest" runat="server" OnClick="btnTest_Click" CssClass="hcSecondaryAction" ValidationGroup="TextRates" />
    </li>
</ul>
