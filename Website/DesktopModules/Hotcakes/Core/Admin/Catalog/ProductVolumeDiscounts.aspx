<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductVolumeDiscounts" Title="Untitled Page" CodeBehind="ProductVolumeDiscounts.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="VolumeDiscounts" />
    <hcc:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        var confirmMessage = "<%=Localization.GetString("ConfirmDelete") %>";
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcForm">
        <div class="hcVolumeDiscountForm dnnClear">
            <div class="hcFormItem">
                <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("Quantity") %></asp:Label>
                <asp:TextBox ID="txtQuantity" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" CssClass="hcLabel"><%=Localization.GetString("Price") %></asp:Label>
                <asp:TextBox ID="txtPrice" runat="server" />
            </div>
            <div class="hcFormItem hcButtonWrapper">
                <asp:LinkButton runat="server" ID="btnNewLevel" OnClick="btnNewLevel_Click" ResourceKey="New" CssClass="hcPrimaryAction hcSmall"/>
            </div>
        </div>
        <asp:GridView ID="VolumeDiscountsGridView" runat="server"
            DataKeyNames="bvin" AutoGenerateColumns="False" GridLines="none"
            OnRowDeleting="VolumeDiscountsGridView_RowDeleting" CssClass="hcGrid" >
            <Columns>
                <asp:BoundField HeaderText="Quantity" DataField="Qty" />
                <asp:BoundField HeaderText="Price" DataField="Amount" DataFormatString="{0:c}" HtmlEncode="False" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="btnDelete" CommandName="Delete" Text="Delete" CssClass="hcIconDelete"
                            OnClientClick="return hcConfirm(event, confirmMessage);" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

