<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="GiftCards.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.GiftCards" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/Pager.ascx" TagName="Pager" TagPrefix="hcc" %>
<%@ Register Src="../Controls/DateRangePicker.ascx" TagName="DateRangePicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CompareControl.ascx" TagPrefix="hcc" TagName="CompareControl" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="reports/" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton CssClass="hcTertiaryAction" ID="lnkExportToExcel" resourcekey="ExportToExcel" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        var hcEditGiftCardDialog = function () {
            $("#hcEditGiftCardDialog").hcDialog({
                title: "<%=Localization.GetString("EditGiftCard") %>",
                width: 500,
                height: 'auto',
                maxHeight: 636,
                parentElement: '#<%=pnlEditGiftCard.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(lnkCancel, "") %>
                }
            });
        };
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm" id="divFilterPanel" runat="server">
        <div class="hcFormItemHor">
            <label class="hcLabelShort"><%=Localization.GetString("Search") %></label>
            <asp:TextBox ID="txtSearchText" runat="server" Width="200px" />
            <asp:LinkButton ID="btnFind" runat="server" resourcekey="Find" CssClass="hcButton hcSmall" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabelShort"><%=Localization.GetString("Amount") %></label>
            <hcc:CompareControl runat="server" ID="ucAmountComare" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabelShort"><%=Localization.GetString("Balance") %></label>
            <hcc:CompareControl runat="server" ID="ucBalanceComare" />
        </div>
        <hcc:DateRangePicker ID="ucDateRangePicker" runat="server" RangeType="ThisMonth" HideButton="true" />
        <div class="hcFormItemHor">
            <label class="hcLabelShort"><%=Localization.GetString("Status") %></label>
            <asp:CheckBox ID="cbShowDisabled" resourcekey="cbShowDisabled" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabelShort"></label>
            <asp:CheckBox ID="cbShowExpired" resourcekey="cbShowExpired" runat="server" />
        </div>
    </div>

    <asp:Label ID="lblNoGiftcards" CssClass="hcInfoLabelLeft" runat="server">
        <%=Localization.GetString("NoGiftCardsFound") %>
    </asp:Label>

    <asp:GridView ID="gvGiftCards" AutoGenerateColumns="false" DataKeyNames="GiftCardId" CssClass="hcGrid" runat="server" OnRowDataBound="gvGiftCards_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:BoundField DataField="IssueDateUtc" DataFormatString="{0:d}" />
            <asp:BoundField DataField="RecipientName" />
            <asp:BoundField DataField="CardNumber" />
            <asp:BoundField DataField="Amount" DataFormatString="{0:c}" />
            <asp:BoundField DataField="UsedAmount" DataFormatString="{0:c}" />
            <asp:CheckBoxField DataField="Enabled" />
            <asp:TemplateField>
                <ItemStyle Width="40px" />
                <ItemTemplate>
                    <asp:LinkButton Text='<%#Eval("OrderNumber") %>' CommandName="GoToOrder" CommandArgument='<%#Eval("OrderNumber") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Width="40px" />
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="btnEdit" CssClass="hcIconEdit"
                        CommandName="Edit" OnPreRender="btnEdit_OnPreRender" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <hcc:Pager ID="ucPager" PageSize="10" PostBackMode="true" runat="server" />

    <asp:Panel ID="pnlFilteredByLineItem" Visible="false" runat="server">
        <span class="hcInfoLabelLeft"><%=Localization.GetString("GiftCardsFilteredByOrderItem") %></span>
        <a href="GiftCards.aspx"><%=Localization.GetString("lnkShowAll") %></a>
    </asp:Panel>

    <asp:Panel ID="pnlEditGiftCard" runat="server" Visible="false">
        <div id="hcEditGiftCardDialog" class="dnnClear">
            <div class="hcForm">
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("Enabled") %></label>
                    <asp:CheckBox ID="cbEnabled" runat="server" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("CardNumber") %></label>
                    <asp:Label ID="lblCardNumber" runat="server" CssClass="hcFormItemValue" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("IssueDate") %></label>
                    <asp:Label ID="lblIssueDate" runat="server" CssClass="hcFormItemValue" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("Expiration") %></label>
                    <asp:TextBox ID="dpExpiration" runat="server" CssClass="hcGiftCardExpirationDateAlign" TextMode="Date" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("Amount") %></label>
                    <asp:TextBox ID="txtAmount" runat="server" CssClass="hcGiftCardAmount" />
                    <asp:RequiredFieldValidator ID="rfvAmount" ControlToValidate="txtAmount" CssClass="hcFormError"
                        ValidationGroup="EditCard" runat="server" />
                    <asp:CompareValidator ID="cvAmount" ControlToValidate="txtAmount"
                        Operator="DataTypeCheck" Type="Currency" CssClass="hcFormError" ValidationGroup="EditCard" runat="server" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("UsedAmount") %></label>
                    <asp:Label ID="lblUsedAmount" runat="server" CssClass="hcFormItemValue" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("RecipientEmail") %></label>
                    <asp:Label ID="lblRecipientEmail" runat="server" CssClass="hcFormItemValue" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("RecipientName") %></label>
                    <asp:Label ID="lblRecipientName" runat="server" CssClass="hcFormItemValue" />
                </div>
                <div class="hcFormItemHor">
                    <label class="hcLabel"><%=Localization.GetString("OrderNumber") %></label>
                    <asp:Label ID="lblOrderNumber" runat="server" CssClass="hcFormItemValue" />
                </div>
                <ul class="hcActions">
                    <li>
                        <asp:LinkButton ID="lnkSave" resourcekey="lnkSave" ValidationGroup="EditCard" CssClass="hcPrimaryAction" runat="server" />
                    </li>
                    <li>
                        <asp:LinkButton ID="lnkCancel" resourcekey="lnkCancel" CssClass="hcSecondaryAction" runat="server" />
                    </li>
                </ul>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
