<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderAddProduct.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderAddProduct" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagPrefix="hcc" TagName="MessageBox" %>

<hcc:MessageBox runat="server" ID="ucMessageBox" AddValidationSummaries="false" />

<asp:Panel ID="pnlAdd" runat="server" DefaultButton="btnAddProductBySku" CssClass="hcBlock">

	<script type="text/javascript">
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(BindTextChangeEvent);
		function BindTextChangeEvent() {

			var txtSKU = $("#<%= NewSkuField.ClientID %>");
        	var lnkBtnBrowse = $("#<%= btnBrowseProducts.ClientID %>");

        	if (txtSKU.val().length > 0)
        		lnkBtnBrowse.disable(false);
        	else
        		lnkBtnBrowse.disable(true);

        	txtSKU.bind('input propertychange', function () {
        		if (txtSKU.val().length > 0)
        			lnkBtnBrowse.disable(false);
        		else
        			lnkBtnBrowse.disable(true);
        	});
		}

		function AmountChange() {
			if ($("#<%=lstAmount.ClientID%>").val() != "") {
				$("#<%=GiftCardAmount.ClientID%>").hide();
			} else {
				$("#<%=GiftCardAmount.ClientID%>").show();
			}
		}

        jQuery(function ($) {
        	BindTextChangeEvent();
        });
	</script>

	<div class="hcForm hcClearfix">
		<div class="hcFormItemInline hcFormItem50p">
			<label class="hcLabel">Add SKU</label>
			<asp:TextBox ID="NewSkuField" runat="Server" Columns="20" TabIndex="200" />
			<asp:LinkButton ID="btnBrowseProducts" runat="server" Text="Browse" CausesValidation="False" CssClass="hcSecondaryAction hcSmall disabled" OnClick="btnBrowseProducts_Click" />
		</div>
		<div class="hcFormItemInline hcFormItem50p">
			<label class="hcLabel">Quantity</label>
			<asp:TextBox ID="NewProductQuantity" runat="server" Text="1" Columns="4" TabIndex="210" ValidationGroup="vgQuantity" />
			<asp:RangeValidator ID="rvNewProductQuantity" runat="server" Type="Integer" Display="Dynamic" MinimumValue="1" MaximumValue="99999999" ControlToValidate="NewProductQuantity" CssClass="hcFormError" ErrorMessage="Enter a valid number for Quantity." ValidationGroup="vgQuantity" EnableClientScript="false"></asp:RangeValidator>
			<asp:LinkButton CausesValidation="true" ID="btnAddProductBySku" runat="server" Text="+ Add To Order" CssClass="hcSecondaryAction hcSmall"
				TabIndex="220" OnClick="btnAddProductBySku_Click" ValidationGroup="vgQuantity" />
		</div>
		<asp:HiddenField ID="AddProductSkuHiddenField" runat="server" />
	</div>

	<asp:MultiView ID="mvPanels" ActiveViewIndex="0" runat="server">
		<asp:View ID="vwEmpty" runat="server">
		</asp:View>
		<asp:View ID="vwProductPicker" runat="server">
			<hr />
			<div class="hcForm hcClearfix hcSelectionArea">
			    <hcc:ProductPicker ID="ucProductPicker" runat="server" IsMultiSelect="false" DisplayPrice="true" DisplayInventory="false" />
				<ul class="hcActions">
					<li>
						<asp:LinkButton CausesValidation="true" ID="btnAddProductBySkuFormPicker" runat="server" Text="+ Add To Order" CssClass="hcSecondaryAction hcSmall"
							TabIndex="220" OnClick="btnProductPickerAddProduct_Click" ValidationGroup="vgQuantity" />
					</li>
					<li>
						<asp:LinkButton ID="btnProductPickerCancel" CausesValidation="false" runat="server" CssClass="hcTertiaryAction hcSmall"
							Text="Close" OnClick="btnProductPickerCancel_Click" />
					</li>
				</ul>
			</div>
		</asp:View>
		<asp:View ID="vwProductChoices" runat="server">
			<hr />
			<div class="hcForm hcClearfix hcSelectionArea">
				<h3>
					<asp:Literal ID="litProductInfo" runat="server" /></h3>

				<div class="hcFormOptions">
					<asp:PlaceHolder ID="phChoices" EnableViewState="false" runat="server" />
				</div>

				<asp:Panel ID="pnlUserPrice" runat="server" CssClass="hcFormItemHor" Visible="false">
					<asp:Label ID="lblUserPrice" CssClass="hcLabel" runat="server" />
					<asp:TextBox ID="txtUserPrice" runat="server" />
					<asp:RequiredFieldValidator ValidationGroup="ProdChoice" ErrorMessage="Price is required" ControlToValidate="txtUserPrice" Display="Dynamic"
						runat="server" />
					<%--            <asp:CompareValidator ValidationGroup="ProdChoice" ErrorMessage="Please enter a correct value" ControlToValidate="txtUserPrice" Display="Dynamic"
                Operator="DataTypeCheck" Type="" runat="server" />--%>
					<asp:CompareValidator ValidationGroup="ProdChoice" ErrorMessage="Price must be greater than 0" ControlToValidate="txtUserPrice" Display="Dynamic"
						Operator="GreaterThan" Type="Double" ValueToCompare="0" runat="server" />
				</asp:Panel>
				<br />
				<asp:Literal ID="litMessage" runat="server" EnableViewState="false"></asp:Literal>

				<ul class="hcActions">
					<li>
						<asp:LinkButton ValidationGroup="ProdChoice" ID="btnAddVariant" runat="server"
							Text="+ Add To Order" CssClass="hcSecondaryAction hcSmall"
							TabIndex="222" OnClick="btnAddVariant_Click" />
					</li>
					<li>
						<asp:LinkButton ID="btnCloseVariants" CausesValidation="false" runat="server" CssClass="hcTertiaryAction hcSmall"
							Text="Close" OnClick="btnCloseVariants_Click" />
					</li>
				</ul>
			</div>
		</asp:View>

		<asp:View ID="vwGiftCard" runat="server">
			<hr />
			<div class="hcForm hcClearfix hcSelectionArea">
				<div class="hcFormItem">
					<label class="hcLabel"><%=Localization.GetString("GiftCardAmount") %>:</label>
					<asp:DropDownList  runat="server" id="lstAmount" onchange="AmountChange();"></asp:DropDownList>
					<asp:TextBox ID="GiftCardAmount" runat="server"></asp:TextBox>
					<asp:CustomValidator id="cvGiftCardAmount" runat="server" OnServerValidate="cvGiftCardAmount_ServerValidate" CssClass="hcFormError" ValidationGroup="ValidationGiftCard"></asp:CustomValidator>
				</div>
				<div class="hcFormItem">
					<label class="hcLabel"><%=Localization.GetString("GiftCardEmail") %>:</label>
					<asp:TextBox ID="GiftCardRecEmail" runat="server"></asp:TextBox>
					<asp:RegularExpressionValidator ID="rvEmailID" runat="server" resourcekey="ValEmail.ErrorMessage" ControlToValidate="GiftCardRecEmail" EnableClientScript="true" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" CssClass="hcFormError" SetFocusOnError="true" ValidationGroup="ValidationGiftCard"></asp:RegularExpressionValidator>
					<asp:RequiredFieldValidator runat="server" id="rqvEmailID" resourcekey="ValEmail.ErrorMessage" controltovalidate="GiftCardRecEmail" EnableClientScript="true" errormessage="Please enter email address!" CssClass="hcFormError" ValidationGroup="ValidationGiftCard"/>
				</div>
				<div class="hcFormItem">
					<label class="hcLabel"><%=Localization.GetString("GiftCardName") %>:</label>
					<asp:TextBox ID="GiftCardRecName" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator runat="server" id="rqGiftCardName" controltovalidate="GiftCardRecName" EnableClientScript="true" resourcekey="ValRecName.ErrorMessage" CssClass="hcFormError" ValidationGroup="ValidationGiftCard"/>
				</div>
				<div class="hcFormItem">
					<label class="hcLabel"><%=Localization.GetString("GiftCardMessage") %>:</label>
					<asp:TextBox ID="GiftCardMessage" runat="server" Rows="5" TextMode="MultiLine" CssClass="hcOrderViewNotes" />
				</div>

				<ul class="hcActions">
					<li>
						<asp:LinkButton ID="btnCloseGiftCardDetails" CausesValidation="false" runat="server" CssClass="hcSecondaryAction hcSmall"
							resourcekey="CloseGiftCard" OnClick="btnCloseGiftCardDetails_Click" ValidationGroup="ValidationGiftCard"/>
					</li>
					<li>
						<asp:LinkButton  ID="btnAddGiftCard" runat="server"
							resourcekey="AddGiftCard" CssClass="hcSecondaryAction hcSmall"
							OnClick="btnAddGiftCard_Click" />
					</li>
				</ul>
			</div>
			<asp:HiddenField ID="IsGiftCardView" runat="server" Value="false" />
		</asp:View>

	</asp:MultiView>
</asp:Panel>
