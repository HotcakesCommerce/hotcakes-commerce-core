<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.PayLeap.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="PayLeapOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Username" CssClass="hcLabel" />
        <asp:TextBox ID="txtUsernameField" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="Password" CssClass="hcLabel" />
        <asp:TextBox ID="txtPasswordField" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="TestMode" AssociatedControlID="chkTestMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkTestMode" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="EnableTracing" AssociatedControlID="chkEnableTracing" CssClass="hcLabel" />
        <asp:CheckBox ID="chkEnableTracing" runat="server" resourcekey="chkEnableTracing" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DeveloperMode" AssociatedControlID="chkDeveloperMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDeveloperMode" runat="server" resourcekey="chkDeveloperMode" />
    </div>
</div>
