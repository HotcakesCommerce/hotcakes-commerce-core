<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Manufacturers" Title="Untitled Page" CodeBehind="Manufacturers.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/Pager.ascx" TagPrefix="hcc" TagName="Pager" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">

    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="people/" />

    <div runat="server" id="divNavBottom">
        <div class="hcBlock hcBlockLight">
            <div class="hcForm">
                <div class="hcFormItem hcGo">
                    <div class="hcLabel"><%=Localization.GetString("Search") %></div>
                    <div class="hcFieldOuter">
                        <asp:TextBox ID="txtKeywords" runat="server" ValidationGroup="SearchManufacturers" />
                        <asp:LinkButton ID="btnGo" resourcekey="btnGo" runat="server" CssClass="hcIconRight" ValidationGroup="SearchManufacturers" />
                    </div>
                </div>
            </div>
        </div>

        <div class="hcBlock">
            <div class="hcForm">
                <div class="hcFormItem">
                    <asp:LinkButton ID="btnNew" resourcekey="btnNew" runat="server" CssClass="hcTertiaryAction" OnClick="btnNew_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <h1><%=PageTitle %></h1>

    <div class="hcInfoLabel"><%=string.Format(Localization.GetString("ResultsFound"), RowCount) %></div>

    <asp:GridView ID="gvManufacturers" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Bvin"
        OnRowEditing="gvManufacturers_RowEditing" OnRowDeleting="gvManufacturers_RowDeleting" OnRowDataBound="gvManufacturers_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:BoundField DataField="DisplayName" />
            <asp:BoundField DataField="EmailAddress" />
            <asp:TemplateField ShowHeader="False">
                <ItemStyle Width="80" CssClass="hcIconWrapper" />
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="hcIconEdit" OnPreRender="btnEdit_OnPreRender" />
                    <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <hcc:Pager ID="ucPager" PageSize="10" runat="server" PostBackMode="true" PageSizeSet="10,50,0" />

</asp:Content>
