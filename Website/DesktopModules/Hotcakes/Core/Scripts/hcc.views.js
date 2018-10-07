$(function () {

    hcc.config.dialogClass = "dnnFormPopup hc-popup-dialog";

    var forms = $('div[data-type="form"]');

    forms.on("keydown", "input:text", function (event) {
        var form = $(this).closest('div[data-type="form"]');
        if (event.which == 13) {
            var inputElement = form.find("input:submit:not([data-nosubmit]), input:image:not([data-nosubmit])").first();
            if (inputElement)
                inputElement.click();

            event.preventDefault();
        }
    });

    forms.on("click", "input:submit:not([data-nosubmit]), input:image:not([data-nosubmit])", function (event) {
        var form = $(this).closest('div[data-type="form"]');
        var submitEl = $(event.target);

        var confirmMessage = submitEl.data("confirm");
        if (confirmMessage && !confirm(confirmMessage)) {
            event.stopPropagation();
            event.preventDefault();
            return;
        }
        
        var validator = $.data(form[0], 'validator');
        if (validator && !validator.form()) {
            event.stopPropagation();
            event.preventDefault();
            return;
        }

        var action = form.data("action");
        var method = form.data("method");
        var moduleidjs = form.data("moduleid");
        var skipxy = form.data("skipxy");

        var elements = form.find("select, input, textarea").serializeArray();

        if (moduleidjs && method != "get")
            elements.push({ "name": "moduleid", "value": moduleidjs });

        if (submitEl.attr("name")) {
            elements.push({ "name": submitEl.attr("name"), "value": submitEl.val() });
        }

        if (submitEl.attr("type") == "image" && !skipxy) {
            addXYImageParams(elements, submitEl, event);
        }

        var postdata = $.param(elements);

        var hcMvcView = $(this).closest('.hcMvcView');
        hcMvcView.ajaxLoader("start");

        if (method == "get") {
            setTimeout(function () {
                window.location = action + (action.indexOf("?") > -1 ? "&" : "?") + postdata;
            }, 0);
        }
        else {
            $.ajax({
                type: method,
                url: action,
                data: postdata,
                beforeSend: function (jqXHR, settings) {
                    jqXHR.setRequestHeader("CustomRedirect", "1");
                },
                success: function (data, textStatus, jqXHR) {
                    //we do not post to other url so this is not used
                    //if (window.location.href != this.url) {
                    //    //html5 feature. Won't work in IE < 10
                    //    if (history && history.pushState) {
                    //        //failes on second call in FIrefox due to replacement of whole document on previous one
                    //        history.pushState({}, document.title, this.url);
                    //    }
                    //    else {
                    //        //window.location.replace(this.url);
                    //        window.location = this.url;
                    //        return;
                    //    }
                    //}

                    document.open();
                    document.write(data);
                    document.close();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (jqXHR.status != 333)
                        if (console && console.log)
                            console.log(errorThrown);
                },
                complete: function (jqXHR, textStatus) {

                },
                statusCode: {
                    333: function (jqXHR, textStatus, errorThrown) {
                        var location = jqXHR.getResponseHeader("CustomLocation");
                        window.location = location;
                    }
                }
            });
        }
        event.preventDefault();
    });

    function addXYImageParams(elements, submitEl, event) {
        var nameX = "x";
        var nameY = "y";
        var name = submitEl.attr("name");
        if (name) {
            nameX = [name, ".", nameX].join("");
            nameY = [name, ".", nameY].join("");
        }

        var valueX = event.offsetX;
        var valueY = event.offsetY;
        valueX = (valueX == undefined || valueX == "") ? 0 : valueX;
        valueY = (valueY == undefined || valueY == "") ? 0 : valueY;

        elements.push({ "name": nameX, "value": valueX });
        elements.push({ "name": nameY, "value": valueY });
    }

    $('.hc-dialog').click(function () {
        var myModal = $(this).attr('href');
        $(myModal).hcDialog();
        return false;
    });

    // NOTE: Obsolete dialog handler. Use "hc-dialog" class instead.
    $('.hc-popup').click(function () {
        var myTitle = $(this).attr('title');
        var modalWidth = $(this).data('width');
        var modalHeight = $(this).data('height');
        var modalMinHeight = $(this).data('min-height');
        var modalMinWidth = $(this).data('min-width');
        var myModal = $(this).attr('href');
        $(myModal).dialog({
            width: modalWidth,
            maxHeight: modalHeight,
            minHeight: modalMinHeight,
            minWidth: modalMinWidth,
            title: myTitle,
            modal: true,
            closeOnEscape: true,
            position: "center",
            dialogClass: "dnnFormPopup hc-popup-dialog"
        });
        return false;
    });

    hcc.autoHeight('.hc-record-grid .hc-recimage');

    if ($.validator) {
        $.validator.setDefaults({ ignore: ".ignore-val, :hidden" });
    }

    $.fn.hcFormMessage = function (status, message) {
        var $this = $(this).html(message).addClass("dnnFormMessage");

        if (status == 'OK')
            $this.removeClass("dnnFormValidationSummary");
        else
            $this.addClass("dnnFormValidationSummary");

        $("body").scrollTo($this);
    };
});
