<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.ProductVariantsPicker" CodeBehind="ProductVariantsPicker.ascx.cs" %>

<h1>(<asp:Label ID="ProducSku" runat="server"></asp:Label>) <asp:Label ID="ProductName" runat="server"></asp:Label></h1>
<div class="hcFormItem">
	<label></label>
</div>
<div class="hcFormItem">
	<div style="float: left;">
		( <asp:Label ID="TotalVariants" runat="server"></asp:Label> ) Available Variants
	</div>
</div>
<div class="hcFormItem">
	<asp:UpdatePanel ID="upVariantsSelection" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="false">
		<ContentTemplate>
		<asp:GridView ID="gvVariants" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" AlternatingRowStyle-Wrap="true" RowStyle-Wrap="true" EnableViewState="true"
					DataKeyNames="bvin" 
					OnRowDataBound="gvVariants_RowDataBound" >
				<HeaderStyle CssClass="hcGridHeader" />
				<RowStyle CssClass="hcGridRow" />
				<Columns>
					<asp:TemplateField HeaderStyle-Width="15%" ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" ItemStyle-VerticalAlign="Middle">
						<HeaderTemplate>
							<a href="#" class="hcCheckAll" style="text-align:center">All</a>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:CheckBox ID="chkSelected" runat="server"  AutoPostBack="true"  EnableViewState="true"/>
							<asp:Literal ID="radioButtonLiteral" runat="server"></asp:Literal>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="Sku" HeaderText="SKU" HeaderStyle-Width="38%" ItemStyle-Width="38%" />
					<asp:TemplateField HeaderText="Variant">
						<ItemTemplate><%#GetVariantName(Container) %> </ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="Price" DataFormatString="{0:c}" HeaderText="Price" />
				</Columns>
			</asp:GridView>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>
<asp:HiddenField ID="CurrentProductBvin" runat="server" />
