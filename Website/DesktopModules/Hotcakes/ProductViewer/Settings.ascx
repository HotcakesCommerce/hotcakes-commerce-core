<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Hotcakes.Modules.ProductViewer.Settings" %>
<%@ Register Src="../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<fieldset>
    <div class="dnnFormItem">
        <div class="dnnFormItem">            
            <dnn:labelcontrol id="ProductLabel" controlname="ProductContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ProductContentLabel" CssClass="dnnFormLabel" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ProductSelectionLabel" controlname="ProductComboBox" suffix=":" runat="server" />
            <telerik:RadComboBox ID="ProductComboBox" runat="server" Width="250px" Height="150px"
                EnableLoadOnDemand="True" ShowMoreResultsBox="false" EnableVirtualScrolling="false"
                OnItemsRequested="ProductComboBox_ItemsRequested" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewLabel" controlname="ViewContentLabel" suffix=":" runat="server" />
            <asp:Label ID="ViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ViewSelectionLabel" controlname="ViewComboBox" suffix=":" runat="server" />
            <telerik:RadComboBox ID="ViewComboBox" runat="server" Width="250px" Height="150px"
                EnableLoadOnDemand="False" ShowMoreResultsBox="false" EnableVirtualScrolling="false" />
        </div>
    </div>
</fieldset>
