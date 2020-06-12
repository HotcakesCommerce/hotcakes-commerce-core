
function HcDrillDownFilterViewModel(data, $form, catId, modId) {
    var DECIMAL_SEP = ".";
    var self = this;

    // Binding
    self.model = ko.observable();
    self.minPrice = ko.observable(data.SelectedMinPrice);
    self.maxPrice = ko.observable(data.SelectedMaxPrice);
    self.minPrice.subscribe(function (val) {
        $priceSlider.slider("values", 0, val);
    });
    self.maxPrice.subscribe(function (val) {
        $priceSlider.slider("values", 1, val);
    });
    self.pager = new HcPager({ total: 0, pageSize: 5, pageNumber: 1, pagerSize: 5 }, function (pageNumber, pageSize) {
        updateHash(pageNumber);
    });
    self.sortOrder = ko.observable(data.SortOrder);

    self.chooseCatFacet = function (facet) { chooseFacet(filter.Categories, facet); };
    self.chooseManFacet = function (facet) { chooseFacet(filter.Manufacturers, facet); };
    self.chooseVenFacet = function (facet) { chooseFacet(filter.Vendors, facet); };
    self.chooseTypeFacet = function (facet) { chooseFacet(filter.Types, facet); };
    self.choosePropFacet = function (parent) {
        var item = this;
        var propId = parent.Id;
        if (item.Id)
            item = item.Id;
        else
            item = item.toString();

        if (!filter.Properties[propId]) {
            filter.Properties[propId] = [];
            filter.PropertyIds.push(propId);
        }

        var items = filter.Properties[propId];
        toggleArrayItem(items, item);

        updateHash(1);
    };
    self.applyPrice = function () {
        updateHash(1);
    };
    self.changeSortOrder = function () {
        updateHash(1);
    };

    // Implementation
    var $priceSlider = $("<div/>").prependTo(".hc-price-slider");
    var filter = {
        Manufacturers: [],
        Vendors: [],
        Types: [],
        Properties: {},
        PropertyIds: [],
        MinPrice: 0,
        MaxPrice: 0
    };

    function chooseFacet(arr, facet) {
        toggleArrayItem(arr, facet.Id);
        updateHash(1);
    }
    function toggleArrayItem(arr, item) {
        var i = $.inArray(item, arr);
        if (i < 0)
            arr.push(item);
        else
            arr.splice(i, 1);
    }
    function updateHash(page) {
        self.pager.pageNumber(page);
        var hashStr =
            "page=" + page +
            "&sort=" + self.sortOrder() +
            "&min=" + self.minPrice().toString().replace(DECIMAL_SEP, "_") +
            "&max=" + self.maxPrice().toString().replace(DECIMAL_SEP, "_");
        if (filter.Manufacturers != undefined) {
            hashStr = hashStr + "&mn=" + filter.Manufacturers.join(",");
        } else {
            hashStr = hashStr + "&mn=";
        }
        if (filter.Vendors != undefined) {
            hashStr = hashStr + "&vn=" + filter.Vendors.join(",");
        } else {
            hashStr = hashStr + "&vn=";
        }
        if (filter.Types != undefined) {
            hashStr = hashStr + "&tp=" + filter.Types.join(",");
        } else {
            hashStr = hashStr + "&tp=";
        }
        hashStr = hashStr + "&prop=" + joinProperties();

        location.hash = hashStr;
    }
    function joinProperties() {
        var propStrings = [];
        if (filter.PropertyIds.length > 0) {
            var pString = "";
            $.each(filter.PropertyIds, function (i, id) {
                if (filter.Properties[id] != "") {
                    pString = id + "|" + encodeArr(filter.Properties[id]).join("|");
                    propStrings.push(pString);
                }
            });
        }
        return propStrings.join(",");
    }
    function parseProperties(props, filter) {
        filter.Properties = {};
        filter.PropertyIds = [];
        if (props.length > 0) {
            var pString = "";
            $.each(props, function (i, p) {
                if (p) {
                    pItems = p.split("|");
                    filter.PropertyIds.push(pItems[0]);
                    filter.Properties[pItems[0]] = decodeArr(pItems.slice(1, pItems.length));
                }
            });
        }

        filter.PropertiesJson = JSON.stringify(filter.Properties);
    }
    function postDrillDown(params) {
        params.ModuleId = modId;
        $form.find(".hc-product-grid").ajaxLoader("start");
        $.ajax(hcc.getServiceUrl("Category/DrillDown"),
            {
                data: ko.toJSON(params),
                contentType: "application/json",
                type: "post"
            })
            .done(function (res) { handleDrillDown(res); })
            .fail(function () { })
            .always(function () { $form.find(".hc-product-grid").ajaxLoader("stop"); });
    }
    function handleDrillDown(data) {
        self.model(data);
        $priceSlider.slider({
            range: true,
            min: self.model().MinPrice,
            max: self.model().MaxPrice,
            values: [self.minPrice(), self.maxPrice()],
            slide: function (event, ui) {
                self.minPrice(ui.values[0]);
                self.maxPrice(ui.values[1]);
            }
        });
        self.minPrice(data.SelectedMinPrice);
        self.maxPrice(data.SelectedMaxPrice);
        self.pager.total(data.PagerData.TotalItems);
        self.pager.pageSize(data.PagerData.PageSize);
        self.pager.setCurrentPage(data.PagerData.CurrentPage);
    }
    function getHashParams() {
        var hashParams = {};
        var e,
            a = /\+/g,  // Regex for replacing addition symbol with a space
            r = /([^&;=]+)=?([^&;]*)/g,
            d = function (s) { return decodeURIComponent(s.replace(a, " ")); },
            q = window.location.hash.substring(1);
        while (e = r.exec(q))
            hashParams[d(e[1])] = d(e[2]);
        return hashParams;
    }
    function encodeArr(arr) {
        var res = [];
        if (arr) {
            $.each(arr, function (i, el) {
                res.push(encodeURIComponent(el).replace(/\./g, '%2E').replace(/%/g, '$'));
            });
        }
        return res;
    }
    function decodeArr(arr) {
        var res = [];
        if (arr) {
            $.each(arr, function (i, el) {
                res.push(decodeURIComponent(el.replace(/\$/g, '%')));
            });
        }
        return res;
    }
    function handleHashchange() {
        if (location.hash) {
            var params = getHashParams();
            filter = {
                PageNumber: params.page,
                SortOrder: params.sort,
                MinPrice: params.min.replace("_", DECIMAL_SEP),
                MaxPrice: params.max.replace("_", DECIMAL_SEP),
                CategoryId: catId,
                Manufacturers: params.mn ? params.mn.split(",") : [],
                Vendors: params.vn ? params.vn.split(",") : [],
                Types: params.tp ? params.tp.split(",") : [],
            };

            parseProperties(params.prop ? params.prop.split(",") : [], filter);
        }
        postDrillDown(filter, 1);
    }
    $(window).on('hashchange', function () {
        handleHashchange();
    });
    if (location.hash.length <= 1) {
        handleDrillDown(data);
    }
}