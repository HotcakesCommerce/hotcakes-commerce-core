<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductIsEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.ProductIsEditor" %>

<%@ Register Src="../../Controls/ProductsPickerWithVariant.ascx" TagName="ProductPicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/ProductVariantsPicker.ascx" TagName="VariantsPicker" TagPrefix="hcc" %>
<h1><%=Title %></h1>
<asp:UpdatePanel ID="upProductPicker" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<table border="0">
			<tr>
				<td style="vertical-align: top; width: 45%">
					<hcc:ProductPicker id="ucProductPicker" runat="server" />
				</td>
				<td>
					<asp:LinkButton ID="btnAddProduct" runat="server" CssClass="hcSecondaryAction" Text=">>" />
				</td>
				<td style="vertical-align: top;">
					<asp:GridView ID="gvProductBvins" runat="server" AutoGenerateColumns="False" DataKeyNames="Bvin" CssClass="hcGrid" ShowHeader="True"
						OnRowDeleting="gvProductBvins_RowDeleting" OnRowDataBound="gvProductBvins_RowDataBound" OnRowCommand="gvProductBvins_RowCommand"
						>
						<RowStyle CssClass="hcGridRow" />
						<Columns>
							<asp:TemplateField HeaderText="Product">
								<ItemTemplate>
									[<%#Eval("Sku") %>] <%#Eval("DisplayName") %>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Variants">
								<ItemTemplate>
									<asp:LinkButton ID="btnProductVariants" runat="server" CssClass="" CausesValidation="false" CommandName="EditVariants" CommandArgument='<%#Container.DataItemIndex %>' DataField="Variants" ></asp:LinkButton>
									<asp:Literal ID="ltrNoVariants" runat="server" ></asp:Literal>
									<asp:HiddenField ID="hdfSelectedVariants" runat="server" />
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField  HeaderText="Remove" >
								<ItemTemplate>
									<asp:LinkButton runat="server" ID="btnDelete" CausesValidation="False" CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
								</ItemTemplate>
							</asp:TemplateField>
						</Columns>
						<EmptyDataTemplate>
							<%=Localization.GetString("NoProductsAdded") %>
						</EmptyDataTemplate>
					</asp:GridView>
				</td>
			</tr>
		</table>
	</ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="upPanel1" runat="server" UpdateMode="Conditional" >
	<ContentTemplate>
		<asp:Panel ID="pnlSelectProductVariants" runat="server" Visible="false">
			<div id="hcShowProductVariantsSelectionDialog" class="dnnClear hcVariantPicker" style="overflow: auto;">
				<hcc:VariantsPicker ID="ProductVariantsPicker" runat="server" Visible="true" />
				<div class="hcVariantPickerActions hcActionsRight" style="">
					<ul class="hcActions">
						<li>
							<asp:LinkButton ID="btnSaveProductVariantsSelection" runat="server" resourcekey="btnSave" CssClass="hcPrimaryAction" OnClick="btnSaveProductVariantsSelection_Click" />
						</li>
						<li>
							<asp:LinkButton ID="btnCloseProductVariantsSelectionEditor" runat="server" resourcekey="btnClose" CssClass="hcSecondaryAction" OnClick="btnCloseProductVariantsSelectionEditor_Click"  />
						</li>
					</ul>
				</div>
			</div>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>
