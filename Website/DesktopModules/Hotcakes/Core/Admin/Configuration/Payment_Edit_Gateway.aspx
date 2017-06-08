<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Payment_Edit_Gateway" Title="Untitled Page" CodeBehind="Payment_Edit_Gateway.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CreditCardGatewayEditor.ascx" TagName="CreditCardGatewayEditor" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
    <hcc:CreditCardGatewayEditor ID="gatewayEditor" runat="server" OnEditingComplete="gatewayEditor_EditingComplete"/>
</asp:Content>

