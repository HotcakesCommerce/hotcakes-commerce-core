<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="true" Inherits="Hotcakes.Modules.Core.Admin.Reports.Summary_Report.View" Title="Untitled Page" CodeBehind="View.aspx.cs" Culture="auto" UICulture="auto" %>

<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>

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

        <asp:Panel ID="pnlReportData" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("TransactionsFound"), ItemsCount, TransactionsData.Count) %></div>

            <table class="hcGrid">
                <colgroup>
                    <col />
                    <col />
                    <col style="width: 100px" />
                </colgroup>
                <tr class="hcGridHeader">
                    <th><%=Localization.GetString("Description") %></th>
                    <th class="hcRight"><%=Localization.GetString("Details") %></th>
                    <th class="hcRight"><%=Localization.GetString("Totals") %></th>
                </tr>
                <asp:Repeater ID="rpSummary" runat="server">
                    <ItemTemplate>
                        <tr class="hcGridRow">
                            <td><%#Eval("Description") %></td>
                            <td class="hcRight"><%#Eval("Details", "{0:C}") %></td>
                            <td class="hcRight"><%#Eval("Total", "{0:C}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>

            <table class="hcNoGrid">
                <colgroup>
                    <col />
                    <col style="width: 100px" />
                </colgroup>
                <tr>
                    <th><%=Localization.GetString("TotalSales") %></th>
                    <td><%=RepGrandTotal.ToString("C") %></td>
                </tr>
                <tr>
                    <th><%=Localization.GetString("TotalReturns") %></th>
                    <td><%=RepCreditAll.ToString("C") %></td>
                </tr>
                <tr>
                    <th><%=Localization.GetString("TotalNet") %></th>
                    <td><%=(RepGrandTotal - RepCreditAll).ToString("C") %></td>
                </tr>
            </table>

            <table class="hcGrid">
                <colgroup>
                    <col />
                    <col style="width: 100px" />
                    <col style="width: 100px" />
                </colgroup>
                <tr class="hcGridHeader">
                    <th><%=Localization.GetString("PaymentsReceived") %></th>
                    <th class="hcRight"><%=Localization.GetString("Count") %></th>
                    <th class="hcRight"><%=Localization.GetString("Amount") %></th>
                </tr>
                <asp:Repeater ID="rpCCSummary" runat="server">
                    <ItemTemplate>
                        <tr class="hcGridRow">
                            <td><%#Eval("CartType") %></td>
                            <td class="hcRight"><%#Eval("Count") %></td>
                            <td class="hcRight"><%#Eval("Amount", "{0:C}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <table class="hcNoGrid">
                <colgroup>
                    <col />
                    <col style="width: 100px" />
                </colgroup>
                <tr>
                    <th><%=Localization.GetString("TotalPayments") %></th>
                    <td><%=RepCCGrandTotal.ToString("C") %></td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>