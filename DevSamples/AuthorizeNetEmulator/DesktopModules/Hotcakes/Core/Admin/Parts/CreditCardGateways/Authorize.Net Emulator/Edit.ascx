<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="AuthorizeNet.Emulator.Edit" %>
<h1>
    <asp:Label runat="server" resourcekey="AuthorizeNetEmulatorOptions" />
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
        <asp:Label runat="server" resourcekey="EmailCustomer" AssociatedControlID="chkEmailCustomer" CssClass="hcLabel" />
        <asp:CheckBox ID="chkEmailCustomer" runat="server" resourcekey="chkEmailCustomer" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DeveloperMode" AssociatedControlID="chkDeveloperMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDeveloperMode" runat="server" resourcekey="chkDeveloperMode" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="TestMode" AssociatedControlID="chkTestMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkTestMode" runat="server" resourcekey="chkTestMode" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DebugMode" AssociatedControlID="chkDebugMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDebugMode" runat="server" resourcekey="chkDebugMode" />
    </div>
</div>
