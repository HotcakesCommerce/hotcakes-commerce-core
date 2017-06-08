<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserIsInGroupEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.UserIsInGroupEditor" %>
<h1><%=Localization.GetString("WhenUserPriceGroupIs") %></h1>

<div class="hcForm">
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstUserIsInGroup" runat="server" />

        <asp:LinkButton runat="server" CssClass="hcSecondaryAction hcSmall" OnClientClick="setPopupHeightForQualification();"
            OnClick="btnAddUserIsInGroup_Click" resourcekey="Add" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvUserIsInGroup" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvUserIsInGroup_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteUserIsInGroup" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteUserIsInGroup_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoUserPriceGroupsAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
