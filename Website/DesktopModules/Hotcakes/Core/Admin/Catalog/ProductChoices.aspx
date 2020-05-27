<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductChoices" CodeBehind="ProductChoices.aspx.cs" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="uc5" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="CustomerChoices" />
	<div class="hcBlock hcBlockLight hcPaddingBottom">
        <div class="hcForm">
            <div class="hcFormItem">
				<label class="hcLabel"><%=Localization.GetString("NewProductChoice") %></label>
				<asp:DropDownList ID="ChoiceTypes" runat="server" >
					<Items>
						<asp:ListItem Value="100" resourcekey="DropdownList" />
						<asp:ListItem Value="200" resourcekey="RadioButtonList" />
						<asp:ListItem Value="300" resourcekey="Checkboxes" />
						<asp:ListItem Value="400" resourcekey="HtmlDescription" />
						<asp:ListItem Value="500" resourcekey="TextInput" />
						<asp:ListItem Value="600" resourcekey="FileUpload" />
					</Items>
				</asp:DropDownList>
			</div>
		</div>
	</div>

	 <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
				 <asp:LinkButton ID="NewChoiceButton" resourcekey="NewChoiceButton" runat="server" CssClass="hcTertiaryAction" OnClick="NewChoiceButton_Click" />
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

			$('.hcIconDelete').click(function (e) {			    
			    if ($(this).attr('title') === 'variant') {
			        curProductChoice = $(this);
			        hcConfirm(e, '<%=Localization.GetJsEncodedString("Confirm")%>', callBackFunRemoveItem);
				} else { RemoveItem($(this)); }
				return false;
			});

            $(".hc-file-upload-btn-add, .hc-file-upload-btn-update").click(function(e) { e.preventDefault(); });
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
	<h1><%=PageTitle %></h1>
	<uc1:MessageBox ID="MessageBox1" runat="server" />
	<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
		<ContentTemplate>
            <asp:Literal ID="litResults" ClientIDMode="Static" runat="server" EnableViewState="false"/>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>

