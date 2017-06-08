<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="GiftCardConfiguration.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.GiftCardConfiguration" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/GiftCardGatewayEditor.ascx" TagName="GiftCardGatewayEditor" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();

        function hcEditGiftCardGatewayDialog() {
            $("#hcEditGiftCardGatewayDialog").hcDialog({
                title: $("#hcEditGiftCardGatewayDialog h1").text(),
                width: 800,
                height: 'auto',
                minHeight: 20,
                parentElement: '#<%=pnlEditGiftCardGateway.ClientID%>',
                open: function () {
                    $("#hcEditGiftCardGatewayDialog h1").remove();
                },
                close: function () {
                <%= Page.ClientScript.GetPostBackEventReference(btnCloseDialog, "") %>
            }
            });
        }
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcColumnLeft" style="width: 48%">
        <div class="hcForm">
            <h2><%=Localization.GetString("GeneralSettings") %></h2>
            <div class="hcFormItem">
                <asp:CheckBox ID="cbEnableGiftCards" resourcekey="cbEnableGiftCards" runat="server" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("ExpirationPeriod") %></label>
                <asp:TextBox ID="txtExpiration" Text="12" runat="server" />
                <asp:RequiredFieldValidator ID="rfvExpiration" ControlToValidate="txtExpiration" Display="Dynamic" CssClass="hcFormError" runat="server" />
                <asp:CompareValidator ID="cvExpiration" ControlToValidate="txtExpiration" Display="Dynamic" CssClass="hcFormError"
                    Type="Integer" ValueToCompare="1" Operator="GreaterThanEqual" runat="server" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel">
                    <%=Localization.GetString("GiftCardFormat") %>
                </label>
                <asp:TextBox ID="txtCardNumberFormat" resourcekey="txtCardNumberFormat" MaxLength="50" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="cbUseSymbols" resourcekey="cbUseSymbols" runat="server" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("MinimumAmount") %></label>
                <asp:TextBox ID="txtMinAmount" Text="50" runat="server" MaxLength="10" />
                <asp:RequiredFieldValidator ID="rfvMinAmount" ControlToValidate="txtMinAmount" Display="Dynamic" CssClass="hcFormError" runat="server" />
                <asp:CompareValidator ID="cvMinAmount" ControlToValidate="txtMinAmount" Display="Dynamic" CssClass="hcFormError"
                    Type="Double" ValueToCompare="0" Operator="GreaterThanEqual" runat="server" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("MaximumAmount") %></label>
                <asp:TextBox ID="txtMaxAmount" Text="10000" runat="server" MaxLength="10" />
                <asp:RequiredFieldValidator ID="rfvMaxAmount" ControlToValidate="txtMaxAmount" Display="Dynamic" CssClass="hcFormError" runat="server" />
                <asp:CompareValidator ID="cvMaxAmount" ControlToValidate="txtMaxAmount" Display="Dynamic" CssClass="hcFormError"
                    Type="Double" ValueToCompare="0" Operator="GreaterThanEqual" runat="server" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel">
                    <%=Localization.GetString("PredefinedAmounts") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("CommaSeparated") %>: 25, 100, 500</span>
                    </i>
                </label>
                <asp:TextBox ID="txtAmounts" Text="25,50,100,250,500" runat="server" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Gateway") %></h2>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="CaptureMode" CssClass="hcLabel" />
                <asp:RadioButtonList runat="server" ID="lstCaptureMode" />
            </div>
            <div class="hcFormItemLabel">
                <asp:Label runat="server" resourcekey="Gateway" CssClass="hcLabel" />
            </div>
            <div class="hcFormItem hcFormItem66p">
                <asp:DropDownList ID="lstGateway" runat="server" CssClass="RadComboBox" />
                <asp:RequiredFieldValidator ID="rfvGateway" runat="server" ControlToValidate="lstGateway" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:LinkButton runat="server" ID="btnOptions" resourcekey="btnOptions" OnClick="btnOptions_Click" CssClass="hcSecondaryAction hcSmall" />
            </div>
            <div class="hcFormItem">
                <h2><%=Localization.GetString("CardNumberFormat") %></h2>
                <p><%=Localization.GetString("CardNumberFormat1") %></p>
                <p><%=Localization.GetString("CardNumberFormat2") %></p>
                <p><%=Localization.GetString("CardNumberFormat3") %></p>
            </div>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" />
        </li>
    </ul>

    <asp:Panel id="pnlEditGiftCardGateway" runat="server">
        <div id="hcEditGiftCardGatewayDialog" style="display:none;">
            <hcc:GiftCardGatewayEditor runat="server" ID="gatewayEditor" OnEditingComplete="gatewayEditor_EditingComplete" />
            <asp:LinkButton ID="btnCloseDialog" Style="display: none" runat="server" />
        </div>
    </asp:Panel>

</asp:Content>
