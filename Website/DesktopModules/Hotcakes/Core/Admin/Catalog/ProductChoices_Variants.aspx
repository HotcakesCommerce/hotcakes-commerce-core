<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductChoices_Variants" CodeBehind="ProductChoices_Variants.aspx.cs" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="Variants" />
    <hcc:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        var hcEditVariantDialog = function () {
            $("#hcEditVariantDialog").hcDialog({
                title: "Edit Variant",
                width: 500,
                height: 'auto',
                maxHeight: 500,
                parentElement: '#<%=pnlEditVariant.ClientID%>',
                close: function () {
                    <%= ClientScript.GetPostBackEventReference(lnkCancel, "") %>
                }
            });
        };
    </script>

    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <h1>Choices - Variants</h1>
    <div class="hcColumnLeft hcChoiceVariants" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItemLabel">
                <label class="hcLabel">Create Variant Based on Selections:</label>
            </div>
            <%--            <asp:Repeater ID="rpVariantOptions" runat="server">
                <ItemTemplate>
                    <div class="hcFormItemHor">
                        <label class="hcLabel"><%#Eval("Name") %></label>
                        <asp:DropDownList Width="100px" DataSource='<%#Eval("Items") %>' DataTextField="Name" DataValueField="Bvin" runat="server">
                        </asp:DropDownList>
                    </div>
                </ItemTemplate>
            </asp:Repeater>--%>
            <div class="hcFormItem">
                <asp:PlaceHolder ID="phLists" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" runat="server" CssClass="hcSecondaryAction" Text="Add Variant" OnClick="btnNew_Click" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <div class="hcFormItem">
                <div class="hcFormMessage">
                    All Possible Variants - <strong><asp:Label ID="lblAllPossibleCount" runat="server" /></strong><br />
                    Generated Variants - <strong><asp:Label ID="lblGeneratedCount" runat="server" /></strong><br />
                    Variants to Generate - <strong><asp:Label ID="lblToGenerateCount" runat="server" /></strong>
                    <p>
                        <asp:Label ID="lblGenerateWarning" runat="server" Visible="true"><i>WARNING: For performance reasons, only <b><%=Hotcakes.Commerce.WebAppSettings.MaxVariants %></b> variants can be generated</i></asp:Label>
                    </p>
                </div>
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnGenerateAll" runat="server" CssClass="hcButton" Text="Generate All Variants" OnClick="btnGenerateAll_Click" />
            </div>
        </div>
    </div>

    <asp:GridView ID="gvVariants" CssClass="hcGrid" AutoGenerateColumns="false" DataKeyNames="Bvin" runat="server">
        <HeaderStyle CssClass="hcGridHeader" />
        <RowStyle CssClass="hcGridRow" />
        <Columns>
            <asp:TemplateField ItemStyle-CssClass="hcNoPadding" ItemStyle-Width="50px">
                <ItemTemplate>
                    <img width="50px" src="<%#GetVariantImageUrl(Container)%>" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Variant">
                <ItemTemplate><%#GetVariantName(Container) %> </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Price" DataFormatString="{0:c}" HeaderText="Price" />
            <asp:BoundField DataField="Sku" HeaderText="SKU" />
            <asp:TemplateField ShowHeader="False">
                <ItemStyle Width="80px" />
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="hcIconEdit"
                        CausesValidation="False" CommandName="Edit" Text="Edit" />

                    <asp:LinkButton runat="server" CssClass="hcIconDelete" OnClientClick="return hcConfirm(event, 'Delete this variant?');"
                        CausesValidation="False" CommandName="Delete" Text="Delete" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:Panel ID="pnlEditVariant" runat="server" Visible="false">
        <div id="hcEditVariantDialog" class="dnnClear">
            <h3><asp:Label ID="lblVariantDescription" runat="server" /></h3>
            <div class="hcColumnLeft" style="width: 50%">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <label class="hcLabel">Picture</label>
                        <hcc:ImageUploader runat="server" ID="ucVariantImage" />
                    </div>
                </div>
            </div>
            <div class="hcColumnLeft" style="width:50%">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <label class="hcLabel">SKU</label>
                        <asp:TextBox ID="txtVariantSku" runat="server" />
                        <asp:CustomValidator ID="cvVariantSku" ControlToValidate="txtVariantSku" CssClass="hcFormError" runat="server"
                            ErrorMessage="The SKU you specified already exists on another product. Please enter a different SKU." />
                    </div>
                    <div class="hcFormItem">
                        <label class="hcLabel">Price</label>
                        <asp:TextBox ID="txtVariantPrice" runat="server" />
                    </div>
                </div>
            </div>
            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="lnkSaveVariant" CssClass="hcPrimaryAction" Text="Save" OnClick="btnSave_Click" runat="server" />
                </li>
                <li>
                    <asp:LinkButton ID="lnkCancel" CssClass="hcSecondaryAction" Text="Cancel" runat="server" />
                </li>
            </ul>
        </div>
    </asp:Panel>

</asp:Content>
