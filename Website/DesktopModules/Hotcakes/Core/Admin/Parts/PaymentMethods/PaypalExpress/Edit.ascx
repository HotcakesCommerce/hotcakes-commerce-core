<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Modules.PaymentMethods.PaypalExpress.Edit" CodeBehind="Edit.ascx.cs" %>

<h1>
    <asp:Label runat="server" resourcekey="PaypalExpressOptions" />
</h1>
<div class="hcForm">
    <div class="hcFormItem">
        <asp:Label runat="server" resourcekey="PaypalMode" CssClass="hcLabel" />
        <asp:RadioButtonList runat="server" ID="lstMode" RepeatDirection="Horizontal">
            <asp:ListItem resourcekey="ProductionMode" Value="Live" Selected="True" />
            <asp:ListItem resourcekey="SandboxMode" Value="Sandbox" />
        </asp:RadioButtonList>
    </div>
</div>
<div class="hcColumnLeft" style="width: 50%;">
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="APIClientId" CssClass="hcLabel" />
            <asp:TextBox ID="txtClientId" runat="server" />
            <asp:RequiredFieldValidator ID="rfvClientId" runat="server" resourcekey="rfvClientId" ControlToValidate="txtClientId" Display="Dynamic" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem">
            <asp:Label runat="server" CssClass="hcLabel">
                <%=Localization.GetString("Currency") %>
                <i class="hcIconInfo">
                    <span class="hcFormInfo" style="display: none;"><%=Localization.GetString("CurrencyHelp") %></span>
                </i>
            </asp:Label>
            <asp:DropDownList runat="server" ID="ddlCurrency" />
        </div>
        <div class="hcFormItem">
            <asp:CheckBox ID="chkUnconfirmedAddress" runat="server" resourcekey="chkUnconfirmedAddress" />
        </div>
        <div class="hcFormItem">
            <asp:CheckBox ID="chkRequirePayPalAccount" runat="server" resourcekey="chkRequirePayPalAccount" />
        </div>
    </div>
</div>
<div class="hcColumnRight" style="width: 50%;">
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="APISecret" CssClass="hcLabel" />
            <asp:TextBox ID="txtSecret" runat="server" />
            <asp:RequiredFieldValidator ID="rfvSecret" runat="server" resourcekey="rfvSecret" ControlToValidate="txtSecret" Display="Dynamic" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="CaptureMode" CssClass="hcLabel" />
            <asp:RadioButtonList runat="server" ID="lstCaptureMode">
                <asp:ListItem resourcekey="CaptureModeAuthorize" Value="1" />
                <asp:ListItem resourcekey="CaptureModeCharge" Value="0" Selected="true"/>
            </asp:RadioButtonList>
        </div>
    </div>
</div>