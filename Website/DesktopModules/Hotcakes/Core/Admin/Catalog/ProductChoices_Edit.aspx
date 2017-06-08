<%@ Page ValidateRequest="false" Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductChoices_Edit" CodeBehind="ProductChoices_Edit.aspx.cs" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>
<%@ Register Src="../Controls/OptionItemEditor.ascx" TagName="OptionItemEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="CustomerChoices" />
    <hcc:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=Localization.GetString("EditProductChoice") %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft">
        <div class="hcFormItem">
            <label class="hcLabel"><%=Localization.GetString("Name") %><i class="hcLocalizable"></i></label>
            <asp:TextBox ID="NameField" runat="server" />
        </div>
        <div class="hcFormItem" id="trRequired" runat="server" visible="false">
			<asp:CheckBox ID="chkRequired" CssClass="hcFloatForIE" resourcekey="chkRequired" AutoPostBack="true" runat="server" />
        </div>
        <div class="hcFormItem">
            <asp:CheckBox ID="chkHideName" CssClass="hcFloatForIE" resourcekey="chkHideName" runat="server" />
        </div>
        <div class="hcFormItem">
            <asp:CheckBox ID="chkIsColorSwatch" CssClass="hcFloatForIE" resourcekey="chkIsColorSwatch" runat="server" />
        </div>
        <div class="hcFormItem" id="trVariant" runat="server" visible="false">
            <asp:CheckBox ID="chkVariant" CssClass="hcFloatForIE" resourcekey="chkVariant" runat="server" />
        </div>
	</div>
	<div class="clearfix"></div>
        <asp:MultiView ID="viewMain" runat="server">
            <asp:View ID="viewHtml" runat="server">
                <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("Description") %><i class="hcLocalizable"></i></asp:Label>
                <hcc:HtmlEditor ID="HtmlEditor1" runat="server" EditorWidth="675" EditorHeight="220" EditorWrap="true" />
            </asp:View>
            <asp:View ID="viewTextInput" runat="server">
                <div class="hcFormItem hcFormItem33p">
                    <label class="hcLabel"><%=Localization.GetString("Columns") %></label>
                    <asp:TextBox ID="ColumnsField" runat="server" />
                </div>
                <div class="hcFormItem hcFormItem33p">
                    <label class="hcLabel"><%=Localization.GetString("Rows") %></label>
                    <asp:TextBox ID="RowsField" runat="server" />
                </div>
                <div class="hcFormItem hcFormItem33p">
                    <label class="hcLabel"><%=Localization.GetString("MaxLength") %></label>
                    <asp:TextBox ID="MaxLengthField" runat="server" />
                </div>
            </asp:View>
            <asp:View ID="viewItems" runat="server">
                <h2><%=Localization.GetString("ChoiceItems") %></h2>
                <hcc:OptionItemEditor ID="ItemsEditor" runat="server" />
            </asp:View>
            <asp:View ID="viewFileInput" runat="server">
                <div class="hcFormItem">
                    <asp:CheckBox ID="MultipleFilesField" resourcekey="MultipleFilesField" runat="server" Checked="true" />
                </div>
            </asp:View>
        </asp:MultiView>
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnSaveOption" resourcekey="btnSaveOption" runat="server" CssClass="hcPrimaryAction" OnClick="btnSaveOption_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnSaveAndClose" resourcekey="btnSaveAndClose" runat="server" CssClass="hcSecondaryAction" OnClick="btnSaveAndClose_Click" />
            </li>
            <li>
                <a class="hcSecondaryAction" href="ProductChoices.aspx?id=<%=ProductId %>"><%=Localization.GetString("Back") %></a>
            </li>
        </ul>
</asp:Content>
