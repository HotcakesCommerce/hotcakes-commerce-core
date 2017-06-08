<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderItemInventory.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderItemInventory" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagPrefix="hcc" TagName="MessageBox" %>

<hcc:MessageBox runat="server" ID="ucMessageBox" AddValidationSummaries="false" />

	<h1>
		<asp:Label ID="lblEditorHading" runat="server" resourcekey="EditInventory" />
	</h1>
	<h3>
		<asp:Label ID="lblCustomMessaege" runat="server" resourcekey="CustomMessage" />
	</h3>
	<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" OnRowDataBound="gvItems_RowDataBound"
		DataKeyNames="Id">
		<Columns>
			<asp:TemplateField HeaderText="Select">
				<ItemTemplate>
					<asp:CheckBox ID="chbSelected" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="SKU">
				<ItemTemplate>
					<asp:Label ID="lblSKU" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Name">
				<ItemTemplate>
					<asp:Label ID="lblName" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Order Qty">
				<ItemStyle CssClass="hcRight" />
				<HeaderStyle CssClass="hcRight" />
				<ItemTemplate>
					<asp:Label ID="lblOrderQty" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Replenish Qty">
				<ItemStyle CssClass="hcRight" />
				<HeaderStyle CssClass="hcRight" />
				<ItemTemplate>
					<asp:TextBox ID="txtQty" Columns="4" runat="server" />
					<asp:RangeValidator ID="rvTxtQty" runat="server" Type="Integer" Display="Dynamic" MinimumValue="1" MaximumValue="999" ControlToValidate="txtQty" CssClass="hcFormError"></asp:RangeValidator>
					<asp:HiddenField ID="hdfLineItemId" runat="server" />
					<asp:HiddenField ID="hdfReplenishedAll" runat="server" Value="False"/>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Replenished Qty">
				<ItemStyle CssClass="hcRight" />
				<HeaderStyle CssClass="hcRight" />
				<ItemTemplate>
					<asp:Label ID="lblReplenishedQty" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>


		</Columns>
	</asp:GridView>
	<ul class="hcActions">
		<li>
			<asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" CssClass="hcPrimaryAction" OnClick="btnSave_Click"/>
		</li>
		<li>
			<asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CausesValidation="false" runat="server" CssClass="hcSecondaryAction" OnClientClick="closeInventoryEditorDialog();" />
		</li>
	</ul>



