<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentInformation.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Orders.PaymentInformation" %>
<div class="hcForm">
    <div class="hcFormItem">
        <label class="hcLabel"><%=Localization.GetString("lblPaymentInformation") %></label>
        <table width="100%">
            <tr>
                <td colspan="2" class="formfield" style="border-bottom: solid 1px #999;">
                    <asp:Label runat="server" ID="lblPaymentSummary" />
                </td>
            </tr>
            <tr>
                <td class="formfield"><%=Localization.GetString("lblAuthorized") %></td>
                <td class="formlabel">
                    <asp:Label ID="lblPaymentAuthorized" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="formfield"><%=Localization.GetString("lblCharged") %></td>
                <td class="formlabel">
                    <asp:Label ID="lblPaymentCharged" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="formfield" style="border-bottom: solid 1px #999; padding-bottom: 3px;"><%=Localization.GetString("lblRefunded") %></td>
                <td class="formlabel" style="border-bottom: solid 1px #999; padding-bottom: 3px;">
                    <asp:Label ID="lblPaymentRefunded" runat="server" />
                </td>
            </tr>
            <tr runat="server" visible="false">
                <td class="formfield"><%=Localization.GetString(("lblReturnedItems")) %></td>
                <td class="formlabel">
                    <asp:Label ID="lblReturnedItems" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="formfield"><%=Localization.GetString("lblAmountDue") %></td>
                <td class="formlabel">
                    <strong>
                        <asp:Label ID="lblPaymentDue" runat="server" />
                    </strong>
                </td>
            </tr>
        </table>
    </div>
</div>
