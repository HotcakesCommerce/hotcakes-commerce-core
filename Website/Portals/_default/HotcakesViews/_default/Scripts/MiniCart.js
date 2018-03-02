alert('js');

jQuery(function ($) {
    // Common ----------------------
    var $target = $('#hcMiniCartTooltip');
    var $minicart = $('#hcMiniCart');
    var $iconbox = $minicart.find(".hc-iconbox");

    function InitCommon() {
        if ($target.length) {
            $minicart.hover(function () {
                $iconbox.css("background-color", "#FFF");
                $target.slideDown().position(
                    'right'
                );
            }, function () {
                $target.slideUp(function () {
                    $iconbox.css("background-color", "");
                });
            });
        }
    }
    
    $(document).ready(function () {
        InitCommon();
    });
});
