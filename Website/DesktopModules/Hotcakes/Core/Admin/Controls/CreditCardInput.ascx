<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Controls.CreditCardInput" Codebehind="CreditCardInput.ascx.cs" %>
<div class="hcCreditCard">
    <table border="0" cellspacing="0" cellpadding="2">
        <tr>
            <td class="formfield" colspan="2">
                <asp:Literal ID="litCardsAccepted" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="hcLabel"><%=Localization.GetString("lblCardNumber") %></td>
            <td class="formfield">
                <span class="creditcardnumber">
                    <asp:TextBox ID="cccardnumber" ClientIDMode="Static" runat="server" Columns="20" MaxLength="20" />
                </span>
            </td>
        </tr>
        <tr>
            <td class="hcLabel"><%=Localization.GetString("lblExpDate") %></td>
            <td class="formfield">
                <asp:DropDownList ID="ccexpmonth" ClientIDMode="Static" runat="server" />
                &nbsp;/&nbsp;
                <asp:DropDownList ID="ccexpyear" ClientIDMode="Static" runat="server" />
            </td>
        </tr>
        <tr id="securityCodeRow" runat="server" visible="false">
            <td class="hcLabel"><%=Localization.GetString("lblSecurityCode") %></td>
            <td class="formfield">
                <asp:TextBox ID="ccsecuritycode" ClientIDMode="Static" runat="server" Columns="5" MaxLength="4" />
            </td>
        </tr>
        <tr>
            <td class="hcLabel"><%=Localization.GetString("lblNameOnCard") %></td>
            <td class="formfield">
                <asp:TextBox ID="cccardholder" ClientIDMode="Static" runat="server" Columns="20" />
            </td>
        </tr>
    </table>
</div>
