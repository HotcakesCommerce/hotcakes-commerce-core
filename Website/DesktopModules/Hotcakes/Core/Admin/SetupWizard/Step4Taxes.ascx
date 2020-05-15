<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Step4Taxes.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.SetupWizard.Step4Taxes" %>
<%@ Register Src="../Controls/TaxScheduleEditor.ascx" TagPrefix="hcc" TagName="TaxScheduleEditor" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    function hcEditTaxScheduleDialog() {
        $("#hcEditTaxScheduleDialog").hcDialog({
            title: '<%=Localization.GetString("EditTaxSchedule") %>',
            width: 900,
            height: 'auto',
            minHeight: 20,
            parentElement: '#<%=upnlTaxContent.ClientID %>',
            close: function () {
                <%= Page.ClientScript.GetPostBackEventReference(btnTaxScheduleCancel, string.Empty) %>
            }
        });
    }
</script>


<div class="hcWizTaxes">
	<h2><%=Localization.GetString("TaxSchedules") %></h2>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcColumn hcColumnLeft" style="width:50%;">
        <asp:UpdatePanel ID="upnlTaxContent" UpdateMode="Always" runat="server">
            <ContentTemplate>
                <div class="hcForm">
                    <div class="hcFormItem">
                        <asp:CheckBox runat="server" ID="chkApplyVATRules" resourcekey="chkApplyVATRules" AutoPostBack="true" OnCheckedChanged="chkApplyVATRules_CheckedChanged"/>
                    </div>
                    <div class="hcFormItem">
						<div style="float: left; width:49%;">
							<asp:TextBox ID="txtDisplayName" runat="server" MaxLength="50" />
							<asp:RequiredFieldValidator ID="DisplayNameValidator" runat="server" CssClass="hcFormError"
                            ControlToValidate="txtDisplayName" ValidationGroup="DisplayName" Display="Dynamic" />
						</div>
						&nbsp;&nbsp;
						<div style="float: right; width:49%;">
							<asp:LinkButton ID="btnCreate" resourcekey="btnCreate" CssClass="hcButton hcSmall" OnClick="btnCreate_Click" runat="server" ValidationGroup="DisplayName" />
						</div>
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView runat="server" ID="gridTaxes" AutoGenerateColumns="False" GridLines="None" CssClass="hcGrid" 
                            OnDeleteCommand="gridTaxes_ItemDelete" OnEditCommand="gridTaxes_ItemEdit" DataKeyNames="Id">
                            <HeaderStyle CssClass="hcGridHeader"/>
                            <RowStyle CssClass="hcGridItem"/>
                            <AlternatingRowStyle CssClass="hcGridAltItem"/>
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="ScheduleName" />
                                <asp:TemplateField>
                                    <ItemStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" CssClass="hcIconEdit" runat="server" CausesValidation="False" CommandName="Edit" OnPreRender="btnEdit_OnPreRender" />
                                        <asp:LinkButton ID="btnDelete" CssClass="hcIconDelete" runat="server" CausesValidation="False" CommandName="Delete" OnPreRender="btnDelete_OnPreRender" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <asp:Panel id="hcEditTaxScheduleDialog" runat="server" ClientIDMode="Static" Visible="false">
                    <hcc:TaxScheduleEditor runat="server" id="ucTaxScheduleEditor" />
                    <ul class="hcActions">
                        <li>
                            <asp:LinkButton ID="btnTaxScheduleSave" resourcekey="btnTaxScheduleSave" CssClass="hcPrimaryAction" runat="server"/>
                        </li>
                        <li>
                            <asp:LinkButton ID="btnTaxScheduleCancel" resourcekey="btnTaxScheduleCancel" CssClass="hcSecondaryAction" runat="server" CausesValidation="false" />
                        </li>
                    </ul>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="hcActionsRight">
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnExit" resourcekey="btnExit" CssClass="hcSecondaryAction" runat="server" OnClick="btnExit_Click" />
            </li>
        </ul>
    </div>
</div>
