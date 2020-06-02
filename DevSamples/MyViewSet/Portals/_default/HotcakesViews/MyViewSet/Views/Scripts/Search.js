$(document).ready(function () {
    if ($.fn.masonry) {
        var $grid = $(".hc-search-results .hc-product-cards").masonry({
            itemSelector: ".hc-product-card",
            columnWidth: ".hc-product-card-sizer",
            percentPosition: true
        });
        $grid.imagesLoaded().progress(function () {
            $grid.masonry("layout");
        });
    }
});