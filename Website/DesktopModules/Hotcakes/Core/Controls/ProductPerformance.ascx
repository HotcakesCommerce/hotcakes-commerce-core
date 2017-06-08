<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductPerformance.ascx.cs" Inherits="Hotcakes.Modules.Core.Controls.ProductPerformance" %>

<div class="hcProductPerformanceData">
    <input type="hidden" runat="server" id="txtProductId" class="hcProductId" />
    <div class="hcProductInfo dnnClear">
        <div class="hcLabelWrapper">
            <asp:Label runat="server" resourcekey="ProductName" CssClass="hcLabel" />
            <asp:Label runat="server" ID="lblProductName" />
            <asp:Label runat="server" resourcekey="LastModifiedOn" CssClass="hcLabel" />
            <asp:Label runat="server" ID="lblLastModifiedOn" />
            <asp:Label runat="server" resourcekey="CreatedOn" CssClass="hcLabel" />
            <asp:Label runat="server" ID="lblCreatedOn" />
        </div>
        <asp:HyperLink runat="server" ID="btnAddProduct" resourcekey="AddProduct" CssClass="hcPrimaryAction hcSmall right" />
        <asp:HyperLink runat="server" ID="btnEditProduct" resourcekey="EditProduct" CssClass="hcPrimaryAction hcSmall right" />
    </div>

    <div class="hcChartArea">
        <div class="hcLeftChartArea">
			<div class="hcProductPerformanceChart" >
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
