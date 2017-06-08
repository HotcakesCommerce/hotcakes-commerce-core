<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.CreditCardGateways.PayPal_Payments_Pro.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="PayPalOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Username" CssClass="hcLabel" />
        <asp:TextBox ID="txtUsername" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Password" CssClass="hcLabel" />
        <asp:TextBox ID="txtPassword" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Signature" CssClass="hcLabel" />
        <asp:TextBox ID="txtSignature" runat="server" />
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
