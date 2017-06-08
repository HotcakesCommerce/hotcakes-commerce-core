<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.CategoryRotator.Editor" CodeBehind="Editor.ascx.cs" %>
<%@ Register Src="../../../Controls/CategoryPicker.ascx" TagName="CategoryPicker"
    TagPrefix="hcc" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    jQuery(function ($) {

        hcUpdatePanelReady(function () {
            $("#<%=gvCategories.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('ColumnsHandler.ashx',
                    {
                        "method": "ResortCategoryRotator",
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
                <h2>All Categories</h2>
                <hcc:CategoryPicker ID="ucCategoryPicker" runat="server" />
            </div>
        </div>
        <div class="hcColumnRight hcLeftBorder" style="width: 49%">
            <div class="hcForm">
                <h2>Selected Categories</h2>

                <asp:GridView ID="gvCategories" runat="server" DataKeyNames="bvin" CssClass="hcGrid"
                    AutoGenerateColumns="false" OnRowDeleting="gvCategories_RowDeleting">
                    <HeaderStyle CssClass="hcGridHeader" />
                    <RowStyle CssClass="hcGridRow" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle Width="22px" />
                            <ItemTemplate>
                                <span class='hcIconMove'></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Category Name" />
                        <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                            <ItemStyle Width="30px" />
                            <ControlStyle CssClass="hcIconDelete" />
                        </asp:CommandField>
                    </Columns>
                    <EmptyDataTemplate>
                        There are no selected categories.
                    </EmptyDataTemplate>
                </asp:GridView>

                <div class="hcFormItem">
                    <asp:CheckBox ID="chkShowInOrder" Text="Rotate categories in the order shown above" runat="server" />
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnOk" runat="server" Text="< Back" CssClass="hcPrimaryAction" OnClick="btnOK_Click" />
    </li>
<%--    <li>
        <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="hcSecondaryAction" />
    </li>--%>
</ul>
