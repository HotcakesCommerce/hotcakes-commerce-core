<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.OptionItemEditor" CodeBehind="OptionItemEditor.ascx.cs" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    var hcEditItemDialog = function () {
        $("#hcEditItemDialog").hcDialog({
            title: "<%=Localization.GetJsEncodedString("DialogHeader")%>",
            width: 400,
            height: 'auto',
            maxHeight: 500,
            parentElement: '#<%=pnlEditItem.ClientID%>',
            close: function () {
                    <%= Page.ClientScript.GetPostBackEventReference(lnkEditorCancel, "") %>
            }
        });
    };

    jQuery(function ($) {
        hcUpdatePanelReady(function () {
            $("#<%=gvItems.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                axis: 'y',
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('CatalogHandler.ashx',
                    {
                        "method": "ResortChoiceItems",
                        "itemIds": ids,
                        "optionId": '<%=OptionId%>'
                    });
                }
            }).disableSelection();
        });
    });
</script>
<asp:UpdatePanel UpdateMode="Conditional" runat="server">
    <ContentTemplate>
        <div class="hcForm" style="width: 50%">
            <div class="hcFormItem hcFormItem66p">
                <asp:TextBox ID="txtNewName" runat="server" Style="width: 100%" />
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton ID="btnNew" resourcekey="btnNew" CssClass="hcButton hcSmall" runat="server" OnClick="btnNew_Click" />
            </div>
        </div>
        <asp:GridView ID="gvItems" CssClass="hcGrid" DataKeyNames="Bvin" AutoGenerateColumns="false" runat="server">
            <RowStyle CssClass="hcGridRow" />
            <HeaderStyle CssClass="hcGridHeader" />
            <Columns>
                <asp:TemplateField>
                    <ItemStyle Width="22px" />
                    <ItemTemplate>
                        <span class='hcIconMove'></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="IsLabel" HeaderText="Label" ItemStyle-Width="40px" />
                <asp:CheckBoxField DataField="IsDefault" HeaderText="Default" ItemStyle-Width="40px" />
                <asp:BoundField DataField="Name" HeaderText="ItemName" />
                <asp:BoundField DataField="PriceAdjustment" DataFormatString="{0:c}" HeaderText="Price" />
                <asp:BoundField DataField="WeightAdjustment" DataFormatString="{0:f3}" HeaderText="Weight" />
                <asp:TemplateField>
                    <ItemStyle Width="80px" HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" resourcekey="btnEdit" CommandName="Edit" CssClass="hcIconEdit" runat="server" />
                        <asp:LinkButton ID="btnDelete" resourcekey="btnDelete" runat="server" CssClass="hcIconDelete" CommandName="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlEditItem" Visible="false" runat="server">
            <div id="hcEditItemDialog" class="dnnClear">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <label class="hcLabel"><%=Localization.GetString("lblName") %><i class="hcLocalizable"></i></label>
                        <asp:TextBox ID="txtItemName" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvItemName" resourcekey="rfvItemName" ControlToValidate="txtItemName" CssClass="hcFormError" Display="Dynamic" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <label class="hcLabel"><%=Localization.GetString("lblPriceAdjustment") %></label>
                        <asp:TextBox ID="txtItemPrice" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvItemPrice" resourcekey="rfvItemPrice" ControlToValidate="txtItemPrice" runat="server" CssClass="hcFormError" Display="Dynamic" />
                        <asp:CompareValidator ID="cvItemPrice" resourcekey="cvItemPrice" ControlToValidate="txtItemPrice" Type="Currency" Operator="DataTypeCheck"
                            CssClass="hcFormError" Display="Dynamic" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <label class="hcLabel"><%=Localization.GetString("lblWeightAdjustment") %></label>
                        <asp:TextBox ID="txtItemWeight" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvItemWeight" resourcekey="rfvItemWeight" ControlToValidate="txtItemWeight" runat="server" CssClass="hcFormError" Display="Dynamic" />
                        <asp:CompareValidator ID="cvItemWeight" resourcekey="cvItemWeight" ControlToValidate="txtItemWeight" Type="Double" Operator="DataTypeCheck"
                            CssClass="hcFormError" Display="Dynamic" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:CheckBox ID="cbItemIsLabel" resourcekey="cbItemIsLabel" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:CheckBox ID="cbItemIsDefault" resourcekey="cbItemIsDefault" runat="server" />
                    </div>
                </div>

                <ul class="hcActions">
                    <li>
                        <asp:LinkButton ID="lnkEditorSave" resourcekey="lnkEditorSave" CssClass="hcPrimaryAction" runat="server" />
                    </li>
                    <li>
                        <asp:LinkButton ID="lnkEditorCancel" resourcekey="lnkEditorCancel" CssClass="hcSecondaryAction" runat="server" />
                    </li>
                </ul>
            </div>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
