<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryDiscountEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.CategoryDiscountEditor" %>
<h1><%=Localization.GetString("DiscountQualifyingItemBy") %></h1>
<table class="hc-popup-table" hc-popup-width="800" style="width: 100%;">
    <tr>
        <td >
            <div class="hcForm">
                <div class="hcFormItem hcFormItem66p">
                    <asp:DropDownList ID="lstProductCategories" runat="server" />
                </div>
                <div class="hcFormItem hcFormItem33p">
                    <asp:LinkButton ID="btnAddProductCategory" runat="server" CssClass="hcButton hcSmall"
                        resourcekey="Add" OnClientClick="setPopupHeightForAction();" OnClick="btnAddProductCategory_Click" />
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
        </td>
        <td style="vertical-align: top; text-align:center; width: 50%;padding-left:100px;">
            <div class="hcForm" style="width: 200px;">
                <div class="hcFormItemLabel">
                    <label class="hcLabel"><%=Localization.GetString("AdjustPriceBy") %></label>
                </div>
                <div class="hcFormItem hcFormItem50p">
                    <asp:TextBox ID="LineItemAdjustAmountField" runat="server" Columns="10"></asp:TextBox>
                </div>
                <div class="hcFormItem hcFormItem50p">
                    <asp:DropDownList ID="lstLineItemAdjustType" runat="server" />
                </div>
            </div>
        </td>
    </tr>
</table>


