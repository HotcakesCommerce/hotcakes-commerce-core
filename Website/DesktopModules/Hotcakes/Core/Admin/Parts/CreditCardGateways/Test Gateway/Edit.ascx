<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.CreditCardGateways.Test_Gateway.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="TestGatewayOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AuthorizeFails" AssociatedControlID="chkAuthorizeFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkAuthorizeFails" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="CaptureFails" AssociatedControlID="chkCaptureFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkCaptureFails" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ChargeFails" AssociatedControlID="chkChargeFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkChargeFails" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="RefundFails" AssociatedControlID="chkRefundFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkRefundFails" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="VoidFails" AssociatedControlID="chkVoidFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkVoidFails" runat="server" />
    </div>
</div>
