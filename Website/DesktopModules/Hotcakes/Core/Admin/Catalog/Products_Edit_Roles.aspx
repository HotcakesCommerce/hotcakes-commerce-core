<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Products_Edit_Roles.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit_Roles" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="Roles" />
    <hcc:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 60%">
        <div class="hcForm">
            <div class="hcFormItemLabel">
                <label class="hcLabel"><%=Localization.GetString("SecurityRole") %></label>
            </div>
            <div class="hcFormItem hcFormItem66p">
                <asp:DropDownList ID="ddlRoles" runat="server">
                </asp:DropDownList>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton ID="btnAdd" ResourceKey="AddRole" CssClass="hcButton hcSmall" runat="server" />
            </div>

            <div class="hcFormItem">
                <asp:GridView ID="gvRoles" AutoGenerateColumns="false" DataKeyNames="CatalogRoleId" CssClass="hcGrid" runat="server">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:BoundField DataField="RoleName" HeaderText="Role" />
                        <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                            <ItemStyle Width="30px" />
                            <ControlStyle CssClass="hcIconDelete" />
                        </asp:CommandField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 39%">
        <div class="hcForm">
            <h2><%=Localization.GetString("RolesHierarchy") %></h2>
            <%=Localization.GetString("RolesHierarchyDescription") %>
        </div>
    </div>
</asp:Content>
