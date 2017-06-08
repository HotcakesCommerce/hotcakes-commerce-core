<%@ Page Title="Create New Order" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="CreateOrder.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.CreateOrder" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/CreditCardInput.ascx" TagName="CreditCardInput" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressNormalizationDialog.ascx" TagName="AddressNormalizationDialog" TagPrefix="hcc" %>
<%@ Register Src="OrderItems.ascx" TagPrefix="hcc" TagName="OrderItems" %>
<%@ Register Src="OrderActions.ascx" TagName="OrderActions" TagPrefix="hcc" %>
<%@ Register Src="PaymentInformation.ascx" TagName="PaymentInformation" TagPrefix="hcc" %>

<asp:Content ID="Nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:OrderActions ID="ucOrderActions" CurrentOrderPage="Details" runat="server" />

    <div class="hcBlock" id="hcPaymentInfo" runat="server">
        <hcc:PaymentInformation ID="ucPaymentInformation" runat="server" />
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel">Codes Used</label>
                <div class="hcCouponCodes">
                    <asp:Label ID="CouponField" runat="server" />
                </div>
            </div>
        </div>
    </div>

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkBacktoAbandonedCartsReport" Text="Back to Abandoned Carts Report" CssClass="hcTertiaryAction" Target="_self" runat="server" />
            </div>
        </div>
    </div>
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
                    
                    var txtCoupon = $("#<%= txtCoupon.ClientID %>");
                    var lnkBtnAdd = $("#<%= btnAddCoupon.ClientID %>");
                    
                    if (txtCoupon.val() != undefined && txtCoupon.val() != null && txtCoupon.val().length > 0) {
                        lnkBtnAdd.disable(false);
                    }
                    else {
                        lnkBtnAdd.disable(true);
                    }
                    

                    txtCoupon.bind('input propertychange', function () {                        
                        if (txtCoupon.val() != undefined && txtCoupon.val() != null && txtCoupon.val().length > 0) {
                            lnkBtnAdd.disable(false);
                        }
                        else {
                            lnkBtnAdd.disable(true);
                        }                        
                    });
                }

                $(document).ready(function () {
                    InIEvent();
                });
            </script>

            <h1>New Order</h1>
            <hcc:MessageBox ID="ucMessageBox" runat="server" />

            <h2>Order Items</h2>
            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <hcc:OrderItems runat="server" Id="ucOrderItems" EditMode="True" AllowUpdateQuantities="True" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="hcColumnLeft hcRightBorder" style="width: 30%">
                <div class="hcForm hcShipToAddress">
                    <h2>Ship To</h2>
                    <div class="hcFormMessage hcShipToAddressMessage" style="margin-left: 2px; margin-right: 2px;"></div>
                    <hcc:AddressEditor ID="ShipToAddress" ShowAddressLine3="false" CreateValidationInputs="True" FormSelector=".hcShipToAddress" ErrorMessageSelector=".hcShipToAddressMessage" runat="server" TabOrderOffSet="1000" />
                    <div class="hcFormItem">
                        <asp:Label ID="EmailAddressLabel" CssClass="hcLabel" AssociatedControlID="EmailAddressTextBox" runat="server" Text="E-mail"></asp:Label>
                        <asp:TextBox ID="EmailAddressTextBox" runat="server" TabIndex="3000" />
                        <asp:RegularExpressionValidator ID="EmailRegularExpressionValidator" runat="server"
                            ControlToValidate="EmailAddressTextBox" ErrorMessage="Invalid E-mail Address." CssClass="hcFormError"
                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
            <div class="hcColumn" style="width: 35%">
                <div class="hcForm">
                    <h2>Bill To</h2>
                    <asp:Panel ID="pnlBillTo" CssClass="hcBillToAddress" runat="server" Visible="false">
                        <div class="hcFormMessage hcBillToAddressMessage" style="margin-left: 2px; margin-right: 2px;"></div>
                        <hcc:AddressEditor ID="BillToAddress" ShowAddressLine3="false" CreateValidationInputs="True" FormSelector=".hcBillToAddress" ErrorMessageSelector=".hcBillToAddressMessage" runat="server" TabOrderOffSet="2000" />
                    </asp:Panel>
                    <div class="hcFormItem">
                        <asp:CheckBox ID="chkBillToSame" Checked="true" runat="server" AutoPostBack="true"
                            Text="Bill to Same Address" OnCheckedChanged="chkBillToSame_CheckedChanged" />
                    </div>
                </div>
            </div>

            <div class="hcColumnRight" style="width: 33%">
                <table border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td>
                            <asp:Button ID="btnFindUsers" CausesValidation="false" Text="Find Customer"
                                runat="server" OnClick="btnFindUsers_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnNewUsers" CausesValidation="false" Text="New Customer"
                                runat="server" OnClick="btnNewUsers_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnFindOrders" CausesValidation="false" Text="Find Orders"
                                runat="server" OnClick="btnFindOrders_Click" />
                        </td>
                    </tr>
                </table>
                <div class="controlarea1" style="padding: 10px;">
                    <asp:MultiView ID="viewFindUsers" runat="server" ActiveViewIndex="0">
                        <asp:View ID="ViewFind" runat="server">
                            <strong>Search for Customer</strong><br />
                            <asp:Label ID="lblFindUserMessage" runat="server"></asp:Label>
                            <asp:Panel ID="pnlFindUser" runat="server" DefaultButton="btnFindUser">
                                <table border="0" cellspacing="0" cellpadding="3">
                                    <tr>
                                        <td class="formlabel">Keyword</td>
                                        <td class="formfield">
                                            <asp:TextBox ID="FilterUserField" runat="server" Columns="15"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel">&nbsp;</td>
                                        <td class="formfield">
                                            <asp:Button ID="btnFindUser" runat="server" Text="Find Customer(s)"
                                                CausesValidation="false" OnClick="btnFindUser_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <telerik:RadGrid ShowHeader="false" ID="gridSelectUser" runat="server" PageSize="10" AutoGenerateColumns="False" AllowCustomPaging="True" AllowPaging="True"
                                BorderColor="#CCCCCC" CellPadding="3" GridLines="None" OnEditCommand="gridSelectUser_OnEditCommand" OnNeedDataSource="gridSelectUser_OnNeedDataSource">
                                <MasterTableView DataKeyNames="Bvin">
                                    <Columns>
                                        <telerik:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:Label ID="lblUsername" runat="server" Text='<%# Bind("Email") %>'>'></asp:Label><br />
                                                <span class="smalltext">
                                                    <asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("FirstName") %>'>'></asp:Label>
                                                    <asp:Label ID="lblLastName" runat="server" Text='<%# Bind("LastName") %>'>'></asp:Label></span>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="SelectUserButton" runat="server" CausesValidation="false" CommandName="Edit"
                                                    ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Select.png" AlternateText="Select Customer"></asp:ImageButton>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </asp:View>
                        <asp:View ID="ViewNew" runat="server">
                            <strong>Add New Customer</strong><br />
                            <asp:Label ID="lblNewUserMessage" runat="server"></asp:Label>
                            <asp:Panel ID="pnlNewUser" runat="server" DefaultButton="btnNewUserSave">
                                <table border="0" cellspacing="0" cellpadding="3">
                                    <tr>
                                        <td class="formlabel">E-Mail</td>
                                        <td class="formfield">
                                            <asp:TextBox ID="NewUserEmailField" runat="server" Columns="15"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel">First Name</td>
                                        <td class="formfield">
                                            <asp:TextBox ID="NewUserFirstNameField" runat="server" Columns="15"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel">Last Name</td>
                                        <td class="formfield">
                                            <asp:TextBox ID="NewUserLastNameField" runat="server" Columns="15"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel">&nbsp;</td>
                                        <td class="formfield">
                                            <asp:Button ID="btnNewUserSave" runat="server" Text="Add New Customer"
                                                CausesValidation="false" OnClick="btnNewUserSave_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </asp:View>
                        <asp:View ID="ViewOrder" runat="server">
                            <strong>Find Customer By Order</strong>
                            <asp:Label ID="lblFindOrderMessage" runat="server"></asp:Label>
                            <asp:Panel ID="pnlFindUserByOrder" runat="server" DefaultButton="btnGoFindOrder">
                                <table border="0" cellspacing="0" cellpadding="3">
                                    <tr>
                                        <td class="formlabel" align="right">Order Number</td>
                                        <td>
                                            <asp:TextBox ID="FindOrderNumberField" runat="server" Columns="20"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:Button ID="btnGoFindOrder" CausesValidation="false" runat="server"
                                                Text="Find This Order" OnClick="btnGoFindOrder_Click" /></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </asp:View>
                    </asp:MultiView>
                    <asp:HiddenField ID="UserIdField" runat="server" />
                </div>
            </div>

            <hr />
            <div class="hcColumnLeft" style="width: 50%">
                <div class="hcForm">
                    <h2>Shipping</h2>
                    <asp:RadioButtonList ID="ShippingRatesList" runat="server" TabIndex="4000"
                        OnSelectedIndexChanged="ShippingRatesList_SelectedIndexChanged">
                    </asp:RadioButtonList>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:LinkButton ID="btnCalculateShipping" runat="server" Text="Get Shipping Rates" CssClass="hcButton hcSmall"
                            CausesValidation="False" TabIndex="4010" OnClick="btnCalculateShipping_Click" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnUpdateShipping" runat="server" Text="Update" CssClass="hcButton hcSmall"
                            CausesValidation="False" OnClick="btnUpdateShipping_Click" />
                    </div>
                </div>
            </div>
            <div class="hcColumnRight hcLeftBorder" style="width: 49%">
                <div class="hcForm hcPaymentForm">
                    <h2>Payment</h2>
                    <div runat="server" id="divNoPaymentNeeded" visible="false">
                        <asp:RadioButton ID="rbNoPayment" GroupName="PaymentGroup" runat="server" Checked="false" />
                    </div>
                    <div runat="server" id="divCreditCard" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCreditCard" GroupName="PaymentGroup" runat="server" Checked="false" TabIndex="5000" />
                        <hcc:CreditCardInput ID="ucCreditCardInput" runat="server" ShowSecurityCode="True" TabIndex="5001" />
                    </div>
                    <div runat="server" id="divPurchaseOrder" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbPurchaseOrder" GroupName="PaymentGroup" runat="server" Checked="false" TabIndex="5010" />
                        #
                <asp:TextBox ID="txtPurchaseOrder" runat="server" Columns="10" TabIndex="5011" />
                    </div>
                    <div runat="server" id="divCompanyAccount" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCompanyAccount" GroupName="PaymentGroup" runat="server" Checked="false" />
                        #
                <asp:TextBox ID="txtAccountNumber" ClientIDMode="Static" runat="server" Columns="10" />
                    </div>
                    <div runat="server" id="divCheck" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCheck" GroupName="PaymentGroup" runat="server" Checked="false" TabIndex="5020" />
                    </div>
                    <div runat="server" id="divTelephone" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbTelephone" GroupName="PaymentGroup" runat="server" Checked="false" TabIndex="5030" />
                    </div>
                    <div runat="server" id="divCashOnDelivery" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCashOnDelivery" GroupName="PaymentGroup" runat="server" Checked="false" TabIndex="5040" />
                    </div>
                </div>
            </div>
            <hr />

            <div class="hcColumnLeft hcRightBorder" style="width: 30%">
                <asp:Panel ID="pnlInstructions" CssClass="hcFormItem" runat="server">
                    <div class="hcFormItemLabel">
                        <label class="hcLabel">Customer's Instructions</label>
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtInstructions" runat="server" Rows="5" TextMode="MultiLine" CssClass="hcOrderViewNotes" />
                    </div>
                </asp:Panel>
            </div>
            <div class="hcColumn" style="width: 34%">
                <asp:Panel ID="pnlCoupons" runat="server" CssClass="hcForm" DefaultButton="btnAddCoupon">
                    <div class="hcFormItemLabel">
                        <label class="hcLabel">Add Promotional Code</label>
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:TextBox ID="txtCoupon" runat="server" TabIndex="9100" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfCoupon" ValidationGroup="vgCoupon" ErrorMessage="Coupon Code is required" ControlToValidate="txtCoupon" Display="Dynamic"
                            runat="server" EnableClientScript="false" CssClass="hcFormError" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnAddCoupon" ValidationGroup="vgCoupon" CausesValidation="true" runat="server" CssClass="hcSecondaryAction hcSmall disabled" Text="Add" OnClick="btnAddCoupon_Click" />
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView CellPadding="3" CellSpacing="0" GridLines="none" ID="CouponGrid" runat="server"
                            AutoGenerateColumns="False" DataKeyNames="CouponCode" ShowHeader="False" OnRowDeleting="CouponGrid_RowDeleting" AlternatingRowStyle-Wrap="true"  RowStyle-Wrap="true"  > 
                            <Columns>
                                <asp:BoundField DataField="CouponCode" ShowHeader="False" ItemStyle-Width="400px" />
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnDeleteCoupon" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/x.png" runat="server"
                                            CausesValidation="false" CommandName="Delete" AlternateText="Delete Coupon" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </div>
            <div class="hcColumnRight hcLeftBorder" style="width: 32%">
                <div class="hcForm">
                    <asp:Literal runat="server" ID="litOrderSummary" />
                </div>
            </div>
            <ul class="hcActions">
                <li>
                    <asp:LinkButton ID="btnSubmit" runat="server" Text="Place Order" CssClass="hcPrimaryAction" TabIndex="9999" OnClick="btnSubmit_Click" />
                </li>
                <li></li>
            </ul>
            <hcc:AddressNormalizationDialog ID="AddressNormalizationDialog" PrimaryAddressLabel="Shipping Address" SecondaryAddressLabel="Billing Address" runat="server" />

            <script type="text/javascript">
                $(function () {
                    var AddressValidator = createAddressValidator();
                    AddressValidator.init($('.hcBillToAddress').data('address-validation-inputs'), "#<%= btnSubmit.ClientID%>", $('.hcShipToAddress').data('address-validation-inputs'), "#<%= chkBillToSame.ClientID%>");
                });
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
