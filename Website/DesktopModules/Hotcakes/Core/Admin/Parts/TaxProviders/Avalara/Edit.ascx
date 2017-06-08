<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.TaxProviders.Avalara.Edit" CodeBehind="Edit.ascx.cs" %>
<%@ Register Src="../../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<h1>
    <asp:Label ID="Label9" runat="server" resourcekey="AvalaraOptions" />
</h1>
<hcc:MessageBox ID="msg" runat="server" />
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label ID="Label2" resourcekey="AccountNumber" AssociatedControlID="txtAccount" runat="server" CssClass="hcLabel" />
        <asp:TextBox ID="txtAccount" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="Label3" resourcekey="LicenseKey" AssociatedControlID="txtLicenseKey" runat="server" CssClass="hcLabel" />
        <asp:TextBox ID="txtLicenseKey" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="Label4" runat="server" CssClass="hcLabel" AssociatedControlID="txtCompanyCode">
                <%=Localization.GetString("CompanyCode") %>
                <i class="hcIconInfo">
                    <span class="hcFormInfo" style="display: none;"><%=Localization.GetString("CompanyCodeHelp") %></span>
                </i>
        </asp:Label>
        <asp:TextBox ID="txtCompanyCode" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="Label5" resourcekey="Url" AssociatedControlID="txtUrl" runat="server" CssClass="hcLabel" />
        <asp:TextBox ID="txtUrl" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="Label6" resourcekey="ShippingTaxCode" AssociatedControlID="txtShippingTaxCode" runat="server" CssClass="hcLabel" />
        <asp:TextBox ID="txtShippingTaxCode" runat="server" />
    </div>
    <div class="hcFormItemHor" style="display: none;">
        <asp:Label ID="Label7" resourcekey="TaxExemptCode" AssociatedControlID="txtTaxExemptCode" runat="server" CssClass="hcLabel" />
        <asp:TextBox ID="txtTaxExemptCode" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="Label8" resourcekey="DebugMode" AssociatedControlID="chkDebug" runat="server" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDebug" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label ID="lblTestConnection" resourcekey="TestConnectionLabel" AssociatedControlID="lnkTestConnection" runat="server" CssClass="hcLabel" />
        <div style="float: left; margin-top: 5px;">
            <asp:LinkButton resourcekey="TestConnectionLink" runat="server" ID="lnkTestConnection" OnClick="lnkTestConnection_Click" />
        </div>
    </div>
    <div class="hcFormItemHor">
        <a href="https://admin-avatax.avalara.net/" target="_blank"><%=Localization.GetString("GoToAvatax") %></a>
    </div>
</div>

