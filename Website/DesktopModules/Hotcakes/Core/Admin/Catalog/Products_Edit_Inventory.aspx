<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit_Inventory" Title="Untitled Page" CodeBehind="Products_Edit_Inventory.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="Inventory" />
    <hcc:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>Inventory</h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:Label runat="server" CssClass="hcLabel">Inventory Mode</asp:Label>
            <asp:DropDownList ID="OutOfStockModeField" runat="server">
                <asp:ListItem Text="Always In Stock (Ignore Inventory)" Value="100" />
                <asp:ListItem Text="When Out of Stock, Remove from Store" Value="101" />
                <asp:ListItem Text="When Out of Stock, Show but Don't Allow Purchases" Value="102" />
                <asp:ListItem Text="When Out of Stock, Allow Back Orders" Value="103" />
            </asp:DropDownList>
        </div>
    </div>
    <asp:GridView ID="EditsGridView" runat="server" DataKeyNames="bvin"
        GridLines="None" AutoGenerateColumns="False"
        OnRowDataBound="EditsGridView_RowDataBound" CssClass="hcGrid">
        <Columns>
            <asp:TemplateField HeaderText="SKU">
                <ItemTemplate>
                    <asp:Label CssClass="smalltext" ID="lblSKU" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="On Hand">
                <ItemTemplate>
                    <asp:Label ID="lblQuantityOnHand" runat="server" Text="0" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Reserved">
                <ItemTemplate>
                    <asp:Label ID="lblQuantityReserved" runat="server" Text="0" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Avail. For Sale">
                <ItemTemplate>
                    <asp:Label ID="lblQuantityAvailableForSale" runat="server" Text="0" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Stock Out At">
                <ItemTemplate>
                    <asp:TextBox ID="OutOfStockPointField" runat="server" Columns="5" Text="0" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Low Stock At">
                <ItemTemplate>
                    <asp:TextBox ID="LowPointField" runat="server" Columns="5" Text="0" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Adjustment">
                <ItemTemplate>
                    <asp:DropDownList ID="AdjustmentModeField" runat="Server">
                        <asp:ListItem Value="1" Text="Add" />
                        <asp:ListItem Value="2" Text="Subtract" />
                        <asp:ListItem Value="3" Text="Set To" />
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Quantity">
                <ItemTemplate>
                    <asp:TextBox ID="AdjustmentField" runat="server" Columns="5" Text="0" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSaveChanges" Text="Save Changes" CssClass="hcPrimaryAction" runat="server" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction" runat="server" CausesValidation="False" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>

