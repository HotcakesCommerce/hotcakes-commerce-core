<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdjustProductPriceEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.AdjustProductPrice" %>
<h1><%=Localization.GetString("DiscountProductPriceBy") %></h1>
<div class="hcColumnLeft hcForm" style="width: 50%">
    <div class="hcFormItemLabel">
        <label class="hcLabel"><%=Localization.GetString("AdjustPriceBy") %></label>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <asp:TextBox ID="AmountField" runat="server" Columns="10"></asp:TextBox>
    </div>
    <div class="hcFormItem hcFormItem50p">
        <asp:DropDownList ID="lstAdjustmentType" runat="server" />
    </div>
</div>
