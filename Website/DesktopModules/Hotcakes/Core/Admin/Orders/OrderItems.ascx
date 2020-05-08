<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderItems.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderItems" %>
<%@ Register Src="OrderAddProduct.ascx" TagPrefix="hcc" TagName="OrderAddProduct" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagPrefix="hcc" TagName="MessageBox" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    function closeDialog() {
        $("#EditInventory").hcDialog('close');
    }

    function openDialog() {
        $("#EditInventory").hcDialog({
            autoOpen: true,
            height: 'auto',
            minHeight: 200,
            width: 500
        });
    }
</script>

<style type="text/css">
    #btnInventory {
        margin-right:5px;
        margin-top:13px;
        padding-right:5px;        
        padding-top:5px;
    }
</style>

<hcc:MessageBox runat="server" ID="ucMessageBox" AddValidationSummaries="false" />

<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" CssClass="hcGrid"
    DataKeyNames="Id" OnDataBinding="gvItems_DataBinding" OnRowDataBound="gvItems_RowDataBound" OnRowDeleting="gridView_RowDeleting" OnRowCommand="gvItems_RowCommand">
    <Columns>
        <asp:TemplateField Visible="false">
            <ItemTemplate>
                <asp:CheckBox ID="chbSelected" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="SKU">
            <ItemTemplate>
                <asp:Label ID="lblSKU" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" />
                <div runat="server" id="divGiftWrap" visible="false">
                    <asp:Literal ID="litGiftCardsNumbers" runat="server" />
                    <asp:HyperLink ID="lnkGiftCards" runat="server" />
                </div>
                <asp:Literal ID="litDiscounts" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Shipping">
            <ItemTemplate>
                <asp:Label ID="lblShippingStatus" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Price">
            <ItemStyle CssClass="hcRight" Width="80" />
            <HeaderStyle CssClass="hcRight" />
            <ItemTemplate>
                <asp:Label ID="lblAdjustedPrice" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qty">
            <ItemStyle CssClass="hcRight" Width="100"/>
            <HeaderStyle CssClass="hcRight"  />
            <ItemTemplate>

                <asp:LinkButton ID="btnInventory" CssClass="hcIconRestore" runat="server" CausesValidation="False" CommandName="Inventory" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" ClientIDMode="Static" />

                <asp:TextBox ID="txtQty" Columns="4" runat="server" />
                <asp:Label ID="lblQty" runat="server" />
                <asp:RangeValidator ID="rvTxtQty" runat="server" Type="Integer" Display="Dynamic" MinimumValue="1" MaximumValue="99999999" ControlToValidate="txtQty" CssClass="hcFormError"></asp:RangeValidator>
                <asp:HiddenField ID="hdfLineItemId" runat="server" />

            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total">
            <ItemStyle CssClass="hcRight" Width="80" />
            <HeaderStyle CssClass="hcRight" />
            <ItemTemplate>
                <asp:Label ID="lblLineTotalWithoutDiscounts" runat="server" CssClass="hcItemDiscount" Visible="false" />
                <asp:Label ID="lblLineTotal" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="30">
            <ItemTemplate>
                <asp:LinkButton ID="btnDelete" runat="server" CssClass="hcIconDelete" CausesValidation="False" CommandName="Delete" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<asp:GridView ID="gvSubscriptions" runat="server" AutoGenerateColumns="False" CssClass="hcGrid"
    DataKeyNames="Id" OnDataBinding="gvSubscriptions_DataBinding" OnRowDataBound="gvSubscriptions_RowDataBound" OnRowDeleting="gridView_RowDeleting" OnRowCommand="gvSubscriptions_RowCommand">
    <Columns>
        <asp:TemplateField Visible="false">
            <ItemTemplate>
                <asp:CheckBox ID="chbSelected" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="SKU">
            <ItemTemplate>
                <asp:Label ID="lblSKU" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" />
                <div runat="server" id="divGiftWrap" visible="false">
                    <asp:Literal ID="litGiftCardsNumbers" runat="server" />
                    <asp:HyperLink ID="lnkGiftCards" runat="server" />
                </div>
                <asp:Literal ID="litDiscounts" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qty">
            <ItemStyle CssClass="hcRight" />
            <HeaderStyle CssClass="hcRight" />
            <ItemTemplate>
                <asp:TextBox ID="txtQty" Columns="4" runat="server" />
                <asp:Label ID="lblQty" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="NextPaymentDate">
            <ItemTemplate>
                <asp:Label ID="lblNextPaymentDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="FeePerPeriod">
            <ItemStyle CssClass="hcRight" />
            <HeaderStyle CssClass="hcRight" />
            <ItemTemplate>
                <asp:Label ID="lblLineTotalWithoutDiscounts" runat="server" CssClass="hcItemDiscount" Visible="false" />
                <asp:Label ID="lblLineTotal" runat="server" />
                <asp:Label ID="lblInterval" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="TotalPayed">
            <ItemStyle CssClass="hcRight" Width="80" />
            <HeaderStyle CssClass="hcRight" />
            <ItemTemplate>
                <asp:Label ID="lblTotalPayed" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="80">
            <ItemTemplate>
                <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete">
                    <i class="hcIconDelete"></i><%#Localization.GetString("Delete") %>
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancel" runat="server" CausesValidation="False" CommandName="CancelSubscription">
                    <i class="hcIconDelete"></i><%#Localization.GetString("Cancel") %>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<%--<asp:LinkButton ID="btnRMA" runat="server" AlternateText="Create RMA" OnClick="btnRMA_Click" Visible="False" />--%>

<div runat="server" id="divUpdateQuantities" class="hcForm" style="float: right">
    <div class="hcFormItemInline">
        <label class="hcLabel">
            <asp:Label runat="server" resourcekey="SubTotal" />
            <asp:Label ID="lblSubTotal" runat="server" />
        </label>
        <asp:LinkButton ID="btnUpdateQuantities" runat="server" resourcekey="UpdateQuantities" CssClass="hcButton hcSmall"
            CausesValidation="False" TabIndex="100" OnClick="btnUpdateQuantities_Click" />
    </div>
</div>
<div class="hcClearfix"></div>
<hcc:OrderAddProduct runat="server" ID="ucOrderAddProduct" />

<div id="EditInventory" style="display: none">
    <asp:UpdatePanel ID="upnlEditDlg" UpdateMode="Always" runat="server">
        <ContentTemplate>
            <asp:PlaceHolder ID="phrEditor" runat="server" EnableViewState="True">
                <hcc:MessageBox ID="MessageBox1" runat="server" />
                <div class="hcForm">
                    <div class="hcFormItem">
                        <asp:Label runat="server" ID="lblInventoryReplenishMsg" Text="Are you sure you want to Replenish this product inventory?" CssClass="hcLabel" />
                        <br />
                        <br />
                        <asp:Label ID="lblReplenishQty" runat="server" Text="Replenish Quantity" class="hcLabel" />
                        <asp:TextBox runat="server" ID="txtReplenishQty"></asp:TextBox>
                        <asp:RangeValidator ID="rvtxtReplenishQty" runat="server" Type="Integer" Display="Dynamic" MinimumValue="1" MaximumValue="10" ControlToValidate="txtReplenishQty" CssClass="hcFormError" ValidationGroup="InventoryReplenish" EnableClientScript="true"></asp:RangeValidator>
                        <br />
                        <asp:HiddenField ID="hdfProductId" runat="server" />
                        <asp:HiddenField ID="hdfVariantId" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phrScripts" runat="server" EnableViewState="False"></asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" CssClass="hcPrimaryAction" ValidationGroup="InventoryReplenish" CausesValidation="true" OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" runat="server" CssClass="hcSecondaryAction" OnClientClick="closeDialog()" />
        </li>
    </ul>
</div>



