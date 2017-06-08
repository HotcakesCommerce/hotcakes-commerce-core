<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCategoryEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.ProductCategoryEditor" %>
<h1><%=Localization.GetString("WhenProductIsInCategory") %></h1>

<div class="hcForm">
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstProductCategories" runat="server" />
    
        <asp:LinkButton ID="btnAddProductCategory" runat="server" CssClass="hcSecondaryAction hcSmall"
            resourcekey="Add" OnClientClick="setPopupHeightForQualification();" OnClick="btnAddProductCategory_Click" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvProductCategories" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvProductCategories_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteProductCategory" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteProductCategory_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoCategoriesAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
