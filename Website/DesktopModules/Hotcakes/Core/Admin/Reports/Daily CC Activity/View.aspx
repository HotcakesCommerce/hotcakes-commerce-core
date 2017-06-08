<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="true" Inherits="Hotcakes.Modules.Core.Admin.Reports.Daily_CC_Activity.View" Title="Untitled Page" CodeBehind="View.aspx.cs" %>

<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="hcReport">
        <h1><%=PageTitle %></h1>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" />
        </div>

        <asp:Label ID="lblNoTransactionsMessage" CssClass="hcInfoLabelLeft" Visible="false" runat="server">
            <%=Localization.GetString("NoTransactionsFound") %>
        </asp:Label>

        <asp:Panel ID="pnlReport" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("TransactionsFound"), TotalCount) %></div>

            <asp:Table ID="tblReport" CssClass="hcGrid" runat="server"/>
        </asp:Panel>

    </div>
</asp:Content>