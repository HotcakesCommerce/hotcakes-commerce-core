<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.Columns_Edit" Title="Untitled Page" CodeBehind="Columns_Edit.aspx.cs" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ContentColumnEditor.ascx" TagName="ContentColumnEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="catalog/" runat="server" />

    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <h2><%=Localization.GetString("lblAdvancedOptions") %></h2>
            <asp:Panel ID="pnlAdvanced" runat="server" DefaultButton="btnClone">
                <div class="hcFormItem hcGo">
                    <label class="hcLabel"><%=Localization.GetString("lblCopy") %></label>
                    <div class="hcSelectOuter">
                        <asp:DropDownList runat="server" ID="CopyToList"/>
                        <asp:LinkButton ID="btnCopyBlocks" resourcekey="btnCopyBlocks" runat="server" CssClass="hcIconRight" OnClick="btnCopyBlocks_Click" />
                    </div>
                </div>
                <div class="hcFormItem hcGo">
                    <label class="hcLabel"><%=Localization.GetString("lblClone") %></label>
                    <asp:TextBox ID="CloneNameField" runat="server"/>
                    <asp:LinkButton ID="btnClone" resourcekey="btnClone" runat="server" CssClass="hcTertiaryAction" OnClick="btnClone_Click" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" EnableViewState="false" />
    <hcc:ContentColumnEditor ID="ContentColumnEditor" runat="server" />

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnOk" resourcekey="btnOk" runat="server" CssClass="hcPrimaryAction" OnClick="btnOk_Click" />
        </li>
    </ul>
</asp:Content>
