<%@ Page Language="C#" MasterPageFile="../Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.FileVaultDetailsView"
    Title="File Vault Details" Codebehind="FileVaultDetailsView.aspx.cs" %>
<%@ Register Src="../Controls/FilePicker.ascx" TagName="FilePicker" TagPrefix="uc4" %>
<%@ Register Src="../Controls/TimespanPicker.ascx" TagName="TimespanPicker" TagPrefix="uc3" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="uc1" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <div>
        <uc2:MessageBox ID="MessageBox1" runat="server" EnableViewState="false" />
    </div>
    <table width="100%">
        <tr>
            <td>
                <h2>Name:</h2>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="NameLabel" runat="server" Text=""></asp:Label>
                <asp:ImageButton ID="ReplaceImageButton" runat="server" AlternateText="Replace" 
                    ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Replace.png" 
                    onclick="ReplaceImageButton_Click" /><br /><br />
                <div style="width:100%;">
                <asp:Panel ID="ReplacePanel" runat="server" Visible="false" CssClass="controlarea2">
                    <uc4:FilePicker ID="FilePicker1" runat="server" DisplayShortDescription="false" />
                    <asp:ImageButton ID="FileReplaceSaveImageButton" runat="server" 
                        ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/SaveChanges.png" 
                        onclick="FileReplaceSaveImageButton_Click" />
                    <asp:ImageButton ID="FileReplaceCancelImageButton" runat="server" 
                        ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Cancel.png" 
                        onclick="FileReplaceCancelImageButton_Click" />                
                </asp:Panel>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <h2>Description:</h2>
            </td>
        </tr>
        <tr>
            <td class="formfield">
                <asp:TextBox ID="DescriptionTextBox" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="DescriptionTextBox"
                    ErrorMessage="Description is required." ValidationGroup="UpdateDescription">*</asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="formfield" colspan="2">
                <asp:ImageButton ID="SaveImageButton" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/SaveChanges.png"
                    ValidationGroup="UpdateDescription" onclick="SaveImageButton_Click" />
                <asp:ImageButton ID="CancelImageButton" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Cancel.png"
                    CausesValidation="False" onclick="CancelImageButton_Click" />
            </td>
        </tr>
    </table>
    &nbsp;&nbsp;
    <asp:GridView ID="ProductsGridView" runat="server" DataKeyNames="bvin" BorderColor="#CCCCCC"
        CellPadding="3" GridLines="None" Width="100%" AutoGenerateColumns="False" 
        BorderWidth="0px" onrowdatabound="ProductsGridView_RowDataBound" 
        onrowdeleting="ProductsGridView_RowDeleting" 
        onrowupdating="ProductsGridView_RowUpdating">
        <RowStyle CssClass="row" />
        <HeaderStyle CssClass="rowheader" />
        <AlternatingRowStyle CssClass="alternaterow" />
        <Columns>
            <asp:BoundField HeaderText="Product Name" DataField="ProductName" HeaderStyle-Width="200px" />
            <asp:BoundField HeaderText="Product Sku" DataField="Sku" HeaderStyle-Width="75px" />
            <asp:TemplateField HeaderText="Max Downloads">
                <ItemTemplate>
                    <asp:TextBox ID="MaxDownloadsTextBox" runat="server" Columns="3"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="MaxDownloadsRegularExpressionValidator" runat="server"
                        ErrorMessage="Max Downloads must be an integer" Text="*" ControlToValidate="MaxDownLoadsTextBox"
                        ValidationExpression="\d{1,5}"></asp:RegularExpressionValidator>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Available Time">
                <ItemTemplate>
                    <uc3:TimespanPicker ID="TimespanPicker" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>            
            <asp:TemplateField HeaderStyle-Width="50px">
                <ItemTemplate>
                    <asp:ImageButton ID="UpdateImageButton" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Update.png"
                        CommandName="Update" />
                    <asp:ImageButton ID="RemoveImageButton" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/remove.png"
                        CommandName="Delete" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            This file has no products associated with it.
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:Panel ID="pnlAdd" runat="server" Style="padding: 10px; margin: 5px 0 20px 0;"
        CssClass="controlarea2" DefaultButton="btnAddProductBySku">
        Add SKU:
        <asp:TextBox ID="NewSkuField" runat="Server" Columns="20" TabIndex="200"></asp:TextBox>
        <asp:ImageButton ID="btnBrowseProducts" runat="server" AlternateText="Browse Products"
            CausesValidation="False" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Browse.png" 
            onclick="btnBrowseProducts_Click" />&nbsp;
        <asp:ImageButton CausesValidation="false" ID="btnAddProductBySku" runat="server"
            AlternateText="Add Product To Order" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/AddToProduct.png"
            TabIndex="220" onclick="btnAddProductBySku_Click" />
        <asp:Panel CssClass="controlarea1" ID="pnlProductPicker" runat="server" Visible="false">
            <uc1:ProductPicker ID="ProductPicker1" runat="server" IsMultiSelect="false" />
            <asp:ImageButton ID="btnProductPickerCancel" CausesValidation="false" runat="server"
                ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Close.png" 
                AlternateText="Close Browser" onclick="btnProductPickerCancel_Click" />
            <asp:ImageButton ID="btnProductPickerOkay" runat="server" AlternateText="Add To Product"
                CausesValidation="false" 
                ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/AddToProduct.png" 
                onclick="btnProductPickerOkay_Click" />
        </asp:Panel>        
        <asp:Panel ID="pnlProductChoices" runat="server" Visible="false">
            <asp:ImageButton ID="btnCloseVariants" CausesValidation="false" runat="server"
                ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Close.png" 
                AlternateText="Close Browser" onclick="btnCloseVariants_Click" />&nbsp;
            <asp:ImageButton CausesValidation="false" ID="btnAddVariant" runat="server"
            AlternateText="Add To Product" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/AddToProduct.png"
            TabIndex="222" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
