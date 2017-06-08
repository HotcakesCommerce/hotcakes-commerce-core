<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories_ManualSelection"
	Title="Category Selection" CodeBehind="Categories_ManualSelection.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="uc1" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
	 <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="catalog/category" runat="server" />
	 <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="Categories.aspx"/>
            </div>
        </div>
    </div>

	 <div class="hcBlock hcBlockNotTopPadding">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkViewInStore" runat="server" CssClass="hcTertiaryAction" Target="_blank">View in Store</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
	<script type="text/javascript">

		function RemoveProduct(lnk) {
			var id = $(lnk).attr('id');
			//id = id.replace('rem', '');
			var categoryid = '<%=CategoryBvin %>';
			$.post('Categories_RemoveProduct.aspx',
				  {
				  	"id": id.replace('rem', ''),
				  	"categoryid": categoryid
				  },
				  function () {
				  	lnk.parent().parent().parent().parent().parent().slideUp('slow', function () {
				  		lnk.parent().parent().parent().parent().parent().remove();
				  	});
				  }
				 );
		}

		jQuery(function ($) {

			$('.trash').click(function () {
				RemoveProduct($(this));
				return false;
			});

			$("#sortable").sortable({
				placeholder: 'ui-state-highlight',
				axis: 'y',
				containment: 'parent',
				opacity: '0.75',
				cursor: 'move',
				update: function (event, ui) {
					var sorted = $(this).sortable('toArray');
					sorted += '';
					$.post('Categories_SortProducts.aspx',
                        {
                        	"ids": sorted,
                        	"categoryid": "<%=CategoryBvin %>"
                        }
                        );
				}
			});
			$("#sortable").disableSelection();

		});

		
	</script>

	<h1>Select Products for Category</h1>
	<uc2:MessageBox ID="msg" runat="server" />
	<div class="hcColumnLeft" style="width: 50%">
		<div class="hcForm">
			<h2>Pick Products To Add</h2>
			<div class="hcFormItem">
				<uc1:ProductPicker ID="ProductPicker1" runat="server" />
				<asp:LinkButton runat="server" ID="btnAdd" CssClass="hcPrimaryAction" Text="Add &raquo;" OnClick="btnAdd_Click" />
			</div>
		</div>
	</div>

	<div class="hcColumnRight hcLeftBorder" style="width: 49%">
		<div class="hcForm">
			<h2>Selected Products</h2>
			<div class="hcFormItem">
				<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
					<ContentTemplate>
						<asp:Literal ID="litProducts" runat="server"></asp:Literal>
					</ContentTemplate>
				</asp:UpdatePanel>
				<asp:HyperLink class="actionlink" ID="lnkBack" CssClass="hcPrimaryAction" runat="server">&laquo; Return to Category</asp:HyperLink>
			</div>
		</div>
	</div>

	

	<asp:HiddenField ID="BvinField" runat="server" />
</asp:Content>
