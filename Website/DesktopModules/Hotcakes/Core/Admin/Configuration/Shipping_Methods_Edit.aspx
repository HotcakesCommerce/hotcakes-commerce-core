<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Shipping_EditMethod" Title="Untitled Page" CodeBehind="Shipping_Methods_Edit.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="NavMenu1" runat="server" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="hcClear">
        <asp:PlaceHolder ID="phEditor" runat="server"/>
    </div>
    <asp:HiddenField ID="BlockIDField" runat="server" />
</asp:Content>
