<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.ReceivePayments" CodeBehind="ReceivePayments.ascx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CreditCardInput.ascx" TagName="CreditCardInput" TagPrefix="hcc" %>
<hcc:MessageBox ID="ucMessageBox" runat="server" />
<ul class="hcRibbonTabs">
    <li>
        <asp:LinkButton ID="lnkCC" runat="server" Text="Credit Cards" OnClick="lnkCC_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkGiftCard" runat="server" Text="Gift Card" OnClick="lnkGiftCard_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkPayPal" runat="server" Text="PayPal" OnClick="lnkPayPal_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkPO" runat="server" Text="PO" OnClick="lnkPO_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkCompanyAccount" runat="server" Text="Comp. Acct." OnClick="lnkCompanyAccount_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkCash" runat="server" Text="Cash" OnClick="lnkCash_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkCheck" runat="server" Text="Check" OnClick="lnkCheck_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkPoints" runat="server" Text="Points" OnClick="lnkPoints_Click" />
    </li>
</ul>
<asp:MultiView ID="mvPayments" runat="server">
    <asp:View ID="viewCreditCards" runat="server">
        <div class="hcBlock">
            <div class="hcForm">
                <table class="hcForm" border="0" cellspacing="0" cellpadding="3" width="100%">
                <tr>
                    <td width="50%">
                        <h4>Pending Holds</h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel">Hold</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstCreditCardAuths" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="CreditCardAuthAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Security Code</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="CreditCardAuthSecurityCode" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>    
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardVoidAuth" runat="server"
                                CssClass="btn" Text="<b>Void Hold</b>" OnClick="lnkCreditCardVoidAuth_Click" />
                            <asp:LinkButton ID="lnkCreditCardCaptureAuth" runat="server"
                                CssClass="btn" Text="<b>Capture Hold</b>"
                                OnClick="lnkCreditCardCaptureAuth_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4>New Charges</h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel">Card</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstCreditCards" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="CreditCardChargeAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Security Code</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="CreditCardChargeSecurityCode" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardNewAuth" runat="server"
                                CssClass="btn" Text="<b>Hold Funds</b>" OnClick="lnkCreditCardNewAuth_Click" />
                            <asp:LinkButton ID="lnkCreditCardCharge" runat="server"
                                CssClass="btn" Text="<b>Charge Card</b>" OnClick="lnkCreditCardCharge_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="50%">
                        <h4>Refunds</h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel">Charge</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstCreditCardCharges" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="CreditCardRefundAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Security Code</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="CreditCardRefundSecurityCode" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardRefund" runat="server"
                                CssClass="btn" Text="<b>Refund Charge</b>"
                                OnClick="lnkCreditCardRefund_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4>Add a New Card</h4>
                        <hcc:CreditCardInput ID="ucCreditCardInput" runat="server" />
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardAddInfo" runat="server" CssClass="btn"
                                Text="<b>Save Card to Order</b>" OnClick="lnkCreditCardAddInfo_Click" />
                        </div>
                    </td>
                </tr>
            </table>
            </div>
        </div>
    </asp:View>
    <asp:View ID="viewPayPal" runat="server">
        <div class="hcBlock">
            <table class="hcFormTable" border="0" cellspacing="0" cellpadding="3" width="100%">
                <tr>
                    <td width="50%">
                        <h4>PayPal Express Holds</h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel">Hold</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstPayPalHold" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="PayPalHoldAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPayPalVoidHold" runat="server"
                                CssClass="btn" Text="<b>Void Hold</b>" OnClick="lnkPayPalVoidHold_Click" />
                            <asp:LinkButton ID="lnkPayPalCaptureHold" runat="server"
                                CssClass="btn" Text="<b>Capture Hold</b>" OnClick="lnkPayPalCaptureHold_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4>PayPal Express Refunds</h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel">Charge</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstPayPalRefund" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="PayPalRefundAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPayPalRefund" runat="server"
                                CssClass="btn" Text="<b>Refund Charge</b>"
                                OnClick="lnkPayPalRefund_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </asp:View>
    <asp:View ID="viewPO" runat="server">
        <div class="hcBlock">
            <table class="hcFormTable" border="0">
                <tbody>
                    <tr>
                        <td width="50%">
                            <h4>Purchase Order</h4>
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class="hcForm">
                                               <table class="hcFormTable">
                                                    <tr>
                                                        <td class="formlabel">PO Number</td>
                                                        <td class="formfield">
                                                            <asp:DropDownList ID="lstPO" runat="server"></asp:DropDownList></td>
                                                    </tr>
                                               </table>
                                           </div>
                                            <div class="hcTransactionLink">
                                                <asp:LinkButton ID="lnkPOAccept" runat="server"
                                                    class="btn" Text="<b>Accept Purchase Order</b>"
                                                    OnClick="lnkPOAccept_Click" />
                                            </div>
                                        </td>
                                    </tr>   
                                    <tr>
                                        <td colspan="2">&nbsp;<br />
                                            <hr />
                                            &nbsp;<br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="hcForm">
                                        <table class="hcFormTable">
                                            <tr>
                                                <td class="formlabel">PO Number</td>
                                                <td class="formfield">
                                                    <asp:TextBox ID="PONewNumber" runat="server" Columns="20"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td class="formlabel">Amount</td>
                                                <td class="formfield">
                                                    <asp:TextBox ID="PONewAmount" runat="server" Columns="10"></asp:TextBox></td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="hcTransactionLink">
                                        <asp:LinkButton ID="lnkPOAdd" runat="server"
                                            class="btn" Text="<b>Add Purchase Order Information</b>"
                                            OnClick="lnkPOAdd_Click" />
                                    </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td width="50%">
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <h4>About Purchase Orders</h4>
                                            <p>A purchase order is a promise made by a company to pay the invoiced amount. Accepting a PO is the same as accepting cash and the system will release the order for shipping. It is your responsibility to send an invoice to the customer and collect payment on the PO.</p>
                                            <p>Before accepting a PO, make sure the company has a good credit rating. Once accepted you can't undo the credit to the order.</p>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            &nbsp;
        </div>
    </asp:View>
    <asp:View ID="viewCompanyAccount" runat="server">
        <div class="hcBlock">
            <table class="hcFormTable" border="0">
                <tbody>
                    <tr>
                        <td width="50%">
                            <h4>Company Account</h4>
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class="hcForm">
                                                <table class="hcFormTable">
                                                    <tr>
                                                        <td class="formlabel">Account Number</td>
                                                        <td class="formfield">
                                                            <asp:DropDownList ID="lstCompanyAccount" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="hcTransactionLink">
                                                <asp:LinkButton ID="lnkCompanyAccountAccept" runat="server"
                                                    class="btn" Text="<b>Accept CompanyAccount</b>"
                                                    OnClick="lnkCompanyAccountAccept_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">&nbsp;<br />
                                            <hr />
                                            &nbsp;<br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="hcForm">
                                                <table class="hcFormTable">
                                                    <tr>
                                                        <td class="formlabel">Account Number</td>
                                                        <td class="formfield">
                                                            <asp:TextBox ID="CompanyAccountNewNumber" runat="server" Columns="20"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formlabel">Amount</td>
                                                        <td class="formfield">
                                                            <asp:TextBox ID="CompanyAccountNewAmount" runat="server" Columns="10"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="hcTransactionLink">
                                                <asp:LinkButton ID="lnkCompanyAccountAdd" runat="server"
                                                    class="btn" Text="<b>Add Company Account Information</b>"
                                                    OnClick="lnkCompanyAccountAdd_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td width="50%">
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <h4>About Company Accounts</h4>
                                            <p>Company Accounts must be verified offline. Accepting a company account as payment is the same as accepting cash and the system will release the order for shipping.</p>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            &nbsp;
        </div>
    </asp:View>
    <asp:View ID="viewCash" runat="server">
        <div class="hcBlock">
            <h4>Cash</h4>
            <div class="hcForm">
                <table class="hcFormTable">
                    <tr>
                        <td class="formlabel">Amount</td>
                        <td class="formfield">
                            <asp:TextBox ID="CashAmount" runat="server" Columns="10"></asp:TextBox></td>
                    </tr>
                </table>
            </div>
            <div class="hcTransactionLink">
                <asp:LinkButton ID="btnCashRefund" runat="server" class="btn"
                    Text="<b>Refund Cash</b>" OnClick="btnCashRefund_Click" />
                <asp:LinkButton ID="btnCashReceive" runat="server"
                    class="btn" Text="<b>Receive Cash</b>" OnClick="btnCashReceive_Click" />
            </div>
            <br />
            &nbsp;
        </div>
    </asp:View>
    <asp:View ID="viewCheck" runat="server">
        <div class="hcBlock">
                <h4>Check</h4>
            <div class="hcForm">
                <table class="hcFormTable">
                    <tr>
                        <td class="formlabel">Check Number</td>
                        <td class="formfield">
                            <asp:TextBox ID="CheckNumberField" runat="server" Columns="10"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="formlabel">Amount</td>
                        <td class="formfield">
                            <asp:TextBox ID="CheckAmountField" runat="server" Columns="10"></asp:TextBox></td>
                    </tr>
                </table>
            </div>
            <div class="hcTransactionLink">
                <asp:LinkButton ID="lnkCheckReturn" runat="server" class="btn"
                    Text="<b>Return Check</b>" OnClick="lnkCheckReturn_Click" />
                        
                <asp:LinkButton ID="lnkCheckReceive" runat="server"
                    class="btn" Text="<b>Receive Check</b>" OnClick="lnkCheckReceive_Click" />
            </div>
            <br />
            &nbsp;
        </div>
    </asp:View>
    <asp:View ID="viewPoints" runat="server">
        <div class="hcBlock">
            <table border="0" cellspacing="0" cellpadding="3" width="100%">
                <tr>
                    <td width="50%">
                        <h4>Points On Hold</h4>
                        <div class="hcForm">
                            <table>
                                <tr>
                                    <td class="formlabel">Hold</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstPointsHeld" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="PointsHeldAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPointsVoidAuth" runat="server"
                                    CssClass="btn" Text="<b>Void Hold</b>" OnClick="lnkPointsVoidAuth_Click" />
                            <asp:LinkButton ID="lnkPointsCaptureAuth" runat="server"
                                CssClass="btn" Text="<b>Capture Hold</b>"
                                OnClick="lnkPointsCaptureAuth_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4>Pay with Points</h4>
                        <div class="hcForm">
                            <table>
                                <tr>
                                    <td class="formlabel">Points Available</td>
                                    <td class="formfield">
                                        <asp:Label ID="lblPointsAvailable" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="PointsNewAmountField" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPointsNewAuth" runat="server"
                                    CssClass="btn" Text="<b>Hold Points</b>" OnClick="lnkPointsNewAuth_Click" />
                            <asp:LinkButton ID="lnkPointsNewCharge" runat="server"
                                CssClass="btn" Text="<b>Pay with Points</b>" OnClick="lnkPointsNewCharge_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="50%">
                        <h4>Refund Points</h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel">Charge</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstPointsRefundable" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="PointsRefundAmount" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPointsRefund" runat="server"
                                    CssClass="btn" Text="<b>Refund To Points</b>"
                                    OnClick="lnkPointsRefund_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </asp:View>

    <asp:View ID="viewGiftCards" runat="server">
        <div class="hcBlock">
            <div class="hcForm">

                <table border="0" cellspacing="0" cellpadding="3" width="100%">
                    <tr>
                        <td width="50%">
                            <h4>Gift Card Holds</h4>
                            <table>
                                <tr>
                                    <td class="formlabel">Hold</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstGiftCardHold" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="AmountGiftCardHold" runat="server" Columns="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="hcTransactionLink">
                                            <asp:LinkButton ID="lnkGiftCardVoidHold" runat="server"
                                                CssClass="btn" Text="<b>Void Hold</b>" />

                                            <asp:LinkButton ID="lnkGiftCardCapturHold" runat="server"
                                                CssClass="btn" Text="<b>Capture Hold</b>" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </td>
                        <td>
                            <h4>New Gift Card Payment</h4>
                            <table>
                                <tr>
                                    <td class="formlabel">Card</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="GiftCardNew" runat="server" Columns="20"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="AmountGiftCardNew" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="hcTransactionLink">
                                            <asp:LinkButton ID="lnkGiftCardHold" runat="server"
                                                CssClass="btn" Text="<b>Hold Gift Card</b>" OnClick="lnkGiftCardHold_Click" />
                                            <asp:LinkButton ID="lnkGiftCardCharge" runat="server"
                                                CssClass="btn" Text="<b>Charge Gift Card</b>" OnClick="lnkGiftCardCharge_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br />

                        </td>
                    </tr>
                    <tr>
                        <td width="50%">
                            <h4>Gift Card Refunds</h4>
                            <table>
                                <tr>
                                    <td class="formlabel">Previous Charges</td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstGiftCardCharges" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="AmountGiftCardRefund" runat="server" Columns="10"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="hcTransactionLink">
                                            <asp:LinkButton ID="lnkGiftCardRefund" runat="server"
                                                CssClass="btn" Text="<b>Refund Card</b>"
                                                OnClick="lnkGiftCardRefund_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            &nbsp;
                        </td>
                        <td>
                            <h4>Gift Card Balance Check</h4>
                            <asp:TextBox ID="GiftCardBalanceCheckCard" runat="server" Columns="20"></asp:TextBox>
                            <div class="hcTransactionLink">
                                <asp:LinkButton ID="lnkGiftCardBalanceCheck" runat="server" CssClass="btn"
                                    Text="<b>Check Balance</b>" OnClick="lnkGiftCardBalanceCheck_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </asp:View>

</asp:MultiView>
