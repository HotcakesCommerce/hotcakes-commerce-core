<%@ Page Title="Create New Order" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="CreateOrder.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.CreateOrder" %>
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
                <label class="hcLabel"><%=Localization.GetString("lblCodesUsed") %></label>
                <div class="hcCouponCodes">
                    <asp:Label ID="CouponField" runat="server" />
                </div>
            </div>
        </div>
    </div>

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkBacktoAbandonedCartsReport" resourcekey="lnkBacktoAbandonedCartsReport" CssClass="hcTertiaryAction" Target="_self" runat="server" />
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

            <h1><%=PageTitle %></h1>
            <hcc:MessageBox ID="ucMessageBox" runat="server" />

            <h2><%=Localization.GetString("lblOrderItems") %></h2>
            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <hcc:OrderItems runat="server" Id="ucOrderItems" EditMode="True" AllowUpdateQuantities="True" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="hcColumnLeft hcRightBorder" style="width: 30%">
                <div class="hcForm hcShipToAddress">
                    <h2><%=Localization.GetString("lblShipTo") %></h2>
                    <div class="hcFormMessage hcShipToAddressMessage" style="margin-left: 2px; margin-right: 2px;"></div>
                    <hcc:AddressEditor ID="ShipToAddress" ShowAddressLine3="false" CreateValidationInputs="True" FormSelector=".hcShipToAddress" ErrorMessageSelector=".hcShipToAddressMessage" runat="server" TabOrderOffSet="1000" />
                    <div class="hcFormItem">
                        <asp:Label ID="EmailAddressLabel" CssClass="hcLabel" AssociatedControlID="EmailAddressTextBox" runat="server" resourcekey="lblEmail"/>
                        <asp:TextBox ID="EmailAddressTextBox" runat="server" TabIndex="3000" />
                        <asp:RegularExpressionValidator ID="EmailRegularExpressionValidator" resourcekey="revEmailAddress" runat="server"
                                                        ControlToValidate="EmailAddressTextBox" CssClass="hcFormError"
                                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"/>
                    </div>
                </div>
            </div>
            <div class="hcColumn" style="width: 35%">
                <div class="hcForm">
                    <h2><%=Localization.GetString("lblBillTo") %></h2>
                    <asp:Panel ID="pnlBillTo" CssClass="hcBillToAddress" runat="server" Visible="false">
                        <div class="hcFormMessage hcBillToAddressMessage" style="margin-left: 2px; margin-right: 2px;"></div>
                        <hcc:AddressEditor ID="BillToAddress" ShowAddressLine3="false" CreateValidationInputs="True" FormSelector=".hcBillToAddress" ErrorMessageSelector=".hcBillToAddressMessage" runat="server" TabOrderOffSet="2000" />
                    </asp:Panel>
                    <div class="hcFormItem">
                        <asp:CheckBox ID="chkBillToSame" Checked="true" runat="server" AutoPostBack="true"
                            resourcekey="chkBillToSame" OnCheckedChanged="chkBillToSame_CheckedChanged" />
                    </div>
                </div>
            </div>

            <div class="hcColumnRight" style="width: 33%">
                <table border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td>
                            <asp:Button ID="btnFindUsers" CausesValidation="false" resourcekey="btnFindUsers" runat="server" OnClick="btnFindUsers_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnNewUsers" CausesValidation="false" resourcekey="btnNewUsers" runat="server" OnClick="btnNewUsers_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnFindOrders" CausesValidation="false" resourcekey="btnFindOrders" runat="server" OnClick="btnFindOrders_Click" />
                        </td>
                    </tr>
                </table>
                <div class="controlarea1" style="padding: 10px;">
                    <asp:MultiView ID="viewFindUsers" runat="server" ActiveViewIndex="0">
                        <asp:View ID="ViewFind" runat="server">
                            <strong><%=Localization.GetString("lblSearchCustomer") %></strong><br />
                            <asp:Label ID="lblFindUserMessage" runat="server"></asp:Label>
                            <asp:Panel ID="pnlFindUser" runat="server" DefaultButton="btnFindUser">
                                <table border="0" cellspacing="0" cellpadding="3">
                                    <tr>
                                        <td class="formlabel"><%=Localization.GetString("lblKeyword") %></td>
                                        <td class="formfield">
                                            <asp:TextBox ID="FilterUserField" runat="server" Columns="15"/></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel">&nbsp;</td>
                                        <td class="formfield">
                                            <asp:Button ID="btnFindUser" runat="server" resourcekey="btnFindUser" CausesValidation="false" OnClick="btnFindUser_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                             <asp:GridView ShowHeader="false" ID="gridSelectUser" runat="server" PageSize="5" AutoGenerateColumns="False" AllowCustomPaging="True" AllowPaging="True"
                                 GridLines="None" OnEditCommand="gridSelectUser_RowEditing" OnPageIndexChanging="gridSelectUser_PageIndexChanging" OnRowCommand="gridSelectUser_RowCommand" DataKeyNames="Bvin" CssClass="hcGrid">
                                 <RowStyle CssClass="hcGridRow" />
                                 <AlternatingRowStyle CssClass="hcGridAltRow" />
                                 <Columns>
                                     <asp:TemplateField>
                                         <ItemTemplate>
                                             <asp:Label ID="lblUsername" runat="server" Text='<%# Bind("Email") %>' /><br />
                                             <span class="smalltext">
                                                 <asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("FirstName") %>' />
                                                 <asp:Label ID="lblLastName" runat="server" Text='<%# Bind("LastName") %>' />
                                             </span>
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:TemplateField>
                                         <ItemTemplate>
                                             <asp:LinkButton ID="SelectUserButton" runat="server" CausesValidation="false" CommandName="SelectUser" CommandArgument='<%# Container.DataItemIndex %>' resourcekey="btnSelectCustomer" />
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                 </Columns>
                             </asp:GridView>
                        </asp:View>
                        <asp:View ID="ViewNew" runat="server">
                            <strong><%=Localization.GetString("lblAddCustomer") %></strong><br />
                            <asp:Label ID="lblNewUserMessage" runat="server"/>
                            <asp:Panel ID="pnlNewUser" runat="server" DefaultButton="btnNewUserSave">
                                <table border="0" cellspacing="0" cellpadding="3">
                                    <tr>
                                        <td class="formlabel"><%=Localization.GetString("lblEmail") %></td>
                                        <td class="formfield">
                                            <asp:TextBox ID="NewUserEmailField" runat="server" Columns="15"/></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel"><%=Localization.GetString("lblFirstName") %></td>
                                        <td class="formfield">
                                            <asp:TextBox ID="NewUserFirstNameField" runat="server" Columns="15"/></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel"><%=Localization.GetString("lblLastName") %></td>
                                        <td class="formfield">
                                            <asp:TextBox ID="NewUserLastNameField" runat="server" Columns="15"/></td>
                                    </tr>
                                    <tr>
                                        <td class="formlabel">&nbsp;</td>
                                        <td class="formfield">
                                            <asp:Button ID="btnNewUserSave" runat="server" resourcekey="btnNewUserSave" CausesValidation="false" OnClick="btnNewUserSave_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </asp:View>
                        <asp:View ID="ViewOrder" runat="server">
                            <strong><%=Localization.GetString("lblFindCustomerByOrder") %></strong>
                            <asp:Label ID="lblFindOrderMessage" runat="server"/>
                            <asp:Panel ID="pnlFindUserByOrder" runat="server" DefaultButton="btnGoFindOrder">
                                <table border="0" cellspacing="0" cellpadding="3">
                                    <tr>
                                        <td class="formlabel" align="right"><%=Localization.GetString("lblOrderNumber") %></td>
                                        <td>
                                            <asp:TextBox ID="FindOrderNumberField" runat="server" Columns="20"/></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:Button ID="btnGoFindOrder" CausesValidation="false" runat="server"
                                                resourcekey="btnGoFindOrder" OnClick="btnGoFindOrder_Click" /></td>
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
                    <h2><%=Localization.GetString("lblShipping") %></h2>
                    <asp:RadioButtonList ID="ShippingRatesList" runat="server" OnSelectedIndexChanged="ShippingRatesList_SelectedIndexChanged">
                    </asp:RadioButtonList>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:LinkButton ID="btnCalculateShipping" runat="server" resourcekey="btnCalculateShipping" CssClass="hcButton hcSmall"
                            CausesValidation="False" OnClick="btnCalculateShipping_Click" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnUpdateShipping" runat="server" resourcekey="btnUpdateShipping" CssClass="hcButton hcSmall"
                            CausesValidation="False" OnClick="btnUpdateShipping_Click" />
                    </div>
                </div>
            </div>
            <div class="hcColumnRight hcLeftBorder" style="width: 49%">
                <div class="hcForm hcPaymentForm">
                    <h2><%=Localization.GetString("lblPayment") %></h2>
                    <div runat="server" id="divNoPaymentNeeded" visible="false">
                        <asp:RadioButton ID="rbNoPayment" GroupName="PaymentGroup" runat="server" Checked="false" />
                    </div>
                    <div runat="server" id="divCreditCard" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCreditCard" GroupName="PaymentGroup" runat="server" Checked="false" />
                        <hcc:CreditCardInput ID="ucCreditCardInput" runat="server" ShowSecurityCode="True" />
                    </div>
                    <div runat="server" id="divPurchaseOrder" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbPurchaseOrder" GroupName="PaymentGroup" runat="server" Checked="false" />
                        #
                <asp:TextBox ID="txtPurchaseOrder" runat="server" Columns="10" />
                    </div>
                    <div runat="server" id="divCompanyAccount" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCompanyAccount" GroupName="PaymentGroup" runat="server" Checked="false" />
                        #
                <asp:TextBox ID="txtAccountNumber" ClientIDMode="Static" runat="server" Columns="10" />
                    </div>
                    <div runat="server" id="divCheck" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCheck" GroupName="PaymentGroup" runat="server" Checked="false" />
                    </div>
                    <div runat="server" id="divTelephone" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbTelephone" GroupName="PaymentGroup" runat="server" Checked="false" />
                    </div>
                    <div runat="server" id="divCashOnDelivery" visible="false" class="hcLabel">
                        <asp:RadioButton ID="rbCashOnDelivery" GroupName="PaymentGroup" runat="server" Checked="false" />
                    </div>
                </div>
            </div>
            <hr />

            <div class="hcColumnLeft hcRightBorder" style="width: 30%">
                <asp:Panel ID="pnlInstructions" CssClass="hcFormItem" runat="server">
                    <div class="hcFormItemLabel">
                        <label class="hcLabel"><%=Localization.GetString("lblCustomerInstructions") %></label>
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtInstructions" runat="server" Rows="5" TextMode="MultiLine" CssClass="hcOrderViewNotes" />
                    </div>
                </asp:Panel>
            </div>
            <div class="hcColumn" style="width: 34%">
                <asp:Panel ID="pnlCoupons" runat="server" CssClass="hcForm" DefaultButton="btnAddCoupon">
                    <div class="hcFormItemLabel">
                        <label class="hcLabel"><%=Localization.GetString("lblAddPromoCode") %></label>
                    </div>
                    <div class="hcFormItem hcFormItem66p">
                        <asp:TextBox ID="txtCoupon" runat="server" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfCoupon" ValidationGroup="vgCoupon" resourcekey="rfCoupon" ControlToValidate="txtCoupon" Display="Dynamic"
                            runat="server" EnableClientScript="false" CssClass="hcFormError" />
                    </div>
                    <div class="hcFormItem hcFormItem33p">
                        <asp:LinkButton ID="btnAddCoupon" ValidationGroup="vgCoupon" CausesValidation="true" runat="server" CssClass="hcSecondaryAction hcSmall disabled" resourcekey="btnAddCoupon" OnClick="btnAddCoupon_Click" />
                    </div>
                    <div class="hcFormItem">
                        <asp:GridView CellPadding="3" CellSpacing="0" GridLines="none" ID="CouponGrid" runat="server"
                            AutoGenerateColumns="False" DataKeyNames="CouponCode" ShowHeader="False" OnRowDeleting="CouponGrid_RowDeleting" AlternatingRowStyle-Wrap="true"  RowStyle-Wrap="true"> 
                            <Columns>
                                <asp:BoundField DataField="CouponCode" ShowHeader="False" ItemStyle-Width="400px" />
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDeleteCoupon" runat="server" CausesValidation="false" CommandName="Delete" resourcekey="btnDeleteCoupon" />
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
                    <asp:LinkButton ID="btnSubmit" runat="server" resourcekey="btnSubmit" CssClass="hcPrimaryAction" OnClick="btnSubmit_Click" />
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
