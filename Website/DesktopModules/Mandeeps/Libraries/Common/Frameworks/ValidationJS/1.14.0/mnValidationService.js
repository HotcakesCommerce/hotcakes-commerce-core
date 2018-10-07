var mnValidationService = {

    AddCustomValidation: function () {

        $.validator.addMethod('ckrequired', function (value, element) {
            var ele = $(element).val();
            if (ele != "") {
                var re = /^[a-zA-Z0-9]+$/;
                if (re.test(ele))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }, $.validator.messages.ckrequired);

        $.validator.addMethod('cbrequired', function (value, element) {
            if ($(element).closest('.ms-form-group').find("input:checked").length < 1)
                return false;
            else
                return true;
        }, $.validator.messages.cbrequired);

        $.validator.addMethod('numeric', function (value, element) {
            var ele = $(element).val();
            if (ele != "") {
                var re = new RegExp("^[0-9]{" + ele.length + "}$");
                if (re.test(ele))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }, $.validator.messages.numberrequired);

        $.validator.addMethod('mlength', function (value, element) {
            var ele = $(element).val();
            if (ele != "") {
                //var re = new RegExp("^[0-9]{" + ele.length + "}$");
                if (ele.length <= 6)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }, $.validator.messages.invalidlength);

        $.validator.addMethod('alphanumeric', function (value, element) {
            var ele = $(element).val();
            if (ele != "") {
                var re = /^[a-zA-Z0-9]+$/;
                if (re.test(ele))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }, $.validator.messages.requiredalphanumeric);

        $.validator.addMethod('alphabetic', function (value, element) {
            var ele = $(element).val();
            if (ele != "") {
                var re = /^[a-zA-Zs]+$/;
                if (re.test(ele))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }, $.validator.messages.requiredalphabetic);

        $.validator.addMethod('validateurl', function (value, element) {
            var ele = $(element).val();
            if (ele != "") {
                var re = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;
                if (re.test(ele))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }, $.validator.messages.invalidurl);

        $.validator.addMethod('validtwitter', function (value, element) {
            var ele = $(element).val();
            if (ele.length > 160)
                return false;
            else
                return true;

        }, $.validator.messages.invalidtwitter);

    },
    editorrequired: function (item) {
        var result = true;
        if ($(item).parent('div').attr('editorrequired') != undefined && $(item).parent('div').attr('editorrequired') == "true") {
            $.each(CKEDITOR.instances, function (key, editor) {
                if ($(item).attr('ckeditor') == $(editor.element.$).attr('ckeditor')) {
                    if (editor.getData() == "") {
                        $("#label" + $(item).attr('ckeditor') + "").remove();
                        $("<label id=label" + $(item).attr('ckeditor') + " class=\"error\">" + $.validator.messages.required + "</label>").insertAfter($(item).parent('div').children('div'));
                        result = false;
                    }
                    else {

                        $("#label" + $(item).attr('ckeditor') + "").remove();
                        result = true;

                        //if ($(item).attr("id").indexOf("tw") > -1 && editor.getData().length > 160)
                        //{
                        //    $("#label" +$(item).attr('twrequired') + "").remove();
                        //    $("<label id=label" +$(item).attr('twrequired') + " class=\"error\">[LS:InvalidTwitterMessage]</label>").insertAfter($(item).parent('div').children('div'));
                        //      result = false;
                        //    }




                    }
                }
            })
        }
        return result;
    },
    filerequired: function (item) {
        var result = true;
        if ($(item).attr('filerequired') == "true") {
            var uploader = $(item).attr('uploader');

            var scope = angular.element($(item)).scope();

            if (scope[uploader].queue.length == 0) {
                $("#label" + $(item).attr('uploader') + "").remove();
                if ($(item).is(':input'))
                    $("<label id=label" + $(item).attr('uploader') + " class=\"error\">" + $.validator.messages.required + "</label>").insertAfter($(item));
                else
                    $("<label id=label" + $(item).attr('uploader') + " class=\"error\">" + $.validator.messages.required + "</label>").insertAfter($(item).children('div'));
                result = false;
            }
            else {
                $("#label" + $(item).attr('uploader') + "").remove();
                result = true;
            }
        }
        return result;
    },
    tagrequired: function (item) {
        var result = true;
        if ($(item).attr('tagrequired') == "true" && $(item).find('.tag-list li').length < 1) {
            $("#label" + $(item).parent('div').attr("data-name") + "").remove();
            $("<label id=label" + $(item).parent('div').attr("data-name") + " class=\"error\">" + $.validator.messages.required + "</label>").insertAfter($(item));
            result = false;
        }
        else {
            $("#label" + $(item).parent('div').attr("data-name") + "").remove();
            result = true;
        }
        return result;
    },
    DoValidationAndSubmit: function (sender, identifier) {
        this.AddCustomValidation();

        // Ascend from the button that triggered this click sender 
        //  until we find a container element flagged with 
        //  .validationGroup and store a reference to that element.
        var group;
        if (sender != undefined && sender.length > 0)
            group = $('#' + sender).parents('.validationGroup');
        else
            group = $('[identifier="' + identifier + '"]');
        var isValid = true;

        // Descending from that .validationGroup element, find any input
        //  elements within it, iterate over them, and run validation on 
        //  each of them.
        //$.validator.messages.required = '[LS:Required]';
        $(group).validate();
        $(group).find(':input').each(function (i, item) {
            if ($(item).is(':visible')) {
                if (!$(item).valid()) {
                    isValid = false;
                }
            }
            else if ($(item).attr('ckeditor') != undefined) {
                if (!mnValidationService.editorrequired(item))
                    isValid = false;
            }
        });

        //fix for dropdown validation
        $(group).find('select').each(function (i, item) {
            if ($(item).is(':visible')) {
                if (!$(item).valid()) {
                    isValid = false;
                }
            }
        });

        $(group).find('[uploader][filerequired]').each(function (key, control) {
            if (!mnValidationService.filerequired(control))
                isValid = false;
        });

        $(group).find('tags-input').each(function (key, control) {
            if (!mnValidationService.tagrequired(control))
                isValid = false;
        });

        return isValid;
    }
};


$(document).ready(function () {
    // Initialize validation on the entire ASP.NET form.
    jQuery.validator.setDefaults({
        //debug: true,
        onsubmit: false,
        ignore: ".novalidate",
        rules: {
            success: function (error) {
                error.remove();
            }
        },
        errorPlacement: function (error, element) {

            if (element.closest('.ms-form-group').attr("data-datatype") == "checkboxlist") {
                error.insertAfter(element.closest('.ms-form-group').find('.ms-input-group'));
                return;
            }

            error.insertAfter(element);

        }
    });

});