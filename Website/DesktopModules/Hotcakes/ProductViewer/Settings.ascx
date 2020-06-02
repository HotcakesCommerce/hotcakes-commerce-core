<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Hotcakes.Modules.ProductViewer.Settings" %>
<%@ Register Src="../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>
<fieldset>
    <div class="dnnFormItem">
        <div class="dnnFormItem">            
            <dnn:labelcontrol id="ProductLabel" controlname="ProductContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ProductContentLabel" CssClass="dnnFormLabel" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ProductSelectionLabel" controlname="ProductComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="ProductComboBox" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewLabel" controlname="ViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewSelectionLabel" controlname="ViewComboBox" suffix=":" runat="server" />
            <asp:DropDownList ID="ViewComboBox" runat="server"/>
        </div>
    </div>
</fieldset>
