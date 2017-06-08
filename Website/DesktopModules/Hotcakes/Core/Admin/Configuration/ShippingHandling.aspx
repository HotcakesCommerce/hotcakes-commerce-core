<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.ShippingHandling" CodeBehind="ShippingHandling.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=Localization.GetString("ShippingHandlingSettings") %></h1>
    <hcc:MessageBox ID="MessageBox1" runat="server" EnableViewState="false" />
    
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("HandlingFeeAmount") %></label>
                <asp:TextBox ID="HandlingFeeAmountTextBox" runat="server"/>
                <asp:RequiredFieldValidator ID="rfvHandlingFeeAmountTextBox" runat="server" ControlToValidate="HandlingFeeAmountTextBox" CssClass="hcFormError" Display="Dynamic"/>
                <asp:CustomValidator ID="cvHandlingFeeAmountTextBox" runat="server" ControlToValidate="HandlingFeeAmountTextBox" Display="Dynamic" CssClass="hcFormError" OnServerValidate="HandlingFeeAmountCustomValidator_ServerValidate"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("ApplyFee") %></label>
                <asp:RadioButtonList ID="HandlingRadioButtonList" runat="server"/>
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="NonShippingCheckBox" resourcekey="NonShippingCheckBox" runat="server" />
            </div>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" runat="server" CssClass="hcPrimaryAction" OnClick="btnSave_Click" />
        </li>
    </ul>

</asp:Content>