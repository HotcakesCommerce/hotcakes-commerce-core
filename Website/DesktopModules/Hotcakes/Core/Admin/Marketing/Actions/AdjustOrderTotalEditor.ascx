<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdjustOrderTotalEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.AdjustOrderTotalEditor" %>
<h1><%=Localization.GetString("DiscountOrderTotalBy") %></h1>
<div class="hcColumnLeft hcForm" style="width: 50%">
    <div class="hcFormItemLabel">
        <label class="hcLabel"><%=Localization.GetString("AdjustPriceBy") %></label>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <asp:TextBox ID="OrderTotalAmountField" runat="server" Columns="10"></asp:TextBox>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <asp:DropDownList ID="lstOrderTotalAdjustmentType" runat="server" />
    </div>
</div>
