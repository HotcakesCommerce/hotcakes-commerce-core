<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.EmailTemplates" Title="Untitled Page" CodeBehind="EmailTemplates.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" BaseUrl="configuration-admin/" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnCreate" resourcekey="btnCreate" CssClass="hcTertiaryAction" runat="server" OnClick="btnNew_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <asp:GridView ID="gvTemplates" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
        CssClass="hcGrid" OnRowDeleting="gvTemplates_RowDeleting" OnRowEditing="gvTemplates_RowEditing" OnRowDataBound="gvTemplates_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:BoundField DataField="DisplayName" />
            <asp:TemplateField ShowHeader="False">
                <ItemStyle Width="80" />
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="hcIconEdit" OnPreRender="btnEdit_OnPreRender" />
                    <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>