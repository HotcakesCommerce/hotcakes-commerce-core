<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.OrderActions" CodeBehind="OrderActions.ascx.cs" %>
<script type="text/javascript">
    jQuery(function ($) {

        function doPrint() {
            if (window.print) {
                window.print();
            } else {
                alert('Please choose the print button from your browser.  Usually in the menu dropdowns at File: Print');
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
        <asp:HyperLink ID="lnkDetails" runat="server" Text="Order Details" />
    </li>
    <li class='<%=GetCurrentCssClass("Edit") %>'>
        <asp:HyperLink ID="lnkEditOrder" runat="server" Text="Edit Order" />
    </li>
    <li class='<%=GetCurrentCssClass("Payment") %>'>
        <asp:HyperLink ID="lnkPayment" runat="server" Text="Payment" />
    </li>
    <li class='<%=GetCurrentCssClass("Shipping") %>'>
        <asp:HyperLink ID="lnkShipping" runat="server" Text="Shipping" />
    </li>
</ul>
<%} %>
<%if(!HideActions){ %>
<div class="hcBlock" >
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:HyperLink ID="lnkPrintNow" runat="server" Text="Print" CssClass="hcTertiaryAction" />
        </div>
        <div class="hcFormItem">
            <asp:HyperLink ID="lnkPrint" runat="server" Text="Options" CssClass="hcTertiaryAction" />
        </div>
        <div class="hcFormItem">
            <asp:HyperLink ID="lnkManager" runat="server" Text="Close" CssClass="hcTertiaryAction" />
        </div>
    </div>
</div>
<%} %>