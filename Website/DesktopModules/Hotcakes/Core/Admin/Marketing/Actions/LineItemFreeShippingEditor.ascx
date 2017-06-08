<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LineItemFreeShippingEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.LineItemFreeShippingEditor" %>
<h1><%=Localization.GetString("MarkLineItemForFreeShipping") %></h1>
<div class="hcColumn hcForm">
    <div class="hcFormItem hcFormItem66p">
        <asp:DropDownList ID="lstFreeShippingMethods" runat="server" />
    </div>
    <div class="hcFormItem hcFormItem33p">
        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="hcButton" OnClientClick="setPopupHeightForAction();" OnClick="btnAddFreeShippingMethod_Click" resourcekey="Add" />
    </div>
    <div class="hcFormItem">
        <asp:GridView ID="gvFreeShippingMethods" runat="server" AutoGenerateColumns="False"
            DataKeyNames="Bvin" OnRowDeleting="gvFreeShippingMethods_RowDeleting" CssClass="hcGrid"
            ShowHeader="False">
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:BoundField DataField="DisplayName" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDeleteFreeShippingMethod" runat="server" CausesValidation="False"
                            CommandName="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteFreeShippingMethod_OnPreRender" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <%=Localization.GetString("NoShippingMethods") %>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
