<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Hotcakes.Modules.CategoryViewer.Settings" %>
<%@ Register Src="../../../controls/labelcontrol.ascx" TagName="labelcontrol" TagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<div class="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" resourcekey="SettingsHint" />
</div>
<div class="dnnForm">
	<fieldset>
		<div class="dnnFormItem">
			<dnn:LabelControl ID="CategoryLabel" ControlName="CategoryContentLabel" Suffix=":" runat="server" />
			<asp:Label ID="CategoryContentLabel" CssClass="dnnFormLabel" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:LabelControl ID="ViewLabel" ControlName="ViewContentLabel" Suffix=":" runat="server" />
			<asp:Label ID="ViewContentLabel" CssClass="dnnFormLabel" runat="server" Text="" />
		</div>
		<div class="dnnFormItem">
			<dnn:LabelControl ID="CategorySelectionLabel" ControlName="CategoryComboBox" Suffix=":" runat="server" />
			<telerik:RadComboBox ID="CategoryComboBox" runat="server" Width="250px" Height="150px"
				EnableLoadOnDemand="false" ShowMoreResultsBox="false" EnableVirtualScrolling="false"/>
		</div>
		<div class="dnnFormItem">
			<dnn:LabelControl ID="ViewSelectionLabel" ControlName="ViewComboBox" Suffix=":" runat="server" />
			<telerik:RadComboBox ID="ViewComboBox" runat="server" Width="250px" Height="150px"
				EnableLoadOnDemand="False" ShowMoreResultsBox="false" EnableVirtualScrolling="false"/>
		</div>
		<div class="dnnFormItem">
			<dnn:LabelControl ID="ProductPageSizeLabel" ControlName="ProductPageSizeTextBox" Suffix=":" runat="server" />
			<asp:TextBox ID="ProductPageSizeTextBox" CssClass="dnnFormLabel" runat="server" />
		</div>
        <div class="dnnFormItem">
			<dnn:LabelControl ID="PreContentColumnLabel" ControlName="txtPreContentColumnId" Suffix=":" runat="server" />
			<telerik:RadComboBox ID="ddlPreContentColumnId"  runat="server" Width="250px" MaxHeight="150px"
				EnableLoadOnDemand="false" ShowMoreResultsBox="false" EnableVirtualScrolling="false" />
		</div>	
		<div class="dnnFormItem">
			<dnn:LabelControl ID="PostContentColumnLabel" ControlName="txtPostContentColumnId" Suffix=":" runat="server" />
			<telerik:RadComboBox ID="ddlPostContentColumnId"  runat="server" Width="250px" MaxHeight="150px"
				EnableLoadOnDemand="false" ShowMoreResultsBox="false" EnableVirtualScrolling="false" />
		</div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ShowManufactures" controlname="ShowManufacturesCheckBox" suffix=":" runat="server" />
            <asp:CheckBox ID="ShowManufacturesCheckBox" CssClass="dnnFormLabel" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="ShowVendors" controlname="ShowVendorsCheckBox" suffix=":" runat="server" />
            <asp:CheckBox ID="ShowVendorsCheckBox" CssClass="dnnFormLabel" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:labelcontrol id="SortingOptions" controlname="SortingOptionsCheckBoxList" suffix=":" runat="server" />
            <asp:CheckBoxList ID="SortingOptionsCheckBoxList" runat="server" />
        </div>
    </fieldset>
</div>
