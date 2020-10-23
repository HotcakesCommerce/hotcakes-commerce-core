var HcAffiliate = function (data /* see AffiliateViewModel fields */) {
    var self = this;
    self.id = ko.observable(data.Id);
    self.userid = ko.observable(data.UserId);
    self.username = ko.observable(data.Username);
    self.email = ko.observable(data.Email);
    self.password = ko.observable(data.Password);
    self.confirmPassword = ko.observable(data.ConfirmPassword);
    self.firstname = ko.observable(data.FirstName);
    self.lastname = ko.observable(data.LastName);

    self.myaffiliateid = ko.observable(data.MyAffiliateId);
    self.referralaffiliateid = ko.observable(data.ReferralAffiliateId);
    self.allowReferral = ko.observable(data.AllowReferral);
    self.confirmterms = ko.observable(false);
    self.countryid = ko.observable(data.CountryId);
    self.addressline = ko.observable(data.AddressLine);
    self.city = ko.observable(data.City);
    self.company = ko.observable(data.Company);
    self.state = ko.observable(data.State);
    self.postalcode = ko.observable(data.PostalCode);
    self.phone = ko.observable(data.Phone);
    self.regions = ko.observableArray(data.Regions);
    self.approved = ko.observable(data.Approved);
};

var HcAffiliateProfileViewModel = function (model, $form) {
    var self = this;

    // Fields -------------
    self.model = model;
    self.message = {
        status: ko.observable("OK"),
        show: ko.observable(false),
        text: ko.observable('')
    };
    self.model.referralaffiliateid.subscribe(function () {
        if (self.model.referralaffiliateid() != undefined && self.model.referralaffiliateid().length > 0) {
            self.changeReferralAffiliateId();
        } else {
            self.message.show(false);
        }
    });

    // Handlers -----------------
    self.update = function () {
        if (!hcc.formIsValid($form))
            return;

        $form.ajaxLoader("start");
        $.ajax(hcc.getServiceUrl("AffiliateDashboard/Update"), {
            data: ko.toJSON(self.model),
            contentType: "application/json",
            type: "post"
        })
            .done(function (resp) {
                self.showMessage(resp.Message, resp.Status);
            })
            .fail(function () { })
            .always(function () { $form.ajaxLoader("stop"); });
    };

    self.changeCountry = function () {
        $form.ajaxLoader("start");
        $.post(hcc.getServiceUrl("AffiliateDashboard/GetRegions"),
            { countryId: self.model.countryid() }, null, "json"
        )
            .done(function (resp) {
                self.model.regions(resp);
            })
            .fail(function () { })
            .always(function () { $form.ajaxLoader("stop"); });

    };

    self.changeReferralAffiliateId = function () {
        $form.ajaxLoader("start");
        $.post(hcc.getServiceUrl("AffiliateRegistration/IsAffiliateValid"),
            { affiliateId: self.model.referralaffiliateid() }, null, "json"
        )
            .done(function (data) {
                if (!data) {
                    self.showMessage(hcc.l10n.affiliate_ReferralAffiliateIDInvalid, "Failed");
                }
                else {
                    self.message.show(false);
                }
            })
            .fail(function () { })
            .always(function () { $form.ajaxLoader("stop"); });

    };

    // Methods -------------
    self.showMessage = function (message, status) {
        self.message.status(status);
        self.message.text(message);
        self.message.show(true);
        $("body").scrollTo(".hcValidationSummary");
    };

    var init = function () {
    };

    init();
};

var HcBaseAffiliateDashboardReport = function (data, $form) {
    var self = this;

    self.totalCount = ko.observable(data.TotalCount);
    self.totalAmount = ko.observable(data.TotalAmount);

    self.loadPage = function (pageNumber, pageSize) {
        $form.ajaxLoader("start");
        $.post(hcc.getServiceUrl("AffiliateDashboard/" + self.getReportName()),
            self.getPageParameters(pageNumber, pageSize), null, "json"
        )
            .done(function (resp) {
                self.loadPageData(resp);
            })
            .fail(function () { })
            .always(function () { $form.ajaxLoader("stop"); });
    };
    self.pager = new HcPager({ total: data.TotalCount, pageSize: 5, pageNumber: 1, pagerSize: 5 }, self.loadPage);

    ko.bindingHandlers.dateText = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var d = ko.utils.unwrapObservable(value);
            if (typeof d === "string") {
                d = new Date(Date.parse(d));
            }
            $(element).text(d.toLocaleDateString('en-us') + ' ' + d.toLocaleTimeString('en-us'));
        }
    };
};

var HcBaseAffiliateDashboardDateReport = function (data, $form) {
    var self = this;

    self.dateRange = ko.observable(10);
    self._prevDateRange = ko.observable(10);
    self.dateRange.subscribe(function (val) {
        if (self._prevDateRange() == val)
            return;
        if (self.pager.pageNumber() != 1)
            self.pager.pageNumber(1);
        else {
            self.loadPage(1, 5);
        }
        self._prevDateRange(val);
    });

    self.getPageParameters = function (pageNumber, pageSize) {
        return { pageSize: pageSize, pageNumber: pageNumber, dateRange: self.dateRange() };
    };

    HcBaseAffiliateDashboardReport.call(self, data, $form);
};

var HcAffiliateOrders = function (data, $form) {
    var self = this;
    self.orders = ko.observableArray(data.Orders);
    self.loadPageData = function (resp) {
        self.orders(resp.Orders);
        self.pager.total(resp.TotalCount);
        self.totalCount(resp.TotalCount);
        self.totalAmount(resp.TotalAmount);
    };
    self.getReportName = function () {
        return 'GetOrdersReport';
    };

    HcBaseAffiliateDashboardDateReport.call(self, data, $form);
};

var HcAffiliatePayments = function (data, $form) {
    var self = this;
    self.payments = ko.observableArray(data.Payments);
    self.loadPageData = function (resp) {
        self.payments(resp.Payments);
        self.pager.total(resp.TotalCount);
        self.totalCount(resp.TotalCount);
        self.totalAmount(resp.TotalAmount);
    };
    self.getReportName = function () {
        return 'GetPaymentsReport';
    };

    HcBaseAffiliateDashboardDateReport.call(self, data, $form);
};

var HcAffiliateReferrals = function (data, $form) {
    var self = this;

    self._prevSearchText = ko.observable();
    self._prevSearchBy = ko.observable('');

    self.searchBy = ko.observable();
    self.searchText = ko.observable('');

    self.referrals = ko.observableArray(data.Referrals);
    self.loadPageData = function (resp) {
        self.referrals(resp.Referrals);
        self.pager.total(resp.TotalCount);
        self.totalCount(resp.TotalCount);
        self.totalAmount(resp.TotalAmount);
    };
    self.getReportName = function () {
        return 'GetReferralsReport';
    };

    self.search = function () {
        if (self.pager.pageNumber() != 1)
            self.pager.pageNumber(1);
        else {
            self.loadPage(1, 5);
        }
    };

    self.getPageParameters = function (pageNumber, pageSize) {
        return { searchText: self.searchText(), searchBy: self.searchBy(), pageNumber: pageNumber, pageSize: pageSize };
    };

    HcBaseAffiliateDashboardReport.call(self, data, $form);
};

var HcUrlBuilderViewModel = function (data, $form) {
    var self = this;

    self.mode = ko.observable();
    self.linkTo = ko.observable();
    self.categories = ko.observableArray(data.Categories);
    self.products = ko.observableArray();
    self.categoryId = ko.observable();
    self.productId = ko.observable();
    self.textUrl = ko.observable(hcc.getUrlProtocol() + document.domain + hcc.getSiteRoot());
    self.generatedUrl = ko.observable();

    self.changeMode = function () {
        if (self.mode() == "Product") {
            getProducts();
        } else if (self.mode() == "Category") {
        } else if (self.mode() == "Website") {
            self.textUrl(hcc.getUrlProtocol() + document.domain + hcc.getSiteRoot());
        } else if (self.mode() == "Registration") {
            self.textUrl(data.RegistrationUrl);
        }
    };
    self.changeCategory = function () {
        if (self.mode() == "Product") {
            getProducts();
        }
    };
    self.generate = function () {
        var id = "";

        if (self.mode() == "Category") {
            id = self.categoryId();
        } else if (self.mode() == "Product") {
            id = self.productId();
        } else {
            id = self.textUrl();
        }

        $form.ajaxLoader("start");
        $.post(hcc.getServiceUrl("AffiliateDashboard/GenerateUrl"),
            { id: id, mode: self.mode() }, null, "json"
        )
            .done(function (resp) { self.generatedUrl(resp); })
            .fail(function () { })
            .always(function () { $form.ajaxLoader("stop"); });

        if ($('.zclip').length == 0) {
            $('#hcCopyToClipboard').zclip({
                path: hcc.getResourceUrl("Scripts/ZeroClipboard.swf"),
                copy: function () {
                    return $("#hcCopySource").val();
                }
            });
        }
    };

    var getProducts = function () {
        $.post(hcc.getServiceUrl("AffiliateDashboard/GetProducts"),
            { categoryId: self.categoryId() }, null, "json"
        )
            .done(function (resp) {
                self.products(resp);
            })
            .fail(function () { })
            .always(function () { $form.ajaxLoader("stop"); });
    };
    ko.bindingHandlers.hasSelection = {
        update: function (element, valueAccessor, allBindings) {
            if (valueAccessor() == true)
                $(element).select();
        }
    };
};

$(function () {
    $('#hcAffiliateTabs').dnnTabs();
    $('#hcCopyToClipboard').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
    });

    var clipboard = new ClipboardJS("#hcCopyToClipboard");

    /*clipboard.on("success", function (e) {
        console.info("Action:", e.action);
        console.info("Text:", e.text);
        console.info("Trigger:", e.trigger);
        e.clearSelection();
    });

    clipboard.on("error", function (e) {
        console.error("Action:", e.action);
        console.error("Trigger:", e.trigger);
        e.preventDefault();
    });*/
});
