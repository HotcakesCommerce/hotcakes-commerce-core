<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.PaymentMethods.Moneris.Edit" CodeBehind="Edit.ascx.cs" %>
<h1>
    <asp:Label runat="server" resourcekey="MonerisOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HostedPayPageId" CssClass="hcLabel" />
        <asp:TextBox runat="server" ID="txtHostedPayPageId" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="HostedPayPageToken" CssClass="hcLabel" />
        <asp:TextBox runat="server" ID="txtHostedPayPageToken" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DeveloperMode" CssClass="hcLabel" />
        <asp:CheckBox runat="server" Id="chkDeveloperMode" />
    </div>
    <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="DebugMode" CssClass="hcLabel" />
        <asp:CheckBox runat="server" Id="chkDebugMode" />
    </div>
        <div class="hcFormItemHor">
        <asp:Label runat="server" resourcekey="AcceptDeclineURL" CssClass="hcLabel" />
        <asp:Label runat="server" ID="lblUrl" CssClass="hcFormItemValue" />
    </div>
</div>