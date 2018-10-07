
    $(document).ready(function () {
        // disable dnnmodal window because of interference between jquery ui and jquery mobile (dnnmodal not opening)
        dnnModal.show = function (e) { window.location = e; };

        $(".actionMenuAdmin li[id$='-Delete'] a").live("click", function () {
            window.location = $(this).attr("href");
        });
    });

    $(document).bind("mobileinit", function () {
        $.extend($.mobile, { ajaxEnabled: false });
    });