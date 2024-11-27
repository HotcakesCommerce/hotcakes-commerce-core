<%@ page language="C#" masterpagefile="../../AdminNav.master" autoeventwireup="true" codebehind="View.aspx.cs" inherits="Hotcakes.Modules.Core.Admin.Reports.PaymentFailures.View" %>

<%@ register src="../../Controls/MessageBox.ascx" tagname="MessageBox" tagprefix="hcc" %>
<%@ register src="../../Controls/DateRangePicker.ascx" tagname="DateRangePicker" tagprefix="hcc" %>
<%@ register src="../../Controls/Pager.ascx" tagname="Pager" tagprefix="hcc" %>
<%@ register src="../../Controls/NavMenu.ascx" tagname="NavMenu" tagprefix="hcc" %>
<%@ register src="../../Controls/ContactAbandonedCartUsers.ascx" tagname="ContactAbandonedCartUsers" tagprefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">

    <hcc:navmenu runat="server" baseurl="reports/" id="NavMenu" />

    <div runat="server" id="divNavBottom">
        <div class="hcBlock">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:LinkButton ID="lnkExportToExcel" CssClass="hcButton" resourcekey="ExportToExcel" runat="server" />
                </div>
                <div class="hcFormItem">
                    <asp:LinkButton ID="lnkDownloadContacts" CssClass="hcButton" resourcekey="DownloadContacts" runat="server" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    <div class="hcReport">
        <h1><%=PageTitle %></h1>
        <hcc:messagebox id="ucMessageBox" runat="server" />

        <div class="hcColumnLeft" style="width: 60%">
            <div class="hcForm">
                <hcc:daterangepicker id="ucDateRangePicker" runat="server" rangetype="ThisMonth" />
            </div>
        </div>

        <div class="hcColumnRight hcLeftBorder" style="width: 39%">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:RadioButtonList ID="rblAgregatedReport" AutoPostBack="true" runat="server">
                        <asp:ListItem Value="False" resourcekey="CartsReport" Selected="True" />
                        <asp:ListItem Value="True" resourcekey="ProductsReport" />
                    </asp:RadioButtonList>
                </div>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:Label ID="lblNoCartsMessage" resourcekey="NoPaymentFailuresFound" CssClass="hcInfoLabelLeft" Visible="false" runat="server" />
            </div>
        </div>

        <asp:Panel ID="pnlReportsData" runat="server">
            <asp:Repeater ID="rpCarts" runat="server">
                <HeaderTemplate>
                    <table class="hcGrid">
                        <tr class="hcGridHeader">
                            <th><%=Localization.GetString("OrderNumber") %></th>
                            <th><%=Localization.GetString("Date") %></th>
                            <th><%=Localization.GetString("User") %></th>
                            <th><%=Localization.GetString("Product") %></th>
                            <th class="hcRight"><%=Localization.GetString("Quantity") %></th>
                            <th class="hcRight"><%=Localization.GetString("LineTotal") %></th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="hcGridRow">
                        <td>
                            <asp:HyperLink runat="server" ID="hlOrderNumber" />
                        </td>
                        <td runat="server" id="tdDate" />
                        <td>
                            <asp:HyperLink runat="server" ID="hlUser" />
                            <asp:Label runat="server" ID="lbUser" />
                        </td>
                        <td colspan="3">
                            <asp:HyperLink runat="server" ID="hlCart" resourcekey="hlCart" />
                        </td>
                    </tr>
                    <asp:Repeater ID="rpCartItems" runat="server">
                        <ItemTemplate>
                            <tr class="hcGridRow">
                                <td />
                                <td />
                                <td />
                                <td><%#Eval("ProductName") %></td>
                                <td class="hcRight"><%#Eval("Quantity") %></td>
                                <td class="hcRight"><%#Eval("LineTotal", "{0:C}") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rpProducts" runat="server">
                <HeaderTemplate>
                    <table class="hcGrid">
                        <tr class="hcGridHeader">
                            <th><%=Localization.GetString("Product") %></th>
                            <th class="hcRight"><%=Localization.GetString("Quantity") %></th>
                            <th class="hcRight"><%=Localization.GetString("Count") %></th>
                            <th class="hcRight"><%=Localization.GetString("ContactsCount") %></th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="hcGridRow">
                        <td>
                            <asp:HyperLink runat="server" ID="hlProduct" />
                            <asp:Label runat="server" ID="lblProduct" />
                        </td>
                        <td runat="server" id="tdQuantity" class="hcRight" />
                        <td runat="server" id="tdCount" class="hcRight" />
                        <td runat="server" class="hcRight">
                            <asp:Label runat="server" ID="lblContactsCount" />
                            <asp:LinkButton runat="server" ID="lnkContactsCount" OnCommand="lnkContactsCount_Command" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <hcc:pager id="ucPager" pagesize="10" postbackmode="true" runat="server" pagesizeset="10,25,50,0" />
        </asp:Panel>

        <hcc:contactabandonedcartusers runat="server" id="ucContactAbandonedCartUsers" />
    </div>
</asp:Content>

