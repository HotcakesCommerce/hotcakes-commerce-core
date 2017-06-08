<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreditCardGatewayEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.CreditCardGatewayEditor" %>

<%@ Register Src="MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<hcc:MessageBox ID="msg" runat="server" AddValidationSummaries="false" />
<asp:PlaceHolder runat="server" ID="phEditor" />
<ul class="hcActions">
    <li>
        <asp:LinkButton runat="server" ID="btnSave" Text="Save Changes" OnClick="btnSave_Click" CssClass="hcPrimaryAction" />
    </li>
    <li>
        <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" CssClass="hcSecondaryAction" />
    </li>
</ul>
