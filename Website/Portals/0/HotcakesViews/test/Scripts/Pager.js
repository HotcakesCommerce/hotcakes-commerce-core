var HcPager = function (data, callback) {
    var self = this;
    self.onPageChange = callback;
    data = $.extend({ pageSize: 5, pageNumber: 1, pagerSize: 10 }, data);

    self.pageNumber = ko.observable(data.pageNumber);
    self.pageSize = ko.observable(data.pageSize);
    self.total = ko.observable(data.total);
    self.pagerSize = ko.observable(data.pagerSize);

    self.pageNumber.subscribe(function () {
        self.computePagesList();
        self.onPageChange(self.pageNumber(), self.pageSize());
    });
    self.pageSize.subscribe(function () {
        self.computePagesList();
        self.onPageChange(self.pageNumber(), self.pageSize());
    });
    self.total.subscribe(function () {
        self.computePagesList();
    });
    self.pagerSize.subscribe(function () {
        self.computePagesList();
    });
    self.calculatePagesCount = function () {
        if (self.pageSize() < 1)
            return 1;

        return Math.ceil(self.total() / self.pageSize());
    };

    self.pagesList = ko.observableArray();
    self.previousPage = ko.observable(-1);
    self.nextPage = ko.observable(-1);

    self.pagesCount = ko.computed(function () {
        return self.calculatePagesCount();
    });

    self.computePagesList = function () {
        var pages = [], pagesCount = self.calculatePagesCount();
        var min = (Math.ceil(self.pageNumber() / self.pagerSize()) - 1) * self.pagerSize() + 1,
            max = Math.min(pagesCount, min + self.pagerSize() - 1);
        if (max > 1) {
            for (var i = min; i <= max; i++) {
                pages.push(i);
            }
        }

        self.previousPage(min > 1 ? min - 1 : -1);
        self.nextPage(max < pagesCount ? max + 1 : -1);
        
        self.pagesList(pages);
    };

    self.setCurrentPage = function (page) {
        if (page > 0 && page <= self.pagesCount())
            self.pageNumber(page);
    };
    self.init = function () {
        self.computePagesList();
    };

    self.init();
};