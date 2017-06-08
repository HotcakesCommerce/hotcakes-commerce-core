<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserIsInRoleEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.UserIsInRoleEditor" %>
<h1><%=Localization.GetString("WhenUserIsInRoles") %></h1>

<div class="hcForm">
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstRoles" runat="server" />

        <asp:LinkButton runat="server" CssClass="hcSecondaryAction hcSmall" OnClientClick="setPopupHeightForQualification();"
            OnClick="btnAddRole_Click" resourcekey="Add" ID="btnAddRole" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="False"
            DataKeyNames="RoleID" OnRowDeleting="gvRoles_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="RoleName" />
                <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteRole" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteRole_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoUserRolesAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
