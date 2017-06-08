<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BundledProducts.aspx.cs" MasterPageFile="../AdminNav.master" Inherits="Hotcakes.Modules.Core.Admin.Catalog.BundledProducts" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" SelectedMenuItem="BundledProducts" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function ($) {
            $("#<%=gvBundledProducts.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('CatalogHandler.ashx',
                    {
                        "method": "ResortBundledProducts",
                        "bundledProductIds": ids
                    });
                }
            });
        });
    </script>
    <h1><%=PageTitle %></h1>
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2>Search Products</h2>
            <hcc:ProductPicker ID="ucProductPicker" runat="server" DisplayGiftCards="false" DisplayBundles="false" DisplayRecurring="false" />
            <div class="hcFormItem">
                <asp:LinkButton ID="btnAddProducts" runat="server" CssClass="hcButton" Text="Add Selected >>" OnClick="btnAddProducts_Click" />
            </div>
        </div>
    </div>

    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2>Selected Products</h2>
            <asp:GridView ID="gvBundledProducts" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Id">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="22px" />
                        <ItemTemplate>
                            <span class='hcIconMove'></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Product">
                        <ItemTemplate>
                            <asp:Literal ID="litProductName" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Qty">
                        <ItemTemplate>
                            <div class="hcFormItem">
                                <asp:TextBox ID="txtQuantity" runat="server" Width="30px" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQuantity" Text="Quantity is required" CssClass="hcFormError" />
                                <asp:CompareValidator runat="server" ControlToValidate="txtQuantity" Operator="DataTypeCheck" Type="Integer"
                                    Text="Quantity must be integer value" CssClass="hcFormError" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                        <ItemStyle Width="30px" />
                        <ControlStyle CssClass="hcIconDelete" />
                    </asp:CommandField>
                </Columns>
                <EmptyDataTemplate>
                    No products selected. Choose products from left column.
                </EmptyDataTemplate>
            </asp:GridView>
            <ul class="hcActions">
                <li>
                    <asp:LinkButton runat="server" ID="btnUpdate" CssClass="hcPrimaryAction" Text="Update" OnClick="btnUpdate_Click" />
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
