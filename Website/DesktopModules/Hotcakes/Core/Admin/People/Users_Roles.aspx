<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Users_Roles.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.People.Users_Roles" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="people/customer" runat="server" />
    
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" resourcekey="Close" CssClass="hcTertiaryAction" NavigateUrl="Default.aspx" />
            </div>
        </div>
    </div>

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
                <asp:DropDownList ID="ddlRoles" runat="server"/>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton ID="btnAdd" resourcekey="AddRole" CssClass="hcButton hcSmall" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:GridView ID="gvRoles" AutoGenerateColumns="false" DataKeyNames="RoleName" CssClass="hcGrid" runat="server" OnRowDataBound="gvRoles_OnRowDataBound">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle Width="25px"/>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbSelect" Checked='<%#IsChecked(Container) %>' Text="" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="RoleName" />
                        <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                            <ItemStyle Width="30px" />
                            <ControlStyle CssClass="hcIconDelete" />
                        </asp:CommandField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="hcFormWarning"><%=Localization.GetString("NoRoles") %></div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnSendNotification" CssClass="hcSecondaryAction" resourcekey="SendEmailNotification" runat="server" />
                </li>
            </ul>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 39%">
        <div class="hcForm">
            <h2><%=Localization.GetString("ProductRoles") %></h2>
            <p><%=Localization.GetString("ProductRolesMessage") %></p>
            <h3><%=Localization.GetString("EmailNotification") %></h3>
            <p><%=Localization.GetString("EmailNotificationMessage") %></p>
        </div>
    </div>
</asp:Content>
