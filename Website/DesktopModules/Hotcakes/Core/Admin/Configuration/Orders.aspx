<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Orders" Title="Untitled Page" CodeBehind="Orders.aspx.cs" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Orders") %></h2>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkZeroDollarOrders" resourcekey="chkZeroDollarOrders" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkForceSiteTerms" resourcekey="chkForceSiteTerms" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox runat="server" ID="chkRequirePhoneNumber" resourcekey="chkRequirePhoneNumber"/>
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkUseChildChoicesAdjustmentsForBundles" resourcekey="chkUseChildChoicesAdjustmentsForBundles" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="MaximumOrderQuantity" CssClass="hcLabel" />
                <asp:TextBox ID="txtOrderLimitQuantity" runat="server" />
                <asp:CompareValidator runat="server" ID="cvOrderLimitQuantity" resourcekey="cvOrderLimitQuantity" ControlToValidate="txtOrderLimitQuantity" Operator="GreaterThan" Type="Integer" ValueToCompare="0" Display="Dynamic" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="MaximumOrderWeight" CssClass="hcLabel" />
                <asp:TextBox ID="txtOrderLimitWeight" runat="server" MaxLength="10" />
                <asp:RequiredFieldValidator ID="rfvOrderLimitWeight" runat="server" resourcekey="rfvOrderLimitWeight" ControlToValidate="txtOrderLimitWeight" Display="Dynamic" CssClass="hcFormError" />
                <asp:CompareValidator ID="cvOrderLimitWeight" runat="server" resourcekey="cvOrderLimitWeight" ControlToValidate="txtOrderLimitWeight" Operator="GreaterThan" Type="Double" ValueToCompare="0" Display="Dynamic" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="LastOrderNumber" CssClass="hcLabel" />
                <asp:TextBox ID="txtLastOrderNumber" runat="server" />
                <asp:RequiredFieldValidator ID="rfvLastOrderNumber" runat="server" resourcekey="rfvLastOrderNumber" ControlToValidate="txtLastOrderNumber" Display="Dynamic" CssClass="hcFormError" />
                <asp:CompareValidator ID="cvLastOrderNumber" runat="server" resourcekey="cvLastOrderNumber" ControlToValidate="txtLastOrderNumber" Operator="GreaterThanEqual" Type="Integer" ValueToCompare="0" Display="Dynamic" CssClass="hcFormError" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Products") %></h2>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkSwatches" resourcekey="chkSwatches" runat="server" />
            </div>
            <div class="hcClearfix"></div>
            <h2><%=Localization.GetString("Cart") %></h2>
            <div class="hcFormItem">
                <label class="hcLabel">
                    <%=Localization.GetString("ShoppingCartStorage") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("ShoppingCartStorageHelp") %></span>
                    </i>
                </label>
                <asp:DropDownList ID="ddlShoppingCartStore" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkSendAbandonedCartEmails" resourcekey="chkSendAbandonedCartEmails" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="SendAbandonedEmailIn" CssClass="hcLabel" />
                <asp:TextBox ID="txtSendAbandonedEmailIn" runat="server" MaxLength="3" />
                <asp:RequiredFieldValidator ID="rfvSendAbandonedEmailIn" runat="server" resourcekey="rfvSendAbandonedEmailIn" ControlToValidate="txtSendAbandonedEmailIn" Display="Dynamic" CssClass="hcFormError" />
				<asp:RangeValidator ID="rvSendAbandoneEmailIn" ValidationExpression="\d+" Display="Static" EnableClientScript="true" runat="server" Type="Integer" MinimumValue="1" MaximumValue="365" ControlToValidate="txtSendAbandonedEmailIn" CssClass="hcFormError"  resourcekey="cvSendAbandonedEmailIn"></asp:RangeValidator>
            </div>
            <h2><%=Localization.GetString("QuickBooks") %></h2>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="OrderAccount" CssClass="hcLabel" />
                <asp:TextBox ID="txtQuickbooksOrderAccount" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="ShippingAccount" CssClass="hcLabel" />
                <asp:TextBox ID="txtQuickbooksShippingAccount" runat="server" />
            </div>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton runat="server" ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" OnClick="btnSave_Click" />
        </li>
    </ul>
</asp:Content>

