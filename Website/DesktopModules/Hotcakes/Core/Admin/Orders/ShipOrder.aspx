<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.ShipOrder" Title="Untitled Page" CodeBehind="ShipOrder.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="OrderStatusDisplay.ascx" TagName="OrderStatusDisplay" TagPrefix="hcc" %>
<%@ Register Src="OrderActions.ascx" TagName="OrderActions" TagPrefix="hcc" %>

<asp:Content ID="Nav" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:OrderActions ID="ucOrderActions" CurrentOrderPage="Shipping" runat="server" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="Server">
	<hcc:MessageBox ID="MessageBox1" runat="server" />

	<h1>
		<asp:Label ID="lblOrderNumber" runat="server"></asp:Label>Shipping</h1>
	<div class="hcColumn" style="width: 30%">
		<label class="hcLabel">Ship To:</label>
		<asp:Label ID="lblShippingAddress" runat="server" />

		<div class="hcFormItem">
			<h4>Shipping/Handling Total:
				 <asp:Label ID="lblShippingTotal" runat="server" /></h4>
		</div>
	</div>
	<div class="hcColumnRight" style="width: 70%;">
		<hcc:OrderStatusDisplay ID="ucOrderStatusDisplay" runat="server" />
	</div>
	<div class="hcClearfix"></div>

	<asp:GridView ID="ItemsGridView" runat="server" AutoGenerateColumns="False" CssClass="hcGrid hcProductReviewTable"
		DataKeyNames="Id" OnRowDataBound="ItemsGridView_RowDataBound">
		<HeaderStyle CssClass="hcGridHeader" />
		<Columns>
			<asp:TemplateField HeaderText="SKU">
				<ItemTemplate>
					<asp:Label ID="SKUField" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Item">
				<ItemTemplate>
					<asp:Label ID="DescriptionField" runat="server" />
					<asp:PlaceHolder ID="CartInputModifiersPlaceHolder" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Ordered" ItemStyle-Width="40" HeaderStyle-Width="40">
				<ItemTemplate>
					<asp:Label ID="QuantityField" runat="server" Text='<%# Bind("Quantity","{0:#}") %>' />
				</ItemTemplate>
				<ItemStyle HorizontalAlign="Right" />
				<HeaderStyle HorizontalAlign="Right" />
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Shipped" ItemStyle-Width="40" HeaderStyle-Width="40">
				<ItemTemplate>
					<asp:Label ID="shipped" runat="server" Text="0" />
				</ItemTemplate>
				<ItemStyle HorizontalAlign="Right" />
				<HeaderStyle HorizontalAlign="Right" />
			</asp:TemplateField>
			<asp:TemplateField HeaderText="To Be Shipped" ItemStyle-Width="90" HeaderStyle-Width="90" >
				<ItemTemplate>
					<asp:TextBox ID="QtyToShip" runat="server" Text="0" Columns="5" />
				</ItemTemplate>
				<ItemStyle HorizontalAlign="Right" />
				<HeaderStyle HorizontalAlign="Right" />
			</asp:TemplateField>
		</Columns>
	</asp:GridView>

	<asp:Panel ID="pnlShip" runat="server" DefaultButton="btnShipItems" CssClass="hcBlock">
		<div class="hcFormItem">
			<asp:Label ID="lblUserSelectedShippingMethod" runat="server" Text="" />
		</div>
		<asp:UpdatePanel runat="server" UpdateMode="Always">
			<ContentTemplate>
				<div class="hcFormItem hcFormItem33p">
					<asp:Label ID="lblShippingBy" runat="server" Text="Shipping By:" CssClass="hcLabel" />
					<asp:DropDownList ID="lstTrackingProvider" runat="server" AutoPostBack="true" EnableViewState="true" ViewStateMode="Enabled">
					</asp:DropDownList>

					<br />
					<asp:Label ID="lblShippingServices" runat="server" CssClass="hcLabel">Shipping Services:</asp:Label>
					<asp:DropDownList ID="lstTrackingProviderServices" runat="server" ViewStateMode="Enabled" EnableViewState="true">
					</asp:DropDownList>
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>
		
		<div class="hcFormItem hcFormItem66p">
			<span class="hcLabel">Tracking Number:</span>
			<asp:TextBox ID="TrackingNumberField" runat="Server" Columns="20" Width="250px" MaxLength="254" />

			<asp:LinkButton ID="btnShipItems" runat="Server" Text="Ship Items"
				OnClick="btnShipItems_Click" CssClass="hcPrimaryAction hcSmall" />
			<asp:LinkButton ID="btnCreatePackage" runat="Server" Text="Create Package"
				OnClick="btnCreatePackage_Click" CssClass="hcSecondaryAction" Visible="False" />
		</div>

	</asp:Panel>

	<hr />

	<h2 id="hPackage" runat="server">Packages</h2>
	<asp:GridView ID="PackagesGridView" GridLines="none" BorderWidth="0"
		ShowHeader="true" DataKeyNames="Id" runat="server"
		AutoGenerateColumns="False" OnRowCommand="PackagesGridView_RowCommand"
		OnRowDataBound="PackagesGridView_RowDataBound"
		OnRowDeleting="PackagesGridView_RowDeleting" CssClass="hcGrid hcProductReviewTable">
		<HeaderStyle CssClass="hcGridHeader" />
		<RowStyle CssClass="hcGridRow" />

		<Columns>
			<asp:TemplateField HeaderText="Ship Date" HeaderStyle-Width="9%" ItemStyle-Width="9%">
				<ItemTemplate>
					<asp:Label ID="ShipDateField" runat="server" Text='<%# Bind("ShipDateUtc") %>' />
				</ItemTemplate>
			</asp:TemplateField>

			<asp:TemplateField HeaderText="Shipped By">
				<ItemTemplate>
					<asp:Label ID="ShippedByField" runat="server" Text='<%# Bind("ShippingProviderId") %>' />
				</ItemTemplate>
			</asp:TemplateField>

			<asp:TemplateField HeaderText="Tracking" HeaderStyle-Width="30%" ItemStyle-Width="30%">
				<ItemTemplate>
					<asp:HyperLink ID="TrackingLink" runat="server" Target="_blank" NavigateUrl="#"  Text="No Tracking Information" />
					<asp:Label ID="TrackingText" runat="server" />
					<br />
					<asp:TextBox ID="TrackingNumberTextBox" runat="server" Width="200px" MaxLength="254"/>
					<asp:LinkButton ID="btnUpdateTrackingNumber" runat="server" CommandName="UpdateTrackingNumber" CommandArgument='<%# Eval("Id") %>'
						Text="Update" CssClass="hcSecondaryAction hcSmall" />
				</ItemTemplate>
			</asp:TemplateField>
<%--			<asp:TemplateField HeaderText="Description">
				<ItemTemplate>
					<asp:Label ID="pakdescription" runat="Server" />
				</ItemTemplate>
			</asp:TemplateField>--%>

			<asp:TemplateField HeaderText="Items">
				<ItemTemplate>
					<asp:Label ID="items" runat="Server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Items" HeaderStyle-Width="5%" ItemStyle-Width="5%">
				<ItemTemplate>
					<asp:LinkButton ID="btnDelete" runat="server" CausesValidation="false" CommandName="Delete" Text="Delete"
									CssClass="hcIconDelete" OnClientClick="return hcConfirm(event,'Are you sure you want to delete this package?');" />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>

</asp:Content>
