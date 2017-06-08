<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Default" Title="Untitled Page" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/Pager.ascx" TagPrefix="hcc" TagName="Pager" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    
    <hcc:NavMenu runat="server" ID="NavMenu" CurrentUrl="people/customer" />

    <div class="hcBlock hcBlockLight">
        <div class="hcForm">
            <div class="hcFormItem hcGo">
                <div class="hcLabel"><%=Localization.GetString("Search") %></div>
                <div class="hcFieldOuter">
                    <asp:TextBox ID="txtKeywords" runat="server" ValidationGroup="SearchCustomers" />
                    <asp:LinkButton ID="btnGo" resourcekey="btnGo" runat="server" CssClass="hcIconRight" ValidationGroup="SearchCustomers" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" resourcekey="btnNew" CssClass="hcTertiaryAction" runat="server" OnClick="btnNew_Click" CausesValidation="False"/>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <h1><%=Localization.GetString("Customers") %></h1>

    <div class="hcInfoLabel"><%=string.Format(Localization.GetString("ResultsFound"), RowCount) %></div>
    
    <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Bvin"
        OnRowEditing="gvCustomers_RowEditing" OnRowDeleting="gvCustomers_RowDeleting" OnRowDataBound="gvCustomers_OnRowDataBound">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetAvatar(DataBinder.Eval(Container.DataItem, "Bvin"), DataBinder.Eval(Container.DataItem, "Email")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetDisplayName(DataBinder.Eval(Container.DataItem, "FirstName"), DataBinder.Eval(Container.DataItem, "LastName")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Email" />
            <asp:TemplateField ShowHeader="False">
                <ItemStyle Width="80" CssClass="hcCenter" />
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="hcIconEdit" OnPreRender="btnEdit_OnPreRender" />
                    <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <hcc:Pager ID="ucPager" PageSize="15" runat="server" PostBackMode="true" PageSizeSet="15,30,50,0" />

</asp:Content>
