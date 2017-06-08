<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.TaxProvider_Edit" Title="Untitled Page" CodeBehind="TaxProvider_Edit.aspx.cs" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/TaxProviderEditor.ascx" TagName="TaxProviderEditor" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
    <hcc:TaxProviderEditor ID="taxProviderEditor" runat="server" OnEditingComplete="taxProviderEditor_EditingComplete"/>
</asp:Content>

