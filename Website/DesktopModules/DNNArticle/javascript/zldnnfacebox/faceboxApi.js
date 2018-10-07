function PopUp(ajax, popWidth, popHeight, userAnimate, refrashWindow, cssContainer) {
    var aurl = ajax != "" ? ajax : "";
    var pxWidth = $(window).width() - 400;
    if (popWidth > 0) pxWidth = popWidth;
    var pxheight = $(window).height() - 280;
    if (popHeight > 0) pxheight = popHeight;
    var animate = true;
    if (userAnimate != null) animate = userAnimate;
    var objrw = window;
    if (refrashWindow != null) objrw = refrashWindow;
    var cssName = "";
    if (cssContainer != null) cssName = cssContainer;
    with (top) {
        $.facebox({
            ajax: aurl,
            refrashWindow: objrw,
            popWidth: pxWidth,
            popHeight: pxheight,
            userAnimate: animate,
            cssContainer: cssName,
            closeText: "Close",
            submitText: "Submit",
            changesizeText: "Resize"
        });
    }
    ;
    return false;
}

function PopUp(ajax) {
    var aurl = ajax != "" ? ajax : "";
    var objrw = window;
    with (top) {
        $.facebox({
            ajax: aurl,
            opacity: .5 , 
            refrashWindow: objrw,
            popWidth: $(window).width() - 400,
            popHeight: $(window).height() - 280,
            userAnimate: true,
            cssContainer: "",
            closeText: "Close",
            submitText: "Submit",
            changesizeText: "Resize"
        });
    }
    ;
    return false;
}

function PopUpNoSubmit(ajax) {
    var aurl = ajax != "" ? ajax : "";
    var objrw = window;
    with (top) {
        $.facebox({
            ajax: aurl,
            opacity: .5,
            refrashWindow: objrw,
            popWidth: $(window).width() - 400,
            popHeight: $(window).height() - 280,
            userAnimate: true,
            cssContainer: "",
            submitVisable:false,
            closeText: "Close",
            changesizeText: "Resize"
        });
    }
    ;
    return false;
}


function PopUpNoActionText(ajax) {
    var aurl = ajax != "" ? ajax : "";
    var objrw = window;
    with (top) {
        $.facebox({
            ajax: aurl,
            opacity: .5,
            refrashWindow: objrw,
            popWidth: $(window).width() - 400,
            popHeight: $(window).height() - 280,
            userAnimate: true,
            cssContainer: ""
        });
    }
    ;
    return false;
}