<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Users_AddressBook.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.People.Users_AddressBook" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">

    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="people/customer" runat="server" />

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" resourcekey="Close" CssClass="hcTertiaryAction" NavigateUrl="Default.aspx" />
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNewAddress" runat="server" resourcekey="NewAddress" CssClass="hcTertiaryAction" OnClick="btnNewAddress_Click" />
            </div>
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkDnnUserProfile" resourcekey="lnkDnnUserProfile" Visible="false" CssClass="hcButton" Target="_blank" runat="server" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">

    <h1><%=PageTitle %></h1>

    <div class="hcForm">
        <asp:GridView ID="gvAddress" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Bvin"
            OnRowDeleting="gvAddress_OnRowDeleting" OnRowEditing="gvAddress_OnRowEditing" OnRowDataBound="gvAddress_OnRowDataBound">
            <HeaderStyle CssClass="hcGridHeader" />
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <%#GetNickName(Eval("NickName")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <%#GetDisplayName(Eval("FirstName"), Eval("LastName")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemStyle Width="80" />
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="hcIconEdit" OnPreRender="btnEdit_OnPreRender" />
                        <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoAddresses") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

</asp:Content>
