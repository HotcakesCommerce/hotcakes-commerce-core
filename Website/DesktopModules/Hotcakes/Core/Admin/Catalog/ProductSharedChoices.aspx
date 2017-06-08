<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductSharedChoices"
	Title="Untitled Page" CodeBehind="ProductSharedChoices.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu runat="server" ID="NavMenu" />

	<div class="hcBlock hcBlockLight hcPaddingBottom">
		<div class="hcForm">
			<div class="hcFormItem">
				<label class="hcLabel">+ New Shared Choice</label>
				<telerik:RadComboBox runat="server" ID="SharedChoiceTypes">
					<Items>
						<telerik:RadComboBoxItem Value="100" Text="Drop Down List" />
						<telerik:RadComboBoxItem Value="200" Text="Radio Button List" />
						<telerik:RadComboBoxItem Value="300" Text="Checkboxes" />
						<telerik:RadComboBoxItem Value="400" Text="Html Description" />
						<telerik:RadComboBoxItem Value="500" Text="Text Input" />
						<telerik:RadComboBoxItem Value="600" Text="File Upload" />
					</Items>
				</telerik:RadComboBox>
			</div>
		</div>
	</div>
	
	<div class="hcBlock">
		<div class="hcForm">
			<div class="hcFormItem">
				<asp:LinkButton ID="NewSharedChoiceImageButton" AlternateText="+ Add New Shared Choice" Text="+ Add New Shared Choice" runat="server" CssClass="hcTertiaryAction" EnableViewState="False" OnClick="NewSharedChoiceImageButton_Click" />
			</div>
		</div>
	</div>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript">
	function callBackFunRemoveItem(event) {
		event.data.param1.dialog('close');
		Remove($("#" + $("#deleteLinkId").val()));
	}

	function Remove(lnk) {
		var id = $(lnk).attr('id');
		$.post('ProductSharedChoices_Delete.aspx',
											{ "id": id.replace('rem', '') },
											function () {
												lnk.parent().parent().slideUp('slow');
												lnk.parent().parent().remove();
											}
											);
	}

	// Jquery Setup
	$(document).ready(function () {
		$('.trash').bind("click", function (event) {
			$("#deleteLinkId").val($(this).attr("id"));
			return hcConfirm(event, 'Deleting this shared choice will affect ALL products that are \nassociated with this shared choice and will result in loss of inventory for \nthose products. Are you sure you want to continue?', callBackFunRemoveItem)
		});
	});


</script>
	<uc1:MessageBox ID="MessageBox1" runat="server" />
	<h1>Shared Choices</h1>
	<asp:Literal ID="litResults" ClientIDMode="Static" runat="server" EnableViewState="false"></asp:Literal>
	<input id="deleteLinkId" type="hidden" value="" />
</asp:Content>
