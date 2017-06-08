<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Reports.Daily_Sales.View" Title="Daily Sales" CodeBehind="View.aspx.cs" %>

<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/DatePickerNavigation.ascx" TagPrefix="hcc" TagName="DatePickerNavigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <div class="hcReport">
        
        <h1><%=PageTitle %></h1>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Date") %></label>
                <hcc:DatePickerNavigation runat="server" id="ucDatePickerNav" />
            </div>
        </div>

        <asp:Label ID="lblNoTransactionsMessage" CssClass="hcInfoLabelLeft" Visible="false" runat="server">
            <%=Localization.GetString("NoTransactions") %>
        </asp:Label>

        <asp:Panel ID="pnlReportData" runat="server">
            <div class="hcInfoLabel"><%=TotalCount %> Transactions Found</div>

            <asp:DataGrid ID="dgList" CssClass="hcGrid" DataKeyField="OrderId" AutoGenerateColumns="False" ShowFooter="True"
                OnEditCommand="dgList_Edit" OnItemDataBound="dgList_ItemDataBound" OnPageIndexChanged="dgList_PageIndexChanged" runat="server">
                <HeaderStyle CssClass="hcGridHeader" />
                <FooterStyle CssClass="hcGridFooter" />
                <ItemStyle CssClass="hcGridRow"/>
                <Columns>
                    <asp:TemplateColumn HeaderText="OrderTimeCustomer">
                        <ItemTemplate>
                            <asp:Literal ID="litOrderNumber" runat="server"/> | 
                            <small>
                                <asp:Literal ID="litTimeStamp" runat="server"/>
                            </small>
                            <br />
                            <asp:Literal ID="litCustomerName" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Description">
                        <ItemTemplate>
                            <asp:Literal ID="litDescription" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="TempEstimatedItemPortion" HeaderText="SubTotal" DataFormatString="{0:C}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:BoundColumn DataField="TempEstimatedItemDiscount" HeaderText="Discounts" DataFormatString="{0:C}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:BoundColumn DataField="TempEstimatedShippingPortion" HeaderText="Shipping" DataFormatString="{0:C}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:BoundColumn DataField="TempEstimatedShippingDiscount" HeaderText="ShippingDiscount" DataFormatString="{0:C}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:BoundColumn DataField="TempEstimatedTaxPortion" HeaderText="Tax" DataFormatString="{0:C}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:TemplateColumn HeaderText="Total">
                        <HeaderStyle CssClass="hcRight" />
                        <ItemStyle CssClass="hcRight" />
                        <ItemTemplate>
                            <asp:Literal ID="litAmount" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton ID="EditButton" runat="server" CssClass="hcIconView" CommandName="Edit"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="hcGridPager" Mode="NumericPages"/>
            </asp:DataGrid>
        </asp:Panel>
    </div>
</asp:Content>
