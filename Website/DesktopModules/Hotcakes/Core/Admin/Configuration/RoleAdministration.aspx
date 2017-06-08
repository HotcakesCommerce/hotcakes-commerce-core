<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="RoleAdministration.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.People.RoleAdministration" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="Message" TagPrefix="hcc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:Message ID="msg" runat="server"/>
    <div class="hcColumnLeft" style="width: 49%">
        <div class="hcForm">
            <p>
                <%=Localization.GetString("IntroMessage") %> 
            </p>
            <div class="hcFormItem">
                <asp:Label resourcekey="CatalogManagement" AssociatedControlID="ddlCatalogManagement" runat="server" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlCatalogManagement" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label resourcekey="OrdersCustomerManagement" AssociatedControlID="ddlOrdersManagement" runat="server" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlOrdersManagement" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label resourcekey="StoreAdministration" AssociatedControlID="ddlStoreAdministration" runat="server" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlStoreAdministration" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label resourcekey="MobileAccess" AssociatedControlID="ddlMobileAccess" runat="server" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlMobileAccess" runat="server" />
            </div>
            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" />
                </li>
            </ul>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 50%">
        <div class="hcForm">
            <p>
                <%=Localization.GetString("HelpMessage") %> 
            </p>
            <ul>
                <li><strong><%=Localization.GetString("CatalogManagement") %></strong>: <%=Localization.GetString("CatalogManagementHelp") %></li>
                <li><strong><%=Localization.GetString("OrdersCustomerManagement") %></strong>: <%=Localization.GetString("OrdersCustomerManagementHelp") %></li>
                <li><strong><%=Localization.GetString("StoreAdministration") %></strong>: <%=Localization.GetString("StoreAdministrationHelp") %></li>
                <li><strong><%=Localization.GetString("MobileAccess") %></strong>: <%=Localization.GetString("MobileAccessHelp") %></li>
            </ul>
        </div>
    </div>
</asp:Content>
