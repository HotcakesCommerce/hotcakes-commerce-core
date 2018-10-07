var CreatemCommonCaptcha = function (Selector, Identifier) {
    if (typeof grecaptcha === "undefined") {
        loadmandeepsJS("https://www.google.com/recaptcha/api.js?render=explicit", "mandeepsrecaptcha")
        setTimeout(function () {
            CreatemCommonCaptcha(Selector, Identifier)
        }, 250);
    }
    else {
        var allrecaptchas = jQuery(Selector + ' .mcommoncaptcha');
        var AllCaptchas = [];
        for (var i = 0; i < allrecaptchas.length; i++) {
            if (typeof jQuery(allrecaptchas[i]).attr("Identifier") === 'undefined') {
                if (typeof grecaptcha != 'undefined') {
                    var googleRecaptchaId = grecaptcha.render(allrecaptchas[i], {
                        'sitekey': jQuery(allrecaptchas[i]).attr('data-sitekey')
                    });
                    AllCaptchas.push({ CaptchaElement: allrecaptchas[i], GoogleCaptchaId: googleRecaptchaId });
                    jQuery(allrecaptchas[i]).attr('googleReCaptchaId', googleRecaptchaId).attr("Identifier", Identifier);
                }
            }

        }
        return AllCaptchas;
    }

};
var GetAllCaptchas = function (Selector, Identifier) {
    var allrecaptchas = jQuery(Selector + ' .mcommoncaptcha[identifier$="' + Identifier + '"]');
    var CaptchaResponseArray = [];
    for (var i = 0; i < allrecaptchas.length; i++) {
        CaptchaResponseArray.push({ Code: grecaptcha.getResponse(jQuery(allrecaptchas[i]).attr("googleReCaptchaId")), CaptchaResult: (grecaptcha.getResponse(jQuery(allrecaptchas[i]).attr("googleReCaptchaId")) != ""), Element: allrecaptchas[i] });
    }
    return CaptchaResponseArray;
};
var ValidatemCaptcha = function (Selector, Identifier, validatemessage) {
    var isvalid = true;
    var captchas = GetAllCaptchas(Selector, Identifier);

    if (captchas.length > 0) {
        $.each(captchas, function (key, row) {
            $("#" + row.Element.id + " .error").remove();
            if (!row.CaptchaResult) {
                $($("#" + row.Element.id)[0]).append('<label class="error">' + validatemessage + '</label>');
                isvalid = false;
            }
        });
    }
    return isvalid;
};
var loadmandeepsJS = function (url, uniqueid) {
    if (document.getElementById(uniqueid) === null) {
        var scriptTag = document.createElement('script');
        scriptTag.src = url;
        scriptTag.id = uniqueid;
        document.head.appendChild(scriptTag);
    }
};

