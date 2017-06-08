<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.PaymentMethods.Ogone.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="OgoneOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="PaymentServiceProviderId" CssClass="hcLabel" />
        <asp:TextBox ID="txtPaymentServiceProviderId" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HashAlgorithm" CssClass="hcLabel" />
        <asp:DropDownList ID="ddlHashAlgorithm" runat="server" CssClass="hcDropdownAlign" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShaInPassPhrase" CssClass="hcLabel" />
        <asp:TextBox ID="txtShaInPassPhrase" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="ShaOutPassPhrase" CssClass="hcLabel" />
        <asp:TextBox ID="txtShaOutPassPhrase" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="TemplatePage" CssClass="hcLabel" />
        <asp:TextBox ID="txtTemplatePage" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DebugMode" AssociatedControlID="chkDebugMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDebugMode" runat="server" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DeveloperMode" AssociatedControlID="chkDeveloperMode" CssClass="hcLabel" />
        <asp:CheckBox ID="chkDeveloperMode" runat="server" />
    </div>
</div>
