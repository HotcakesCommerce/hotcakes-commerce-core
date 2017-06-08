<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AffiliatePaymentEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.AffiliatePaymentEditor" %>
<%@ Register Src="MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<hcc:MessageBox ID="ucMessageBox" runat="server" UseDefaultValidationGroup="false" />

<div class="hcForm">
    <div id="divAffiliateIDRow" class="hcFormItemHor" runat="server">
        <label class="hcLabel">Affiliate ID</label>
        <asp:TextBox ID="txtAffiliateID" runat="server" />
        <asp:RequiredFieldValidator ErrorMessage="Please enter Affiliate ID." ControlToValidate="txtAffiliateID" CssClass="hcFormError"
            ValidationGroup="AffiliatePayment" runat="server" />
    </div>
    <div id="divAmountRow" class="hcFormItemHor" runat="server">
        <label class="hcLabel">Amount</label>
        <asp:TextBox ID="txtAmount" runat="server" />
        <asp:RequiredFieldValidator ErrorMessage="Please enter amount." ControlToValidate="txtAmount" CssClass="hcFormError"
            ValidationGroup="AffiliatePayment" runat="server" />
        <asp:CompareValidator ErrorMessage="Please enter correct currency value." ControlToValidate="txtAmount"
            Operator="DataTypeCheck" Type="Currency" CssClass="hcFormError" ValidationGroup="AffiliatePayment" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <label class="hcLabel">Memo</label>
        <asp:TextBox ID="txtMemo" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <label class="hcLabel">Attachment</label>
        <asp:FileUpload ID="fuAttachment" runat="server" />
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" Text="Save Changes" CssClass="hcPrimaryAction" ValidationGroup="AffiliatePayment" runat="server" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" />
    </li>
</ul>