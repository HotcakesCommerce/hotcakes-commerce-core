<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderStatusDisplay" CodeBehind="OrderStatusDisplay.ascx.cs" %>
<div class="hcForm hcOrderStatus">
    <div class="hcFormItemHor">
        <label class="hcLabel">
            <asp:Literal ID="litPaymentStatus" runat="server" />
            /
            <asp:Literal ID="litShippingStatus" runat="server" />
            / 
        </label>
        <asp:DropDownList ID="lstStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstStatus_SelectedIndexChanged" />
    </div>
</div>
