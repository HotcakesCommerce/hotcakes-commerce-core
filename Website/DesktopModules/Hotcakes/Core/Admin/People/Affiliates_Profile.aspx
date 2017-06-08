<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Affiliates_Profile.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.People.Affiliates_Profile" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/Pager.ascx" TagName="Pager" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AffiliatePaymentDialog.ascx" TagPrefix="hcc" TagName="AffiliatePaymentDialog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" BaseUrl="reports/" ID="NavMenu" />

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkEditProfile" CssClass="hcTertiaryAction" resourcekey="EditProfile" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="lnkAddPayment" CssClass="hcTertiaryAction" resourcekey="AddPayment" runat="server" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();
    </script>
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm">
        <div class="hcFormItemLabel">
            <label class="hcLabel"><%=string.Format(Localization.GetString("AffiliateID"), Affiliate.AffiliateId) %></label>
        </div>
        <div class="hcFormItemLabel">
            <a href="mailto:<%=Affiliate.Email %>"><%=Affiliate.Email %></a>
        </div>
    </div>

    <hr />

    <h2><%=Localization.GetString("Payments") %></h2>
    <table class="hcBigTable">
        <colgroup>
            <col style="width: 33.3%" />
            <col style="width: 33.3%" />
            <col style="width: 33.3%" />
        </colgroup>
        <tr>
            <th><%=Localization.GetString("CommissionEarnedThisMonth") %></th>
            <th><%=Localization.GetString("CommissionOwed") %></th>
            <th><%=Localization.GetString("CommissionPaidAllTime") %></th>
        </tr>
        <tr>
            <td><%=TotalsByMonth.Commission.ToString("c") %></td>
            <td><%=Totals.CommissionOwed.ToString("c") %></td>
            <td><%=Totals.CommissionPaid.ToString("c") %></td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>

            <hcc:DateRangePicker ID="ucPaymentsDateRange" runat="server" RangeType="ThisMonth" />

            <asp:GridView ID="gvPayments" runat="server" CssClass="hcGrid" DataKeyNames="Id" AutoGenerateColumns="false" OnRowDataBound="gvPayments_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <EmptyDataRowStyle CssClass="hcEmptyRow" />
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoPaymentsFound") %>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="PaymentAmount" DataFormatString="{0:c}" />
                    <asp:BoundField DataField="PaymentDateUtc" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="Notes" />
                    <asp:TemplateField>
                        <ItemStyle Width="40px" />
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkEdit" NavigateUrl='<%#GetAttachmentUrl(Container) %>' Visible='<%#ShowAttachmentUrl(Container) %>' CssClass="hcIconAttachment" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hcc:Pager ID="ucPaymentsPager" PageSize="10" PostBackMode="true" runat="server" PageSizeSet="10,50,0" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <br/>
    <hr />

    <h2><%=Localization.GetString("Orders") %></h2>
    <table class="hcBigTable">
        <colgroup>
            <col style="width: 50%" />
            <col style="width: 50%" />
        </colgroup>
        <tr>
            <th><%=Localization.GetString("OrdersThisMonth") %></th>
            <th><%=Localization.GetString("RevenueThisMonth") %></th>
        </tr>
        <tr>
            <td><%=TotalsByMonth.OrdersCount.ToString() %></td>
            <td><%=TotalsByMonth.SalesAmount.ToString("c") %></td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <hcc:DateRangePicker ID="ucOrdersDateRange" runat="server" RangeType="ThisMonth" />

            <asp:GridView ID="gvOrders" runat="server" CssClass="hcGrid" DataKeyNames="Bvin" AutoGenerateColumns="false" OnRowDataBound="gvOrders_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow hcRight" />
                <EmptyDataRowStyle CssClass="hcEmptyRow" />
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoOrdersFound") %>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="TotalOrderAfterDiscounts" DataFormatString="{0:c}" />
                    <asp:BoundField DataField="OrderNumber" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetCommission(Container) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TimeOfOrderUtc" DataFormatString="{0:d}" />
                </Columns>
            </asp:GridView>
            <hcc:Pager ID="ucOrdersPager" PageSize="10" PostBackMode="true" runat="server" PageSizeSet="10,50,0" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <br/>
    <hr />
    
    <h2><%=Localization.GetString("Referrals") %></h2>
    <table class="hcBigTable">
        <colgroup>
            <col style="width: 50%" />
            <col style="width: 50%" />
        </colgroup>
        <tr>
            <th><%=Localization.GetString("AffiliateRefferedThisMonth") %></th>
            <th><%=Localization.GetString("AffiliateRefferedAllTime") %></th>
        </tr>
        <tr>
            <td><%=TotalsByMonth.ReferralsCount.ToString() %></td>
            <td><%=Totals.ReferralsCount.ToString() %></td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>

            <div class="hcForm">
                <div class="hcFormItemHor">
                    <label class="hcLabelShort"><%=Localization.GetString("Search") %></label>
                    <asp:TextBox ID="txtSearchText" runat="server" Width="200px" />
                    <asp:LinkButton ID="btnFind" runat="server" resourcekey="Find" CssClass="hcButton hcSmall" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabelShort"><%=Localization.GetString("SearchBy") %></label>
                    <asp:dropdownlist ID="ddlSearchBy" runat="server" Width="200px">
                        <asp:ListItem resourcekey="LastName" Value="0" />
                        <asp:ListItem resourcekey="ID" Value="1" />
                        <asp:ListItem resourcekey="Email" Value="2" />
                    </asp:dropdownlist>
                </div>
            </div>
            <asp:GridView ID="gvReferrals" runat="server" CssClass="hcGrid" AutoGenerateColumns="false" OnRowDataBound="gvReferrals_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <EmptyDataRowStyle CssClass="hcEmptyRow" />
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoReferralsFound") %>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="FullName" />
                    <asp:BoundField DataField="AffiliateID" ItemStyle-CssClass="hcRight" />
                    <asp:BoundField DataField="Email" />
                    <asp:BoundField DataField="Commission" ItemStyle-CssClass="hcRight" DataFormatString="{0:c}" />
                </Columns>
            </asp:GridView>
            <hcc:Pager ID="ucReferralPager" PageSize="10" PostBackMode="true" runat="server" PageSizeSet="10,50,0" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <hcc:AffiliatePaymentDialog runat="server" id="ucPaymentDialog" />

</asp:Content>