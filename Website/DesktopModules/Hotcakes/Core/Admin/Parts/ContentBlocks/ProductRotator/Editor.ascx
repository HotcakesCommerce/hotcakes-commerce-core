<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ProductRotator.Editor" CodeBehind="Editor.ascx.cs" %>
<%@ Register Src="../../../Controls/ProductPicker.ascx" TagName="ProductPicker"
    TagPrefix="hcc" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    jQuery(function ($) {

        hcUpdatePanelReady(function () {
            $("#<%=gvProducts.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('ColumnsHandler.ashx',
                    {
                        "method": "ResortProductRotator",
                        "itemIds": ids,
                        "blockId": '<%=BlockId%>'
                    });
                }
            }).disableSelection();
        });

    });
</script>

<asp:UpdatePanel ID="upMain" runat="server">
    <ContentTemplate>

        <div class="hcColumnLeft" style="width: 50%">
            <div class="hcForm">
                <h2>Search Products</h2>
                <hcc:ProductPicker ID="ucProductPicker" runat="server" />
                <div class="hcFormItem">
                    <asp:LinkButton ID="btnAddProducts" runat="server" CssClass="hcButton" Text="Add Selected >>" OnClick="btnAddProducts_Click" />
                </div>
                <asp:HiddenField ID="EditBvinField" runat="server" />
            </div>
        </div>

        <div class="hcColumnRight hcLeftBorder" style="width: 49%">
            <div class="hcForm">
                <h2>Selected Products</h2>
                <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" CssClass="hcGrid"
                    DataKeyNames="Id" OnRowDeleting="gvProducts_RowDeleting">
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
    </ContentTemplate>
</asp:UpdatePanel>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnOkay" runat="server" Text="< Back" CssClass="hcPrimaryAction" OnClick="btnOkay_Click" />
    </li>
</ul>
