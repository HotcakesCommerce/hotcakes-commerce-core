<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Dashboard.Dashboard"  %>

<script type="text/html" id="hcTopProductsTemplate">
    <table>
        <thead>
            <tr>
                <th>
                    <asp:Label runat="server" resourcekey="Product" />
                </th>
                <th>
                    <asp:HyperLink runat="server" CssClass="hcChangeOrder">
                        <%=Localization.GetString("Change") %>
                        <i data-bind="attr: { class: sortVisual() }"></i>
                    </asp:HyperLink>
                </th>
            </tr>
        </thead>
        <tbody>
            <!-- ko foreach: Items -->
            <tr>
                <td>
                    <a data-bind="html: ProductName, attr: { href: ProductUrl }" target="_blank" />
                </td>
                <td>
                    <!-- ko if: IsChangeGrowing() != null -->
                    <i data-bind="css: { hcGrowingTrend: IsChangeGrowing, hcDroppingTrend: !IsChangeGrowing() }"></i>
                    <!-- /ko -->
                    <span class="hcChange" data-bind="html: Change"></span>
                </td>
            </tr>
            <!-- /ko -->
        </tbody>
        <tfoot>
            <tr>
                <td colspan="2">
                    <div class="hcPageInfo">
                        <span data-bind="html: showFrom"></span>
                        -
                        <span data-bind="html: showTo"></span>
                        <%=Localization.GetString("Of") %>
                        <span data-bind="html: TotalCount" />
                    </div>
                    <div class="hcPageNav">
                        <a class="hcPrevPage hcIconLeft">Prev</a>
                        <a class="hcNextPage hcIconRight">Next</a>
                    </div>
                </td>
            </tr>
        </tfoot>
    </table>
</script>

<div runat="server" id="divGetStarted" class="hcBlockRow hcGetStartedRow">
    <div class="hcBlock">
        <div class="hcBlockContent hcGetStarted">
            <asp:Label runat="server" resourcekey="GetStarted" />
            <ul class="hcActions">
                <li>
                    <asp:HyperLink runat="server" resourcekey="AddSamples" CssClass="hcCreateSamples hcPrimaryAction hcSmall" />
                </li>
                <li>
                    <asp:Label runat="server" resourcekey="Or" />
                </li>
                <li>
                    <asp:HyperLink runat="server" resourcekey="AddProducts" CssClass="hcPrimaryAction hcSmall" NavigateUrl="../catalog/Products_Edit.aspx" />
                </li>
                <li>
                    <asp:HyperLink runat="server" resourcekey="AddCategories" CssClass="hcPrimaryAction hcSmall" NavigateUrl="../catalog/Categories_Edit.aspx" />
                </li>
            </ul>
        </div>
    </div>
</div>
<div runat="server" id="divSampleData" class="hcBlockRow hcSampleDataRow">
    <div class="hcBlock">
        <div class="hcBlockContent hcSampleData">
            <asp:Label runat="server" resourcekey="ViewingSampleData" />
            <asp:HyperLink runat="server" resourcekey="FindOutMore" CssClass="hcFindOutMore" />
        </div>
    </div>
</div>
<div class="hcBlockRow">
    <div class="hcBlock">
        <div class="hcBlockContent hcOrderSummary">
            <ul>
                <li>
                    <a href="./orders/default.aspx?mode=1">
                        <i class="hcDIconNew"></i><%=Localization.GetString("NewOrders") %><span><%=OrdersSummary.NewCount %></span>
                    </a>
                </li>
                <li>
                    <a href="./orders/default.aspx?mode=5">
                        <i class="hcDIconHold"></i><%=Localization.GetString("OnHold") %><span><%=OrdersSummary.OnHoldCount %></span>
                    </a>
                </li>
                <li>
                    <a href="./orders/default.aspx?mode=2">
                        <i class="hcDIconPayment"></i><%=Localization.GetString("ReadyForPayment") %><span><%=OrdersSummary.ReadyForPaymentCount %></span>
                    </a>
                </li>
                <li>
                    <a href="./orders/default.aspx?mode=3">
                        <i class="hcDIconShipping"></i><%=Localization.GetString("ReadyForShipping") %><span><%=OrdersSummary.ReadyForShippingCount %></span>
                    </a>
                </li>
            </ul>
        </div>
    </div>
</div>
<%if (ReviewsCount > 0)
  { %>
<div class="hcBlockRow">
    <div class="hcBlock">
        <div class="hcBlockContent hcModerateReviews">
            <a href="catalog/ReviewsToModerate.aspx">
                <i class="hcDIconReview"></i><%= Localization.GetFormattedString("ReviewsToModerate", ReviewsCount) %>
            </a>
        </div>
    </div>
</div>
<%} %>
<div class="hcBlockRow">
    <div class="hcBlock hcSalesBlock">
        <div class="hcBlockContent50p hcSalesChart">
            <h3><%=Localization.GetString("SalesOverTime") %> -</h3>
            <asp:DropDownList ID="ddlRow1Period" runat="server" CssClass="hcPeriodSelector hcSelectionList">
                <asp:ListItem resourcekey="Range_Year" Value="1" />
                <asp:ListItem resourcekey="Range_Quarter" Value="2" />
                <asp:ListItem resourcekey="Range_Month" Value="3" />
                <asp:ListItem resourcekey="Range_Week" Value="4" />
            </asp:DropDownList>
			<div id="hcPSalesPerformanceChart" style="width:750px; height:330px;">
				<canvas id="hcDashboardSalesOverTimeChart" class="" width="750" height="330"></canvas>
			</div>
        </div>

        <div class="hcBlockContent25p hcSalesTotal">
            <h3><%=Localization.GetString("SalesTotal") %></h3>
            <div class="hcAmountA">
                <%=GetCurrencyHtmlLabel() %><span data-bind="html: OrdersTotalSum"></span>
            </div>
            <div class="hcAmountB">
                <span data-bind="html: OrdersSuccessfulTransactions"></span>
                <label><%=Localization.GetString("Transactions") %></label>
            </div>
        </div>
        <div class="hcBlockContent25p hcSalesByDevice">
            <h3><%=Localization.GetString("SalesByDevice") %></h3>
            <ul>
                <li>
                    <i class="hcDIconDesktop" title="<%=Localization.GetString("Desktop") %>"></i>
                    <div class="hcAmount">
                        <span data-bind="html: SalesByDesktopCount"></span>
                        <label>
                            <i data-bind="css: { hcGrowingTrend: SalesByDesktopGrowing, hcDroppingTrend: SalesByDesktopGrowing() == false, hcTrendNoChange: SalesByDesktopGrowing() == null }"></i>
                            <%=Localization.GetString("Desktop") %> -
                            <span data-bind="html: SalesByDesktopComparison" />
                        </label>
                    </div>
                </li>
                <li>
                    <i class="hcDIconTablet" title="<%=Localization.GetString("Tablet") %>"></i>
                    <div class="hcAmount">
                        <span data-bind="html: SalesByTabletCount"></span>
                        <label>
                            <i data-bind="css: { hcGrowingTrend: SalesByTabletGrowing, hcDroppingTrend: SalesByTabletGrowing() == false, hcTrendNoChange: SalesByTabletGrowing() == null }"></i>
                            <%=Localization.GetString("Tablet") %> -
                            <span data-bind="html: SalesByTabletComparison" />
                        </label>
                    </div>
                </li>
                <li>
                    <i class="hcDIconPhone" title="<%=Localization.GetString("Phone") %>"></i>
                    <div class="hcAmount">
                        <span data-bind="html: SalesByPhoneCount"></span>
                        <label>
                            <i data-bind="css: { hcGrowingTrend: SalesByPhoneGrowing, hcDroppingTrend: SalesByPhoneGrowing() == false, hcTrendNoChange: SalesByPhoneGrowing() == null }"></i>
                            <%=Localization.GetString("Phone") %> -
                            <span data-bind="html: SalesByPhoneComparison" />
                        </label>
                    </div>
                </li>
            </ul>
        </div>
    </div>
</div>

<div class="hcBlockRow">
    <div class="hcBlock hcChartArea">
        <div class="hcBlockContent75p hcLeftChartArea">
            <h3><%=Localization.GetString("ProductPerformance") %> -</h3>
            <asp:DropDownList ID="ddlRow2Period" runat="server" CssClass="hcPeriodSelector hcSelectionList">
                <asp:ListItem resourcekey="Range_Year" Value="1" />
                <asp:ListItem resourcekey="Range_Quarter" Value="2" />
                <asp:ListItem resourcekey="Range_Month" Value="3" />
                <asp:ListItem resourcekey="Range_Week" Value="4" />
            </asp:DropDownList>
            <div id="hcPProductPerformanceChart"><canvas id="hcProductPerformanceChart" width="1150" height="340" style="width: 1150px; height:340px;"></canvas></div>
        </div>

        <div class="hcBlockContent25p hcSalesFunnel">
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
    <div class="hcBlock hcTopChangeBlock">
        <div class="hcBlockContent33p hcTopChangeByBounces hcReverseTrend">
            <h3><%=Localization.GetString("TopBouncedProducts") %></h3>
            <div data-bind="template: { name: 'hcTopProductsTemplate', data: $data }"></div>
        </div>
        <div class="hcBlockContent33p hcTopChangeByAbandoments hcReverseTrend">
            <h3><%=Localization.GetString("TopAbandonedProducts") %></h3>
            <div data-bind="template: { name: 'hcTopProductsTemplate', data: $data }"></div>
        </div>
        <div class="hcBlockContent33p hcTopChangeByPurchases">
            <h3><%=Localization.GetString("TopPurchasedProducts") %></h3>
            <div data-bind="template: { name: 'hcTopProductsTemplate', data: $data }"></div>
        </div>
    </div>
    <div class="hcBlock hcTopChangeJointBlock">
        <div class="hcBlockContent">
            <h3><%=Localization.GetString("MostAffectedProducts") %></h3>
            <table>
                <thead>
                    <tr>
                        <th>
                            <asp:Label runat="server" resourcekey="Product" />
                        </th>
                        <th>
                            <asp:HyperLink runat="server" resourcekey="Change" CssClass="hcChangeOrder hcByChange">
                                <%=Localization.GetString("Change") %>
                                <i data-bind="attr: { class: sortVisual('ByChange') }"></i>
                            </asp:HyperLink>
                        </th>
                        <th>
                            <asp:HyperLink runat="server" resourcekey="Bounce" CssClass="hcChangeOrder hcByBouncesChange">
                                <%=Localization.GetString("Bounce") %>
                                <i data-bind="attr: { class: sortVisual('ByBouncesChange') }"></i>
                            </asp:HyperLink>
                        </th>
                        <th>
                            <asp:HyperLink runat="server" resourcekey="Abandon" CssClass="hcChangeOrder hcByAbandomentsChange">
                                <%=Localization.GetString("Abandon") %>
                                <i data-bind="attr: { class: sortVisual('ByAbandomentsChange') }"></i>
                            </asp:HyperLink>
                        </th>
                        <th>
                            <asp:HyperLink runat="server" resourcekey="Complete" CssClass="hcChangeOrder hcByPurchasesChange">
                                <%=Localization.GetString("Complete") %>
                                <i data-bind="attr: { class: sortVisual('ByPurchasesChange') }"></i>
                            </asp:HyperLink>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <!-- ko foreach: Items -->
                    <tr>
                        <td>
                            <a data-bind="html: ProductName, attr: { href: ProductUrl }" target="_blank" />
                        </td>
                        <td>
                            <!-- ko if: IsChangeGrowing() != null -->
                            <i data-bind="css: { hcGrowingTrend: IsChangeGrowing, hcDroppingTrend: !IsChangeGrowing() }"></i>
                            <!-- /ko -->
                            <span class="hcChange" data-bind="html: Change"></span>
                        </td>
                        <td class="hcReverseTrend">
                            <!-- ko if: IsBouncesChangeGrowing() != null -->
                            <i data-bind="css: { hcGrowingTrend: IsBouncesChangeGrowing, hcDroppingTrend: !IsBouncesChangeGrowing() }"></i>
                            <!-- /ko -->
                            <span class="hcChange" data-bind="html: BouncesChange"></span>
                        </td>
                        <td class="hcReverseTrend">
                            <!-- ko if: IsAbandomentsChangeGrowing() != null -->
                            <i data-bind="css: { hcGrowingTrend: IsAbandomentsChangeGrowing, hcDroppingTrend: !IsAbandomentsChangeGrowing() }"></i>
                            <!-- /ko -->
                            <span class="hcChange" data-bind="html: AbandomentsChange"></span>
                        </td>
                        <td>
                            <!-- ko if: IsPurchasesChangeGrowing() != null -->
                            <i data-bind="css: { hcGrowingTrend: IsPurchasesChangeGrowing, hcDroppingTrend: !IsPurchasesChangeGrowing() }"></i>
                            <!-- /ko -->
                            <span class="hcChange" data-bind="html: PurchasesChange"></span>
                        </td>
                    </tr>
                    <!-- /ko -->
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="5">
                            <div class="hcPageInfo">
                                <span data-bind="html: showFrom"></span>
                                -
                                <span data-bind="html: showTo"></span>
                                <%=Localization.GetString("Of") %>
                                <span data-bind="html: TotalCount" />
                            </div>
                            <div class="hcPageNav">
                                <a class="hcPrevPage hcIconLeft">Prev</a>
                                <a class="hcNextPage hcIconRight">Next</a>
                            </div>
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </div>
</div>

<div runat="server" id="divSampleDataOverlay" class="hcSampleDataOverlay"></div>

<div class="hcFindOutMorePopup" data-title="<%=Localization.GetString("ViewingSampleDataInfoTitle") %>">
    <%=Localization.GetString("ViewingSampleDataInfo") %>
</div>

<div class="hcCreateSamplesPopup" data-title="<%=Localization.GetString("CreationSamplesInfoTitle") %>">
    <%=Localization.GetString("CreationSamplesInfo") %>
    <div class="hcActionsRight dnnClear">
        <ul class="hcActions">
            <li>
                <asp:LinkButton runat="server" ID="lnkAddSamples" resourcekey="Ok" CssClass="hcPrimaryAction hcSmall" OnClick="lnkAddSamples_Click" OnClientClick="DisableButton();return true;" />
            </li>
            <li>
                <asp:HyperLink runat="server" ID="lnkCancel" resourcekey="Cancel" CssClass="hcCancel hcSecondaryAction hcSmall" />
            </li>
        </ul>
    </div>
</div>

<script type="text/javascript">
    var AllowPostBack = true;
  
        $('body').on('click', 'a.disabled', function (event) {
            if (AllowPostBack == true) {
                AllowPostBack = false;
            }
            else {
                event.preventDefault();
            }
        });
    
    
        jQuery.fn.extend({
            disable: function (state) {
                return this.each(function () {
                    var $this = $(this);
                    $this.toggleClass('disabled', state);
                });
            }
        });
        function DisableButton() {
            var lnkBtnAdd = $("#<%= lnkAddSamples.ClientID %>");
            lnkBtnAdd.disable(true);
        }
    
</script>

