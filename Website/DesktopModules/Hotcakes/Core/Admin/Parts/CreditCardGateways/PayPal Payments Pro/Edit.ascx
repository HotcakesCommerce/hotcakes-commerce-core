<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.CreditCardGateways.PayPal_Payments_Pro.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="PayPalOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="lblClientId" CssClass="hcLabel" />
        <asp:TextBox ID="txtClientId" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="lblSecret" CssClass="hcLabel" />
        <asp:TextBox ID="txtSecret" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Currency" CssClass="hcLabel" />
        <asp:DropDownList ID="ddlCurrency" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PaypalMode" CssClass="hcLabel" />
        <asp:RadioButtonList runat="server" ID="rblMode">
            <asp:ListItem resourcekey="PaypalModeTest" Value="Sandbox" Selected="true" />
            <asp:ListItem resourcekey="PaypalModeProduction" Value="Live" />
        </asp:RadioButtonList>
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DebugMode" AssociatedControlID="chkDebugMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDebugMode" runat="server" />
    </div>
</div>
