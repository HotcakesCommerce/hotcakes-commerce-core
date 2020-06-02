<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.FileVaultDetailsView"
    Title="File Vault Details" Codebehind="FileVaultDetailsView.aspx.cs" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/FilePicker.ascx" TagName="FilePicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/TimespanPicker.ascx" TagName="TimespanPicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" runat="server" />
</asp:Content>
    
<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    <style>
        .hcFileDetailsWidth hcForm {
            width: 90%; 
        }
        input.hcSmall {
            max-width: 50px;
        }
    </style>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="MessageBox1" runat="server" EnableViewState="false" />
    
    <div class="hcFileDetailsWidth">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblFile") %></label>
                <asp:Label runat="server" ID="lblFileName"/>
                <asp:LinkButton ID="ReplaceImageButton" resourcekey="ReplaceImageButton" runat="server" onclick="ReplaceImageButton_Click" />
            </div>
            <div class="hcFormItem" runat="server" id="ReplacePanel" Visible="False">
                <hcc:FilePicker ID="FilePicker1" runat="server" DisplayShortDescription="false" />
                <asp:LinkButton ID="FileReplaceSaveImageButton" resourcekey="FileReplaceSaveImageButton" runat="server" onclick="FileReplaceSaveImageButton_Click" CausesValidation="false" CssClass="hcTertiaryAction" />
                <asp:LinkButton ID="FileReplaceCancelImageButton" resourcekey="FileReplaceCancelImageButton" runat="server" onclick="FileReplaceCancelImageButton_Click" CausesValidation="False" CssClass="hcTertiaryAction"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblDescription") %></label>
                <asp:TextBox ID="DescriptionTextBox" runat="server" ValidationGroup="UpdateDescription"/>
                <asp:RequiredFieldValidator runat="server" resourcekey="rfvDescription" ControlToValidate="DescriptionTextBox" Display="Dynamic" CssClass="hcFormError" ValidationGroup="UpdateDescription"/>
            </div>
            <div class="hcFormItem">
                <ul class="hcActions">
                    <li>
                        <asp:LinkButton ID="SaveImageButton" resourcekey="SaveImageButton" runat="server" ValidationGroup="UpdateDescription" CssClass="hcPrimaryAction" onclick="SaveImageButton_Click" />
                    </li>
                    <li>
                        <asp:LinkButton ID="CancelImageButton" resourcekey="CancelImageButton" runat="server" CausesValidation="False" CssClass="hcSecondaryAction" onclick="CancelImageButton_Click" />
                    </li>
                </ul>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblProducts") %></label>
            </div>
            <div class="hcFormItem">
                <asp:GridView ID="ProductsGridView" runat="server" DataKeyNames="bvin" AutoGenerateColumns="False" 
                    onrowdatabound="ProductsGridView_RowDataBound" onrowdeleting="ProductsGridView_RowDeleting" 
                    onrowupdating="ProductsGridView_RowUpdating" CssClass="hcGrid">
                    <RowStyle CssClass="hcGridRow" />
                    <HeaderStyle CssClass="hcGridHeader" />
                    <AlternatingRowStyle CssClass="hcGridAltRow" />
                    <Columns>
                        <asp:BoundField HeaderText="ProductName" DataField="ProductName" HeaderStyle-Width="200px" />
                        <asp:BoundField HeaderText="ProductSku" DataField="Sku" HeaderStyle-Width="75px" />
                        <asp:TemplateField HeaderText="MaxDownloads">
                            <ItemTemplate>
                                <asp:TextBox ID="MaxDownloadsTextBox" runat="server" CssClass="hcSmall" />
                                <asp:RegularExpressionValidator ID="MaxDownloadsRegularExpressionValidator" resourcekey="rfvMaxDownloadsTextBox" runat="server"
                                                                ControlToValidate="MaxDownLoadsTextBox" ValidationExpression="\d{1,5}" Display="Dynamic" CssClass="hcFormError"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="AvailableTime">
                            <ItemTemplate>
                                <hcc:TimespanPicker ID="TimespanPicker" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>            
                        <asp:TemplateField HeaderStyle-Width="68px">
                            <ItemTemplate>
                                <asp:LinkButton ID="UpdateImageButton" resourcekey="UpdateImageButton" runat="server" CommandName="Update" CssClass="hcIconEdit" />
                                <asp:LinkButton ID="RemoveImageButton" resourcekey="RemoveImageButton" runat="server" CommandName="Delete" CssClass="hcIconDelete hcDeleteColumn" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <%=Localization.GetString("NoFiles") %>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
    
    <div class="hcFileDetailsWidth">
        <div class="hcFormItem">
            <label class="hcLabel"><%=Localization.GetString("lblAddProduct") %></label>
            <asp:TextBox ID="NewSkuField" runat="Server" />
        </div>
        <div class="hcFormItem">
            <asp:LinkButton ID="btnBrowseProducts" resourcekey="btnBrowseProducts" runat="server" CssClass="hcTertiaryAction" CausesValidation="False" onclick="btnBrowseProducts_Click" /> &nbsp;
            <asp:LinkButton CausesValidation="false" ID="btnAddProductBySku" resourcekey="btnAddProductBySku" runat="server" CssClass="hcTertiaryAction" onclick="btnAddProductBySku_Click" />
        </div>
        <div class="hcFormItem" runat="server" id="pnlProductPicker" Visible="False">
            <hcc:ProductPicker ID="ProductPicker1" runat="server" IsMultiSelect="false" />
            <asp:LinkButton ID="btnProductPickerCancel" resourcekey="btnProductPickerCancel" CausesValidation="false" runat="server" CssClass="hcTertiaryAction" onclick="btnProductPickerCancel_Click" />
            <asp:LinkButton ID="btnProductPickerOkay" resourcekey="btnProductPickerOkay" runat="server" CausesValidation="false" CssClass="hcTertiaryAction" onclick="btnProductPickerOkay_Click" />
        </div>
        <div class="hcFormItem" runat="server" id="pnlProductChoices" Visible="False">
            <asp:LinkButton ID="btnCloseVariants" resourcekey="btnCloseVariants" CausesValidation="false" runat="server" onclick="btnCloseVariants_Click" />&nbsp;
            <asp:LinkButton CausesValidation="false" ID="btnAddVariant" resourcekey="btnAddVariant" runat="server" />
        </div>
    </div>

</asp:Content>