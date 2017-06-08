<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.ProductClone" Title="Clone Product" CodeBehind="ProductClone.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">

    <h1><%=this.PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcForm">
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Sku" CssClass="hcLabel" />
            <asp:TextBox ID="txtSku" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Slug" CssClass="hcLabel" />
            <asp:TextBox ID="txtSlug" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Clone Product Roles" AssociatedControlID="chkProductRoles" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkProductRoles" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Clone Product Choices" AssociatedControlID="chkProductChoices" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkProductChoices" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Clone Product Images" AssociatedControlID="chkImages" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkImages" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Clone Category Placement" AssociatedControlID="chkCategoryPlacement" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkCategoryPlacement" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Clone Product Reviews" AssociatedControlID="chkReviews" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkReviews" runat="server" />
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" Text="Create As Inactive" AssociatedControlID="chkInactive" CssClass="hcLabel" />
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkInactive" runat="server" />
                <span></span>
            </div>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnClone" Text="Clone Product" CssClass="hcPrimaryAction" runat="server" OnClick="btnClone_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" Text="Cancel" CssClass="hcSecondaryAction" runat="server" CausesValidation="false" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>

