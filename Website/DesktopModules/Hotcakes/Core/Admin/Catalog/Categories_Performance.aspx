<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories_Performance"
    Title="Untitled Page" CodeBehind="Categories_Performance.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/CategoryPerformance.ascx" TagName="CategoryPerformance" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="catalog/category" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="Categories.aspx" />
            </div>
        </div>
    </div>
    <div class="hcBlock hcBlockNotTopPadding">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkViewInStore" runat="server" CssClass="hcTertiaryAction" Target="_blank">View in Store</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcCategoryPerformance">
        <hcc:CategoryPerformance runat="server" ID="ucCategoryPerformance" />
    </div>
</asp:Content>
