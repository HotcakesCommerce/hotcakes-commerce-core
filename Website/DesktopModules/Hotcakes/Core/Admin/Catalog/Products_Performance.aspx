<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master"
    AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Performance"
    Title="Untitled Page" CodeBehind="Products_Performance.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/ProductPerformance.ascx" TagName="ProductPerformance" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" SelectedMenuItem="Performance" runat="server" />

    <div class="hcBlock hcBlockNotTopPadding">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkViewInStore" runat="server" CssClass="hcTertiaryAction" Target="_blank">View in Store</asp:HyperLink>
            </div>
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkBacktoAbandonedCartsReport" Text="Back to Abandoned Carts Report" CssClass="hcButton" Target="_self" runat="server" />
            </div>
        </div>
    </div>
    <hcc:ProductEditing ID="ucProductEditing" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcProductPerformance">
        <hcc:ProductPerformance runat="server" ID="ucProductPerformance" />
        <div class="hcProductPurchasedWith">
            <!-- ko if: $data.length -->
            <div class="hcHeader">
                <h3>
                    <asp:Label runat="server" resourcekey="PurchasedWith" /></h3>
                <asp:LinkButton runat="server" ID="btnCreateBundle" resourcekey="CreateBundle" CssClass="hcCreateBundle hcSecondaryAction hcSmall right" />
            </div>
            <div class="hcProductsBlock">
                <!-- ko foreach: $data -->
                <div class="hcForm hcProductBlock">
                    <div class="hcSection">
                        <input type="checkbox" name="createBundle" data-bind="attr: { 'data-productid': ProductId }" />
                        <div>
                            <img data-bind="attr: { src: ImageUrl, alt: ProductName }" />
                        </div>
                    </div>
                    <div class="hcSection">
                        <div class="hcSectionItem">
                            <span class="hcTextValue" data-bind="html: ProductName"></span>
                            <label class="hcLabel"><%=Localization.GetString("ProductName") %></label>
                        </div>
                        <div class="hcSectionItem">
                            <span class="hcTextValue" data-bind="html: UpdatedOn"></span>
                            <label class="hcLabel"><%=Localization.GetString("LastModifiedOn") %></label>
                        </div>
                        <div class="hcSectionItem">
                            <span class="hcTextValue" data-bind="html: CreatedOn"></span>
                            <label class="hcLabel"><%=Localization.GetString("CreatedOn") %></label>
                        </div>
                    </div>
                    <div class="hcSection">
                        <div class="hcSectionItem">
                            <span class="hcAmount" data-bind="html: QuantitySold"></span>
                            <span data-bind="css: { hcGrowingTrend: IsQuantitySoldGrowing, hcDroppingTrend: !IsQuantitySoldGrowing() }"></span>
                            <strong class="hcPercentageChange" data-bind="text: QuantitySoldPercentageChange"></strong>
                            <span data-bind="text: QuantitySoldComparison"></span>
                            <label class="hcLabel"><%=Localization.GetString("QuantitySold") %></label>
                        </div>
                        <div class="hcSectionItem">
                            <span class="hcAmount" data-bind="html: Revenue"></span>
                            <span data-bind="css: { hcGrowingTrend: IsRevenueGrowing, hcDroppingTrend: !IsRevenueGrowing() }"></span>
                            <strong class="hcPercentageChange" data-bind="text: RevenuePercentageChange"></strong>
                            <span data-bind="text: RevenueComparison"></span>
                            <label class="hcLabel"><%=Localization.GetString("Revenue") %></label>
                        </div>
                    </div>
                </div>
                <!-- /ko -->
            </div>
            <!-- /ko -->
        </div>
    </div>
</asp:Content>
