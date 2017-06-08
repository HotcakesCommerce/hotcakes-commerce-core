<%@ Page MasterPageFile="../AdminNav.master" ValidateRequest="False"
    Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit_Images" CodeBehind="Products_Edit_Images.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ucProductNavigator" runat="server" SelectedMenuItem="AdditionalImages" />
    <hcc:ProductEditing ID="ucProductEditing" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function ($) {
            $("#<%=gvAditionalImages.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('CatalogHandler.ashx',
                    {
                        "method": "ResortAdditionalImages",
                        "itemIds": ids,
                        "productId": '<%=ProductId%>'
                    });
                }
            }).disableSelection();
        });
    </script>
    <h1><%=this.PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:FileUpload ID="fuImageUpload" runat="server" Columns="40" />
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="hcPrimaryAction hcSmall" ResourceKey="AddNewImage" />
        </div>
        <div class="hcFormItem">
            <asp:GridView ID="gvAditionalImages" runat="server" AutoGenerateColumns="False" CssClass="hcGrid" DataKeyNames="Bvin">
                <HeaderStyle CssClass="hcGridHeader" />
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="22px" />
                        <ItemTemplate>
                            <span class='hcIconMove'></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Image Preview">
                        <ItemTemplate>
                            <asp:Image Width="70px" ID="imgImage" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="File Name">
                        <ItemTemplate>
                            <asp:Literal ID="litFileName" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ButtonType="Link" ShowDeleteButton="True">
                        <ItemStyle Width="30px" />
                        <ControlStyle CssClass="hcIconDelete" />
                    </asp:CommandField>
                </Columns>
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoImagesFound") %>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
