<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductTypeEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.ProductTypeEditor" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<h1><%=Localization.GetString("WhenProductTypeIs") %></h1>

<div class="hcColumn hcForm hcPromotionProductTypePopup">
    <div class="hcFormItem">
        <asp:CheckBox runat="server" ID="cbIsNot" Text="Only qualify items when NOT in these product types" />
    </div>
    <div class="hcFormItem hcFormItem66p">
		<telerik:RadComboBox ID="lstProductTypes" runat="server"></telerik:RadComboBox>
    </div>
    <div class="hcFormItem hcFormItem33p">
        <asp:LinkButton ID="btnAddProductType" runat="server" resourcekey="Add" CssClass="hcSecondaryAction hcSmall"
            OnClientClick="setPopupHeightForQualification();" OnClick="btnAddProductType_Click" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvProductTypes" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvProductTypes_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteProductType" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteProductType_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoProductTypesAdded") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
