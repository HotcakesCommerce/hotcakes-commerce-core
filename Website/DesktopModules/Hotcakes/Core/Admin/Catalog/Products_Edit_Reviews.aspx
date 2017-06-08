<%@ Page MasterPageFile="../AdminNav.master" Language="C#"
	AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit_Reviews" CodeBehind="Products_Edit_Reviews.aspx.cs" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="uc5" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="ProductReviews" />
	<div class="hcBlock">
		<div class="hcForm">
			<div class="hcFormItem">
				<asp:LinkButton ID="btnNew" runat="server" AlternateText="+ New Review" Text="+ New Review" class="hcTertiaryAction" OnClick="btnNew_Click" />
			</div>
		</div>
	</div>
	<uc5:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<h1>Customer Reviews</h1>
	<asp:Label ID="lblNoReviews" runat="server" Text="No Reviews" Visible="false" class="Formlabel"></asp:Label>
	<asp:DataGrid runat="server" ID="dlReviews" DataKeyField="Bvin" CssClass="hcGrid hcProductReviewTable"
		AutoGenerateColumns="False"
		OnEditCommand="dlReviews_EditCommand"
		OnItemDataBound="dlReviews_ItemDataBound"
		OnDeleteCommand="dlReviews_DeleteCommand">
		<HeaderStyle CssClass="hcGridHeader" />
		<ItemStyle CssClass="hcGridRow" />
		<Columns>
			<asp:TemplateColumn HeaderText="Rating" ItemStyle-Width="7%" HeaderStyle-Width="7%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle">
				<ItemTemplate>
					<asp:Panel runat="server" ID="panelRating" />
				</ItemTemplate>
			</asp:TemplateColumn>

			<asp:TemplateColumn HeaderText="Review Date" ItemStyle-Width="9%" HeaderStyle-Width="9%">
				<ItemTemplate>
					<asp:Label ID="lblReviewDate" runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>

			<asp:TemplateColumn HeaderText="Product">
				<ItemTemplate>
					<asp:Label ID="lblProductID" runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>

			<asp:TemplateColumn HeaderText="Review" ItemStyle-Width="50%" HeaderStyle-Width="50%">
				<ItemTemplate>
					<asp:Label ID="lblReview" runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>

			<asp:TemplateColumn HeaderText="Action" ItemStyle-Width="10%"  HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle">
				<ItemTemplate>
					<asp:LinkButton ID="btnEdit" runat="server" class="hcIconEdit"
						CommandName="Edit" AlternateText="Edit"></asp:LinkButton>

					&nbsp;&nbsp;
					<asp:LinkButton ID="btnDelete" Text="Delete" CssClass="hcIconDelete" CommandName="Delete"
						OnClientClick="return hcConfirm(event,'Are you sure you want to delete this item?');" runat="server" CommandArgument="Delete" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>

	</asp:DataGrid>
</asp:Content>
