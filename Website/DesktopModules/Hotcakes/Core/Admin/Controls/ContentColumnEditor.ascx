<%@ Control Language="C#" AutoEventWireup="True"
    Inherits="Hotcakes.Modules.Core.Admin.Controls.ContentColumnEditor" CodeBehind="ContentColumnEditor.ascx.cs" %>
<script type="text/javascript">
    jQuery(function ($) {
        $("#<%=gvBlocks.ClientID%> tbody").sortable({
            items: "tr.hcGridRow",
            placeholder: "ui-state-highlight",
            update: function (event, ui) {
                var ids = $(this).sortable('toArray');
                ids += '';
                $.post('ColumnsHandler.ashx',
                {
                    "method": "ResortColumnItems",
                    "itemIds": ids,
                    "columnId": '<%=ColumnId%>'
                });
            }
        }).disableSelection();
    });
</script>
<div class="hcContentBlock">
    <h2>
        <asp:Label ID="lblTitle" runat="server" />
    </h2>

    <div class="hcForm" style="width: 60%">
        <div class="hcFormItem  hcFormItem50p">
            <asp:DropDownList ID="lstBlocks" runat="server" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:LinkButton ID="btnNew" runat="server" Text="Add Content Block" CssClass="hcPrimaryAction hcSmall" OnClick="btnNew_Click" />
        </div>
    </div>

    <div class="hcForm">
        <div class="hcFormItem">
            <asp:GridView ID="gvBlocks" runat="server" AutoGenerateColumns="False" DataKeyNames="bvin"
                CssClass="hcGrid" OnRowDataBound="gvBlocks_RowDataBound" OnRowDeleting="gvBlocks_RowDeleting">
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="22px" />
                        <ItemTemplate>
                            <span class='hcIconMove'></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Content Block"></asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="80px" HorizontalAlign="Right" />
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkEdit" CssClass="hcIconEdit" runat="server" NavigateUrl='<%# Eval("bvin", "~/DesktopModules/Hotcakes/Core/Admin/Content/Columns_EditBlock.aspx?id={0}") %>' Text="Edit"></asp:HyperLink>
                            <asp:LinkButton ID="btnDelete" OnClientClick="return hcConfirm(event, 'Delete this content block?');"  runat="server" CssClass="hcIconDelete" CommandName="Delete"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
