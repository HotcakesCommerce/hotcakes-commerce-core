<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.ReceivePayments" CodeBehind="ReceivePayments.ascx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CreditCardInput.ascx" TagName="CreditCardInput" TagPrefix="hcc" %>
<hcc:MessageBox ID="ucMessageBox" runat="server" />
<ul class="hcRibbonTabs">
    <li>
        <asp:LinkButton ID="lnkCC" runat="server" resourcekey="CreditCards" OnClick="lnkCC_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkGiftCard" runat="server" resourcekey="GiftCards" OnClick="lnkGiftCard_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkPayPal" runat="server" Text="PayPal" OnClick="lnkPayPal_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkPO" runat="server" resourcekey="PurchaseOrders" OnClick="lnkPO_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkCompanyAccount" runat="server" resourcekey="CoAccount" OnClick="lnkCompanyAccount_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkCash" runat="server" resourcekey="Cash" OnClick="lnkCash_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkCheck" runat="server" resourcekey="Check" OnClick="lnkCheck_Click" />
    </li>
    <li>
        <asp:LinkButton ID="lnkPoints" runat="server" resourcekey="RewardPoints" OnClick="lnkPoints_Click" />
    </li>
</ul>
<asp:MultiView ID="mvPayments" runat="server">
    <asp:View ID="viewCreditCards" runat="server">
        <div class="hcBlock">
            <div class="hcForm">
                <table class="hcForm" border="0" cellspacing="0" cellpadding="3" width="100%">
                <tr>
                    <td width="50%">
                        <h4><%=Localization.GetString("PendingHolds") %></h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Hold") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstCreditCardAuths" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="CreditCardAuthAmount" runat="server" Columns="10"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("SecurityCode") %></td>
                                    <td class="formfield"><asp:TextBox ID="CreditCardAuthSecurityCode" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>    
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardVoidAuth" runat="server" CssClass="btn" resourcekey="VoidHold" OnClick="lnkCreditCardVoidAuth_Click" />
                            <asp:LinkButton ID="lnkCreditCardCaptureAuth" runat="server" CssClass="btn" resourcekey="CaptureHold" OnClick="lnkCreditCardCaptureAuth_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4><%=Localization.GetString("NewCharges") %></h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Card") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstCreditCards" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="CreditCardChargeAmount" runat="server" Columns="10"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("SecurityCode") %></td>
                                    <td class="formfield"><asp:TextBox ID="CreditCardChargeSecurityCode" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardNewAuth" runat="server" CssClass="btn" resourcekey="HoldFunds" OnClick="lnkCreditCardNewAuth_Click" />
                            <asp:LinkButton ID="lnkCreditCardCharge" runat="server" CssClass="btn" resourcekey="ChargeCard" OnClick="lnkCreditCardCharge_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="50%">
                        <h4><%=Localization.GetString("Refunds") %></h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Charge") %></td>
                                    <td class="formfield">
                                        <asp:DropDownList ID="lstCreditCardCharges" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="CreditCardRefundAmount" runat="server" Columns="10"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("SecurityCode") %></td>
                                    <td class="formfield"><asp:TextBox ID="CreditCardRefundSecurityCode" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkCreditCardRefund" runat="server" CssClass="btn" resourcekey="RefundCharge" OnClick="lnkCreditCardRefund_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4><%=Localization.GetString("AddNewCard") %></h4>
                        <hcc:CreditCardInput ID="ucCreditCardInput" runat="server" />
                        <div class="hcTransactionLink"><asp:LinkButton ID="lnkCreditCardAddInfo" runat="server" CssClass="btn" resourcekey="SaveCardOrder" OnClick="lnkCreditCardAddInfo_Click" />
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
                        <h4><%=Localization.GetString("PayPalExpressHolds") %></h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Hold") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstPayPalHold" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="PayPalHoldAmount" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPayPalVoidHold" runat="server" CssClass="btn" resourcekey="VoidHold" OnClick="lnkPayPalVoidHold_Click" />
                            <asp:LinkButton ID="lnkPayPalCaptureHold" runat="server" CssClass="btn" resourcekey="CaptureHold" OnClick="lnkPayPalCaptureHold_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4><%=Localization.GetString("PayPalExpressRefunds") %></h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Charge") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstPayPalRefund" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="PayPalRefundAmount" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPayPalRefund" runat="server" CssClass="btn" resourcekey="RefundCharge" OnClick="lnkPayPalRefund_Click" />
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
                            <h4><%=Localization.GetString("PurchaseOrders") %></h4>
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class="hcForm">
                                               <table class="hcFormTable">
                                                    <tr>
                                                        <td class="formlabel"><%=Localization.GetString("PoNumber") %></td>
                                                        <td class="formfield">
                                                            <asp:DropDownList ID="lstPO" runat="server"/></td>
                                                    </tr>
                                               </table>
                                           </div>
                                            <div class="hcTransactionLink">
                                                <asp:LinkButton ID="lnkPOAccept" runat="server" class="btn" resourcekey="AcceptPurchaseOrder" OnClick="lnkPOAccept_Click" />
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
                                                <td class="formlabel"><%=Localization.GetString("PoNumber") %></td>
                                                <td class="formfield"><asp:TextBox ID="PONewNumber" runat="server" Columns="20"/></td>
                                            </tr>
                                            <tr>
                                                <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                                <td class="formfield"><asp:TextBox ID="PONewAmount" runat="server" Columns="10"/></td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="hcTransactionLink">
                                        <asp:LinkButton ID="lnkPOAdd" runat="server" class="btn" resourcekey="AddPo" OnClick="lnkPOAdd_Click" />
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
                                            <h4><%=Localization.GetString("AboutPo") %></h4>
                                            <p><%=Localization.GetString("AboutPoPara1") %></p>
                                            <p><%=Localization.GetString("AboutPoPara2") %></p>
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
                            <h4><%=Localization.GetString("CompanyAccount") %></h4>
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class="hcForm">
                                                <table class="hcFormTable">
                                                    <tr>
                                                        <td class="formlabel"><%=Localization.GetString("AccountNumber") %></td>
                                                        <td class="formfield"><asp:DropDownList ID="lstCompanyAccount" runat="server"/></td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="hcTransactionLink">
                                                <asp:LinkButton ID="lnkCompanyAccountAccept" runat="server" class="btn" resourcekey="AcceptCompanyAccount" OnClick="lnkCompanyAccountAccept_Click" />
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
                                                        <td class="formlabel"><%=Localization.GetString("AccountNumber") %></td>
                                                        <td class="formfield"><asp:TextBox ID="CompanyAccountNewNumber" runat="server" Columns="20"/></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                                        <td class="formfield"><asp:TextBox ID="CompanyAccountNewAmount" runat="server" Columns="10"/></td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="hcTransactionLink">
                                                <asp:LinkButton ID="lnkCompanyAccountAdd" runat="server" class="btn" resourcekey="AddCompanyAccountInformation" OnClick="lnkCompanyAccountAdd_Click" />
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
                                            <h4><%=Localization.GetString("AboutCompanyAccounts") %></h4>
                                            <p><%=Localization.GetString("AboutCompanyPara1") %></p>
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
            <h4><%=Localization.GetString("Cash") %></h4>
            <div class="hcForm">
                <table class="hcFormTable">
                    <tr>
                        <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                        <td class="formfield"><asp:TextBox ID="CashAmount" runat="server" Columns="10"/></td>
                    </tr>
                </table>
            </div>
            <div class="hcTransactionLink">
                <asp:LinkButton ID="btnCashRefund" runat="server" class="btn" resourcekey="RefundCash" OnClick="btnCashRefund_Click" />
                <asp:LinkButton ID="btnCashReceive" runat="server" class="btn" resourcekey="ReceiveCash" OnClick="btnCashReceive_Click" />
            </div>
            <br />
            &nbsp;
        </div>
    </asp:View>
    <asp:View ID="viewCheck" runat="server">
        <div class="hcBlock">
            <h4><%=Localization.GetString("Check") %></h4>
            <div class="hcForm">
                <table class="hcFormTable">
                    <tr>
                        <td class="formlabel"><%=Localization.GetString("CheckNumber") %></td>
                        <td class="formfield"><asp:TextBox ID="CheckNumberField" runat="server" Columns="10"/></td>
                    </tr>
                    <tr>
                        <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                        <td class="formfield"><asp:TextBox ID="CheckAmountField" runat="server" Columns="10"/></td>
                    </tr>
                </table>
            </div>
            <div class="hcTransactionLink">
                <asp:LinkButton ID="lnkCheckReturn" runat="server" class="btn" resourcekey="ReturnCheck" OnClick="lnkCheckReturn_Click" />
                <asp:LinkButton ID="lnkCheckReceive" runat="server" class="btn" resourcekey="ReceiveCheck" OnClick="lnkCheckReceive_Click" />
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
                        <h4><%=Localization.GetString("PointsOnHold") %></h4>
                        <div class="hcForm">
                            <table>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Hold") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstPointsHeld" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="PointsHeldAmount" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPointsVoidAuth" runat="server" CssClass="btn" resourcekey="VoidHold" OnClick="lnkPointsVoidAuth_Click" />
                            <asp:LinkButton ID="lnkPointsCaptureAuth" runat="server" CssClass="btn" resourcekey="CaptureHold" OnClick="lnkPointsCaptureAuth_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                    <td>
                        <h4><%=Localization.GetString("PayWithPoints") %></h4>
                        <div class="hcForm">
                            <table>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("PointsAvailable") %></td>
                                    <td class="formfield"><asp:Label ID="lblPointsAvailable" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="PointsNewAmountField" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPointsNewAuth" runat="server" CssClass="btn" resourcekey="HoldPoints" OnClick="lnkPointsNewAuth_Click" />
                            <asp:LinkButton ID="lnkPointsNewCharge" runat="server" CssClass="btn" resourcekey="PayWithPoints" OnClick="lnkPointsNewCharge_Click" />
                        </div>
                        <br />
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="50%">
                        <h4><%=Localization.GetString("RefundPoints") %></h4>
                        <div class="hcForm">
                            <table class="hcFormTable">
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Charge") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstPointsRefundable" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="PointsRefundAmount" runat="server" Columns="10"/></td>
                                </tr>
                            </table>
                        </div>
                        <div class="hcTransactionLink">
                            <asp:LinkButton ID="lnkPointsRefund" runat="server" CssClass="btn" resourcekey="RefundToPoints" OnClick="lnkPointsRefund_Click" />
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
                            <h4><%=Localization.GetString("GiftCardHolds") %></h4>
                            <table>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Hold") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstGiftCardHold" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="AmountGiftCardHold" runat="server" Columns="10"/></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="hcTransactionLink">
                                            <asp:LinkButton ID="lnkGiftCardVoidHold" runat="server" CssClass="btn" resourcekey="VoidHold" />
                                            <asp:LinkButton ID="lnkGiftCardCapturHold" runat="server" CssClass="btn" resourcekey="CaptureHold" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </td>
                        <td>
                            <h4><%=Localization.GetString("NewGiftCardPayment") %></h4>
                            <table>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Card") %></td>
                                    <td class="formfield"><asp:TextBox ID="GiftCardNew" runat="server" Columns="20"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="AmountGiftCardNew" runat="server" Columns="10"/></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="hcTransactionLink">
                                            <asp:LinkButton ID="lnkGiftCardHold" runat="server" CssClass="btn" resourcekey="HoldGiftCard" OnClick="lnkGiftCardHold_Click" />
                                            <asp:LinkButton ID="lnkGiftCardCharge" runat="server" CssClass="btn" resourcekey="ChargeGiftCard" OnClick="lnkGiftCardCharge_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br />

                        </td>
                    </tr>
                    <tr>
                        <td width="50%">
                            <h4><%=Localization.GetString("GiftCardRefunds") %></h4>
                            <table>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("PreviousCharges") %></td>
                                    <td class="formfield"><asp:DropDownList ID="lstGiftCardCharges" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="formlabel"><%=Localization.GetString("Amount") %></td>
                                    <td class="formfield"><asp:TextBox ID="AmountGiftCardRefund" runat="server" Columns="10"/></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="hcTransactionLink">
                                            <asp:LinkButton ID="lnkGiftCardRefund" runat="server" CssClass="btn" resourcekey="RefundCard" OnClick="lnkGiftCardRefund_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            &nbsp;
                        </td>
                        <td>
                            <h4><%=Localization.GetString("GiftCardBalanceCheck") %></h4>
                            <asp:TextBox ID="GiftCardBalanceCheckCard" runat="server" Columns="20"/>
                            <div class="hcTransactionLink">
                                <asp:LinkButton ID="lnkGiftCardBalanceCheck" runat="server" CssClass="btn" resourcekey="CheckBalance" OnClick="lnkGiftCardBalanceCheck_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </asp:View>
</asp:MultiView>