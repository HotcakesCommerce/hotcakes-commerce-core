<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.Shipping.US_Postal_Service___Domestic.Edit" Codebehind="Edit.ascx.cs" %>

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
        <asp:DropDownList id="lstHighlights" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShippingZone" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstZones" runat="server" ValidationGroup="ShippingMethod" />
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
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Services" CssClass="hcLabel"/>
        <asp:CheckBoxList ID="ShippingTypesCheckBoxList" runat="server" ValidationGroup="ShippingMethod" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PackageType" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstPackageType" runat="server" ValidationGroup="ShippingMethod"/>
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
        <asp:HyperLink NavigateUrl="~/DesktopModules/Hotcakes/Core/Admin/Configuration/ShippingUSPSTester.aspx" resourcekey="TestEstimateRates" runat="server" Target="_blank"/>
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" onclick="btnSave_Click" ValidationGroup="ShippingMethod" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" onclick="btnCancel_Click" ValidationGroup="ShippingMethod" />
    </li>
</ul>