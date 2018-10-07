function onlyfarsitype(event) {
    if (window.event) {
        var kCode = window.event.keyCode || window.event.charCode;
    }
    else if (event.which) {
        var kCode = event.which;
    } else {
        var kCode = event.charCode;
    }
    
    if (kCode == 13) {
        return false;
    }
    
    return kCode + 1;
}

function keyEnterDis(event) {
    if (window.event) {
        var kCode = window.event.keyCode || window.event.charCode;
    }
    else if (event.which) {
        var kCode = event.which;
    } else {
        var kCode = event.charCode;
    }

    if (kCode == 13) {
        return false;
    }
}

function InputNumericValuesOnly(event) {
    if (window.event) {
        var kCode = window.event.keyCode || window.event.charCode;
    }
    else if (event.which) {
        var kCode = event.which;
    } else {
        var kCode = event.charCode;
    }

    if (!(kCode == 48 || kCode == 49 || kCode == 50 || kCode == 51 || kCode == 52 || kCode == 53 || kCode == 54 || kCode == 55 || kCode == 56 || kCode == 57)) {
        return false;
    }
    if (kCode == 13) {
        return false;
    }
}

function InputTextValuesOnly(event) {
    if (window.event) {
        var kCode = window.event.keyCode || window.event.charCode;
    }
    else if (event.which) {
        var kCode = event.which;
    } else {
        var kCode = event.charCode;
    }

    if (event.keyCode == 48 || event.keyCode == 49 || event.keyCode == 50 || event.keyCode == 51 || event.keyCode == 52 || event.keyCode == 53 || event.keyCode == 54 || event.keyCode == 55 || event.keyCode == 56 || event.keyCode == 57) {
        return false;
    }

    if (kCode == 13) {
        return false;
    }
}


function digit3(event) {
    if (window.event) {
        var kCode = window.event.keyCode || window.event.charCode;
        var ve = window.event;
    }
    else if (event.which) {
        var kCode = event.which;
        var ve = event;
    } else {
        var kCode = event.charCode;
        var ve = event;
    }

    if (!(kCode == 48 || kCode == 49 || kCode == 50 || kCode == 51 || kCode == 52 || kCode == 53 || kCode == 54 || kCode == 55 || kCode == 56 || kCode == 57)) {
        return false;
    }
    if (kCode == 13) {
        return false;
    }
    if (kCode == 32) {
        return false;
    }

    var obj = document.getElementById(ve.srcElement.id);
    var vtext = obj.value;
    vtext = vtext.replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "");
    vtext = vtext.toString() + String.fromCharCode(kCode);
    var vlen = Number(vtext.length);
    if (vlen == 0) {
        return;
    }
    if (vlen > 14) {
        return;
    }
    vlen = vlen - 1;

    var newtext = "";
    var n = 0;
    for (var x = Number(vlen) ; x >= 0; x--) {
        if (n == 3) {
            newtext = " " + newtext.toString();
            n = 0;
        }
        newtext = vtext.substr(x, 1).toString() + newtext.toString();
        n = n + 1;
    }
    obj.value = newtext.toString();
    return false;
}

function digit3obj_en(vtext) {
    vtext = vtext.replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "");

    var vlen = Number(vtext.length);
    if (vlen == 0) {
        return vtext.toString();
    }
    vlen = vlen - 1;

    var newtext = "";
    var n = 0;
    for (var x = Number(vlen) ; x >= 0; x--) {
        if (n == 3) {
            newtext = " " + newtext.toString();
            n = 0;
        }
        newtext = vtext.substr(x, 1).toString() + newtext.toString();
        n = n + 1;
    }

    return newtext.toString();
}

function digit3obj_fa(vtext) {
    vtext = vtext.replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "").replace(" ", "");

    var vlen = Number(vtext.length);
    if (vlen == 0) {
        return vtext.toString();
    }
    vlen = vlen - 1;

    var newtext = "";
    var newtext_temp = "";
    var n = 0;
    for (var x = Number(vlen) ; x >= 0; x--) {
        if (n == 3) {
            newtext_temp = newtext_temp.substr(2, 1).toString() + newtext_temp.substr(1, 1).toString() + newtext_temp.substr(0, 1).toString() + " ";
            newtext = newtext.toString() + newtext_temp.toString();
            n = 0;
            newtext_temp = "";
        }
        newtext_temp = newtext_temp.toString() + vtext.substr(x, 1).toString();
        n = n + 1;
    }
    if (!newtext_temp == "") {
        for (var x = newtext_temp.length - 1 ; x >= 0; x--) {
            newtext = newtext.toString() + newtext_temp.substr(x, 1).toString();
        }
    }
    if (newtext.substr(newtext.length - 1, 1) == " ") {
        newtext = newtext.substr(0, newtext.length - 1);
    }

    return newtext.toString();
}

function tabnext(event, vdigit, vnextobjname, vmax) {
    if (window.event) {
        var kCode = window.event.keyCode || window.event.charCode;
        var ve = window.event;
    }
    else if (event.which) {
        var kCode = event.which;
        var ve = event;
    } else {
        var kCode = event.charCode;
        var ve = event;
    }

    if (!(kCode == 48 || kCode == 49 || kCode == 50 || kCode == 51 || kCode == 52 || kCode == 53 || kCode == 54 || kCode == 55 || kCode == 56 || kCode == 57)) {
        return false;
    }
    if (kCode == 32) {
        return false;
    }
    var vnextobj = document.getElementById("ContentPlaceHolder1_" + vnextobjname.toString());
    if (kCode == 13) {
        vnextobj.focus();
        return false;
    }

    var obj = document.getElementById(ve.srcElement.id);

    var selectedText;
    // IE version

    if (document.selection != undefined) {
        obj.focus();
        var sel = document.selection.createRange();
        selectedText = sel.text;
    }
        // Mozilla version
    else if (obj.selectionStart != undefined) {
        var startPos = obj.selectionStart;
        var endPos = obj.selectionEnd;
        selectedText = obj.value.substring(startPos, endPos)
    }


    if (selectedText != "") {
        return;
    }


    var vtext = obj.value;
    vtext = vtext.replace(" ", "").replace(" ", "").replace(" ", "");
    vtext = vtext.toString() + String.fromCharCode(kCode);
    var vlen = Number(vtext.length);
    if (Number(vlen) >= Number(vdigit)) {
        vtext = vtext.substr(0, Number(vdigit)).toString();
        if (Number(vtext) > Number(vmax)) {
            vtext = vmax.toString();
        }
        obj.value = vtext.toString();
        if (vnextobj != null) {
            vnextobj.focus();
        }
        return false;
    }
    return;
}