function textblur(e) {

    var objtext = e.target || e.srcElement;
    if (objtext == null) {
        return;
    }
    if (!objtext.getAttribute("vbr")) {
        return;
    }
    var objtextvalue = objtext.getAttribute("vbr");
    var vlang = objtext.getAttribute("vbrl").toString().split("|");

    if (objtext.value == "" || objtext.value == objtextvalue) {
        objtext.value = objtextvalue;
        objtext.style.color = "#A0A0A0";
        objtext.className = vlang[2].toString();
        objtext.type = "text";
    }

}



function textblur_all() {

    var frm = document.forms[0];
    var objtextvalue;
    var vlang;
    
    for (i = 0; i < frm.elements.length; i++) {
        if ((frm.elements[i].type == "text" || frm.elements[i].type == "password" || frm.elements[i].type == "textarea") && frm.elements[i].getAttribute("vbr") && (frm.elements[i].value == "" || frm.elements[i].value == frm.elements[i].getAttribute("vbr").toString())) {

            objtextvalue = frm.elements[i].getAttribute("vbr");
            vlang = frm.elements[i].getAttribute("vbrl").toString().split("|");

            frm.elements[i].value = objtextvalue.toString();
            frm.elements[i].style.color = "#A0A0A0";
            frm.elements[i].className = vlang[2].toString();
            frm.elements[i].type = "text";

        }
    }
    
}

function textactive(e) {

    var objtext = e.target || e.srcElement;
    if (objtext == null) {
        return;
    }
    if (!objtext.getAttribute("vbr")) {
        return;
    }
    var objtextvalue = objtext.getAttribute("vbr");
    var vlang = objtext.getAttribute("vbrl").toString().split("|");

    if (objtext.value == objtextvalue) {
        objtext.value = "";
        objtext.style.color = "#000000";
        switch (vlang[0].toString()) {
            case "en":
                objtext.className = vlang[1].toString();
                break;
            case "fa":
                objtext.className = vlang[2].toString();
                break;
            case "ps":
                objtext.className = vlang[1].toString();
                objtext.type = "password";
                break;
        }
    }

}

function text_numeric(e) {

    var kCode="0";
    if (e.keyCode) {
        kCode = e.keyCode;
    } else if (e.which) {
        kCode = e.which;
    } else {
        kCode = e.charCode;
    }

    if (!(kCode == 48 || kCode == 49 || kCode == 50 || kCode == 51 || kCode == 52 || kCode == 53 || kCode == 54 || kCode == 55 || kCode == 56 || kCode == 57 || kCode == 37 || kCode == 38 || kCode == 39 || kCode == 40 || kCode == 46 || kCode == 8 || kCode == 35 || kCode == 36 || kCode == 96 || kCode == 97 || kCode == 98 || kCode == 99 || kCode == 100 || kCode == 101 || kCode == 102 || kCode == 103 || kCode == 104 || kCode == 105)) {
        return false;
    }
}

function OpenURL(vurl, vtarget) {

    var vroot;
    if (window.location.origin) {
        vroot = window.location.origin;
    } else {
        vroot = window.location.href;
    }
    //vroot = vroot.toString() + "/ittic";
    vurl = vurl.replace("~", vroot.toString());
    //if ((vtarget != null || vtarget != undefined) && vtarget.toString() == "top") {
    window.top.location.href = (vurl.toString());
    //} else {
    //    window.location.href = (vurl.toString());
    //}

}

