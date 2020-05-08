<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.PageConfiguration" Title="Untitled Page" CodeBehind="PageConfiguration.aspx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=Localization.GetString("PageConfiguration") %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcForm">
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label AssociatedControlID="txtCategoryUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("Category") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divCategoryUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtCategoryUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlCategoryTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label  AssociatedControlID="txtProductsUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("Products") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divProductsUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtProductsUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlProductsTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label  AssociatedControlID="txtCheckoutUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("Checkout") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divCheckoutUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtCheckoutUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlCheckoutTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label AssociatedControlID="txtAddressBookUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("AddressBook") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divAddressBookUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtAddressBookUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlAddressBookTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label AssociatedControlID="txtCartUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("Cart") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divCartUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtCartUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlCartTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label  AssociatedControlID="txtOrderHistoryUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("OrderHistory") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divOrderHistoryUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtOrderHistoryUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlOrderHistoryTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label  AssociatedControlID="txtProductReviewUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("ProductReview") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divProductReviewUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtProductReviewUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlProductReviewTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label  AssociatedControlID="txtSearchUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("Search") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divSearchUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtSearchUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlSearchTab" runat="server" />
        </div>
        <div class="hcFormItemLabel hcFormItem50p">
            <asp:Label AssociatedControlID="txtWishListUrl" runat="server" CssClass="hcLabel" ><%=Localization.GetString("WishList") %> <i class="hcLocalizable"></i></asp:Label>
        </div>
        <div runat="server" id="divWishListUrl" class="hcFormItem hcFormItem50p">
            <asp:TextBox ID="txtWishListUrl" runat="server" MaxLength="255" />
        </div>
        <div class="hcFormItem hcFormItem50p">
            <asp:DropDownList ID="ddlWishListTab" runat="server" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
        </li>
    </ul>
</asp:Content>

