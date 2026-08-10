$(document).ready(function () {
    if ($.fn.masonry) {
        var $grid = $(".hc-product-grid .hc-product-cards").masonry({
            itemSelector: ".hc-product-card",
            columnWidth: ".hc-product-card-sizer",
            percentPosition: true
        });
        $grid.imagesLoaded().progress(function () {
            $grid.masonry("layout");
        });
    }
});