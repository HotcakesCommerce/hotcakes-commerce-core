<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Reports.Top_Products.View" Title="Top Products" CodeBehind="View.aspx.cs" %>

<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    
    <div class="hcReport">
        
        <h1><%=PageTitle %></h1>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" />
        </div>

        <asp:Label ID="lblNoTransactionsMessage" resourcekey="NoProductsFound" CssClass="hcInfoLabelLeft" Visible="false" runat="server"/>

        <asp:Panel ID="pnlReportData" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("ProductsFound"), ProductCount) %></div>

            <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" DataKeyNames="bvin"
                AllowPaging="True" AllowSorting="True" PagerSettings-Mode="Numeric" CssClass="hcGrid"
                OnPageIndexChanging="gvProducts_PageIndexChanging" OnRowEditing="gvProducts_RowEditing" OnRowDataBound="gvProducts_OnRowDataBound">
                <RowStyle CssClass="hcGridRow" />
                <HeaderStyle CssClass="hcGridHeader" />
                <Columns>
                    <asp:BoundField DataField="SKU" />
                    <asp:BoundField DataField="ProductName" />
                </Columns>
            </asp:GridView>
        </asp:Panel>
    
    </div>

</asp:Content>