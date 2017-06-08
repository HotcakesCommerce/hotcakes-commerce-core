<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductUpSellCrossSell" CodeBehind="ProductUpSellCrossSell.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ucProductNavigator" SelectedMenuItem="UpSellCrossSell" runat="server" />
    <hcc:ProductEditing ID="ucProductEditing" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        jQuery(function ($) {
            $("#<%=gvRelatedProducts.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('CatalogHandler.ashx',
                    {
                        "method": "ResortRelatedProducts",
                        "itemIds": ids,
                        "productId": '<%=ProductId%>'
                    });
                }
            }).disableSelection();
        });
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2>Search Products</h2>
            <hcc:ProductPicker ID="ucProductPicker" runat="server" />
            <div class="hcFormItem">
                <asp:LinkButton ID="btnAdd" runat="server" CssClass="hcSecondaryAction" OnClick="btnAdd_Click" Text="Add Selected Products >>" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2>Selected Products</h2>
            <asp:GridView ID="gvRelatedProducts" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Id">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="22px" />
                        <ItemTemplate>
                            <span class='hcIconMove'></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image Width="70px" ID="imgProduct" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Product">
                        <ItemTemplate>
                            <asp:Literal ID="litProductName" runat="server" />
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
        </div>
    </div>
</asp:Content>
