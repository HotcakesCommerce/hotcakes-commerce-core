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
				<asp:HyperLink ID="hypClose" resourcekey="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="ProductSharedChoices.aspx" />
			</div>
		</div>
	</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
	<h1><%=PageTitle %></h1>
	<uc2:MessageBox ID="MessageBox1" runat="server" />

	<div class="controlarea1">
		<div class="hcForm">
			<div class="hcFormItem">
				<label class="hcLabel"><%=Localization.GetString("lblName") %><i class="hcLocalizable"></i></label>
				<asp:TextBox ID="txtName" runat="server" Width="300px"></asp:TextBox>
			</div>
			<div class="hcFormItem">
				<asp:CheckBox ID="chkHideName" resourcekey="chkHideName" CssClass="hcFloatForIE" runat="server" />
			</div>
			<div class="hcFormItem" id="trVariant" runat="server" visible="false">
				<asp:CheckBox ID="chkVariant" resourcekey="chkVariant" CssClass="hcFloatForIE" runat="server" />
			</div>
		</div>
	</div>
	<asp:MultiView ID="viewMain" runat="server">
		<asp:View ID="viewHtml" runat="server">
			<div class="controlarea1">
				<div class="hcForm">
					<div class="hcFormItem">
						<asp:Label runat="server" class="hcLabel"><%=Localization.GetString("lblDescription") %><i class="hcLocalizable"></i></asp:Label>
						<uc1:HtmlEditor ID="HtmlEditor1" runat="server" EditorWidth="910" EditorHeight="220" EditorWrap="true" />
					</div>
				</div>
			</div>
		</asp:View>
		<asp:View ID="viewTextInput" runat="server">
			<div class="controlarea1">
				<div class="hcForm">
					<div class="hcFormItem">
						<label class="hcLabel"><%=Localization.GetString("lblColumns") %></label>
						<asp:TextBox ID="ColumnsField" runat="server" Columns="10" />
					</div>
					<div class="hcFormItem">
						<label class="hcLabel"><%=Localization.GetString("lblRows") %></label>
						<asp:TextBox ID="RowsField" runat="server" Columns="10" />
					</div>
					<div class="hcFormItem">
						<label class="hcLabel"><%=Localization.GetString("lblMaxLength") %></label>
						<asp:TextBox ID="MaxLengthField" runat="server" Columns="10" />
					</div>
				</div>
			</div>

		</asp:View>
		<asp:View ID="viewFileUpload" runat="server">
			<div class="controlarea1">
				<div class="hcForm">
					<div class="hcFormItem">
						<asp:CheckBox ID="MultipleFilesField" resourcekey="MultipleFilesField" runat="server" Checked="true" />
					</div>
				</div>
			</div>
		</asp:View>
		<asp:View ID="viewItems" runat="server">
			<div class="padded">
				<h2><%=Localization.GetString("Header") %></h2>
				<uc3:OptionItemEditor ID="ItemsEditor" runat="server" />
			</div>
		</asp:View>
	</asp:MultiView>
	<ul class="hcActions editorcontrols">
		<li>
			<asp:LinkButton ID="btnSaveOption" resourcekey="btnSaveOption" CssClass="hcPrimaryAction" ClientIDMode="Static" runat="server" OnClick="btnSaveOption_Click" />
		</li>
        <li>
            <asp:LinkButton ID="btnSaveOptionAndClose" resourcekey="btnSaveOptionAndClose" CssClass="hcSecondaryAction" ClientIDMode="Static" runat="server" OnClick="btnSaveAndCloseOption_Click" />
        </li>
	</ul>
</asp:Content>
