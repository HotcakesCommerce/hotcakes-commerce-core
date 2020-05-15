<%@ Page ValidateRequest="false" Title="Edit Product Tab" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductsEdit_TabsEdit" Codebehind="ProductsEdit_TabsEdit.aspx.cs" %>
<%@ Register src="../Controls/ProductEditMenu.ascx" tagname="ProductEditMenu" tagprefix="uc5" %>
<%@ Register src="../Controls/ProductEditingDisplay.ascx" tagname="ProductEditing" tagprefix="uc5" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="uc1" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="InfoTabs" />
    <uc5:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div class="hcForm">
		<h1>Edit Product Tab</h1>
		<uc2:MessageBox ID="MessageBox1" runat="server" />
		<div class="hcFormItem">
			<label class="hcLabel">Tab Title<i class="hcLocalizable"></i>:</label>
			<asp:TextBox id="TabTitleField" runat="server" width="300px"></asp:TextBox>
		</div>
		<div class="hcFormItem">
			<label class="hcLabel">HTML<i class="hcLocalizable"></i>:</label>
			 <uc1:HtmlEditor ID="HtmlDataField" runat="server" EditorHeight="350" EditorWidth="500"
                EditorWrap="true"/>
		</div>
	</div>

	 <ul class="hcActions">
		<li>
            <asp:LinkButton ID="btnSaveOption" runat="server" Text="Save" CssClass="hcPrimaryAction" OnClick="btnSaveOption_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnSaveAndClose" runat="server" Text="Save and Close" CssClass="hcSecondaryAction" OnClick="btnSaveAndClose_Click" />
        </li>
        <li>
			 <asp:LinkButton ID="LinkButton1" runat="server" Text="Cancel" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="LinkButton1_Click" />
        </li>
	</ul>

</asp:Content>
