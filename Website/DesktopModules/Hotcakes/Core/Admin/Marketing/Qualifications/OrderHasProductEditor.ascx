<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderHasProductEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.OrderHasProductEditor" %>

<%@ Register Src="../../Controls/ProductsPickerWithVariant.ascx" TagName="ProductPicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/ProductVariantsPicker.ascx" TagName="VariantsPicker" TagPrefix="hcc" %>

<asp:Panel runat="server" ID="pnlHasHeader">
	<h1><%=Localization.GetString("WhenOrderHasProducts") %></h1>
</asp:Panel>
<asp:Panel runat="server" ID="pnlHasNotHeader">
	<h1><%=Localization.GetString("WhenOrderHasNotProducts") %></h1>
</asp:Panel>

<asp:UpdatePanel ID="upProductPicker" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<table border="0">
			<tr>
				<td style="vertical-align: top; width: 45%">
					<hcc:ProductPicker ID="ProductPickerOrderProducts" runat="server" />
				</td>
				<td>
					<asp:LinkButton ID="btnAddOrderProduct" runat="server" Text=">>" CssClass="hcSecondaryAction" OnClick="btnAddOrderProduct_Click" />
				</td>
				<td style="vertical-align: top;">
					<asp:Panel runat="server" ID="pnlHas">
						<%=Localization.GetString("WhenOrderHasAtLeast") %>
						<asp:TextBox ID="OrderProductQuantityField" runat="server" Columns="10" />
						<asp:RangeValidator ID="rvOrderProductQuantityField" runat="server" Type="Integer" MinimumValue="1" MaximumValue="9999" ControlToValidate="OrderProductQuantityField" CssClass="hcFormError" EnableClientScript="true" resourcekey="ErrorOrderProductQuantityFieldRange"></asp:RangeValidator>
						<asp:RequiredFieldValidator ID="rfvOrderProductQuantityField" runat="server" ControlToValidate="OrderProductQuantityField" Display="Dynamic" CssClass="hcFormError" EnableClientScript="true" resourcekey="ErrorOrderProductQuantityField" />
						<%=Localization.GetString("Of") %>
						<asp:DropDownList ID="lstOrderProductSetMode" runat="server" />
						<%=Localization.GetString("OfTheseProducts") %>
					</asp:Panel>
					<asp:Panel runat="server" ID="pnlHasNot">
						<%=Localization.GetString("WhenOrderHasNot") %>
					</asp:Panel>
					<asp:GridView ID="gvOrderProducts" runat="server" AutoGenerateColumns="False"
						DataKeyNames="Bvin" CssClass="hcGrid" OnRowDeleting="gvOrderProducts_RowDeleting" OnRowDataBound="gvOrderProducts_RowDataBound"	 OnRowCommand="gvOrderProducts_RowCommand"
						ShowHeader="True">
						<RowStyle CssClass="hcGridRow" />
						<Columns>
							<asp:BoundField DataField="DisplayName" HeaderText="Product"/>
							<asp:TemplateField HeaderText="Variants">
								<ItemTemplate>
									<asp:LinkButton ID="btnProductVariants" runat="server" CssClass="" CausesValidation="false" CommandName="EditVariants" CommandArgument='<%#Container.DataItemIndex %>' DataField="Variants" ></asp:LinkButton>
									<asp:Literal ID="ltrNoVariants" runat="server" ></asp:Literal>
									<asp:HiddenField ID="hdfSelectedVariants" runat="server" />
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Remove">
								<ItemTemplate>
									<asp:LinkButton ID="btnDeleteOrderProduct" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteOrderProduct_OnPreRender" />
								</ItemTemplate>
							</asp:TemplateField>
						</Columns>
						<EmptyDataTemplate>
							<%=Localization.GetString("NoProductsAdded.Text") %>
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
