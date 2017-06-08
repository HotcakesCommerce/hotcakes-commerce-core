<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryPerformance.ascx.cs" Inherits="Hotcakes.Modules.Core.Controls.CategoryPerformance" %>

<div class="hcCategoryPerformanceData">
    <input type="hidden" runat="server" id="txtCategorytId" class="hcCategoryId" />
    <div class="hcCategoryInfo">
        <asp:Label runat="server" resourcekey="CategoryName" CssClass="hcLabel" />
        <asp:Label runat="server" ID="lblCategoryName" />
        <asp:Label runat="server" resourcekey="LastModifiedOn" CssClass="hcLabel" />
        <asp:Label runat="server" ID="lblLastModifiedOn" />
        <asp:HyperLink runat="server" ID="btnAddCategory" resourcekey="AddCategory" CssClass="hcPrimaryAction hcSmall right" />
        <asp:HyperLink runat="server" ID="btnEditCategory" resourcekey="EditCategory" CssClass="hcPrimaryAction hcSmall right" />
    </div>

    <div class="hcChartArea">
        <div class="hcLeftChartArea">
            <div class="hcCategoryPerformanceChart">
				<canvas id="hcPerformanceChart" height="410" width="1058" style="height:410px; width:1058px;"></canvas>
            </div>
            <asp:DropDownList ID="ddlRowPeriod" runat="server" CssClass="hcPeriodSelector hcSelectionList">
                <asp:ListItem resourcekey="Range_Year" Value="Year" />
                <asp:ListItem resourcekey="Range_Quarter" Value="Quarter" />
                <asp:ListItem resourcekey="Range_Month" Value="Month" />
                <asp:ListItem resourcekey="Range_Week" Value="Week" />
            </asp:DropDownList>
        </div>

        <div class="hcSalesFunnel">
            <h3><%=Localization.GetString("SalesFunnel") %></h3>
            <div>
                <div class="hcSalesBlock">
                    <h4><%=Localization.GetString("Views") %></h4>
                    <span class="hcAmount" data-bind="html: Views"></span>
                    <span data-bind="css: { hcGrowingTrend: IsViewsGrowing, hcDroppingTrend: IsViewsGrowing() == false, hcTrendNoChange: IsViewsGrowing() == null }"></span>
                    <strong class="hcPercentageChange" data-bind="text: ViewsPercentageChange"></strong>
                    <span data-bind="text: ViewsComparison"></span>
                </div>
                <div class="hcSalesBlock">
                    <h4><%=Localization.GetString("AddedToCart") %></h4>
                    <span class="hcAmount" data-bind="html: AddsToCart"></span>
                    <span data-bind="css: { hcGrowingTrend: IsAddsToCartGrowing, hcDroppingTrend: IsAddsToCartGrowing() == false, hcTrendNoChange: IsAddsToCartGrowing() == null }"></span>
                    <strong class="hcPercentageChange" data-bind="text: AddsToCartPercentageChange"></strong>
                    <span data-bind="text: AddsToCartComparison"></span>
                </div>
                <div class="hcSalesBlock">
                    <h4><%=Localization.GetString("Purchased") %></h4>
                    <span class="hcAmount" data-bind="html: Purchases"></span>
                    <span data-bind="css: { hcGrowingTrend: IsPurchasesGrowing, hcDroppingTrend: IsPurchasesGrowing() == false, hcTrendNoChange: IsPurchasesGrowing() == null }"></span>
                    <strong class="hcPercentageChange" data-bind="text: PurchasesPercentageChange"></strong>
                    <span data-bind="text: PurchasesComparison"></span>
                </div>
            </div>
        </div>
    </div>
</div>
