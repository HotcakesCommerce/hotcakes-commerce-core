<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductChoices" CodeBehind="ProductChoices.aspx.cs" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="uc5" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="CustomerChoices" />
		 <div class="hcBlock hcBlockLight hcPaddingBottom">
        <div class="hcForm">
            <div class="hcFormItem">
				<label class="hcLabel">New Product Choice</label>
				<telerik:RadComboBox ID="ChoiceTypes" runat="server" >
					<Items>
						<telerik:RadComboBoxItem Value="100" Text="Drop Down List" />
						<telerik:RadComboBoxItem Value="200" Text="Radio Button List" />
						<telerik:RadComboBoxItem Value="300" Text="Checkboxes" />
						<telerik:RadComboBoxItem Value="400" Text="HTML Description" />
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
				 <asp:LinkButton ID="NewChoiceButton" AlternateText="+ New Product Choice" Text="+ New Product Choice" runat="server" CssClass="hcTertiaryAction"  OnClick="NewChoiceButton_Click" />
			</div>
		</div>
	</div>

	<uc5:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
	<script type="text/javascript">
	    hcAttachUpdatePanelLoader();
	    var curProductChoice;
	    function callBackFunRemoveItem(event) {	    
	        event.data.param1.dialog('close');
	        RemoveItem(curProductChoice);
	    }

		jQuery(function () {
			$("#dragitem-list  ").sortable({
				placeholder: 'ui-state-highlight',
				axis: 'y',
				opacity: '0.75',
				cursor: 'move',
				update: function (event, ui) {
					var productBvin = getParameterByName("id");
					var sorted = $(this).sortable('toArray');
					var sortedQuoted = sorted + ' ';
					$.post('ProductChoices_Sort.aspx',
					  {
					  	"ids": sortedQuoted,
					  	"bvin": productBvin
					  }, function () { return; });
				}
			});
			$('#dragitem-list').disableSelection();

			$('.trash').click(function (e) {			    
			    if ($(this).attr('title') === 'variant') {
			        curProductChoice = $(this);
			        hcConfirm(e, 'Deleting this choice will DELETE YOUR VARIANTS. You will lose inventory, price and picture settings for variants. Are you sure you want to continue?', callBackFunRemoveItem);
				} else { RemoveItem($(this)); }
				return false;
			});

		});

		
		function RemoveItem(lnk) {
			var productBvin = getParameterByName("id");
		  	var id = $(lnk).attr('id');
		  	$.post('ProductChoices_Delete.aspx',
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
	<h1>Choices - Edit</h1>
	<uc1:MessageBox ID="MessageBox1" runat="server" />
	    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
			<ContentTemplate>
				<asp:Literal ID="litResults" ClientIDMode="Static" runat="server" EnableViewState="false"></asp:Literal>
			</ContentTemplate>
		</asp:UpdatePanel>
</asp:Content>

