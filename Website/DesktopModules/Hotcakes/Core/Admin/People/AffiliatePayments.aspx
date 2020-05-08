<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.AffiliatePayments" CodeBehind="AffiliatePayments.aspx.cs" %>
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
                    <asp:LinkButton ID="lnkAddPayment" CssClass="hcTertiaryAction" resourcekey="AddPayment" runat="server" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        hcAttachUpdatePanelLoader();
    </script>

    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm">
        <div class="hcFormItemHor">
            <label class="hcLabelShort"><%=Localization.GetString("Search") %></label>
            <asp:TextBox ID="txtSearchText" runat="server" />
            <asp:LinkButton ID="btnFind" runat="server" resourcekey="Find" CssClass="hcButton hcSmall" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabelShort"><%=Localization.GetString("SearchBy") %></label>
            <asp:DropDownList ID="ddlSearchBy" runat="server">
                <asp:ListItem resourcekey="PaymentID" Value="0" />
                <asp:ListItem resourcekey="AffiliateID" Value="1" />
            </asp:DropDownList>
        </div>
        <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" HideButton="true" />
    </div>

    <div class="hcInfoLabel"><%=string.Format(Localization.GetString("AffiliatePaymentsFound"), AffiliatePaymentsCount) %></div>

    <asp:GridView ID="gvAffiliatePayments" runat="server" CssClass="hcGrid" DataKeyNames="Id" AutoGenerateColumns="false" OnRowDataBound="gvAffiliatePayments_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:BoundField DataField="Id" />
            <asp:BoundField DataField="AffiliateId" />
            <asp:BoundField DataField="PaymentDateUtc" />
            <asp:BoundField DataField="PaymentAmount" DataFormatString="{0:c}" />
            <asp:BoundField DataField="Notes" />
            <asp:TemplateField>
                <ItemStyle Width="110px" />
                <ItemTemplate>
                    <asp:LinkButton CommandName="EditPaymant" CommandArgument='<%#Eval("Id") %>' CssClass="hcIconEdit" runat="server" OnPreRender="EditPayment_OnPreRender" />
                    <asp:LinkButton runat="server" CssClass="hcIconDelete" CausesValidation="False" CommandName="Delete" OnPreRender="DeletePayment_OnPreRender" />
                    <asp:HyperLink NavigateUrl='<%#GetAttachmentUrl(Container) %>' Visible='<%#ShowAttachmentUrl(Container) %>' CssClass="hcIconAttachment" runat="server" OnPreRender="Attachment_OnPreRender" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <hcc:Pager ID="ucPager" PageSize="10" PostBackMode="true" runat="server" />

    <hcc:AffiliatePaymentDialog runat="server" id="ucPaymentDialog" />

</asp:Content>
