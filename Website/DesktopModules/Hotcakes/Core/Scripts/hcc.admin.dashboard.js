$(function () {
	$.fn.adminDashboard = function () {
        return this.each(function () {

            var topChangeViewModel = function () {
                var self = this;

                self.isInitialized = false;

                self.pageSize = ko.observable(5);
                self.pageNumber = ko.observable(1);
                self.sortDirection = ko.observable("Descending");

                self.showFrom = ko.computed(function () {
                    return self.pageSize() * (self.pageNumber() - 1) + 1;
                });
                self.showTo = ko.computed(function () {
                    return self.pageSize() * self.pageNumber();
                });

                self.reverseSortDirection = function () {
                    switch (self.sortDirection()) {
                        case "Descending":
                            self.sortDirection("Ascending");
                            break;
                        case "Ascending":
                            self.sortDirection("Descending");
                            break;
                    }
                };

                self.sortVisual = function () {
                    if (self.sortDirection() == "Descending")
                        return "hcIconArrowDown";
                    else
                        return "hcIconArrowUp";
                };
            };

            var topChangeJointViewModel = function () {
                var self = this;

                self.isInitialized = false;

                self.pageSize = ko.observable(5);
                self.pageNumber = ko.observable(1);
                self.sortBy = ko.observable("ByChange");
                self.sortDirection = ko.observable("Descending");

                self.showFrom = ko.computed(function () {
                    return self.pageSize() * (self.pageNumber() - 1) + 1;
                });
                self.showTo = ko.computed(function () {
                    return self.pageSize() * self.pageNumber();
                });

                self.reverseSortDirection = function () {
                    switch (self.sortDirection()) {
                        case "Descending":
                            self.sortDirection("Ascending");
                            break;
                        case "Ascending":
                            self.sortDirection("Descending");
                            break;
                    }
                };

                self.sortVisual = function (sortBy) {
                    if (sortBy == self.sortBy()) {
                        if (self.sortDirection() == "Descending")
                            return "hcIconArrowDown";
                        else
                            return "hcIconArrowUp";
                    }
                    return "";
                };
            };

            var self = $(this);

            self.slModel = null;
            self.pfModel = null;
            self.tcbModel = new topChangeViewModel();
            self.tcaModel = new topChangeViewModel();
            self.tcpModel = new topChangeViewModel();
            self.tcjModel = new topChangeJointViewModel();
            self.hcCreateSamples = self.find(".hcCreateSamples");
            self.hcCreateSamplesPopup = self.find(".hcCreateSamplesPopup");
            self.hcCreateSamplesPopupCancel = self.find(".hcCreateSamplesPopup .hcCancel");
            self.hcFindOutMore = self.find(".hcFindOutMore");
            self.hcFindOutMorePopup = self.find(".hcFindOutMorePopup");
            self.hcSalesBlock = self.find(".hcSalesBlock");
            self.hcTopChangeBlock = self.find(".hcTopChangeBlock");
            self.hcTopChangeByBouncesBlock = self.find(".hcTopChangeByBounces");
            self.hcTopChangeByAbandomentsBlock = self.find(".hcTopChangeByAbandoments");
            self.hcTopChangeByPurchasesBlock = self.find(".hcTopChangeByPurchases");
            self.hcTopChangeJointBlock = self.find(".hcTopChangeJointBlock");
            self.slPeriod = self.find(".hcSalesBlock select.hcPeriodSelector");
            self.pfPeriod = self.find(".hcChartArea select.hcPeriodSelector");
            self.salesChart = self.find(".hcDashboardChart");
            self.performanceChart = self.find(".hcProductPerformanceChart");
            self.productPerformanceChart = self.find("#hcProductPerformanceChart");

            self.salesChartParent = self.find("#hcPSalesPerformanceChart");


            self.createSamples = function () {
                self.hcCreateSamplesPopup.hcDialog('open');
            }

            self.createSamplesCancel = function () {
                self.hcCreateSamplesPopup.hcDialog('close');
            }

            self.findOutMore = function () {
                self.hcFindOutMorePopup.hcDialog('open');
            }

            self.bindSalesBlock = function () {
            	var scope = self.hcSalesBlock;
                scope.ajaxLoader("start");

                $.post(hcc.getResourceUrl("Admin/Dashboard/DashboardHandler.ashx"),
                {
                    "method": "GetSalesData",
                    "period": self.slPeriod.val()
                }, function (data) {
                    if (self.slModel) {
                        ko.mapping.fromJS(data, self.slModel);
                    } else {
                        self.slModel = ko.mapping.fromJS(data);
                        ko.applyBindings(self.slModel, scope[0]);
                    }
                    self.loadSalesChart(data.ChartData, data.ChartLabels);
                }).always(function () { scope.ajaxLoader("stop"); });
            };

            self.bindPerformanceData = function () {
                var scope = self.find(".hcChartArea");
                scope.ajaxLoader("start");

                $.post(hcc.getResourceUrl("Admin/Dashboard/DashboardHandler.ashx"),
                    {
                        "method": "GetProductPerformanceData",
                        "period": self.pfPeriod.val()
                    },
                    function (data) {
                        if (self.pfModel) {
                            ko.mapping.fromJS(data, self.pfModel);
                        } else {
                            self.pfModel = ko.mapping.fromJS(data);
                            ko.applyBindings(self.pfModel, scope[0]);
                        }

                        self.loadProductPerformanceChart(data);
                        //self.loadPerformanceChart(data);
                    }).always(function () {
                        scope.ajaxLoader("stop");
                    });
            };

            self.bindTopChangeByBouncesData = function () {
                var scope = self.hcTopChangeByBouncesBlock;
                scope.ajaxLoader("start");

                $.post(hcc.getResourceUrl("Admin/Dashboard/DashboardHandler.ashx"),
                    {
                        "method": "GetTopChangeByBouncesData",
                        "period": self.pfPeriod.val(),
                        "sortDirection": self.tcbModel.sortDirection(),
                        "pageNumber": self.tcbModel.pageNumber(),
                        "pageSize": self.tcjModel.pageSize(),
                    },
                    function (data) {
                        ko.mapping.fromJS(data, {}, self.tcbModel);
                        if (!self.tcbModel.isInitialized) {
                            self.tcbModel.isInitialized = true;
                            ko.applyBindings(self.tcbModel, scope[0]);
                        }
                    }).always(function () {
                        scope.ajaxLoader("stop");
                    });
            };

            self.bindTopChangeByAbandomentsData = function () {
                var scope = self.hcTopChangeByAbandomentsBlock;
                scope.ajaxLoader("start");

                $.post(hcc.getResourceUrl("Admin/Dashboard/DashboardHandler.ashx"),
                    {
                        "method": "GetTopChangeByAbandomentsData",
                        "period": self.pfPeriod.val(),
                        "sortDirection": self.tcaModel.sortDirection(),
                        "pageNumber": self.tcaModel.pageNumber(),
                        "pageSize": self.tcaModel.pageSize(),
                    },
                    function (data) {
                        ko.mapping.fromJS(data, {}, self.tcaModel);
                        if (!self.tcaModel.isInitialized) {
                            self.tcaModel.isInitialized = true;
                            ko.applyBindings(self.tcaModel, scope[0]);
                        }
                    }).always(function () {
                        scope.ajaxLoader("stop");
                    });
            };

            self.bindTopChangeByPurchasesData = function () {
                var scope = self.hcTopChangeByPurchasesBlock;
                scope.ajaxLoader("start");

                $.post(hcc.getResourceUrl("Admin/Dashboard/DashboardHandler.ashx"),
                    {
                        "method": "GetTopChangeByPurchasesData",
                        "period": self.pfPeriod.val(),
                        "sortDirection": self.tcpModel.sortDirection(),
                        "pageNumber": self.tcpModel.pageNumber(),
                        "pageSize": self.tcpModel.pageSize(),
                    },
                    function (data) {
                        ko.mapping.fromJS(data, {}, self.tcpModel);
                        if (!self.tcpModel.isInitialized) {
                            self.tcpModel.isInitialized = true;
                            ko.applyBindings(self.tcpModel, scope[0]);
                        }
                    }).always(function () {
                        scope.ajaxLoader("stop");
                    });
            };

            self.bindTopChangeJointData = function () {
                var scope = self.hcTopChangeJointBlock;
                scope.ajaxLoader("start");

                $.post(hcc.getResourceUrl("Admin/Dashboard/DashboardHandler.ashx"),
                    {
                        "method": "GetTopChangeJointData",
                        "period": self.pfPeriod.val(),
                        "sortBy": self.tcjModel.sortBy(),
                        "sortDirection": self.tcjModel.sortDirection(),
                        "pageNumber": self.tcjModel.pageNumber(),
                        "pageSize": self.tcjModel.pageSize(),
                    },
                    function (data) {
                        ko.mapping.fromJS(data, {}, self.tcjModel);
                        if (!self.tcjModel.isInitialized) {
                            self.tcjModel.isInitialized = true;
                            ko.applyBindings(self.tcjModel, scope[0]);
                        }
                    }).always(function () {
                        scope.ajaxLoader("stop");
                    });
            };

            self.prevBouncesPageClick = function (e) {
                e.preventDefault();

                if (self.tcbModel.pageNumber() > 1)
                    self.tcbModel.pageNumber(self.tcbModel.pageNumber() - 1);

                self.bindTopChangeByBouncesData();
            };
            self.nextBouncesPageClick = function (e) {
                e.preventDefault();

                if (self.tcbModel.pageNumber() < Math.ceil(self.tcbModel.TotalCount() / self.tcbModel.pageSize()))
                    self.tcbModel.pageNumber(self.tcbModel.pageNumber() + 1);

                self.bindTopChangeByBouncesData();
            };
            self.changeBouncesOrder = function (e) {
                e.preventDefault();

                self.tcbModel.reverseSortDirection();

                self.bindTopChangeByBouncesData();
            };

            self.prevAbandomentsPageClick = function (e) {
                e.preventDefault();

                if (self.tcaModel.pageNumber() > 1)
                    self.tcaModel.pageNumber(self.tcaModel.pageNumber() - 1);

                self.bindTopChangeByAbandomentsData();
            };
            self.nextAbandomentsPageClick = function (e) {
                e.preventDefault();

                if (self.tcaModel.pageNumber() < Math.ceil(self.tcaModel.TotalCount() / self.tcaModel.pageSize()))
                    self.tcaModel.pageNumber(self.tcaModel.pageNumber() + 1);

                self.bindTopChangeByAbandomentsData();
            };
            self.changeAbandomentsOrder = function (e) {
                e.preventDefault();

                self.tcaModel.reverseSortDirection();

                self.bindTopChangeByAbandomentsData();
            };

            self.prevPurchasesPageClick = function (e) {
                e.preventDefault();

                if (self.tcpModel.pageNumber() > 1)
                    self.tcpModel.pageNumber(self.tcpModel.pageNumber() - 1);

                self.bindTopChangeByPurchasesData();
            };
            self.nextPurchasesPageClick = function (e) {
                e.preventDefault();

                if (self.tcpModel.pageNumber() < Math.ceil(self.tcpModel.TotalCount() / self.tcpModel.pageSize()))
                    self.tcpModel.pageNumber(self.tcpModel.pageNumber() + 1);

                self.bindTopChangeByPurchasesData();
            };
            self.changePurchasesOrder = function (e) {
                e.preventDefault();

                self.tcpModel.reverseSortDirection();

                self.bindTopChangeByPurchasesData();
            };

            self.prevJointPageClick = function (e) {
                e.preventDefault();

                if (self.tcjModel.pageNumber() > 1)
                    self.tcjModel.pageNumber(self.tcjModel.pageNumber() - 1);

                self.bindTopChangeJointData();
            };
            self.nextJointPageClick = function (e) {
                e.preventDefault();

                if (self.tcjModel.pageNumber() < Math.ceil(self.tcjModel.TotalCount() / self.tcjModel.pageSize()))
                    self.tcjModel.pageNumber(self.tcjModel.pageNumber() + 1);

                self.bindTopChangeJointData();
            };
            self.changeJointOrder = function (e) {
                e.preventDefault();

                var currentSortBy = self.tcjModel.sortBy();
                if (currentSortBy == e.data) {
                    self.tcjModel.reverseSortDirection();
                }
                else {
                    self.tcjModel.sortBy(e.data);
                    self.tcjModel.sortDirection("Descending");
                }

                self.bindTopChangeJointData();
            };

            self.loadSalesChart = function (data, labels) {
			
                var max = Math.max.apply(Math, data) * 1.2;
                var step = Math.pow(10, max.toFixed().length - 1) / 5;
                max = (Math.ceil(max / step) * step).toFixed(2);

                var invData = [];
                $.each(data, function (i, e) { invData[i] = max - data[i]; });
                var chartData = {
                    labels: labels,
                    datasets: [
						{
							label: "data",
							backgroundColor: "rgba(13, 177, 185, 1)",
							data: data
						},
						{
							label: "inverse",
							backgroundColor: "rgba(233, 233, 233, 1)",
							data: invData
                        },
                    ]
                };

                var chartOptions = {
                	//responsive: true,
                	scales: {
                		scaleLabel: {
                			fontSize: 10,
                		},
                		xAxes: [{
                			stacked: true,
                			gridLines: {
                				display: false,
                				drawBorder: true,
                				drawOnChartArea: false,
                			},
                			ticks: {
                				stepSize: 5,
                				beginAtZero: true,
                				min: 0,
                			}
                		}],
                		yAxes: [{
                			stacked: true,
                			gridLines: {
                				display: false,
                				drawBorder: true,
                				drawOnChartArea: false,
                			},
                			ticks: {
                				//stepSize: 5,
                				beginAtZero: true,
                				min: 0,
                			}
                		}],
                	},
                	tooltips: {
                		enabled: false,
                	},
                	legend: {
						display: false,
                	}
                };

            	var chartDiv = document.getElementById("hcPSalesPerformanceChart");
            	var chartCanvas = document.getElementById("hcDashboardSalesOverTimeChart"); //$(".hcDashboardChart");
            	var ctx = document.getElementById("hcDashboardSalesOverTimeChart").getContext("2d"); //self.salesChart.get(0).getContext("2d");

				if (window.salesPerformanceChart != null) {
					window.salesPerformanceChart.destroy();
				}

                window.salesPerformanceChart = new Chart(ctx, {
                    type: 'bar',
                    data: chartData,
                    options: chartOptions
                });

            };

            self.loadProductPerformanceChart = function(performanceData){
            	var chartData = {
            		labels: performanceData.ChartLabels,
            		datasets: [
						{
							type: 'line',
							label: performanceData.BouncedName,
							borderColor: 'rgba(240,79,48,0.0)',
							backgroundColor: 'rgba(240,79,48,0.5)',
							lineTension: 0,
							pointRadius: 1,
							data: performanceData.BouncedData
						},
						{
							type: 'line',
							label: performanceData.AbandonedName,
							borderColor: 'rgba(240,151,39,0.0)',
							backgroundColor: 'rgba(240,151,39,0.5)',
							lineTension: 0,
							pointRadius: 1,
							data: performanceData.AbandonedData
						},
						{
							type: 'line',
							label: performanceData.PurchasedName,
							borderColor: 'rgba(13,178,186,0.0)',
							backgroundColor: 'rgba(13,178,186,0.5)',
							lineTension: 0,
							pointRadius: 1,
							data: performanceData.PurchasedData
						},
            		]
            	}

            	var chartOptions = {
            		//datasetFill : true,
            		//responsive: true,
            		scales: {
            			xAxes: [{
            				gridLines: {
            					display: true,
            					drawBorder: true,
            					drawOnChartArea: false,
            				},
            				ticks: {
            					maxRotation : 0,
            					minRotation : 0,
            				}
            			}],
            			yAxes: [{
            				gridLines: {
            					display: true,
            					drawBorder: true,
            					drawOnChartArea: true,
            				},
            			}],
            		},
            		tooltips: {
            			enabled: true,
						
            			callbacks: {
            				title: function(tooltipItems, data) {
            					return '';
            				},
            				label: function(tooltipItem, data) {
            					var dataset = data.datasets[tooltipItem.datasetIndex];
            					var datasetLabel = dataset.label || '';
            					var dataPoint =  dataset.type == "milestone" ? '' : " : " + dataset.data[tooltipItem.index];
            					return datasetLabel + dataPoint;
            				}
            			}
            		},
            		legend: {
            			display: true,
            			labels : {
            				boxWidth: 10,
            			}

            		}
            	};

            	var chartDiv = document.getElementById("hcPProductPerformanceChart");
            	var chartCanvas = document.getElementById("hcProductPerformanceChart"); 
            	var ctx = document.getElementById("hcProductPerformanceChart").getContext("2d"); 

            	if (window.productPerformanceChart != null) {
            		window.productPerformanceChart.destroy();
            	}

            	window.productPerformanceChart = new Chart(ctx, {
            		type: 'line',
            		data: chartData,
            		options: chartOptions
            	});
            };


            self.slPeriod.change($.proxy(self.bindSalesBlock, self));
            self.pfPeriod.change($.proxy(self.bindPerformanceData, self));
            self.pfPeriod.change($.proxy(self.bindTopChangeByBouncesData, self));
            self.pfPeriod.change($.proxy(self.bindTopChangeByAbandomentsData, self));
            self.pfPeriod.change($.proxy(self.bindTopChangeByPurchasesData, self));
            self.pfPeriod.change($.proxy(self.bindTopChangeJointData, self));

            self.hcTopChangeByBouncesBlock.on("click", ".hcPrevPage", $.proxy(self.prevBouncesPageClick, self));
            self.hcTopChangeByBouncesBlock.on("click", ".hcNextPage", $.proxy(self.nextBouncesPageClick, self));
            self.hcTopChangeByAbandomentsBlock.on("click", ".hcPrevPage", $.proxy(self.prevAbandomentsPageClick, self));
            self.hcTopChangeByAbandomentsBlock.on("click", ".hcNextPage", $.proxy(self.nextAbandomentsPageClick, self));
            self.hcTopChangeByPurchasesBlock.on("click", ".hcPrevPage", $.proxy(self.prevPurchasesPageClick, self));
            self.hcTopChangeByPurchasesBlock.on("click", ".hcNextPage", $.proxy(self.nextPurchasesPageClick, self));
            self.hcTopChangeJointBlock.on("click", ".hcPrevPage", $.proxy(self.prevJointPageClick, self));
            self.hcTopChangeJointBlock.on("click", ".hcNextPage", $.proxy(self.nextJointPageClick, self));

            self.hcTopChangeByBouncesBlock.on("click", ".hcChangeOrder", $.proxy(self.changeBouncesOrder, self));
            self.hcTopChangeByAbandomentsBlock.on("click", ".hcChangeOrder", $.proxy(self.changeAbandomentsOrder, self));
            self.hcTopChangeByPurchasesBlock.on("click", ".hcChangeOrder", $.proxy(self.changePurchasesOrder, self));

            self.hcTopChangeJointBlock.on("click", ".hcChangeOrder.hcByChange", "ByChange", $.proxy(self.changeJointOrder, self));
            self.hcTopChangeJointBlock.on("click", ".hcChangeOrder.hcByBouncesChange", "ByBouncesChange", $.proxy(self.changeJointOrder, self));
            self.hcTopChangeJointBlock.on("click", ".hcChangeOrder.hcByAbandomentsChange", "ByAbandomentsChange", $.proxy(self.changeJointOrder, self));
            self.hcTopChangeJointBlock.on("click", ".hcChangeOrder.hcByPurchasesChange", "ByPurchasesChange", $.proxy(self.changeJointOrder, self));

            self.hcCreateSamples.on("click", $.proxy(self.createSamples, self));
            self.hcCreateSamplesPopupCancel.on("click", $.proxy(self.createSamplesCancel, self));
            self.hcFindOutMore.on("click", $.proxy(self.findOutMore, self));

            self.hcCreateSamplesPopup.hcDialog({
                autoOpen: false,
                height: 'auto',
                minHeight: 200,
                width: 500
            });

            self.hcFindOutMorePopup.hcDialog({
                autoOpen: false,
                height: 'auto',
                minHeight: 200,
                width: 500
            });

            self.slPeriod.change();
            self.pfPeriod.change();
        });
    }
    $(".hcDashboard").adminDashboard();

});
