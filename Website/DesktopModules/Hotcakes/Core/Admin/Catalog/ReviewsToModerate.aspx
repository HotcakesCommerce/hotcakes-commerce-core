<%@ Page MasterPageFile="../AdminNav.master" Language="C#" AutoEventWireup="True"
	Inherits="Hotcakes.Modules.Core.Admin.Catalog.ReviewsToModerate" CodeBehind="ReviewsToModerate.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu runat="server" ID="NavMenu" />

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<h1>Product Reviews to Moderate</h1>

		<asp:Label ID="lblNoReviews" runat="server" Text="No Reviews to Moderate" Visible="false" class="Formlabel"></asp:Label>

		<asp:DataGrid runat="server" ID="dlReviews" DataKeyField="bvin" CssClass="hcGrid hcProductReviewTable"
			AutoGenerateColumns="False"
			OnDeleteCommand="dlReviews_DeleteCommand" OnEditCommand="dlReviews_EditCommand"
				OnItemDataBound="dlReviews_ItemDataBound"
				OnUpdateCommand="dlReviews_UpdateCommand">
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

				<asp:TemplateColumn HeaderText="Review" ItemStyle-Width="46%" HeaderStyle-Width="46%">
					<ItemTemplate>
						<asp:Label ID="lblReview" runat="server"/>
					</ItemTemplate>
				</asp:TemplateColumn>

				<asp:TemplateColumn HeaderText="Action" ItemStyle-Width="15%"  HeaderStyle-Width="15%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle">
					<ItemTemplate>
						<asp:LinkButton ID="btnApprove" CommandName="Update" runat="server" CssClass="hcIconApprove" Text="Approve"></asp:LinkButton>
						&nbsp;&nbsp;
						<asp:LinkButton ID="btnEdit" runat="server" CssClass="hcIconEdit" CommandName="Edit" Text="Edit"></asp:LinkButton>
						<asp:LinkButton ID="btnDelete" Text="Delete" CssClass="hcIconDelete" CommandName="Delete"
                                            OnClientClick="return hcConfirm(event,'Are you sure you want to delete this item?');" runat="server" CommandArgument="Delete" />
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>

</asp:Content>
