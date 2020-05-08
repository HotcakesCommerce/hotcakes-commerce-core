<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories" Title="Categories" CodeBehind="Categories.aspx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu runat="server" ID="NavMenu" />

	<div class="hcBlock hcBlockLight hcPaddingBottom">
		<div class="hcForm">
			<div class="hcFormItem">
				<label class="hcLabel">+ New Category</label>
				<asp:DropDownList runat="server" ID="lstType">
					<Items>
						<asp:ListItem Value="0" Text="Category Page" />
						<asp:ListItem Value="2" Text="Custom Link" />
					</Items>
				</asp:DropDownList>
			</div>
		</div>
	</div>

	<div class="hcBlock hcBlockLight hcPaddingBottom">
		<div class="hcForm">
			<div class="hcFormItem">
                <asp:DropDownList runat="server" ID="lstParents"/>
			</div>
		</div>
	</div>

	<div class="hcBlock">
		<div class="hcForm">
			<div class="hcFormItem">
				<asp:LinkButton ID="btnNew" AlternateText="Add New Category" Text="Add New Category" runat="server" resourcekey="btnNew" CssClass="hcTertiaryAction" EnableViewState="False" OnClick="btnNew_Click" />
			</div>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
	<script src="Categories.js" type="text/javascript"></script>
	<h1>Categories / Pages</h1>
	<asp:Literal ID="litMain" runat="server" EnableViewState="false"></asp:Literal>
</asp:Content>
