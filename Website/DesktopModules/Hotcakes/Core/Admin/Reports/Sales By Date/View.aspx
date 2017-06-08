<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Reports.Sales_By_Date.View" Title="Sales By Date" CodeBehind="View.aspx.cs" %>

<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="hcReport">

        <h1><%=PageTitle %></h1>

        <div class="hcFormMessage">
            <%=Localization.GetString("ReportInfo") %>
        </div>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <hcc:DateRangePicker ID="DateRangeField" runat="server" RangeType="ThisMonth" />
        </div>

        <asp:Label ID="lblNoTransactionsMessage" CssClass="hcInfoLabelLeft" Visible="false" runat="server">
            <%=Localization.GetString("NoOrdersFound") %>
        </asp:Label>

        <asp:Panel ID="pnlReportData" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("OrdersTotaling"), TotalCount, TotalGrand.ToString("C")) %></div>

            <asp:DataGrid DataKeyField="bvin" ID="dgList" runat="server" AutoGenerateColumns="False"
                ShowFooter="True" CssClass="hcGrid" OnItemDataBound="dgList_ItemDataBound">
                <HeaderStyle CssClass="hcGridHeader hcRight" />
                <ItemStyle CssClass="hcGridRow hcRight" />
                <FooterStyle CssClass="hcGridFooter" />
                <Columns>
                    <asp:TemplateColumn HeaderText="Date">
                        <ItemTemplate>
                            <asp:Label ID="lblDate" runat="server" Text='' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="OrderNumber" HeaderText="OrderNumber" />
                    <asp:BoundColumn DataField="TotalOrderBeforeDiscounts" HeaderText="SubTotal" DataFormatString="{0:C}" />
                    <asp:BoundColumn DataField="TotalOrderDiscounts" HeaderText="Discounts" DataFormatString="{0:C}" />
                    <asp:BoundColumn DataField="TotalShippingBeforeDiscounts" HeaderText="Shipping" DataFormatString="{0:C}" />
                    <asp:BoundColumn DataField="TotalShippingDiscounts" HeaderText="ShippingDiscount" DataFormatString="{0:C}" />
                    <asp:BoundColumn DataField="TotalTax" HeaderText="Tax" DataFormatString="{0:C}" />
                    <asp:BoundColumn DataField="TotalGrand" HeaderText="GrandTotal" DataFormatString="{0:C}" ItemStyle-Width="100px" />
                    <asp:TemplateColumn>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkViewOrder" runat="server" CssClass="hcIconView" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="FormLabel" />
            </asp:DataGrid>
        </asp:Panel>

        <ul runat="server" id="divActions" class="hcActions">
            <li>
                <asp:LinkButton ID="lnkExport" CssClass="hcSecondaryAction" resourcekey="ExportExcel" runat="server" />
            </li>
        </ul>

    </div>

</asp:Content>
