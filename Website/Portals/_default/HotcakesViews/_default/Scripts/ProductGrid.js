$(document).ready(function () {
    if ($.fn.masonry) {
        $(".hc-product-grid .hc-product-cards").masonry({
            itemSelector: ".hc-product-card",
            columnWidth: ".hc-product-card-sizer",
            percentPosition: true
        });
    }
});