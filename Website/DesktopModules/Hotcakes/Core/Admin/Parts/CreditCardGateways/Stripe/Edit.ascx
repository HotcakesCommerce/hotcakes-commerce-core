<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.Stripe.Edit" %>
<h1>
    <asp:Label runat="server" resourcekey="StripeOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ApiKey" CssClass="hcLabel" />
        <asp:TextBox ID="txtApiKey" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PublicKey" CssClass="hcLabel" />
        <asp:TextBox ID="txtPublicKey" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Currency" CssClass="hcLabel" />
        <asp:DropDownList ID="ddlCurrency" runat="server" />
    </div>
</div>
