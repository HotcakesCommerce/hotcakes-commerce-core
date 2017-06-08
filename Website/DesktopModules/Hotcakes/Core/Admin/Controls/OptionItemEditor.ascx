<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.OptionItemEditor" CodeBehind="OptionItemEditor.ascx.cs" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    var hcEditItemDialog = function () {
        $("#hcEditItemDialog").hcDialog({
            title: "Edit Item",
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
            <%--            <div class="hcFormLabel">
                <label class="hcLabel"></label>
            </div>--%>
            <div class="hcFormItem hcFormItem66p">
                <asp:TextBox ID="txtNewName" Text="New item" runat="server" Style="width: 100%" />
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton ID="btnNew" CssClass="hcButton hcSmall" runat="server" Text="Add" OnClick="btnNew_Click" />
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
                <asp:BoundField DataField="Name" HeaderText="Item Name" />
                <asp:BoundField DataField="PriceAdjustment" DataFormatString="{0:c}" HeaderText="Price" />
                <asp:BoundField DataField="WeightAdjustment" DataFormatString="{0:f3}" HeaderText="Weight" />
                <asp:TemplateField>
                    <ItemStyle Width="80px" HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" CommandName="Edit" CssClass="hcIconEdit" runat="server" Text="Edit" />
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="hcIconDelete" CommandName="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlEditItem" Visible="false" runat="server">
            <div id="hcEditItemDialog" class="dnnClear">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <label class="hcLabel">Name<i class="hcLocalizable"></i></label>
                        <asp:TextBox ID="txtItemName" runat="server" />
                        <asp:RequiredFieldValidator ErrorMessage="Name is required" ControlToValidate="txtItemName" CssClass="hcFormError" Display="Dynamic" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <label class="hcLabel">Price Adjustment</label>
                        <asp:TextBox ID="txtItemPrice" runat="server" />
                        <asp:RequiredFieldValidator ErrorMessage="Required field" ControlToValidate="txtItemPrice" runat="server" CssClass="hcFormError" Display="Dynamic" />
                        <asp:CompareValidator ErrorMessage="Invalid price value" ControlToValidate="txtItemPrice" Type="Currency" Operator="DataTypeCheck"
                            CssClass="hcFormError" Display="Dynamic" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <label class="hcLabel">Weight Adjustment</label>
                        <asp:TextBox ID="txtItemWeight" runat="server" />
                        <asp:RequiredFieldValidator ErrorMessage="Required field" ControlToValidate="txtItemWeight" runat="server" CssClass="hcFormError" Display="Dynamic" />
                        <asp:CompareValidator ErrorMessage="Invalid price value" ControlToValidate="txtItemWeight" Type="Double" Operator="DataTypeCheck"
                            CssClass="hcFormError" Display="Dynamic" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:CheckBox ID="cbItemIsLabel" Text="Is a Label" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:CheckBox ID="cbItemIsDefault" Text="Is Default" runat="server" />
                    </div>
                </div>

                <ul class="hcActions">
                    <li>
                        <asp:LinkButton ID="lnkEditorSave" CssClass="hcPrimaryAction" Text="Save" runat="server" />
                    </li>
                    <li>
                        <asp:LinkButton ID="lnkEditorCancel" CssClass="hcSecondaryAction" Text="Cancel" runat="server" />
                    </li>
                </ul>
            </div>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
