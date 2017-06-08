<%@ Page Language="C#" MasterPageFile="../../AdminNav.master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Reports.Sales_By_Coupon.View" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="hcReport">
        
        <h1><%=PageTitle %></h1>
        <hcc:MessageBox ID="ucMessageBox" runat="server" />

        <div class="hcForm">
            <div class="hcFormItemHor">
                <label class="hcLabelShort"><%=Localization.GetString("CouponCode") %></label>
                <telerik:RadComboBox ID="lstCouponCode" AllowCustomText="true" MarkFirstMatch="true" Width="200px" AutoPostBack="true" runat="server" />
            </div>
            <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" />
        </div>

        <asp:Label ID="lblNoRecordsMessage" resourcekey="NoOrdersFound" CssClass="hcInfoLabelLeft" Visible="false" runat="server"/>

        <asp:Panel ID="pnlReportData" runat="server">
            <div class="hcInfoLabel"><%=string.Format(Localization.GetString("OrdersFound"), OrdersCount) %></div>

            <asp:DataGrid ID="dgList" AutoGenerateColumns="False" DataKeyField="bvin" CssClass="hcGrid" runat="server" OnItemDataBound="dgList_OnItemDataBound">
                <ItemStyle CssClass="hcGridRow"/>
                <HeaderStyle CssClass="hcGridHeader"/>
                <Columns>
                    <asp:BoundColumn DataField="OrderNumber" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:HyperLinkColumn DataNavigateUrlField="UserID" DataTextField="UserEmail" DataNavigateUrlFormatString="~/DesktopModules/Hotcakes/Core/Admin/People/users_edit.aspx?id={0}" />
                    <asp:BoundColumn DataField="TimeOfOrderUtc" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" />
                    <asp:BoundColumn DataField="TotalGrand" DataFormatString="{0:c}" ItemStyle-CssClass="hcRight" HeaderStyle-CssClass="hcRight" ItemStyle-Width="100px" />
                    <asp:TemplateColumn>
                        <ItemStyle Width="10px" />
                        <ItemTemplate>
                            <asp:HyperLink NavigateUrl='<%#Eval("bvin","~/DesktopModules/Hotcakes/Core/Admin/Orders/ViewOrder.aspx?id={0}") %>' Text='<%#Eval("OrderNumber") %>' CssClass="hcIconView" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <table class="hcNoGrid">
                <colgroup>
                    <col />
                    <col style="width: 100px" />
                    <col style="width: 30px" />
                </colgroup>
                <tr>
                    <td><%=Localization.GetString("Total") %></td>
                    <td><%=OrderTotal.ToString("C")%></td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
    
    </div>

</asp:Content>