$(document).ready(function () {
    if ($.fn.masonry) {
        $(".hc-search-results .hc-product-cards").masonry({
            itemSelector: ".hc-product-card",
            columnWidth: ".hc-product-card-sizer",
            percentPosition: true
        });
    }
});