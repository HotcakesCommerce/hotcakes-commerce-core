(function (Chart) {

	var helpers = Chart.helpers,
		globalOpts = Chart.defaults.global,
		defaultColor = globalOpts.defaultColor;

	globalOpts.elements.milestone = {
		radius: 3,
		pointStyle: 'rect', //circle
		backgroundColor: defaultColor,
		borderWidth: 1,
		borderColor: defaultColor,
		// Hover
		hitRadius: 1,
		hoverRadius: 4,
		hoverBorderWidth: 1
	};

	function xRange(mouseX) {
		var vm = this._view;
		return vm ? (Math.pow(mouseX - vm.x, 2) < Math.pow(vm.radius + vm.hitRadius, 2)) : false;
	}

	function yRange(mouseY) {
		var vm = this._view;
		return vm ? (Math.pow(mouseY - vm.y, 2) < Math.pow(vm.radius + vm.hitRadius, 2)) : false;
	}

	Chart.elements.Milestone = Chart.Element.extend({
		//Inherited functions for element
		inRange: function (mouseX, mouseY) {
			var vm = this._view;
			return vm ? ((Math.pow(mouseX - vm.x, 2) + Math.pow(mouseY - vm.y, 2)) < Math.pow(vm.hitRadius + vm.radius, 2)) : false;
		},
		inLabelRange: xRange,
		inXRange: xRange,
		inYRange: yRange,

		draw: function () {
			var vm = this._view;

			if (vm.skip || isNaN(vm.radius)) {
				return;
			}

			var ctx = this._chart.ctx;
			var pointStyle = vm.pointStyle;
			var radius = vm.radius;
			var x = vm.x;
			var y = vm.y;


			ctx.strokeStyle = vm.borderColor || defaultColor;
			ctx.lineWidth = helpers.getValueOrDefault(vm.borderWidth, globalOpts.elements.point.borderWidth);
			ctx.fillStyle = vm.backgroundColor || defaultColor;

			//var chartArea = this._chart.chartArea;
			var chartBottom = this._yScale.bottom;
			var chartTop = this._yScale.top;

			ctx.beginPath();
			ctx.moveTo(x, chartBottom);
			ctx.lineTo(x, chartTop);
			ctx.stroke();

			Chart.canvasHelpers.drawPoint(ctx, pointStyle, radius, x, y);
		},
		tooltipPosition: function () {
			var vm = this._view;
			return {
				x: vm.x,
				y: vm.y,
				padding: vm.radius + vm.borderWidth
			};
		},
		getCenterPoint: function () {
			var vm = this._view;
			return {
				x: vm.x,
				y: vm.y
			};
		},
		getArea: function () {
			return Math.PI * Math.pow(this._view.radius, 2);
		},
	});

	Chart.defaults.milestone = {
		hover: {
			mode: 'single'
		},
		scales: {
			xAxes: [{
				type: 'linear', // bubble should probably use a linear scale by default
				position: 'bottom',
				id: 'x-axis-0' // need an ID so datasets can reference the scale
			}],
			yAxes: [{
				type: 'linear',
				position: 'left',
				id: 'y-axis-0'
			}]
		},
		tooltips: {
			enabled: true,
			callbacks: {
				title: function (tooltipItems, data) {
					// Title doesn't make sense for scatter since we format the data as a point
					return '';
				},
				label: function (tooltipItems, data) {
					var datasetLabel = data.datasets[tooltipItem.datasetIndex].label || '';
					var dataPoint = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];

					var xLabel = ''; //tooltipItem.xLabel
					var dataLable = ''; //dataPoint.r 
					var yLabel = ''; //tooltipItem.yLabel 
					return datasetLabel + ': (' + xLabel + ', ' + yLabel + ', ' + dataLable + ')';
				}
			}
		}
	};

	Chart.controllers.milestone = Chart.DatasetController.extend({

		dataElementType: Chart.elements.Milestone, //.Point,

		// @@ Optional methods to override in dataset controllers @@ //
		// Initializes the controller
		initialize: function (chart, datasetIndex) {
			Chart.DatasetController.prototype.initialize.call(this, chart, datasetIndex);
		},

		// Create elements for each piece of data in the dataset. Store elements in an array on the dataset as dataset.metaData
		addElements: function () {
			Chart.DatasetController.prototype.addElements.call(this);
		},

		// Create a single element for the data at the given index and reset its state
		addElementAndReset: function (index) {
			Chart.DatasetController.prototype.addElementAndReset.call(this, index);
		},

		// Draw the representation of the dataset
		// @param ease : if specified, this number represents how far to transition elements. See the implementation of draw() in any of the provided controllers to see how this should be used
		draw: function (ease) {
			var me = this;
			Chart.DatasetController.prototype.draw.call(me, ease);
		},

		// Add hover styling to the given element
		setHoverStyle: function (point) {
			var me = this;
			Chart.DatasetController.prototype.setHoverStyle.call(me, point);

			// Radius
			var dataset = me.chart.data.datasets[point._datasetIndex];
			var index = point._index;
			var custom = point.custom || {};
			var model = point._model;
			model.radius = custom.hoverRadius ? custom.hoverRadius : (helpers.getValueAtIndexOrDefault(dataset.hoverRadius, index, me.chart.options.elements.point.hoverRadius)) + me.getRadius(dataset.data[index]);
		},

		// Remove hover styling from the given element
		removeHoverStyle: function (point) {
			var me = this;
			Chart.DatasetController.prototype.removeHoverStyle.call(me, point, me.chart.options.elements.point);

			var dataVal = me.chart.data.datasets[point._datasetIndex].data[point._index];
			var custom = point.custom || {};
			var model = point._model;

			model.radius = custom.radius ? custom.radius : me.getRadius(dataVal);
		},


		// Update the elements in response to new data
		// @param reset : if true, put the elements into a reset state so they can animate to their final values
		update: function (reset) {
			var me = this;
			Chart.DatasetController.prototype.update.call(me, reset);
			var meta = me.getMeta();
			var points = meta.data;

			// Update Points
			helpers.each(points, function (point, index) {
				me.updateElement(point, index, reset);
			});
		},

		// Ensures that the dataset represented by this controller is linked to a scale. Overridden to helpers.noop in the polar area and doughnut controllers as these
		// chart types using a single scale
		linkScales: function () {
			Chart.DatasetController.prototype.linkScales.call(this);
		},

		// Called by the main chart controller when an update is triggered. The default implementation handles the number of data points changing and creating elements appropriately. 
		buildOrUpdateElements: function () {
			Chart.DatasetController.prototype.buildOrUpdateElements.call(this);
		},

		// @@ Private functions of controller @@ //

		updateElement: function (point, index, reset) {

			var me = this;
			var meta = me.getMeta();
			var xScale = me.getScaleForId(meta.xAxisID);
			var yScale = me.getScaleForId(meta.yAxisID);

			var custom = point.custom || {};
			var dataset = me.getDataset();
			var data = dataset.data[index];
			var pointElementOptions = me.chart.options.elements.point;
			var dsIndex = me.index;

			helpers.extend(point, {
				// Utility
				_xScale: xScale,
				_yScale: yScale,
				_datasetIndex: dsIndex,
				_index: index,

				// Desired view properties
				_model: {
					x: reset ? xScale.getPixelForDecimal(0.5) : xScale.getPixelForValue(typeof data === 'object' ? data : NaN, index, dsIndex, me.chart.isCombo),
					y: reset ? xScale.getPixelForDecimal(0.5) : me.chart.chartArea.top,
					// Appearance
					radius: reset ? 0 : custom.radius ? custom.radius : me.getRadius(data),

					// Tooltip
					hitRadius: custom.hitRadius ? custom.hitRadius : helpers.getValueAtIndexOrDefault(dataset.hitRadius, index, pointElementOptions.hitRadius)
				}
			});

			// Trick to reset the styles of the point
			Chart.DatasetController.prototype.removeHoverStyle.call(me, point, pointElementOptions);

			var model = point._model;
			model.skip = custom.skip ? custom.skip : (isNaN(model.x) || isNaN(model.y));

			point.pivot();
		},

		getRadius: function (value) {
			if (value != null) {
				return value.r || this.chart.options.elements.point.radius;
			}
		},

	});
}).call(this, Chart);