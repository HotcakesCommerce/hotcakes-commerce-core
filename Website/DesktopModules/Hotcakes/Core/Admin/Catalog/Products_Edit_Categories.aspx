<%@ Page MasterPageFile="../AdminNav.master" Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit_Categories" Codebehind="Products_Edit_Categories.aspx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<%@ Register src="../Controls/ProductEditMenu.ascx" tagname="ProductEditMenu" tagprefix="uc5" %>
<%@ Register src="../Controls/ProductEditingDisplay.ascx" tagname="ProductEditing" tagprefix="uc5" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="Categories" />
    <uc5:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div class="hcColumnLeft" style="width: 50%">
		<div class="hcForm">
			 <h1><%=PageTitle %></h1>
			<uc2:MessageBox ID="msg" runat="server" />
			<div class="hcFormItem">
				<div>
					<asp:CheckBoxList ID="chkCategories" runat="server"></asp:CheckBoxList>
				</div>
			</div>
		</div>
	</div>

	 <ul class="hcActions">
		 <li>
			 <asp:LinkButton ID="SaveButton" runat="server" Text="Save" CssClass="hcPrimaryAction" OnClick="SaveButton_Click" />
		 </li>
		 <li>
			 <asp:LinkButton ID="CancelButton" runat="server" Text="Cancel" CssClass="hcSecondaryAction" OnClick="CancelButton_Click1" />
		 </li>
	</ul>
</asp:Content>
