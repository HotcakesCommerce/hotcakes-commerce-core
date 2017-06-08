<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.Ogone.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="OgoneOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PaymentServiceProviderId" CssClass="hcLabel" />
        <asp:TextBox ID="txtPaymentServiceProviderId" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="UserId" CssClass="hcLabel" />
        <asp:TextBox ID="txtUserId" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Password" CssClass="hcLabel" />
        <asp:TextBox ID="txtPassword" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Currency" CssClass="hcLabel" />
        <asp:DropDownList ID="ddlCurrency" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DebugMode" AssociatedControlID="chkDebugMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDebugMode" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DeveloperMode" AssociatedControlID="chkDeveloperMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDeveloperMode" runat="server" />
    </div>
</div>
