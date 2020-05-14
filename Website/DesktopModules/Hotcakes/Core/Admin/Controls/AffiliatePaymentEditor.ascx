<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AffiliatePaymentEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.AffiliatePaymentEditor" %>
<%@ Register Src="MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<hcc:MessageBox ID="ucMessageBox" runat="server" UseDefaultValidationGroup="false" />

<div class="hcForm">
    <div id="divAffiliateIDRow" class="hcFormItemHor" runat="server">
        <label class="hcLabel"><%=Localization.GetString("lblAffiliateId") %></label>
        <asp:TextBox ID="txtAffiliateID" runat="server" />
        <asp:RequiredFieldValidator resourcekey="rfvAffiliateId" ControlToValidate="txtAffiliateID" CssClass="hcFormError" ValidationGroup="AffiliatePayment" runat="server" />
    </div>
    <div id="divAmountRow" class="hcFormItemHor" runat="server">
        <label class="hcLabel"><%=Localization.GetString("lblAmount") %></label>
        <asp:TextBox ID="txtAmount" runat="server" />
        <asp:RequiredFieldValidator resourcekey="rfvAmount" ControlToValidate="txtAmount" CssClass="hcFormError" ValidationGroup="AffiliatePayment" runat="server" />
        <asp:CompareValidator resourcekey="cvAmount" ControlToValidate="txtAmount" Operator="DataTypeCheck" Type="Currency" CssClass="hcFormError" ValidationGroup="AffiliatePayment" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <label class="hcLabel"><%=Localization.GetString("lblMemo") %></label>
        <asp:TextBox ID="txtMemo" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <label class="hcLabel"><%=Localization.GetString("lblAttachment") %></label>
        <asp:FileUpload ID="fuAttachment" runat="server" />
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" ValidationGroup="AffiliatePayment" runat="server" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" />
    </li>
</ul>