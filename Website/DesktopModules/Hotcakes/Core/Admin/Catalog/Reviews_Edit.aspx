<%@ Page MasterPageFile="../AdminNav.master"  ValidateRequest="False" Language="C#"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Reviews_Edit" Codebehind="Reviews_Edit.aspx.cs" %>
<%@ Register TagPrefix="hcc" TagName="ProductReviewEditor" Src="../controls/ProductReviewEditor.ascx" %>
<%@ Register src="../Controls/ProductEditMenu.ascx" tagname="ProductEditMenu" tagprefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="ProductReviews" />
</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:ProductReviewEditor ID="ProductReviewEditor1" runat="server"/>
</asp:Content>
