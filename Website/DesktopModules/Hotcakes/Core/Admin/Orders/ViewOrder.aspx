<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.ViewOrder" Title="Untitled Page" CodeBehind="ViewOrder.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="OrderStatusDisplay.ascx" TagName="OrderStatusDisplay" TagPrefix="hcc" %>
<%@ Register Src="PaymentInformation.ascx" TagName="PaymentInformation" TagPrefix="hcc" %>
<%@ Register Src="OrderActions.ascx" TagName="OrderActions" TagPrefix="hcc" %>
<%@ Register Src="OrderItems.ascx" TagPrefix="hcc" TagName="OrderItems" %>
<%@ Register Src="OrderItemInventory.ascx" TagPrefix="hcc" TagName="InventoryEditorControl" %>

<asp:Content ID="Nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:OrderActions ID="ucOrderActions" CurrentOrderPage="Details" runat="server" />
    <div class="hcBlock">
        <hcc:PaymentInformation ID="ucPaymentInformation" runat="server" />
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel">Codes Used</label>
                <div class="hcCouponCodes">
                    <asp:Label ID="CouponField" runat="server" />
                </div>
            </div>
        </div>
    </div>

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton
                    ID="btnDelete" runat="server" CssClass="hcTertiaryAction"
                    Text="Delete Order" CausesValidation="false" OnClick="btnDelete_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

	 <script type="text/javascript">
	 	hcAttachUpdatePanelLoader();

	 	function closeInventoryEditorDialog() {
	 		$("#InventoryEidtor").hcDialog('close');
	 	}

	 	function openInventoryEditorDialog() {
	 		$("#InventoryEidtor").hcDialog({
	 			autoOpen: true,
	 			height: 'auto',
	 			minHeight: 200,
	 			width: 800
	 		});
	 	}

	 	function closeRefundAmountDialog() {
	 		$("#RefundAmount").hcDialog('close');
	 	}

	 	function openRefundAmountDialog() {
	 		$("#RefundAmount").hcDialog({
	 			autoOpen: true,
	 			height: 'auto',
	 			minHeight: 200,
	 			width: 500
	 		});
	 	}

	</script>

    <h1>Order
        <asp:Label ID="OrderNumberField" runat="server" Text="000000" />
    </h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <hr />
    <div class="hcColumn" style="width: 30%">
        <asp:Label ID="TimeOfOrderField" runat="server" />
        <br />
        Fraud Score:
        <asp:Label ID="lblFraudScore" runat="server" />
    </div>
    <div class="hcColumnRight" style="width: 70%;">
        <hcc:OrderStatusDisplay ID="ucOrderStatusDisplay" runat="server" />
    </div>
    <div class="hcClearfix"></div>

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2>Bill To</h2>
            <asp:Label ID="lblBillingAddress" runat="server" />
            <asp:Literal ID="ltEmailAddress" runat="server" />
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2>Ship To</h2>
            <asp:Label ID="lblShippingAddress" runat="server" />
        </div>
    </div>
    <div class="hcClearfix"></div>
    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <hcc:OrderItems runat="server" ID="ucOrderItems" EditMode="False" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="hcColumnLeft" style="width: 50%">
        <asp:Panel ID="pnlInstructions" CssClass="hcForm" runat="server" Visible="false">
            <div class="hcFormItem">
                <label class="hcLabel">Customer's Instructions</label>
                <asp:Label ID="lblInstructions" runat="server" CssClass="hcInstructions" />
            </div>
        </asp:Panel>
    </div>
    <div class="hcColumnRight" style="width: 50%">
        <div class="hcForm">
            <asp:Literal ID="litTotals" runat="server" />
            <hr />
            <div class="hcFormItem hcFormItem66p" id="EmailTemplates">
                <asp:DropDownList ID="lstEmailTemplate" runat="server" CssClass="RadComboBox" />
            </div>
            <div class="hcFormItem hcFormItem33p" id="EmailSend">
                <asp:LinkButton CssClass="hcButton hcSmall" ID="btnSendStatusEmail" runat="server" ToolTip="Send Update by E-Mail"
                    Text="E-Mail" OnClick="btnSendStatusEmail_Click" />
            </div>
        </div>
    </div>

    <hr />
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2>Private Notes</h2>
            <asp:GridView ID="PrivateNotesField" runat="server" CssClass="hcGrid"
                ShowHeader="false" AutoGenerateColumns="False"
                DataKeyNames="Id" OnRowDeleting="PrivateNotesField_RowDeleting">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblAuditDate" runat="server" Text='<%# Bind("AuditDate","{0:d}") %>' /><br />
                            <asp:Label ID="NoteField" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="30px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteNote" OnClientClick="return hcConfirm(event, 'Delete this note?');"
                                runat="server" CssClass="hcIconDelete" CommandArgument='<%# Bind("Id") %>'
                                CausesValidation="False" CommandName="Delete" Text="Delete" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="hcFormItem">
                <asp:TextBox ID="NewPrivateNoteField" runat="server" ToolTip="Add a new note to this order" Rows="3" TextMode="MultiLine" CssClass="hcOrderViewNotes" />
            </div>

            <asp:LinkButton ID="btnNewPrivateNote" runat="server" Text="New" CssClass="hcSecondaryAction hcSmall" OnClick="btnNewPrivateNote_Click" />
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2>Public Notes</h2>
            <asp:GridView ID="PublicNotesField" runat="server" ShowHeader="False" AutoGenerateColumns="False" CssClass="hcGrid"
                DataKeyNames="Id" OnRowDeleting="PublicNotesField_RowDeleting">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblAuditDate" runat="server" Text='<%# Bind("AuditDate","{0:d}") %>'></asp:Label><br />
                            <asp:Label ID="NoteField" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="30px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteNote" OnClientClick="return hcConfirm(event, 'Delete this note?');"
                                runat="server" CssClass="hcIconDelete" CommandArgument='<%# Bind("Id") %>'
                                CausesValidation="False" CommandName="Delete" Text="Delete" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="hcFormItem">
                <asp:TextBox ID="NewPublicNoteField" runat="server" ToolTip="Add a new note to this order" Rows="3" TextMode="MultiLine" CssClass="hcOrderViewNotes" />
            </div>

            <asp:LinkButton ID="btnNewPublicNote" runat="server" Text="New" CssClass="hcSecondaryAction hcSmall" OnClick="btnNewPublicNote_Click" />
        </div>
    </div>

	<div id="RefundAmount" class="hcForm" style="display: none">
		<asp:UpdatePanel ID="upnlRefundAmount" UpdateMode="Always" runat="server">
			<ContentTemplate>
				<asp:PlaceHolder ID="phrRefundAmountControls" runat="server" EnableViewState="True">
					<asp:Label ID="Label1" CssClass="hcLabel" runat="server" Text="The customer hasn't been refunded yet. Would you like to refund the customer first?" />
					<br />
					<ul class="hcActions">
						<li>
							<asp:LinkButton runat="server" ID="btnOK" CssClass="hcPrimaryAction" Text="Yes" OnClick="btnOK_Click"/>
						</li>
						<li>
							<asp:LinkButton runat="server" ID="btnCancel" CssClass="hcSecondaryAction" OnClientClick="closeRefundAmountDialog()" Text="No" OnClick="btnCancel_Click"/>
						</li>
					</ul>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="phrRefundAmountScript" runat="server" EnableViewState="False"></asp:PlaceHolder>
			</ContentTemplate>
		</asp:UpdatePanel>
	</div>


	<div id="InventoryEidtor" class="hcForm" style="display: none">
		<asp:UpdatePanel ID="upnlInventoryEditor" UpdateMode="Always" runat="server">
			<ContentTemplate>
				<asp:PlaceHolder ID="phrInventoryEditor" runat="server" EnableViewState="True">
					<hcc:InventoryEditorControl runat="server" ID="InventoryControl" AllowUpdateQuantities="False" />
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="phrInventoryScripts" runat="server" EnableViewState="False"></asp:PlaceHolder>
			</ContentTemplate>
		</asp:UpdatePanel>
	</div>

</asp:Content>
