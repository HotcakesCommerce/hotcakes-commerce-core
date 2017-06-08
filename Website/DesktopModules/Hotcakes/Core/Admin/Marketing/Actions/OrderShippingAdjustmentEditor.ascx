<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderShippingAdjustmentEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.OrderShippingAdjustmentEditor" %>
<h1><%=Localization.GetString("DiscountShippingBy") %></h1>
<div class="hcColumnLeft hcForm" style="width: 50%">
    <div class="hcFormItemLabel">
        <label class="hcLabel"><%=Localization.GetString("AdjustPriceBy") %></label>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <asp:TextBox ID="OrderShippingAdjustmentAmount" runat="server" Columns="10"></asp:TextBox>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <asp:DropDownList ID="lstOrderShippingAdjustmentType" runat="server" />
    </div>
</div>
