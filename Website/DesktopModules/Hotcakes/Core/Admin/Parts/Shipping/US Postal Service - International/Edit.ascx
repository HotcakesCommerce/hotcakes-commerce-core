<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="Hotcakes.Modules.Core.Modules.Shipping.US_Postal_Service___International.Edit" %>

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
        <asp:DropDownList ID="lstHighlights" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShippingZone" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstZones" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AdjustPriceBy" CssClass="hcLabel"/>
        <asp:TextBox ID="AdjustmentTextBox" runat="server" ValidationGroup="ShippingMethod" />
        <asp:RequiredFieldValidator ID="rfvAdjustmentTextBox" runat="server" CssClass="hcFormError" ControlToValidate="AdjustmentTextBox" ValidationGroup="ShippingMethod" />
        <asp:CustomValidator ID="cvAdjustmentTextBox" runat="server" ControlToValidate="AdjustmentTextBox" CssClass="hcFormError" Display="Dynamic" OnServerValidate="cvAdjustmentTextBox_ServerValidate" ValidationGroup="ShippingMethod"/>
    </div>
    <div class="hcFormItemHor">
        <label class="hcLabel"></label>
        <asp:DropDownList ID="AdjustmentDropDownList" runat="server" ValidationGroup="ShippingMethod"/>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Services" CssClass="hcLabel"/>
        <asp:CheckBoxList ID="ShippingTypesCheckBoxList" runat="server" ValidationGroup="ShippingMethod" />
    </div>
</div>

<h2>
    <asp:Label runat="server" resourcekey="GlobalSettings"/>
</h2>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="UserId" CssClass="hcLabel"/>
        <asp:TextBox ID="txtUserId" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DiagnosticsMode" CssClass="hcLabel"/>
        <asp:CheckBox ID="chbDiagnostics" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:HyperLink NavigateUrl="~/DesktopModules/Hotcakes/Core/Admin/Configuration/ShippingUSPSInternationalTester.aspx" resourcekey="TestEstimateInternationalRates" runat="server" Target="_blank"/>
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" ValidationGroup="ShippingMethod" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" OnClick="btnCancel_Click" ValidationGroup="ShippingMethod" />
    </li>
</ul>