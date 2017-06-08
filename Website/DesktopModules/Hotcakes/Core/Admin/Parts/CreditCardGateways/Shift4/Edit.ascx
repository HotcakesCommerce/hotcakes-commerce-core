<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.Shift4.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>Shift4 Options</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Utg Secure Connection" CssClass="hcLabel" />
        <asp:CheckBox ID="chkUtgSecure" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Utg Server" CssClass="hcLabel" />
        <asp:TextBox ID="txtUtgServer" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Utg Port" CssClass="hcLabel" />
        <asp:TextBox ID="txtUtgPort" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" Text="SerialNumber" CssClass="hcLabel" />
        <asp:TextBox ID="txtSerialNumber" runat="server" />
    </div>
        <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Username" CssClass="hcLabel" />
        <asp:TextBox ID="txtUsername" runat="server" />
    </div>
        <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Password" CssClass="hcLabel" />
        <asp:TextBox ID="txtPassword" runat="server" />
    </div>
        <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Merchant Id" CssClass="hcLabel" />
        <asp:TextBox ID="txtMerchantId" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Vendor" CssClass="hcLabel" />
        <asp:TextBox ID="txtVendor" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" Text="Debug Mode" AssociatedControlID="chkDebugMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDebugMode" runat="server" />
    </div>
</div>