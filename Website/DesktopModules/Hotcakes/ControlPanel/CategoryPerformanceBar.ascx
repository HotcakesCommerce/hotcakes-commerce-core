<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryPerformanceBar.ascx.cs" Inherits="Hotcakes.Modules.ControlPanel.CategoryPerformanceBar" %>

<div id="hcCategoryPerformanceBar" class="hcCategoryPerformanceBar">
    <div class="hcCategoryPerformanceDataBar">
        <input type="hidden" runat="server" id="txtCategoryId" class="hcCategoryId" />
        <ul>
            <li class="hcViews">
                <label><%=Localization.GetString("Views") %></label>
                <div class="hcChart"><canvas id="hcViewsChart" height="30" width="100" style="margin-top:10px; height:30px; width:100px;"></canvas></div>
                <span class="hcAmount" data-bind="html: Views"></span>
                <div class="hcTrendPercentageWrapper">
                    <span data-bind="css: { hcGrowingTrend: IsViewsGrowing, hcDroppingTrend: IsViewsGrowing() == false, hcTrendNoChange: IsViewsGrowing() == null }"></span>
                    <strong class="hcPercentageChange" data-bind="text: ViewsPercentageChange"></strong>
                </div>
            </li>
            <li class="hcCart">
                <label><%=Localization.GetString("AddedToCart") %></label>
                <div class="hcChart"><canvas id="hcCartChart" height="30" width="100" style="margin-top:10px; height:30px; width:100px;"></canvas></div>
                <span class="hcAmount" data-bind="html: AddsToCart"></span>
                <div class="hcTrendPercentageWrapper">
                    <span data-bind="css: { hcGrowingTrend: IsAddsToCartGrowing, hcDroppingTrend: IsAddsToCartGrowing() == false, hcTrendNoChange: IsAddsToCartGrowing() == null }"></span>
                    <strong class="hcPercentageChange" data-bind="text: AddsToCartPercentageChange"></strong>
                </div>
            </li>
            <li class="hcPurchased">
                <label><%=Localization.GetString("Purchased") %></label>
                <div class="hcChart"><canvas id="hcPurchasedChart" height="30" width="100" style="margin-top:10px; height:30px; width:100px;"></canvas></div>
                <span class="hcAmount" data-bind="html: Purchases"></span>
                <div class="hcTrendPercentageWrapper">
                    <span data-bind="css: { hcGrowingTrend: IsPurchasesGrowing, hcDroppingTrend: IsPurchasesGrowing() == false, hcTrendNoChange: IsPurchasesGrowing() == null }"></span>
                    <strong class="hcPercentageChange" data-bind="text: PurchasesPercentageChange"></strong>
                </div>
            </li>
            <li class="hcViewMore">
                <label><%=Localization.GetString("ViewMore") %></label>
                <a></a>
            </li>
        </ul>
    </div>
</div>

<div class="hcPerformance hcCategoryPerformance" style="display: none;">
    <asp:PlaceHolder ID="phrCategoryPerformanceView" runat="server" />
</div>
