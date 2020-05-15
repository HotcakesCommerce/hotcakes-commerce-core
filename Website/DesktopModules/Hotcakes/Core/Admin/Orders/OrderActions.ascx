<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderActions" CodeBehind="OrderActions.ascx.cs" %>
<script type="text/javascript">
    jQuery(function ($) {

        function doPrint() {
            if (window.print) {
                window.print();
            } else {
                alert('<%=Localization.GetString("PrintAlert")%>');
            }
        }

        var $lnkPrint = $('#<%=lnkPrintNow.ClientID%>');
        $lnkPrint.click(function () {
            doPrint();
        });

        if (hcc.getUrlVar('autoprint') == "1") {
            doPrint();
        }
    });
</script>
<%if(!AreMultipleTemplates){ %>
<ul class="hcNavMenu">
    <li class='<%=GetCurrentCssClass("Details") %>'>
        <asp:HyperLink ID="lnkDetails" runat="server" resourcekey="btnOrderDetails" />
    </li>
    <li class='<%=GetCurrentCssClass("Edit") %>'>
        <asp:HyperLink ID="lnkEditOrder" runat="server" resourcekey="btnEditOrder" />
    </li>
    <li class='<%=GetCurrentCssClass("Payment") %>'>
        <asp:HyperLink ID="lnkPayment" runat="server" resourcekey="btnPayment" />
    </li>
    <li class='<%=GetCurrentCssClass("Shipping") %>'>
        <asp:HyperLink ID="lnkShipping" runat="server" resourcekey="btnShipping" />
    </li>
</ul>
<%} %>
<%if(!HideActions){ %>
<div class="hcBlock" >
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:HyperLink ID="lnkPrintNow" runat="server" resourcekey="lnkPrintNow" CssClass="hcTertiaryAction" />
        </div>
        <div class="hcFormItem">
            <asp:HyperLink ID="lnkPrint" runat="server" resourcekey="lnkPrint" CssClass="hcTertiaryAction" Target="_blank" />
        </div>
        <div class="hcFormItem">
            <asp:HyperLink ID="lnkManager" runat="server" resourcekey="lnkManager" CssClass="hcTertiaryAction" />
        </div>
    </div>
</div>
<%} %>