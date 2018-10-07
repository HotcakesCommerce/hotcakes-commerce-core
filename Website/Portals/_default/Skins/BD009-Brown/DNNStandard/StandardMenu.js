$(document).ready(function () {

    function HoverOver() {
        $(this).addClass('hover');
    }

    function HoverOut() {
        $(this).removeClass('hover');
    }

    var config = {
        sensitivity: 2,
        interval: 200,
        over: HoverOver,
        timeout: 10,
        out: HoverOut
    };

    $(".dnnMenu .topLevel > li.haschild").hoverIntent(config);

    $(".subLevel li.haschild").hover(HoverOver, HoverOut);

});