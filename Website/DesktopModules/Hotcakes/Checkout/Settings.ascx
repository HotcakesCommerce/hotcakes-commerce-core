<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Hotcakes.Modules.Checkout.Settings" %>
<%@ Register Src="../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>
<div class="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" resourcekey="SettingsHint" />
</div>
<div class="dnnForm">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewLabel" controlname="ViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewSelectionLabel" controlname="ViewComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="ViewComboBox" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ReceiptViewLabel" controlname="ReceiptViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ReceiptViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ReceiptViewSelectionLabel" controlname="ReceiptViewComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="ReceiptViewComboBox" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="PaymentErrorViewLabel" controlname="PaymentErrorViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="PaymentErrorViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="PaymentErrorViewSelectionLabel" controlname="PaymentErrorViewComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="PaymentErrorViewComboBox" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="PayPalViewLabel" controlname="PayPalViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="PayPalViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="PayPalViewSelectionLabel" controlname="PayPalViewComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="PayPalViewComboBox" runat="server"/>
        </div>
    </fieldset>
</div>
