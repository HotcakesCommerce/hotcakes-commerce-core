<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.CreditCardGateways.Test_Recurring_Gateway.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="TestGatewayOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="CreateFails" AssociatedControlID="chkCreateFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkCreateFails" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="UpdateFails" AssociatedControlID="chkUpdateFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkUpdateFails" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="CancelFails" AssociatedControlID="chkCancelFails" CssClass="hcLabel" />
        <asp:CheckBox ID="chkCancelFails" runat="server" />
    </div>
</div>
