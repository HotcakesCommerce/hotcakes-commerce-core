<%@ Page MasterPageFile="../AdminNav.master" ValidateRequest="False" Language="C#"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductTypesEdit" CodeBehind="ProductTypesEdit.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        jQuery(function ($) {
            hcUpdatePanelReady(function () {
                $("#<%=gvProperties.ClientID%> tbody").sortable({
                    items: "tr.hcGridRow",
                    placeholder: "ui-state-highlight",
                    update: function (event, ui) {
						var ids = $(this).sortable('toArray');
                        ids += '';
                        $.post('CatalogHandler.ashx',
                        {
                            "method": "ResortProductProperties",
                            "propertyIds": ids,
                            "typeId": '<%=TypeId%>'
                        });
                    }
                }).disableSelection();
            });
        });
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcColumnLeft hcRightBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("GeneralSettings") %></h2>
            <div class="hcFormItem">
                <asp:Label runat="server" AssociatedControlID="txtProductTypeNameField" CssClass="hcLabel"><%=Localization.GetString("ProductTypeName") %><i class="hcLocalizable"></i></asp:Label>
                <asp:TextBox ID="txtProductTypeNameField" runat="server" CssClass="FormInput" Columns="40"/>
                <asp:RequiredFieldValidator ID="rfvProductTypeNameField" runat="server" ResourceKey="rfvProductTypeNameField.ErrorMessage" ControlToValidate="txtProductTypeNameField" ValidationGroup="Save" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem">
                <asp:Label ResourceKey="ProductsViewTemplate" runat="server" AssociatedControlID="ddlTemplateList" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlTemplateList" runat="server" />
            </div>

            <asp:UpdatePanel ID="upProperties" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <h2><%=Localization.GetString("ProductTypeProperties") %></h2>
                    <div class="hcFormItemLabel ">
                        <asp:Label ResourceKey="AvailableProperties" CssClass="hcLabel" AssociatedControlID="lstAvailableProperties" runat="server" />
                    </div>
                    <div class="hcFormItemHor hcFormItem66p">
                        <asp:DropDownList ID="lstAvailableProperties" runat="server" />
                    </div>
                    <div class="hcFormItemHor hcFormItem33p">
                        <asp:LinkButton ID="btnAddProperty" runat="server" ResourceKey="Add" CssClass="hcSecondaryAction hcSmall" OnClick="btnAddProperty_Click" />
                    </div>

                    <asp:GridView ID="gvProperties" AutoGenerateColumns="false" CssClass="hcGrid" DataKeyNames="Id" runat="server" OnRowDataBound="gvProperties_RowDataBound">
                        <RowStyle CssClass="hcGridRow" />
                        <HeaderStyle CssClass="hcGridHeader" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemStyle Width="22px" />
                                <ItemTemplate>
                                    <span class='hcIconMove'></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="FriendlyTypeName" HeaderText="FriendlyTypeName" />
                            <asp:BoundField DataField="PropertyName" HeaderText="PropertyName" />
                            <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                                <ItemStyle Width="30px" />
                                <ControlStyle CssClass="hcIconDelete" />
                            </asp:CommandField>
                        </Columns>
                        <EmptyDataTemplate>
                            <%=Localization.GetString("NoPropertiesAdded") %>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="hcColumnRight" style="width: 50%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Roles") %></h2>
            <asp:UpdatePanel ID="upRoles" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div class="hcFormItemLabel">
                        <label class="hcLabel"><%=Localization.GetString("SecurityRole") %></label>
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:DropDownList ID="ddlRoles" runat="server"/>
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnAddRole" ResourceKey="Add" CssClass="hcSecondaryAction hcSmall" runat="server" />
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
                                <%=Localization.GetString("NoRolesAdded") %>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" runat="server" ResourceKey="SaveChanges" OnClick="btnSave_Click" ValidationGroup="Save" CssClass="hcPrimaryAction" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" runat="server" ResourceKey="Cancel" OnClick="btnCancel_Click" CausesValidation="false" CssClass="hcSecondaryAction" />
        </li>
    </ul>

</asp:Content>
