<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecurringPayments.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.RecurringPayments" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CreditCardInput.ascx" TagName="CreditCardInput" TagPrefix="hcc" %>
<hcc:MessageBox ID="ucMessageBox" runat="server" />
<ul class="hcRibbonTabs">
    <li>
        <asp:LinkButton ID="lnkCC" runat="server" Text="Credit Cards" OnClick="lnkCC_Click" />
    </li>
</ul>
<asp:MultiView ID="mvPayments" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewCreditCards" runat="server">
        <div class="hcBlock">
            <table>
                <tr>
                    <td valign="top">
                        <div class="hcForm">
                            <h4>Update Credit Card</h4>
                            <hcc:CreditCardInput ID="ucCreditCardInput" runat="server" ShowSecurityCode="True" />
                            <asp:LinkButton ID="lnkUpdateCreditCard" runat="server" CssClass="btn"
                                Text="<b>Update Credit Card</b>" OnClick="lnkUpdateCreditCard_Click" />
                        </div>
                    </td>
                    <td valign="top">
                        <div class="hcForm">
                            <h4>Manual Payment Registration</h4>
                            <table>
                                <tr>
                                    <td class="formlabel">Hold:</td>
                                    <td class="formfield">
                                        <asp:DropDownList runat="server" ID="ddlItems" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formlabel">Amount:</td>
                                    <td class="formfield">
                                        <asp:TextBox ID="txtAmount" runat="server" Columns="10" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formlabel">
                                        <asp:LinkButton ID="lnkRegisterPayment" runat="server" CssClass="btn"
                                            Text="<b>Register Payment</b>" OnClick="lnkRegisterPayment_Click" /></td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:View>
</asp:MultiView>