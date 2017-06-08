<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VendorOrManufacturerEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.VendorOrManufacturerEditor" %>
<h1><%=Localization.GetString("WhenProductTypeIs") %></h1>

<div class="hcColumn hcForm">
    <div class="hcFormItem">
        <asp:CheckBox runat="server" ID="cbIsNot" Text="Only qualify items when NOT assigned to these vendors/manufacturers" />
    </div>
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstItems" runat="server" />
    </div>
    <div class="hcFormItem hcFormItem33p">
        <asp:LinkButton ID="btnAddItem" runat="server"
            resourcekey="Add" CssClass="hcButton" OnClientClick="setPopupHeightForQualification();" OnClick="btnAddItem_Click" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvItems_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteItem" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteItem_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoVendorsAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
