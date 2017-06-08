<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AffiliatePaymentDialog.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.AffiliatePaymentDialog" %>
<%@ Register Src="../Controls/AffiliatePaymentEditor.ascx" TagPrefix="hcc" TagName="AffiliatePaymentEditor" %>

<script type="text/javascript">
    var hcAddPaymentDialog = function () {
        $("#hcAddPaymentDialog").hcDialog({
            title: "<%= GetDialogTitle() %>",
            width: 500,
            height: 575,
            maxHeight: 600,
            parentElement: '#<%=pnlAddPayment.ClientID%>',
            close: function () {
                <%= Page.ClientScript.GetPostBackEventReference(ucPaymentEditor.CancelButton, "") %>
                }
            });
        };

</script>
<asp:Panel ID="pnlAddPayment" runat="server" Visible="false">
    <div id="hcAddPaymentDialog" class="dnnClear">
        <div class="hcForm">
            <hcc:affiliatepaymenteditor runat="server" id="ucPaymentEditor" />
        </div>
    </div>
</asp:Panel>
