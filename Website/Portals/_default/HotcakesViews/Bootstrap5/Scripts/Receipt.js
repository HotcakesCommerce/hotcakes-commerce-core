jQuery(function ($) {

    function CheckChanged() {
        var chk = $('#chksetpassword');
        if (chk.attr('checked')) {
            $('#hcFirstPasswordForm').slideDown();
        }
        else {
            $('#hcFirstPasswordForm').slideUp();
        }
    }

    function SetFirstPassword() {
        //$('#changing').show();
        $("#hcSetFirstPassword").ajaxLoader("start");
        $('#hcPasswordMessage').html('');

        var userid = $('#userid').val();
        var passwordfield = $('#password').val();
        var orderbvinfield = $('#orderbvin').val();

        $.post(hcc.getServiceUrl("account/setfirstpassword"),
                {
                    "userid": userid,
                    "password": passwordfield,
                    "orderbvin": orderbvinfield
                },
                function (data) {
                    //$('#changing').hide();
                    $("#hcSetFirstPassword").ajaxLoader("stop");
                    if (data.Success == "True" || data.Success == true) {
                        $("#hcPasswordMessage").html(hcc.l10n.checkout_SetPasswordSuccess).attr("class", "dnnFormMessage dnnFormSuccess").show();

                        $("#hcFirstPasswordForm").hide();
                        $("#chksetpassword").parent().hide();
                    }
                    else {
                        $("#hcPasswordMessage").html(data.Messages).attr("class", "dnnFormMessage dnnFormWarning").show();
                    }
                },
                "json")
                .error(function () {
                    //$('#changing').hide();
                    $("#hcSetFirstPassword").ajaxLoader("stop");
                    $("#hcPasswordMessage").html(hcc.l10n.common_AjaxError).attr("class", "dnnFormMessage dnnFormError").show();
                })
                .complete(function () {
                    //$('#changing').hide();
                    $("#hcSetFirstPassword").ajaxLoader("stop");
                });
    }

    // Inititalization 

    $('#chksetpassword').click(function () { CheckChanged(); return true; });
    $('#setpasswordbutton').click(function () { SetFirstPassword(); return false; });
    CheckChanged();
});


