<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.SimpleProductFilter" CodeBehind="SimpleProductFilter.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel CssClass="hcForm" runat="server" DefaultButton="btnGo">
    <div class="hcFormItem hcGo">
        <label class="hcLabel"><%=Localization.GetString("SearchProductsByKeyword") %></label>
        <div class="hcFieldOuter">
            <asp:TextBox ID="txtFilterField" runat="server" />
            <asp:LinkButton ID="btnGo" runat="server" Text="Filter Results" CssClass="hcIconRight" OnClick="btnGo_Click" />
        </div>
    </div>
    <div class="hcFormItem">
       <%-- <asp:DropDownList ID="ddlProductTypeFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ProductTypeFilter_SelectedIndexChanged" />--%>
		<telerik:RadComboBox ID="ddlProductTypeFilter" runat="server" OnSelectedIndexChanged="ddlProductTypeFilter_SelectedIndexChanged" AutoPostBack="True"></telerik:RadComboBox>
    </div>
	<br />
    <div class="hcFormItem">
       <%-- <asp:DropDownList ID="ddlCategoryFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="CategoryFilter_SelectedIndexChanged" />--%>
		<telerik:RadComboBox ID="ddlCategoryFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged"></telerik:RadComboBox>
    </div>
	<br />
    <div class="hcFormItem">
       <%-- <asp:DropDownList ID="ddlManufacturerFilter" runat="server"
            AutoPostBack="True"
            OnSelectedIndexChanged="ManufacturerFilter_SelectedIndexChanged" />--%>
		<telerik:RadComboBox ID="ddlManufacturerFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlManufacturerFilter_SelectedIndexChanged"></telerik:RadComboBox>
    </div>
	<br />
    <div class="hcFormItem">
      <%--  <asp:DropDownList ID="ddlVendorFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="VendorFilter_SelectedIndexChanged" />--%>
		<telerik:RadComboBox ID="ddlVendorFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlVendorFilter_SelectedIndexChanged"></telerik:RadComboBox>
    </div>
	<br />
    <div class="hcFormItem">
        <%--<asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="StatusFilter_SelectedIndexChanged" />--%>
		<telerik:RadComboBox ID="ddlStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged"></telerik:RadComboBox>
    </div>
	<br />
    <div class="hcFormItem">
        <%--<asp:DropDownList ID="ddlInventoryStatusFilter" runat="server"
            AutoPostBack="True"
            OnSelectedIndexChanged="InventoryStatusFilter_SelectedIndexChanged" />--%>
		<telerik:RadComboBox ID="ddlInventoryStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlInventoryStatusFilter_SelectedIndexChanged"></telerik:RadComboBox>
    </div>
	<br />
</asp:Panel>