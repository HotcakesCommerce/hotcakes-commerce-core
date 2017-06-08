<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Categories_Roles.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories_Roles" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="catalog/category" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="Categories.aspx" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Category Roles</h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 60%">
        <div class="hcForm">
            <div class="hcFormItemLabel">
                <label class="hcLabel">Security Role</label>
            </div>
            <div class="hcFormItem hcFormItem66p">
                <asp:DropDownList ID="ddlRoles" runat="server">
                </asp:DropDownList>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton ID="btnAdd" Text="Add Role" CssClass="hcSecondaryAction hcSmall" runat="server" />
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
                    <EmptyDataTemplate>
                        No roles added. Category is public.
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
	 <div class="hcColumnRight hcLeftBorder" style="width: 39%">
        <div class="hcForm">
            <h2>Roles Hierarchy</h2>
            <p>Security roles can be selected in the Product Type, Category, and Product edit views.</p>
            <p>Product is public if no roles added.</p>
            <p>If no product roles specified than all assigned category roles will be used. 
                If no category roles specified than product type roles will be used.</p>
            <p>Store administrators and host users are able to see all products.</p>
        </div>
    </div>
</asp:Content>
