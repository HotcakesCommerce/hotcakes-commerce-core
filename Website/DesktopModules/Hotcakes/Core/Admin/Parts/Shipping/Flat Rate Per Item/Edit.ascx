<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.Shipping.Flat_Rate_Per_Item.Edit" CodeBehind="Edit.ascx.cs" %>

<h1>
    <asp:Label runat="server" resourcekey="EditShippingMethod" />
</h1>
<div class="hcForm" style="width: 75%;">
    <div class="hcFormItemHor">
        <label class="hcLabel"><%=Localization.GetString("Name") %><i class="hcLocalizable"></i></label>
        <asp:TextBox ID="NameField" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HighlightColor" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstHighlights" runat="server"/>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PerItemCharge" CssClass="hcLabel"/>
        <asp:TextBox ID="AmountField" runat="server" MaxLength="10" />
		<asp:CustomValidator id="cvAmountField" runat="server"  OnServerValidate="TextValidate"  ControlToValidate="AmountField" CssClass="hcFormError" ></asp:CustomValidator>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShippingZone" CssClass="hcLabel"/>
        <asp:DropDownList ID="lstZones" runat="server"/>
    </div>
</div>
<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" CssClass="hcPrimaryAction" OnClick="btnSave_Click" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CausesValidation="false" runat="server" CssClass="hcSecondaryAction" OnClick="btnCancel_Click" />
    </li>
</ul>