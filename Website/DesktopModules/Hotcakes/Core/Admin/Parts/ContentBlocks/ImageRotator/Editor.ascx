<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Editor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ImageRotator.Editor" %>
<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    var hcShowEditImageDialog = function () {
        $("#hcEditImageDialog").hcDialog({
            title: "Add/Modify Image",
            width: 600,
            height: 'auto',
            maxHeight: 900,
            parentElement: '#<%=pnlEditImageDialog.ClientID%>',
            close: function () {
                <%= Page.ClientScript.GetPostBackEventReference(btnCancel, "") %>
            }
        });
    };

    jQuery(function ($) {
        hcUpdatePanelReady(function () {
            $("#<%=gvImages.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('ColumnsHandler.ashx',
                    {
                        "method": "ResortImageRotator",
                        "itemIds": ids,
                        "blockId": '<%=BlockId%>'
                    });
                }
            }).disableSelection();
        });
    });

</script>

<div class="hcColumnLeft" style="width: 50%">
    <div class="hcForm">
        <div class="hcFormItem">
            <label class="hcLabel">CSS Class <small>(optional)</small></label>
            <asp:TextBox ID="cssclass" runat="server" Columns="40" />
        </div>
        <div class="hcFormItem">
            <label class="hcLabel">Pause for</label>
            <asp:TextBox ID="PauseField" Columns="5" Width="150px" runat="server">2</asp:TextBox>
            seconds
        </div>
        <div class="hcFormItem">
            <label class="hcLabel">Width</label>
            <asp:TextBox ID="WidthField" runat="server" Columns="5" />
        </div>
        <div class="hcFormItem">
            <label class="hcLabel">Height</label>
            <asp:TextBox ID="HeighField" runat="server" Columns="5" />
        </div>
    </div>
</div>
<div class="hcColumnRight hcLeftBorder" style="width: 49%">
    <div class="hcForm">
        <asp:UpdatePanel ID="upImages" runat="server">
            <ContentTemplate>
                <div class="hcFormItem">
                    <asp:CheckBox ID="chkShowInOrder" Text="Rotate images in the order shown here" runat="server" />
                </div>
                <div class="hcClear"></div>
                <asp:GridView ID="gvImages" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="Id" CssClass="hcGrid"
                    OnRowDataBound="gvImages_RowDataBound"
                    OnRowDeleting="gvImages_RowDeleting" OnRowEditing="gvImages_RowEditing">
                    <RowStyle CssClass="hcGridRow" />
                    <HeaderStyle CssClass="hcGridHeader" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle Width="22px" />
                            <ItemTemplate>
                                <span class='hcIconMove'></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phImagePreview" runat="server" />
                                <a runat="server" id="aPreview">
                                    <div style="height: 75px; overflow: hidden">
                                        <img runat="server" id="imgPreview" style="width: 75px" />
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle Width="80px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="hcIconEdit" CommandName="Edit" Text="Edit" />
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="hcIconDelete" 
                                    CommandName="Delete" Text="Delete" OnClientClick="return hcConfirm(event, 'Are you sure you want to delete this image?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:LinkButton ID="lnkAddImage" Text="Add Image" CssClass="hcSecondaryAction" runat="server" />
                <asp:Panel ID="pnlEditImageDialog" runat="server" Visible="false">
                    <div id="hcEditImageDialog">
                        <div class="hcForm">
                            <div class="hcFormItem">
                                <label class="hcLabel">Image URL</label>
                                <asp:TextBox ID="ImageUrlField" runat="server" Columns="20" Style="width: 60% !important" />
                                <a href="javascript:popUpWindow('?returnScript=SetImage&WebMode=1');" class="hcButton hcSmall" style="width: 30%;">Browse
                                </a>
                            </div>
                            <div class="hcFormItem">
                                <label class="hcLabel">Link To</label>
                                <asp:TextBox ID="ImageLinkField" runat="Server" Columns="40" />
                            </div>
                            <div class="hcFormItem">
                                <label class="hcLabel">Tool Tip</label>
                                <asp:TextBox ID="AltTextField" runat="Server" Columns="40" />
                            </div>
                            <div class="hcFormItem">
                                <asp:CheckBox ID="chkOpenInNewWindow" Text="Open in New Window" runat="server" />
                            </div>
                        </div>
                        <ul class="hcActions">
                            <li>
                                <asp:LinkButton ID="btnNew" runat="server" Text="OK" CssClass="hcPrimaryAction" OnClick="btnNew_Click" />
                            </li>
                            <li>
                                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="hcSecondaryAction" OnClick="btnCancel_Click" />
                            </li>
                        </ul>
                    </div>
                    <asp:HiddenField ID="EditBvinField" runat="server" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnOkay" Text="Save Changes" runat="server" CssClass="hcPrimaryAction" OnClick="btnOkay_Click" />
    </li>
    <li>
        <asp:LinkButton ID="btnBack" Text="Cancel" runat="server" CssClass="hcSecondaryAction" OnClick="btnBack_Click" />
    </li>
</ul>
