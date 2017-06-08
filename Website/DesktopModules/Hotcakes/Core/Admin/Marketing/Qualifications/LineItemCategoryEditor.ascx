<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LineItemCategoryEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.LineItemCategoryEditor" %>
<h1><%=Localization.GetString("WhenOrderItemIsInCategory") %></h1>
<div class="hcColumn hcForm">
    <div class="hcFormItem">
        <asp:CheckBox ID="chkLineItemCategoryNot" resourcekey="chkLineItemCategoryNot" runat="server" />
    </div>
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstLineItemCategories" runat="server" />
    </div>
    <div class="hcFormItem hcFormItem33p">
        <asp:LinkButton runat="server" CssClass="hcButton" OnClientClick="setPopupHeightForQualification();" OnClick="btnAddLineItemCategory_Click" resourcekey="Add" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvLineItemCategories" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvLineItemCategories_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteLineItemCategory" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteLineItemCategory_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoCategoriesAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
