$(function () {
    $.fn.productPerformanceBar = function () {
        return this.each(function () {
            var self = $(this);

            self.pcModel = null;
            self.productId = self.find(".hcProductId").val();
            self.chart = self.find(".hcViews .hcChart");
            self.chartCart = self.find(".hcCart .hcChart");
            self.chartPurchased = self.find(".hcPurchased .hcChart");

            self.viewMore = self.find(".hcViewMore");
            self.productPerformance = $(".hcProductPerformance");

            self.viewsPerformanceChart = self.find("#hcViewsChart");
            self.cartPerformanceChart = self.find("#hcCartChart");
            self.purchasedPerformanceChart = self.find("#hcPurchasedChart");

            self.periodSelector = $(".hcProductPerformanceData").find("select.hcPeriodSelector");

            self.bindProductPerformanceChartDataBar = function () {
                var self = this;
                var scope = self.find(".hcProductPerformanceDataBar");
                
                scope.ajaxLoader("start");
                $.post(hcc.getResourceUrl("Performance.ashx"),
                    {
                        "method": "GetProductPerformanceData",
                        "productId": self.productId,
                        "period": self.periodSelector.val()
                    },
                    function (data) {
                        if (self.pcModel) {
                            ko.mapping.fromJS(data, self.pcModel);
                        } else {
                            self.pcModel = ko.mapping.fromJS(data);
                            ko.applyBindings(self.pcModel, scope[0]);
                        }

                    	// self.loadChart(data);
                        self.loadProductPerformanceChart(data);
                    }).always(function () {
                        scope.ajaxLoader("stop");
                    });
            };

            self.loadProductPerformanceChart = function (performanceData) {
            	var chartOptions = {
            		responsive: false,
            		scales: {
            			display: false,
            			xAxes: [{
            				gridLines: {
            					display: false,
            					drawBorder: false,
            					drawOnChartArea: false,
            					zeroLineWidth: 0,
            					lineWidth: 0,
            				},
            				ticks: {
            					display: false,
            				},
            			}],
            			yAxes: [{
            				gridLines: {
            					display: false,
            					drawBorder: false,
            					drawOnChartArea: false,
            					zeroLineWidth: 0,
            					lineWidth: 0,
            				},
            				ticks: {
            					display: false,
            				},
            			}],
            		},
            		tooltips: {
						enabled: false,
            		},
            		legend: {
						display: false,
            		}
            	};


            	var viewsChartData = {
            		labels: performanceData.ChartLabels,
            		datasets: [
						{
							type: 'line',
							label: performanceData.BouncedName,
							backgroundColor: performanceData.IsViewsGrowing ? 'rgba(0, 127, 0, 1)' : 'rgba(127, 0, 0, 1)',
							lineTension: 0,
							pointRadius: 0,
							borderWidth: 0,
							data: performanceData.BouncedData
							
						}
					]
            	}
            	var viewsCtx = self.viewsPerformanceChart.get(0).getContext("2d");
            	var viewsChart = new Chart(viewsCtx, {
            		type: 'line',
            		data: viewsChartData,
            		options: chartOptions
            	});


            	var abandonedCartChartData = {
            		labels: performanceData.ChartLabels,
            		datasets: [
						{
							type: 'line',
							label: performanceData.AbandonedName,
							backgroundColor: performanceData.IsAddsToCartGrowing ? 'rgba(0, 127, 0, 1)' : 'rgba(127, 0, 0, 1)',
							lineTension: 0,
							pointRadius: 0,
							borderWidth: 0,
							data: performanceData.AbandonedData

						}
            		]
            	}
            	var abandonedCartCtx = self.cartPerformanceChart.get(0).getContext("2d");
            	var abandonedCartChart = new Chart(abandonedCartCtx, {
            		type: 'line',
            		data: abandonedCartChartData,
            		options: chartOptions
            	});

            	var purchasedChartData = {
            		labels: performanceData.ChartLabels,
            		datasets: [
						{
							type: 'line',
							label: performanceData.PurchasedName,
							backgroundColor: performanceData.IsPurchasesGrowing ? 'rgba(0, 127, 0, 1)' : 'rgba(127, 0, 0, 1)',
							lineTension: 0,
							pointRadius: 0,
							borderWidth: 0,
							data: performanceData.PurchasedData

						}
            		]
            	}
            	var purchasedCtx = self.purchasedPerformanceChart.get(0).getContext("2d");
            	var purchasedChart = new Chart(purchasedCtx, {
            		type: 'line',
            		data: purchasedChartData,
            		options: chartOptions
            	});



            };

            self.viewMoreClick = function (e) {
                var self = this;

                e.preventDefault();

                // using attr() instead of data() since we have css rules linked to that
                var state = self.viewMore.attr("data-state");
                if (state == "visible") {
                    self.viewMore.attr("data-state", "hidden");

                    $(".hcPerformance").animate(
                        { height: 'hide' },
                        300,
                        self.shiftFormPane);
                }
                else {
                    self.viewMore.attr("data-state", "visible");

                    $(".hcPerformance").animate(
                        { height: 'show' },
                        300,
                        self.shiftFormPane);
                    $(".ControlModulePanel").animate(
                        { height: 'hide' },
                        300,
                        self.shiftFormPane);

                    $(".hcPerformance").delay(300).productPerformance("resizeChart");
                }
            };

            self.shiftFormPane = function () {
                var panelHeight = $('#ControlBar_ControlPanel').height();
                $('#Form').css('margin-top', panelHeight);
                $(window).resize();
            }

            self.viewMore.click($.proxy(self.viewMoreClick, self));
            self.periodSelector.change($.proxy(self.bindProductPerformanceChartDataBar, self));

            self.productPerformance.hide();
            self.periodSelector.change();
        });
    }
    $(".hcProductPerformanceBar").productPerformanceBar();
});
