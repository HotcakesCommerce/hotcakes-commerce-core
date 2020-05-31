<%@ Page Title="Product Tabs" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductsEdit_Tabs" CodeBehind="ProductsEdit_Tabs.aspx.cs" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="uc5" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="InfoTabs" />
	 <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
				<asp:LinkButton ID="NewTabButton" runat="server" class="hcTertiaryAction" resourcekey="AddNewTab" AlternateText="+ New Tab"	OnClick="NewTabButton_Click" />
			</div>
		</div>
	</div>
	<uc5:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
	<script type="text/javascript">
		hcAttachUpdatePanelLoader();

		jQuery(function () {
			$("#dragitem-list").sortable({
				placeholder:  'ui-state-highlight',
			axis:   'y',
			opacity:  '0.75',
			cursor:  'move',
			update: function (event, ui) {
				var productBvin = getParameterByName("id");
				var sorted = $(this).sortable('toArray');
				sorted += '';
				$.post('ProductsEdit_TabsSort.aspx',
				  {
				  	"ids": sorted,
				  	"bvin": productBvin
				  });
			}
		});

		$('#dragitem-list').disableSelection();

		$('.trash').click(function() {
			RemoveItem($(this));
			return false;
		});

		});
		// End of Document Ready

		function RemoveItem(lnk) {
			var id = $(lnk).attr('id');
			var productBvin = getParameterByName("id");
			$.post('ProductsEdit_TabsDelete.aspx',
              {
              	"id": id.replace('rem', ''),
              	"bvin": productBvin
              },
              function () {
              	lnk.parent().parent().parent().parent().parent().slideUp('slow', function () {
              		lnk.parent().parent().parent().parent().parent().remove();
              	});
              }
             );
		}

		function getParameterByName(name) {
			name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
			var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
				results = regex.exec(location.search);
			return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
		}
	</script>

	<h1><%=PageTitle %></h1>
	<uc1:MessageBox ID="MessageBox1" runat="server" />
	<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
		<ContentTemplate>
            <asp:Literal ID="litResults" ClientIDMode="Static" runat="server" EnableViewState="false"/>
		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>


