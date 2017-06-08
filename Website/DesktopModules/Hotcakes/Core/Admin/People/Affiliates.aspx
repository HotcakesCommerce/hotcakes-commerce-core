<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Affiliates" CodeBehind="Affiliates.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/Pager.ascx" TagName="Pager" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AffiliatePaymentDialog.ascx" TagPrefix="hcc" TagName="AffiliatePaymentDialog" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">

    <hcc:NavMenu runat="server" BaseUrl="reports/" ID="NavMenu" />

    <div runat="server" id="divNavBottom">
        <div class="hcBlock">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:LinkButton ID="lnkApprove" CssClass="hcTertiaryAction" resourcekey="ApproveSelected" Visible="false" runat="server" />
                </div>
                <div class="hcFormItem">
                    <asp:LinkButton ID="lnkAddPayment" CssClass="hcTertiaryAction" resourcekey="AddPayment" runat="server" />
                </div>
                <div class="hcFormItem">
                    <asp:LinkButton CssClass="hcTertiaryAction" ID="lnkExportToExcel" resourcekey="ExportToExcel" runat="server" />
                </div>
            </div>
        </div>
        <div class="hcBlockNoBorder">
            <div class="hcFormMessage">
                <%=Localization.GetString("AffiliateNote") %>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        hcUpdatePanelReady(function () {
            $("select.hcSelectionRibbon").selectionRibbon();
        });

        $(function () {
            hcUpdatePanelReady(function () {
                $(".hcSelectAll input").change(function () {

                    var isSelectAll = $(this).is(":checked");
                    if (isSelectAll) {
                        $(".hcSelect input").attr("checked", "checked");
                    } else {
                        $(".hcSelect input").removeAttr("checked");
                    }
                });
            });
        });
    </script>

    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" UseDefaultValidationGroup="false" />

    <div class="hcColumnLeft hcRightBorder" style="width: 65%">
        <div class="hcForm">
            <div class="hcFormItemHor">
                <label class="hcLabelShort"><%=Localization.GetString("Search") %></label>
                <asp:TextBox ID="txtSearchText" runat="server" Width="200px" />
                <asp:LinkButton ID="btnFind" runat="server" resourcekey="Find" CssClass="hcButton hcSmall" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabelShort"><%=Localization.GetString("SearchBy") %></label>
                <asp:DropDownList ID="ddlSearchBy" runat="server" Width="200px">
                    <asp:ListItem resourcekey="LastName" Value="0" />
                    <asp:ListItem resourcekey="AffiliateID" Value="1" />
                    <asp:ListItem resourcekey="Email" Value="2" />
                    <asp:ListItem resourcekey="CompanyName" Value="3" />
                </asp:DropDownList>
            </div>
            <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" HideButton="true" />
        </div>
    </div>

    <div class="hcColumnRight" style="width: 34%">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:RadioButtonList ID="rblFilterMode" AutoPostBack="true" runat="server">
                    <asp:ListItem Value="" resourcekey="ApplyFilter" Selected="True" />
                    <asp:ListItem Value="OnlyNonApproved" resourcekey="ShowOnlyNonApproved" />
                    <asp:ListItem Value="OnlyOwed" resourcekey="ShowCommissionOwed" />
                </asp:RadioButtonList>
            </div>
        </div>
    </div>

    <table class="hcBigTable">
        <colgroup>
            <col style="width: 25%" />
            <col style="width: 25%" />
            <col style="width: 25%" />
            <col style="width: 25%" />
        </colgroup>
        <tr>
            <th><%=Localization.GetString("AffiliateOrders") %></th>
            <th><%=Localization.GetString("RevenueFromAffiliates") %></th>
            <th><%=Localization.GetString("CommissionOwed") %></th>
            <th><%=Localization.GetString("CommissionPaid") %></th>
        </tr>
        <tr>
            <td><%=Totals.OrdersCount %></td>
            <td><%=Totals.SalesAmount.ToString("c") %></td>
            <td><%=Totals.CommissionOwed.ToString("c") %></td>
            <td><%=Totals.CommissionPaid.ToString("c") %></td>
        </tr>
    </table>

    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="hcForm hcSorting">
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("SortBy") %></label>
                    <asp:DropDownList ID="ddlSortBy" runat="server" AutoPostBack="true" CssClass="hcSelectionRibbon">
                        <asp:ListItem resourcekey="Sales" Value="0" />
                        <asp:ListItem resourcekey="Orders" Value="1" />
                        <asp:ListItem resourcekey="Commission" Value="2" />
                        <asp:ListItem resourcekey="Signups" Value="3" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="hcInfoLabel"><%=Localization.GetFormattedString("AffiliatesFound", AffiliatesCount) %></div>

            <asp:GridView ID="gvAffiliates" runat="server" CssClass="hcGrid" DataKeyNames="Id" AutoGenerateColumns="false" OnRowDataBound="gvAffiliates_OnRowDataBound">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chbAllItem" CssClass="hcSelectAll" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chbItem" CssClass="hcSelect" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Company" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink NavigateUrl='<%#Eval("Id", "Affiliates_Profile.aspx?id={0}") %>' Text='<%#Eval("FullName") %>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="AffiliateId" />
                    <asp:BoundField DataField="SalesAmount" DataFormatString="{0:c}" ItemStyle-CssClass="hcRight" />
                    <asp:BoundField DataField="OrdersCount" ItemStyle-CssClass="hcRight" />
                    <asp:BoundField DataField="Commission" DataFormatString="{0:c}" ItemStyle-CssClass="hcRight" />
                    <asp:BoundField DataField="CommissionOwed" DataFormatString="{0:c}" ItemStyle-CssClass="hcRight" />
                    <asp:TemplateField ItemStyle-CssClass="hcRight">
                        <ItemTemplate>
                            <%#Eval("SignupsCount") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="80px" />
                        <ItemTemplate>
                            <asp:HyperLink NavigateUrl='<%#Eval("Id", "Affiliates_Edit.aspx?id={0}") %>' CssClass="hcIconEdit" resourcekey="Edit" runat="server" />
                            <asp:LinkButton runat="server" CssClass="hcIconDelete" CausesValidation="False" CommandName="Delete" OnPreRender="Delete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hcc:Pager ID="ucPager" PageSize="10" PostBackMode="true" PageSizeSet="10,20,50" runat="server" />


        </ContentTemplate>
    </asp:UpdatePanel>

    <hcc:AffiliatePaymentDialog runat="server" id="ucPaymentDialog" />

</asp:Content>
