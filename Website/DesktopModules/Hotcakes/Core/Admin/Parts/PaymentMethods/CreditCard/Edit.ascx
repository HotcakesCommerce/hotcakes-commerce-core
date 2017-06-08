<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.PaymentMethods.CreditCard.Edit" CodeBehind="Edit.ascx.cs" %>

<%@ Register Src="../../../Controls/CreditCardGatewayEditor.ascx" TagName="CreditCardGatewayEditor" TagPrefix="hcc" %>

<asp:MultiView runat="server" ID="mvEditors" ActiveViewIndex="0">
	<asp:View runat="server" ID="vCreditCardEditor">
		<h1>
			<asp:Label runat="server" resourcekey="CreditCardOptions" />
		</h1>
		<div class="hcColumnLeft hcRightBorder" style="width: 40%">
			<div class="hcForm">
				<asp:Label ID="Label1" runat="server" resourcekey="AcceptedCards" CssClass="hcLabel" />
				<div class="hcFormItem">
					<span class="cc-visa right"></span>&nbsp;<asp:CheckBox ID="chkCardVisa" runat="server" resourcekey="CardVisa" /><br />
				</div>
				<div class="hcFormItem">
					<span class="cc-mastercard right"></span>&nbsp;<asp:CheckBox ID="chkCardMasterCard" runat="server" resourcekey="CardMasterCard" /><br />
				</div>
				<div class="hcFormItem">
					<span class="cc-amex right"></span>&nbsp;<asp:CheckBox ID="chkCardAmex" runat="server" resourcekey="CardAmex" /><br />
				</div>
				<div class="hcFormItem">
					<span class="cc-discover right"></span>&nbsp;<asp:CheckBox ID="chkCardDiscover" runat="server" resourcekey="CardDiscover" /><br />
				</div>
				<div class="hcFormItem">
					<span class="cc-diners right"></span>&nbsp;<asp:CheckBox ID="chkCardDiners" runat="server" resourcekey="CardDiners" /><br />
				</div>
				<div class="hcFormItem">
					<span class="cc-jcb right"></span>&nbsp;<asp:CheckBox ID="chkCardJCB" runat="server" resourcekey="CardJCB" /><br />
				</div>
			</div>
		</div>
		<div class="hcColumnRight" style="width: 59%">
			<div class="hcForm">
				<asp:Label ID="Label2" runat="server" resourcekey="CaptureMode" CssClass="hcLabel" />
				<div class="hcFormItem">
					<asp:RadioButtonList runat="server" ID="lstCaptureMode">
						<asp:ListItem resourcekey="CaptureModeAuthorize" Value="1" />
						<asp:ListItem resourcekey="CaptureModeCharge" Value="0" Selected="True" />
					</asp:RadioButtonList>
				</div>
				<div class="hcFormItemLabel">
					<asp:Label runat="server" resourcekey="Gateway" CssClass="hcLabel" />
				</div>
				<div class="hcFormItem hcFormItem66p">
					<asp:DropDownList ID="lstGateway" runat="server" />
					<asp:RequiredFieldValidator runat="server" ID="rfvGateway" ControlToValidate="lstGateway" resourcekey="rfvGateway" CssClass="hcFormError" />
					&nbsp;&nbsp;
					<asp:LinkButton runat="server" ID="btnOptions" resourcekey="Edit" OnClick="btnOptions_Click" CssClass="hcSecondaryAction hcSmall" />
				</div>
				<div class="hcFormItem">
					<asp:CheckBox ID="chkRequireCreditCardSecurityCode" runat="server" resourcekey="chkRequireCreditCardSecurityCode" />
				</div>
				<div class="hcFormItem">
					<asp:CheckBox ID="chkDisplayFullCardNumber" runat="server" resourcekey="chkDisplayFullCardNumber" />
				</div>
			</div>
		</div>
		<ul class="hcActions">
			<li>
				<asp:LinkButton runat="server" ID="btnSave" resourcekey="Save" OnClick="btnSave_Click" CssClass="hcPrimaryAction" />
			</li>
			<li>
				<asp:LinkButton runat="server" ID="btnCancel" CausesValidation="false" resourcekey="Cancel" OnClick="btnCancel_Click" CssClass="hcSecondaryAction" />
			</li>
		</ul>
	</asp:View>
	<asp:View runat="server" ID="vGatewayEditor">
		<hcc:CreditCardGatewayEditor ID="gatewayEditor" runat="server" OnEditingComplete="gatewayEditor_EditingComplete"></hcc:CreditCardGatewayEditor>
	</asp:View>
</asp:MultiView>


