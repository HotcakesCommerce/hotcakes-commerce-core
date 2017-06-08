<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentMethodEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.PaymentMethodEditor" %>
<%@ Register Src="MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<hcc:MessageBox ID="msg" runat="server" AddValidationSummaries="false" />
<asp:PlaceHolder runat="server" ID="phEditor" />
<ul runat="server" id="ulActions" class="hcActions">
    <li>
        <asp:LinkButton runat="server" ID="btnSave" resourcekey="btnSave" OnClick="btnSave_Click" CssClass="hcPrimaryAction" />
    </li>
    <li>
        <asp:LinkButton runat="server" ID="btnCancel" resourcekey="btnCancel" CausesValidation="false" OnClick="btnCancel_Click" CssClass="hcSecondaryAction" />
    </li>
</ul>
