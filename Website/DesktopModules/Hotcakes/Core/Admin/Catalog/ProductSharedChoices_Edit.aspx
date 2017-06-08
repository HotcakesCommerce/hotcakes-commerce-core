<%@ Page ValidateRequest="false" Title="" Language="C#" MasterPageFile="../AdminNav.master"
	AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductSharedChoices_Edit" CodeBehind="ProductSharedChoices_Edit.aspx.cs" %>

<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="uc1" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<%@ Register Src="../Controls/OptionItemEditor.ascx" TagName="OptionItemEditor" TagPrefix="uc3" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>


<asp:Content ID="Content1" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu runat="server" ID="NavMenu" />
	<div class="hcBlock">
		<div class="hcForm">
			<div class="hcFormItem">
				<asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="ProductSharedChoices.aspx" />
			</div>
		</div>
	</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
	<uc2:MessageBox ID="MessageBox1" runat="server" />
	<h1>Edit Shared Choice</h1>

	<div class="controlarea1">
		<div class="hcForm">
			<div class="hcFormItem">
				<label class="hcLabel">Name<i class="hcLocalizable"></i>:</label>
				<asp:TextBox ID="txtName" runat="server" Width="300px"></asp:TextBox>
			</div>
			<div class="hcFormItem">
				<asp:CheckBox ID="chkHideName" CssClass="hcFloatForIE" runat="server" Text="Hide name on store" />
			</div>
			<div class="hcFormItem" id="trVariant" runat="server" visible="false">
				<asp:CheckBox ID="chkVariant" CssClass="hcFloatForIE" runat="server" Text="This choice changes <i>Inventory</i>, <i>Pictures</i>, <i>Prices</i> and/or <i>SKU</i>" />
			</div>
		</div>
	</div>
	<asp:MultiView ID="viewMain" runat="server">
		<asp:View ID="viewHtml" runat="server">
			<div class="controlarea1">
				<div class="hcForm">
					<div class="hcFormItem">
						<asp:Label runat="server" class="hcLabel">Description<i class="hcLocalizable"></i>:</asp:Label>
						<uc1:HtmlEditor ID="HtmlEditor1" runat="server" EditorWidth="910" EditorHeight="220" EditorWrap="true" />
					</div>
				</div>
			</div>
		</asp:View>
		<asp:View ID="viewTextInput" runat="server">
			<div class="controlarea1">
				<div class="hcForm">
					<div class="hcFormItem">
						<label class="hcLabel">Columns:</label>
						<asp:TextBox ID="ColumnsField" runat="server" Columns="10" />
					</div>
					<div class="hcFormItem">
						<label class="hcLabel">Rows:</label>
						<asp:TextBox ID="RowsField" runat="server" Columns="10" />
					</div>
					<div class="hcFormItem">
						<label class="hcLabel">Max Length:</label>
						<asp:TextBox ID="MaxLengthField" runat="server" Columns="10" />
					</div>
				</div>
			</div>

		</asp:View>
		<asp:View ID="viewFileUpload" runat="server">
			<div class="controlarea1">
				<div class="hcForm">
					<div class="hcFormItem">
						<asp:CheckBox ID="MultipleFilesField" runat="server" Checked="true" Text="Allow Multiple Files" />
					</div>
				</div>
			</div>
		</asp:View>
		<asp:View ID="viewItems" runat="server">
			<div class="padded">
				<h2>Choice Items</h2>
				<uc3:OptionItemEditor ID="ItemsEditor" runat="server" />
			</div>
		</asp:View>
	</asp:MultiView>
	<ul class="hcActions editorcontrols">
		<li>
			<asp:ImageButton ID="btnSaveOption" CssClass="hcPrimaryAction" ClientIDMode="Static" runat="server"
				AlternateText="Save Changes"
				OnClick="btnSaveOption_Click" />
		</li>
	</ul>
</asp:Content>
