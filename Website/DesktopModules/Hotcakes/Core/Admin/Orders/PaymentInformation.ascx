<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentInformation.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.PaymentInformation" %>
<div class="hcForm">
    <div class="hcFormItem">
        <label class="hcLabel">Payment Information</label>
        <table width="100%">
            <tr>
                <td colspan="2" class="formfield" style="border-bottom: solid 1px #999;">
                    <asp:Label runat="server" ID="lblPaymentSummary" />
                </td>
            </tr>
            <tr>
                <td class="formfield">Authorized:</td>
                <td class="formlabel">
                    <asp:Label ID="lblPaymentAuthorized" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="formfield">Charged:</td>
                <td class="formlabel">
                    <asp:Label ID="lblPaymentCharged" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="formfield" style="border-bottom: solid 1px #999; padding-bottom: 3px;">Refunded:</td>
                <td class="formlabel" style="border-bottom: solid 1px #999; padding-bottom: 3px;">
                    <asp:Label ID="lblPaymentRefunded" runat="server" />
                </td>
            </tr>
            <tr runat="server" visible="false">
                <td class="formfield">Returned Items:</td>
                <td class="formlabel">
                    <asp:Label ID="lblReturnedItems" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="formfield">Amount Due:</td>
                <td class="formlabel">
                    <strong>
                        <asp:Label ID="lblPaymentDue" runat="server" />
                    </strong>
                </td>
            </tr>
        </table>
    </div>
</div>
