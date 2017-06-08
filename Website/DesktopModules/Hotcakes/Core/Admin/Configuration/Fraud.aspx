<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Fraud" title="Untitled Page" Codebehind="Fraud.aspx.cs" %>

<%@ Register src="../Controls/NavMenu.ascx" tagname="NavMenu" tagprefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <h1><%=Localization.GetString("FraudScreening") %></h1>
    <div class="hcForm">
        <div class="hcFormItem">
            <%=Localization.GetString("OnHoldMessage") %>
        </div>
    </div>
    <div class="hcColumnLeft" style="width: 50%;">
        <div class="hcForm">

            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Emails") %></label>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:TextBox ID="EmailField" runat="server" ValidationGroup="Email" CssClass="RadComboBox" /><span class="hcInset"><%=Localization.GetString("5Points") %></span>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:LinkButton ID="btnNewEmail" resourcekey="CreateNew" CssClass="hcPrimaryAction hcSmall" runat="server" onclick="btnNewEmail_Click" ValidationGroup="Email"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Added") %></label>
            </div>
            <div class="hcFormItem hcFormItemLeft">
                <asp:ListBox ID="lstEmail" runat="server" CssClass="hcSelectList RadComboBox" Rows="10" Width="140px" SelectionMode="Multiple" ValidationGroup="EmailDelete"/>
            </div>
            <div class="hcFormItem hcFormItemRight">
                <asp:LinkButton ID="btnDeleteEmail" resourcekey="Delete" CssClass="hcSecondaryAction hcSmall" runat="server" onclick="btnDeleteEmail_Click" ValidationGroup="EmailDelete"/>
            </div>
            
            <div class="hcFormItem">
                <hr/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("IPAddresses") %></label>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:TextBox ID="IPField" runat="server" ValidationGroup="IP" CssClass="RadComboBox" /><span class="hcInset"><%=Localization.GetString("1Point") %></span>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:LinkButton ID="btnNewIP" resourcekey="CreateNew" CssClass="hcPrimaryAction hcSmall" runat="server" onclick="btnNewIP_Click" ValidationGroup="IP"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Added") %></label>
            </div>
            <div class="hcFormItem hcFormItemLeft">
                <asp:ListBox ID="lstIP" runat="server" CssClass="hcSelectList RadComboBox" Rows="16" Width="140px" SelectionMode="Multiple" ValidationGroup="IPDelete"/>
            </div>
            <div class="hcFormItem hcFormItemRight">
                <asp:LinkButton ID="btnDeleteIP" resourcekey="Delete" CssClass="hcSecondaryAction hcSmall" runat="server" onclick="btnDeleteIP_Click" ValidationGroup="IPDelete"/>
            </div>
        
            <div class="hcFormItem">
                <hr/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("DomainNames") %></label>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:TextBox ID="DomainField" runat="server" ValidationGroup="Domain" CssClass="RadComboBox" /><span class="hcInset"><%=Localization.GetString("3Points") %></span>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:LinkButton ID="btnNewDomain" resourcekey="CreateNew" CssClass="hcPrimaryAction hcSmall" runat="server" onclick="btnNewDomain_Click" ValidationGroup="Domain"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Added") %></label>
            </div>
            <div class="hcFormItem hcFormItemLeft">
                <asp:ListBox ID="lstDomain" runat="server" CssClass="hcSelectList RadComboBox" Rows="16" Width="140px" SelectionMode="Multiple" ValidationGroup="DomainDelete"/>
            </div>
            <div class="hcFormItem hcFormItemRight">
                <asp:LinkButton ID="btnDeleteDomain" resourcekey="Delete" CssClass="hcSecondaryAction hcSmall" runat="server" onclick="btnDeleteDomain_Click" ValidationGroup="DomainDelete"/>
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("PhoneNumbers") %></label>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:TextBox ID="PhoneNumberField" runat="server" ValidationGroup="Phone" CssClass="RadComboBox" /><span class="hcInset"><%=Localization.GetString("3Points") %></span>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:LinkButton ID="btnNewPhoneNumber" resourcekey="CreateNew" CssClass="hcPrimaryAction hcSmall" runat="server" onclick="btnNewPhoneNumber_Click" ValidationGroup="Phone"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Added") %></label>
            </div>
            <div class="hcFormItem hcFormItemLeft">
                <asp:ListBox ID="lstPhoneNumber" runat="server" CssClass="hcSelectList RadComboBox" Rows="16" Width="140px" SelectionMode="Multiple" ValidationGroup="PhoneDelete"/>
            </div>
            <div class="hcFormItem hcFormItemRight">
                <asp:LinkButton ID="btnDeletePhoneNumber" resourcekey="Delete" CssClass="hcSecondaryAction hcSmall" runat="server" onclick="btnDeletePhoneNumber_Click" ValidationGroup="PhoneDelete"/>
            </div>
        
            <div class="hcFormItem">
                <hr/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("CreditCardNumbers") %></label>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:TextBox ID="CreditCardField" runat="server" ValidationGroup="CreditCard" CssClass="RadComboBox" /><span class="hcInset"><%=Localization.GetString("7Points") %></span>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:LinkButton ID="btnNewCCNumber" resourcekey="CreateNew" CssClass="hcPrimaryAction hcSmall" runat="server" onclick="btnNewCCNumber_Click" ValidationGroup="CreditCard"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Added") %></label>
            </div>
            <div class="hcFormItem hcFormItemLeft">
                <asp:ListBox ID="lstCreditCard" runat="server" CssClass="hcSelectList RadComboBox" Rows="16" Width="140px" SelectionMode="Multiple" ValidationGroup="CreditCardDelete"/>
            </div>
            <div class="hcFormItem hcFormItemRight">
                <asp:LinkButton ID="btnDeleteCCNumber" resourcekey="Delete" CssClass="hcSecondaryAction hcSmall" runat="server" onclick="btnDeleteCCNumber_Click" ValidationGroup="CreditCardDelete"/>
            </div>
        </div>
    </div>

</asp:Content>