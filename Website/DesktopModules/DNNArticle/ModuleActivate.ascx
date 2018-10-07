<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleActivate.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleActivate" %>
<asp:Panel ID="panelAutoActivate" runat="server">
    <table class="dnnFormItem">
        <tr>
            <td>
                <asp:Label ID="lblInvoiceNumber" runat="server" resourcekey="lblInvoiceNumber"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtInvoiceNumber" runat="server" Width="489px"></asp:TextBox>
                <asp:Button ID="btnAutoActivate" runat="server" class="StandardButton" resourcekey="btnAutoActivate"
                    OnClick="btnAutoActivate_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="panelManualActivate" runat="server" Visible="false">
    <table class="dnnFormItem">
        <tr>
            <td>
                <asp:Label ID="lblActivateURL" runat="server" resourcekey="lblActivateURL"></asp:Label>
            </td>
            <td>
                <asp:HyperLink ID="hlActivateURL" runat="server" resourcekey="hlActivateURL"></asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblActivateCode" runat="server" resourcekey="lblActivateCode"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtActivateCode" runat="server" TextMode="MultiLine" ReadOnly="true"
                    Rows="10" Columns="60" MaxLength="2000"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblLicenseCode" runat="server" resourcekey="lblLicenseCode"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtLicenseCode" runat="server" TextMode="MultiLine" Rows="10" Columns="60"
                    MaxLength="2000"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right">
                <asp:Button ID="btnMadeLicense" runat="server" class="StandardButton" resourcekey="btnMadeLicense"
                    OnClick="btnMadeLicense_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
<br />
<hr />
<table class="dnnFormItem">
    <tr>
        <td>
            <asp:Button ID="btnManualpage" runat="server" CssClass="StandardButton" resourcekey="btnManualpage"
                OnClick="btnManualpage_Click" />&nbsp;&nbsp;
            <asp:Button ID="btnCancel" runat="server" CssClass="StandardButton" resourcekey="btnCancel"
                OnClick="btnCancel_Click" />
        </td>
    </tr>
</table>
