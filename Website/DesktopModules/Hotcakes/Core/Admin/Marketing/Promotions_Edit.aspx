<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Promotions_Edit.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Promotions_Edit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="Promotions_Edit_Qualification.ascx" TagName="Promotions_Edit_Qualification" TagPrefix="hcc" %>
<%@ Register Src="Promotions_Edit_Actions.ascx" TagName="Promotions_Edit_Actions" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        hcAttachUpdatePanelLoader();

        function setPopupHeightForQualification() {
            var popupHeight = $("#hcQualificationDialog").height();
            if (typeof popupHeight != "undefined") {
                if (popupHeight > 500) {
                    $("#hcQualificationDialog").height(popupHeight);
                }
            }
        };

        function setPopupHeightForAction() {
            var popupHeight = $("#hcActionDialog").height();
            if (typeof popupHeight != "undefined") {
                if (popupHeight > 500) {
                    $("#hcActionDialog").height(popupHeight);
                }
            }
        };

        var hcShowQualificationDialog = function () {
            $("#hcQualificationDialog").hcDialog({
                title: "<%=Localization.GetString("EditQualification.Text")%>",
                width: 800,
                height: 'auto',
                maxHeight: 'auto',
                resizable: true,
                parentElement: '#<%=pnlEditQualification.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(btnCloseQualificationEditor, "") %>
                }
            });
        };
        var hcShowActionDialog = function (curwidth) {

            var target = $('#hcActionDialog table.hc-popup-table');
            var width = 500;
            if (curwidth == undefined || curwidth == null) {
                if (target != null) {
                    width = target.attr('hc-popup-width');

                    if (typeof width == "undefined") {
                        width = 500;
                    }
                }
            }
            else {
                width = curwidth;
            }

            $("#hcActionDialog").hcDialog({
                title: "<%=Localization.GetString("EditAction")%>",
                width: width,
                height: 'auto',
                maxHeight: 800,
                resizable: true,
                parentElement: '#<%=pnlEditAction.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(btnCloseActionEditor, "") %>
                }
            });
        };
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" AddValidationSummaries="false" />

    <div class="hcForm">
        <div class="hcFormItem">
            <asp:CheckBox ID="chkEnabled" resourcekey="chkEnabled" runat="server" />
            <asp:CheckBox ID="chkDoNotCombine" resourcekey="chkDoNotCombine" runat="server" />
        </div>
    </div>
    <div class="clearfix"></div>
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("PromotionName") %></label>
                <asp:TextBox ID="txtName" runat="server" MaxLength="225" Style="width: 100%" ValidationGroup="SavePromotion" />
                <asp:RequiredFieldValidator ID="valNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="txtName" resourcekey="valNameRequired" CssClass="hcFormError" ValidationGroup="SavePromotion"  />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("CustomerDescription") %><i class="hcLocalizable"></i></label>
                <asp:TextBox ID="txtCustomerDescription" runat="server" MaxLength="225" Style="width: 100%" ValidationGroup="SavePromotion"/>
                <asp:RequiredFieldValidator ID="valDescriptionRequired" runat="server" Display="Dynamic"
                    ControlToValidate="txtCustomerDescription" resourcekey="valDescriptionRequired" CssClass="hcFormError"  ValidationGroup="SavePromotion"/>
            </div>
        </div>
    </div>
    <div class="hcColumnRight" style="width: 49%">
        <div class="hcForm">
            <div class="hcFormItem" style="width: 20%">
                <label class="hcLabel"><%=Localization.GetString("StartDate") %></label>
                <telerik:RadDatePicker ID="radDateStart" runat="server" CssClass="hcPromotionsStartDate" ValidationGroup="SavePromotion" />
                <asp:RequiredFieldValidator ID="valDateStartRequired" runat="server" Display="Dynamic"
                    ControlToValidate="radDateStart" resourcekey="valDateStartRequired" CssClass="hcFormError"  ValidationGroup="SavePromotion"/>
            </div>
            <div class="hcFormItem" style="width: 20%">
                <label class="hcLabel"><%=Localization.GetString("EndDate") %></label>
                <telerik:RadDatePicker ID="radDateEnd" runat="server" CssClass="hcPromotionsEndDate" ValidationGroup="SavePromotion"/>
                <asp:RequiredFieldValidator ID="valDateEndRequired" runat="server" Display="Dynamic"
                    ControlToValidate="radDateEnd" resourcekey="valDateEndRequired" CssClass="hcFormError" ValidationGroup="SavePromotion" />
                <asp:CompareValidator ID="valDateCompare" runat="server" Display="Dynamic" ControlToValidate="radDateEnd" ControlToCompare="radDateStart"
                    resourcekey="valDateCompare" Type="Date" Operator="GreaterThan" CssClass="hcFormError" ValidationGroup="SavePromotion"/>
            </div>
        </div>
    </div>

    <div class="clearfix"></div>
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Qualifications") %></h2>
            <asp:UpdatePanel ID="pnlQualifications" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="hcFormItem">
                        <%=Localization.GetString("QualificationHelp") %>
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <telerik:RadComboBox ID="lstNewQualification" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnNewQualification" runat="server"
                            resourcekey="btnNewQualification" CssClass="hcButton hcSmall"
                            OnClick="btnNewQualification_Click" />
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView ID="gvQualifications" DataKeyNames="Id" AutoGenerateColumns="false" CssClass="hcGrid hcQualificationsGrid" runat="server" OnRowDataBound="gvQualifications_OnRowDataBound">
                            <HeaderStyle CssClass="hcGridHeader" />
                            <RowStyle CssClass="hcGridRow" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# GetQualificationDescription(Container) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Width="80px" HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" CssClass="hcIconEdit"
                                            CommandArgument='<%#Container.DataItemIndex %>' CommandName="Edit"
                                            Visible='<%#HasQualificationOptions(Container) %>' runat="server" OnPreRender="lnkEdit_OnPreRender" />
                                        <asp:LinkButton ID="lnkDelete" Text="Delete" CssClass="hcIconDelete"
                                            CommandArgument='<%#Container.DataItemIndex %>' CommandName="Delete"
                                            runat="server" OnPreRender="lnkDelete_OnPreRender" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Panel ID="pnlEditQualification" runat="server" Visible="false">
                        <div id="hcQualificationDialog" class="dnnClear" style="overflow: auto;">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <hcc:Promotions_Edit_Qualification ID="Promotions_Edit_Qualification1" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="hcActionsRight">
                                <ul class="hcActions">
                                    <li>
                                        <asp:LinkButton ID="btnSaveQualification" runat="server"
                                            resourcekey="btnSave" CssClass="hcPrimaryAction" OnClick="btnSaveQualification_Click" />
                                    </li>
                                    <li>
                                        <asp:LinkButton ID="btnCloseQualificationEditor" runat="server"
                                            resourcekey="btnClose" CssClass="hcSecondaryAction" />
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Actions") %></h2>
            <asp:UpdatePanel ID="pnlActions" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="hcFormItem">
                        <%=Localization.GetString("ActionHelp") %>
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <telerik:RadComboBox ID="lstNewAction" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnNewAction" runat="server"
                            resourcekey="btnNewAction" CssClass="hcButton hcSmall" OnClick="btnNewAction_Click"  />
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView ID="gvActions" DataKeyNames="Id" AutoGenerateColumns="false" CssClass="hcGrid hcPromoItemGrid" runat="server" OnRowDataBound="gvActions_OnRowDataBound" AlternatingRowStyle-Wrap="true" RowStyle-Wrap="true">
                            <HeaderStyle CssClass="hcGridHeader" />
                            <RowStyle CssClass="hcGridRow" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# GetActionDescription(Container) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:LinkButton CssClass="hcIconEdit"
                                            CommandArgument='<%#Container.DataItemIndex %>' CommandName="Edit" runat="server" OnPreRender="lnkEdit_OnPreRender" />
                                        <asp:LinkButton CssClass="hcIconDelete"
                                            CommandArgument='<%#Container.DataItemIndex %>' CommandName="Delete"
                                            runat="server" OnPreRender="lnkDelete_OnPreRender" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Panel ID="pnlEditAction" runat="server" Visible="false">
                        <div id="hcActionDialog" class="dnnClear" style="overflow: auto;">
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <hcc:Promotions_Edit_Actions ID="Promotions_Edit_Actions1" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="hcActionsRight">
                                <ul class="hcActions">
                                    <li>
                                        <asp:LinkButton ID="btnSaveAction" runat="server"
                                            resourcekey="btnSave" CssClass="hcPrimaryAction" OnClick="btnSaveAction_Click" />
                                    </li>
                                    <li>
                                        <asp:LinkButton ID="btnCloseActionEditor" runat="server"
                                            resourcekey="btnClose" CssClass="hcSecondaryAction" />
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>


    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" runat="server" resourcekey="btnSave" ValidationGroup="SavePromotion" CssClass="hcPrimaryAction" OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:HyperLink ID="lnkBack" runat="server" resourcekey="lnkBack" CssClass="hcSecondaryAction" NavigateUrl="Promotions.aspx" />
        </li>
    </ul>
</asp:Content>
