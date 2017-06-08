<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderPayments"
    Title="Payments" Codebehind="OrderPayments.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register src="ReceivePayments.ascx" tagname="ReceivePayments" tagprefix="hcc" %>
<%@ Register src="RecurringPayments.ascx" tagname="RecurringPayments" tagprefix="hcc" %>
<%@ Register src="OrderActions.ascx" tagname="OrderActions" tagprefix="hcc" %>
<%@ Register Src="PaymentInformation.ascx" TagName="PaymentInformation" TagPrefix="hcc" %>
<%@ Register src="OrderStatusDisplay.ascx" tagname="OrderStatusDisplay" tagprefix="hcc" %>

<asp:Content ID="Nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:OrderActions ID="ucOrderActions" CurrentOrderPage="Payment" runat="server" />

    <div class="hcBlock">
        <hcc:PaymentInformation ID="ucPaymentInformation" runat="server" />
    </div>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="Server">
    <hcc:MessageBox id="ucMessageBox" runat="server" />
    <table border="0" cellspacing="0" cellpadding="3" width="100%">
        <tr>
            <td width="50%" class="formfield">
                <h1>Order <asp:Label ID="lblOrderNumber" runat="server" Text="000000"></asp:Label> Payments</h1>
            </td>
            <td align="left" valign="top">
                <hcc:OrderStatusDisplay ID="ucOrderStatusDisplay" runat="server" /><br />
            </td>
        </tr>
    </table>
    <div class="hcContentWrapper">
        <h2>Actions</h2>
        <hcc:ReceivePayments ID="ucReceivePayments" runat="server" />
        <hcc:RecurringPayments ID="ucRecurringPayments" runat="server" />
    </div>
    <div class="hcForm">
        <h2>Transactions</h2>
		<div class="hcPaymentTransaction">
			<asp:Literal ID="litTransactions" runat="server" EnableViewState="false" />
		</div>
    </div>
</asp:Content>
