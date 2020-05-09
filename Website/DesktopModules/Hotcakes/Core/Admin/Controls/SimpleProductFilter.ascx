<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.SimpleProductFilter" CodeBehind="SimpleProductFilter.ascx.cs" %>
<asp:Panel CssClass="hcForm" runat="server" DefaultButton="btnGo">
    <div class="hcFormItem hcGo">
        <label class="hcLabel"><%=Localization.GetString("SearchProductsByKeyword") %></label>
        <div class="hcFieldOuter">
            <asp:TextBox ID="txtFilterField" runat="server" />
            <asp:LinkButton ID="btnGo" runat="server" Text="Filter Results" CssClass="hcIconRight" OnClick="btnGo_Click" />
        </div>
    </div>
    <div class="hcFormItem">
        <asp:DropDownList ID="ddlProductTypeFilter" runat="server" OnSelectedIndexChanged="ddlProductTypeFilter_SelectedIndexChanged" AutoPostBack="True"/>
    </div>
	<br />
    <div class="hcFormItem">
        <asp:DropDownList ID="ddlCategoryFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged"/>
    </div>
	<br />
    <div class="hcFormItem">
        <asp:DropDownList ID="ddlManufacturerFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlManufacturerFilter_SelectedIndexChanged"/>
    </div>
	<br />
    <div class="hcFormItem">
        <asp:DropDownList ID="ddlVendorFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlVendorFilter_SelectedIndexChanged"/>
    </div>
	<br />
    <div class="hcFormItem">
        <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged"/>
    </div>
	<br />
    <div class="hcFormItem">
        <asp:DropDownList ID="ddlInventoryStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlInventoryStatusFilter_SelectedIndexChanged"/>
    </div>
	<br />
</asp:Panel>