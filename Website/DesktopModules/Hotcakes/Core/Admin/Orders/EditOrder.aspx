<%@ Page ValidateRequest="false" Title="Edit Order" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="EditOrder.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.EditOrder" %>

<%@ Register Src="../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressNormalizationDialog.ascx" TagName="AddressNormalizationDialog" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/UserPicker.ascx" TagName="UserPicker" TagPrefix="hcc" %>

<%@ Register Src="OrderActions.ascx" TagName="OrderActions" TagPrefix="hcc" %>
<%@ Register Src="PaymentInformation.ascx" TagName="PaymentInformation" TagPrefix="hcc" %>
<%@ Register Src="OrderStatusDisplay.ascx" TagName="OrderStatusDisplay" TagPrefix="hcc" %>
<%@ Register Src="OrderItems.ascx" TagPrefix="hcc" TagName="OrderItems" %>
<%@ Register Src="OrderItemInventory.ascx" TagPrefix="hcc" TagName="InventoryEditorControl" %>

<asp:Content ID="Nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:OrderActions ID="ucOrderActions" CurrentOrderPage="Edit" runat="server" />

    <asp:UpdatePanel runat="server" ID="upPaymentInfo" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="hcBlock">
                <hcc:PaymentInformation ID="ucPaymentInformation" runat="server" />
            </div>
            <div class="hcBlock">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="hcTertiaryAction" resourcekey="btnDeleteOrder" CausesValidation="false" OnClick="btnDelete_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server" ID="upMain" UpdateMode="Conditional">
        <ContentTemplate>
            <script type="text/javascript">
                hcAttachUpdatePanelLoader();

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(InIEvent);

                function InIEvent() {

                    $('body').on('click', 'a.disabled', function (event) {
                        event.preventDefault();
                    });

                    jQuery.fn.extend({
                        disable: function (state) {
                            return this.each(function () {
                                var $this = $(this);
                                $this.toggleClass('disabled', state);
                            });
                        }
                    });

                    var lstCoupn = $("#<%= lstCoupons.ClientID %>");
                    lstCoupn.change(function (e) { Validate_CouponItems(this); });

                    var txtCoupon = $("#<%= txtCoupon.ClientID %>");
                    var lnkBtnAdd = $("#<%= btnAddCoupon.ClientID %>");

                    if (txtCoupon.val().length > 0) {
                        lnkBtnAdd.disable(false);
                    }
                    else {
                        lnkBtnAdd.disable(true);
                    }

                    txtCoupon.bind('input propertychange', function () {
                        if (txtCoupon.val().length > 0) {
                            lnkBtnAdd.disable(false);
                        }
                        else {
                            lnkBtnAdd.disable(true);
                        }
                    });
                  
                }

                function closeInventoryEditorDialog() {
                    $("#InventoryEidtor").hcDialog('close');
                }

                function openInventoryEditorDialog() {
                    $("#InventoryEidtor").hcDialog({
                        autoOpen: true,
                        height: 'auto',
                        minHeight: 200,
                        width: 800
                    });
                }

                function closeRefundAmountDialog() {
                    $("#RefundAmount").hcDialog('close');
                }

                function openRefundAmountDialog() {
                    $("#RefundAmount").hcDialog({
                        autoOpen: true,
                        height: 'auto',
                        minHeight: 200,
                        width: 500
                    });
                }

                function Validate_CouponItems(sender) {

                    var result = $("#<%=lstCoupons.ClientID %> option:selected").val();
                    var btnDelete = $("#<%= btnDeleteCoupon.ClientID %>");
                    if (result != null && result != undefined) {
                        if (result.length > 0)
                            btnDelete.disable(false);
                        else
                            btnDelete.disable(true);

                    }
                    else btnDelete.disable(true);
                }

            </script>
            <h1><%=PageTitle %> 
                <asp:Label ID="OrderNumberField" runat="server" Text="000000" />
            </h1>

            <hcc:MessageBox ID="ucMessageBox" runat="server" />
            <hr />
            <div class="hcColumn" style="width: 30%">
                <asp:Label ID="TimeOfOrderField" runat="server" />
            </div>
            <div class="hcColumnRight" style="width: 70%;">
                <hcc:OrderStatusDisplay ID="ucOrderStatusDisplay" runat="server" />
            </div>
            <div class="hcClearfix"></div>

            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="hcBlock">
                        <div class="hcForm">
                            <hcc:UserPicker UserNameFieldSize="50" ID="ucUserPicker" runat="server" />
                            <asp:HiddenField ID="UserIdField" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="hcColumnLeft" style="width: 50%">
                <div class="hcForm hcBillToAddress">
                    <h2><%=Localization.GetString("lblBillTo") %></h2>
                    <div class="hcFormMessage hcBillToAddressMessage"></div>
                    <hcc:AddressEditor TabOrderOffSet="2000" ID="ucBillingAddress" CreateValidationInputs="True" FormSelector=".hcBillToAddress" ErrorMessageSelector=".hcBillToAddressMessage" runat="server" />
                </div>
            </div>
            <div class="hcColumnRight hcLeftBorder" style="width: 49%">
                <div class="hcForm hcShipToAddress">
                    <h2><%=Localization.GetString("lblShipTo") %></h2>
                    <div runat="server" id="divShipToAddress">
                        <div class="hcFormMessage hcShipToAddressMessage"></div>
                        <hcc:AddressEditor TabOrderOffSet="2500" ID="ucShippingAddress" CreateValidationInputs="True" FormSelector=".hcShipToAddress" ErrorMessageSelector=".hcShipToAddressMessage" runat="server" />
                    </div>
                    <asp:Label runat="server" ID="lblShippingAddress" />
                </div>
            </div>
            <hcc:AddressNormalizationDialog ID="AddressNormalizationDialog" PrimaryAddressLabel="Shipping Address" SecondaryAddressLabel="Billing Address" runat="server" />
            <script type="text/javascript">
                jQuery(function ($) {
                    var AddressValidator = createAddressValidator();
                    AddressValidator.init($('.hcBillToAddress').data('address-validation-inputs'), "#<%= btnSaveChanges.ClientID%>", $('.hcShipToAddress').data('address-validation-inputs'));

                    InIEvent();
                });
            </script>
            <div class="hcClearfix"></div>
            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <hcc:OrderItems runat="server" ID="ucOrderItems" AllowUpdateQuantities="False" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="hcColumnLeft" style="width: 50%">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <label class="hcLabel"><%=Localization.GetString("lblCustomerInstructions") %></label>
                        <asp:TextBox ID="txtInstructions" runat="server" TextMode="MultiLine" Wrap="true" Rows="5" CssClass="hcOrderViewNotes" />
                    </div>
                </div>
            </div>
            <div class="hcColumnRight" style="width: 50%">
                <div class="hcForm">
                    <asp:Literal ID="litTotals" runat="server" />
                </div>
            </div>

            <hr />

            <div class="hcColumnLeft" style="width: 50%">
                <asp:Panel ID="pnlCoupons" runat="server" CssClass="hcForm" DefaultButton="btnAddCoupon">
                    <div class="hcFormItemLabel">
                        <label class="hcLabel"><%=Localization.GetString("lblPromotionCode") %></label>
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:TextBox ID="txtCoupon" runat="server" MaxLength="50" ValidationGroup="vgCoupon" />
                        <asp:RequiredFieldValidator ID="rfCoupon" resourcekey="rfCoupon" ValidationGroup="vgCoupon" ControlToValidate="txtCoupon" Display="Dynamic"
                            runat="server" EnableClientScript="false" CssClass="hcFormError" />
                        <asp:LinkButton ID="btnAddCoupon" runat="server" CssClass="hcSecondaryAction hcSmall disabled" resourcekey="btnAddCoupon" CausesValidation="true" OnClick="btnAddCoupon_Click" ValidationGroup="vgCoupon" />
                    </div>
                    <div class="hcFormItem">
                        <asp:ListBox ID="lstCoupons" runat="server" DataTextField="CouponCode" DataValueField="CouponCode" Height="150px" SelectionMode="Multiple" CssClass="RadComboBox" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:LinkButton ID="btnDeleteCoupon" CausesValidation="false" runat="server"  CssClass="hcSecondaryAction hcSmall disabled" resourcekey="btnDeleteCoupon" OnClick="btnDeleteCoupon_Click" />
                    </div>
                </asp:Panel>
            </div>
            <div class="hcColumnRight hcLeftBorder" style="width: 49%">
                <div class="hcForm">
                    <div class="hcFormItem hcProductEditChoice">
                        <label class="hcLabel"><%=Localization.GetString("lblShippingMethod") %></label>
                        <asp:Literal ID="litShippingMethods" runat="server"/>
                    </div>
                    <div class="hcFormItem">
                        <label class="hcLabel"><%=Localization.GetString("lblForceShipping") %></label>
                        <asp:TextBox ID="ShippingOverride" runat="server" />
                    </div>
                </div>
            </div>

            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnSaveChanges" runat="server" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" OnClick="btnSaveChanges_Click" />
                </li>
                <li>
                    <asp:HyperLink runat="server" resourcekey="btnCancel" CssClass="hcSecondaryAction" NavigateUrl="Default.aspx" />
                </li>
            </ul>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div id="RefundAmount" class="hcForm" style="display: none">
        <asp:UpdatePanel ID="upnlRefundAmount" UpdateMode="Always" runat="server">
            <ContentTemplate>
                <asp:PlaceHolder ID="phrRefundAmountControls" runat="server" EnableViewState="True">
                    <asp:Label ID="Label1" CssClass="hcLabel" runat="server" resoucekey="NotRefunded" />
                    <br />
                    <ul class="hcActions">
                        <li>
                            <asp:LinkButton runat="server" ID="btnOK" CssClass="hcPrimaryAction" resourcekey="btnOkay" OnClick="btnOK_Click" />
                        </li>
                        <li>
                            <asp:LinkButton runat="server" ID="btnCancel" CssClass="hcSecondaryAction" OnClientClick="closeRefundAmountDialog()" resourcekey="btnCancel" OnClick="btnCancel_Click" />
                        </li>
                    </ul>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phrRefundAmountScript" runat="server" EnableViewState="False"/>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div id="InventoryEidtor" class="hcForm" style="display: none">
        <asp:UpdatePanel ID="upnlInventoryEditor" UpdateMode="Always" runat="server">
            <ContentTemplate>
                <asp:PlaceHolder ID="phrInventoryEditor" runat="server" EnableViewState="True">
                    <hcc:InventoryEditorControl runat="server" ID="InventoryControl" AllowUpdateQuantities="False" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phrInventoryScripts" runat="server" EnableViewState="False"></asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</asp:Content>
