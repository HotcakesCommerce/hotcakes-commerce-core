<%@ Page Language="C#" MasterPageFile="../AdminNav_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Default" title="Untitled Page" Codebehind="Default.aspx.cs" %>
<%@ Register src="NavMenu.ascx" tagname="NavMenu" tagprefix="uc2" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <uc2:NavMenu ID="NavMenu1" Path="Item[@Text='Settings']" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>&laquo;Pick an Option</h1>

</asp:Content>

