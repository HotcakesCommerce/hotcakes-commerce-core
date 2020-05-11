<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductSharedChoices"
	Title="Untitled Page" CodeBehind="ProductSharedChoices.aspx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu runat="server" ID="NavMenu" />

	<div class="hcBlock hcBlockLight hcPaddingBottom">
		<div class="hcForm">
			<div class="hcFormItem">
				<label class="hcLabel"><%=Localization.GetString("NewSharedChoiceImageButton") %></label>
				<asp:DropDownList runat="server" ID="SharedChoiceTypes">
					<Items>
						<asp:ListItem Value="100" Text="Drop Down List" />
						<asp:ListItem Value="200" Text="Radio Button List" />
						<asp:ListItem Value="300" Text="Checkboxes" />
						<asp:ListItem Value="400" Text="Html Description" />
						<asp:ListItem Value="500" Text="Text Input" />
						<asp:ListItem Value="600" Text="File Upload" />
					</Items>
				</asp:DropDownList>
			</div>
		</div>
	</div>
	
	<div class="hcBlock">
		<div class="hcForm">
			<div class="hcFormItem">
				<asp:LinkButton ID="NewSharedChoiceImageButton" resourcekey="NewSharedChoiceImageButton" runat="server" CssClass="hcTertiaryAction" EnableViewState="False" OnClick="NewSharedChoiceImageButton_Click" />
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
                if ($(".hcGrid tr").length == 0) {
                    location.reload();
                }
            }
		);
	}

	// Jquery Setup
	$(document).ready(function () {
		$('.trash').bind("click", function (event) {
			$("#deleteLinkId").val($(this).attr("id"));
			return hcConfirm(event, "<%=Localization.GetJsEncodedString("Confirm")%>", callBackFunRemoveItem)
		});
	});
</script>
    <h1><%=PageTitle %></h1>
	<uc1:MessageBox ID="MessageBox1" runat="server" />
    <asp:Literal ID="litResults" ClientIDMode="Static" runat="server" EnableViewState="false" />
	<input id="deleteLinkId" type="hidden" value="" />
</asp:Content>
