<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="true" Inherits="Hotcakes.Modules.Core.Admin.Reports.CustomersByProduct.View" Title="Untitled Page" CodeBehind="View.aspx.cs" %>
<%@ Register Src="../../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="reports/" runat="server" />
    <div class="hcBlock" runat="server">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton CssClass="hcButton" ID="lnkReturn" resourcekey="lnkReturn" runat="server" CausesValidation="False" OnClick="lnkReturn_OnClick" />
            </div>
        </div>
    </div>
</asp:Content>

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
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("TransactionsFound"), TotalCount, ProductName) %></div>

            <asp:Table ID="tblReport" CssClass="hcGrid" runat="server">
                <asp:TableHeaderRow CssClass="hcGridHeader">
                    <asp:TableHeaderCell resourcekey="FirstName" />
                    <asp:TableHeaderCell resourcekey="LastName" CssClass="hcRight" />
                    <asp:TableHeaderCell resourcekey="Email" CssClass="hcRight" />
                </asp:TableHeaderRow>
            </asp:Table>
        </asp:Panel>
        
        <asp:LinkButton ID="lnkExport" CssClass="hcSecondaryAction" resourcekey="ExportExcel" runat="server" OnClick="lnkExport_OnClick" />
    </div>

</asp:Content>