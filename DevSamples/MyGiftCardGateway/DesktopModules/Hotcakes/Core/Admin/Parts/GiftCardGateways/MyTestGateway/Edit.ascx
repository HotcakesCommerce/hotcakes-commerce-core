<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="MyCompany.MyGiftCardGateway.Edit" %>
<h1>
    <asp:Label runat="server" resourcekey="MyTestGatewayOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="UserName" CssClass="hcLabel" />
        <asp:TextBox ID="txtUserName" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Password" CssClass="hcLabel" />
        <asp:TextBox ID="txtPassword" runat="server" />
    </div>
</div>
