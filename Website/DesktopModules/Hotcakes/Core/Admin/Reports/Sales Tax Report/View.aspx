<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Reports.Sales_Tax_Report.View" %>
<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="hcReport">
        
        <h1><%=PageTitle %></h1>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" />
        </div>

        <asp:Label ID="lblNoTransactionsMessage" resourcekey="NoTaxesFound" CssClass="hcInfoLabelLeft" Visible="false" runat="server"/>

        <asp:Panel ID="pnlReportData" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("TaxesFound"), TaxCount) %></div>

            <asp:gridview runat="server" ID="gvTaxReport" AutoGenerateColumns="False" CssClass="hcGrid" OnRowDataBound="gvTaxReport_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundField DataField="CountryName" />
                    <asp:BoundField DataField="RegionName" />
                    <asp:BoundField DataField="TotalTax" DataFormatString="{0:c}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                </Columns>
            </asp:gridview>

            <asp:LinkButton runat="server" ID="btnDownloadReport" resourcekey="DownloadExcelReport" CssClass="hcSecondaryAction" />
        </asp:Panel>
    
    </div>

</asp:Content>