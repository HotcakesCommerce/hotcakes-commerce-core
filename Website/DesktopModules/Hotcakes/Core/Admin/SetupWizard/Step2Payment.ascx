<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Step2Payment.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.SetupWizard.Step2Payment" %>
<%@ Register Src="../Controls/PaymentMethodEditor.ascx" TagName="PaymentMethodEditor" TagPrefix="hcc" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    function hcEditPaymentDialog() {
        $("#hcEditPaymentDialog").hcDialog({
            title: $("#hcEditPaymentDialog h1").text(),
            width: 1100,
            height: 'auto',
            minHeight: 20,
            parentElement: '#<%=upPayment.ClientID %>',
            open: function () {
                $("#hcEditPaymentDialog h1").remove();
            },
            close: function () {
                <%= Page.ClientScript.GetPostBackEventReference(btnCloseDialog, "") %>
            }
        });
    }
</script>



<div class="hcForm hcWizPayment">
	<h2><%=Localization.GetString("PaymentOptions") %></h2>

    <asp:UpdatePanel ID="upPayment" runat="server">
        <ContentTemplate>
		<div class="hcForm">
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="Display" CssClass="hcLabel" />
                <asp:GridView ID="PaymentMethodsGrid" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                    DataKeyNames="MethodId" CellPadding="3" BorderWidth="0px" GridLines="None"
                    OnRowDataBound="PaymentMethodsGrid_RowDataBound"
                    OnRowEditing="PaymentMethodsGrid_RowEditing">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
								<div class="hcCheckboxOuter">
									<asp:CheckBox runat="server" ID="chkEnabled" />
									<span></span>
								</div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMethodName" AssociatedControlID="chkEnabled" CssClass="hcWizPaymentLabel" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="false" CommandName="Edit">
                                    <i class="hcIconEdit"></i><%=Localization.GetString("Edit") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="hcEditPaymentDialog" style="display:none;">
                <hcc:PaymentMethodEditor ID="methodEditor" runat="server" OnEditingComplete="methodEditor_EditingComplete" />
                <asp:LinkButton ID="btnCloseDialog" Style="display: none" runat="server" />
            </div>
		</div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="hcActionsRight">
        <ul class="hcActions clear">
            <li>
                <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnLater" resourcekey="btnLater" CssClass="hcSecondaryAction" runat="server" OnClick="btnLater_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnExit" resourcekey="btnExit" CssClass="hcSecondaryAction" runat="server" OnClick="btnExit_Click" />
            </li>
        </ul>
    </div>
</div>
