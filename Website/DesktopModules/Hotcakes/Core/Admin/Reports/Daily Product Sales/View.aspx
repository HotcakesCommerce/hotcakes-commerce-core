<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="true" Inherits="Hotcakes.Modules.Core.Admin.Reports.Daily_Product_Sales.View" Title="Untitled Page" CodeBehind="View.aspx.cs" %>

<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    
    <div class="hcReport">
        <h1><%=PageTitle %></h1>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" />
        </div>

        <asp:Label ID="lblNoTransactionsMessage" CssClass="hcInfoLabelLeft" Visible="false" runat="server">
            <%=Localization.GetString("NoTransactions") %>
        </asp:Label>

        <asp:Panel ID="pnlReport" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("TransactionsFound"), TotalCount) %></div>

            <asp:Table ID="tblReport" CssClass="hcGrid" runat="server">
                <asp:TableHeaderRow CssClass="hcGridHeader">
                    <asp:TableHeaderCell resourcekey="SKU" />
                    <asp:TableHeaderCell resourcekey="ProductName" />
                    <asp:TableHeaderCell resourcekey="Quantity" CssClass="hcRight" />
                    <asp:TableHeaderCell resourcekey="AveragePrice" CssClass="hcRight" />
                    <asp:TableHeaderCell resourcekey="Sales" CssClass="hcRight" />
                    <asp:TableHeaderCell resourcekey="Discounts" CssClass="hcRight" />
                    <asp:TableHeaderCell resourcekey="GrossSales" CssClass="hcRight" />
                </asp:TableHeaderRow>
            </asp:Table>
        </asp:Panel>
    </div>

</asp:Content>