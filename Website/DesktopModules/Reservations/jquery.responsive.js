;(function ($) {
    $(document).ready(function () {
        var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
        pageRequestManager.add_endRequest(function () {
            addResizeHandler();
        });

        addResizeHandler();
    })

    function addResizeHandler() {
        $(window).resize(function () {
            resize();
        })

        resize();
    }

    function resize() {
        var width = $(".DNNSpecialists_Modules_Reservations").width();

        //console.log(width);

        if (width < 905) {
            $(".DNNSpecialists_Modules_Reservations_Step").addClass("DNNSpecialists_Modules_Reservations_Step_900");
        }
        else {
            $(".DNNSpecialists_Modules_Reservations_Step").removeClass("DNNSpecialists_Modules_Reservations_Step_900");
        }

        if (width < 485) {
            $(".DNNSpecialists_Modules_Reservations_Step").addClass("DNNSpecialists_Modules_Reservations_Step_480");
        }
        else {
            $(".DNNSpecialists_Modules_Reservations_Step").removeClass("DNNSpecialists_Modules_Reservations_Step_480");
        }
    }
})(jQuery);