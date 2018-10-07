function pageScrollToPos(pos_y_des) {
    var pos_y_now = document.documentElement.scrollTop || document.body.scrollTop;
    var clh = document.documentElement.clientHeight || document.body.clientHeight;

    var vy = Number(pos_y_now) - Number(pos_y_des);
    vy = Math.abs(vy);

    vy = vy / 8;

    if (Number(vy) > 50) {
        vy = 50;
    }

    var vtype = 1;
    if (Number(pos_y_now) > Number(pos_y_des)) {
        pos_y_now = Number(pos_y_now) - vy - 1;
        vtype = 1;
    } else {
        pos_y_now = Number(pos_y_now) + vy + 1;
        vtype = 2;
    }

    window.scrollTo(0, pos_y_now);
    
    if (Number(vtype) == 1) {
        if (Number(pos_y_now) >= Number(pos_y_des)) {
            scrolldelay = setTimeout('pageScrollToPos(' + pos_y_des + ')', 15);
        }
    } else {
        if (Number(pos_y_now) <= Number(pos_y_des)) {
            scrolldelay = setTimeout('pageScrollToPos(' + pos_y_des + ')', 15);
        }
    }

}

function get_offset_left(obj) {
    objdiv = obj;
    var vout = Number(objdiv.offsetLeft);
    while (objdiv.offsetParent) {
        vout = Number(vout) + Number(objdiv.offsetParent.offsetLeft);

        if (objdiv == document.getElementsByTagName("body")[0]) {
            break;
        }
        else {
            objdiv = objdiv.offsetParent;
        }
    }
    return vout.toString();
}
function get_offset_top(obj) {
    objdiv = obj;
    var vout = Number(objdiv.offsetTop);
    while (objdiv.offsetParent) {
        vout = Number(vout) + Number(objdiv.offsetParent.offsetTop);

        if (objdiv == document.getElementsByTagName("body")[0]) {
            break;
        }
        else {
            objdiv = objdiv.offsetParent;
        }
    }
    return vout.toString();
}

function show_error_panel(vpanel_name, vobjname, verror_message, vtimeout) {

    var clw = document.documentElement.clientWidth || document.body.clientWidth;
    document.getElementById('d_panel_error_text').innerHTML = verror_message.toString();
    objdiv_showhide('d_panel_error', 'show');
    setTimeout(function () {
        objdiv_showhide('d_panel_error', 'hide');
    }, Number(vtimeout));

    if (vpanel_name != "") {
        var vtop = get_offset_top(document.getElementById(vpanel_name.toString()));
        if (Number(clw) > 850) {
            vtop = Number(vtop) - 100;
        }
    }

    pageScrollToPos(vtop);

    var vobjfocus;
    
    if (vobjname != "") {
        vobjfocus = document.getElementById(vobjname.toString());
        if (vobjfocus != null) {
            vobjfocus.focus();
        } else {
            vobjfocus = document.getElementById("ContentPlaceHolder1_" + vobjname.toString());
            if (vobjfocus != null) {
                vobjfocus.focus();
            }
        }
    }

}

function Key_Hide_Proc(e) {

    var obj_key = e.target || e.srcElement;
    obj_key.style.display = "none";

}

function show_message_panel(verror_message) {

    var obj_test = document.getElementById('d_panel_wait_text');
    if (obj_test == null) {
        setTimeout("show_message_panel('" + verror_message.toString() + "')", 100);
        return;
    }

    document.getElementById('d_panel_wait_text').innerHTML = verror_message.toString();
    objdiv_showhide('d_panel_wait', 'show');

}

var vview_totaltop = [];
var vview_name = [];
var vview_action = [];

function div_view_trans(vdivnames, vactions) {

    vview_name = vdivnames.toString().split(",");
    vview_action = vactions.toString().split(",");

    for (var i = 0; i < vview_name.length; i++) {

        var objdiv = document.getElementById(vview_name[i].toString());

        if (objdiv != null) {
            switch (vview_action[i].toString().toLowerCase()) {
                case "top":
                    objdiv.style.transform = "translate(0px,-200px)";
                    break;
                case "bottom":
                    objdiv.style.transform = "translate(0px,200px)";
                    break;
                case "left":
                    objdiv.style.transform = "translate(-200px,0px)";
                    break;
                case "right":
                    objdiv.style.transform = "translate(200px,0px)";
                    break;
                default:
                    //
            }

            objdiv.style.filter = 'alpha(opacity=0)';
            objdiv.style.opacity = 0;

            var vnodename = "";
            vview_totaltop[i] = 0;

            vview_totaltop[i] = get_offset_top(objdiv)
            

        }
    }

}

function check_div_view_trans() {

    var pos_y_now = document.documentElement.scrollTop || document.body.scrollTop;
    var clh = document.documentElement.clientHeight || document.body.clientHeight;
    var clw = document.documentElement.clientWidth || document.body.clientWidth;

    var clh_add = 500;

    if (Number(clw) < 1000) {
        clh_add = 200;
    }
    if (Number(clw) < 700) {
        clh_add = 200;
    }
    if (Number(clw) < 600) {
        clh_add = 200;
    }

    var vview_top = Number(pos_y_now) + Number(clh) - Number(clh_add);
    for (var i = 0; i < vview_name.length; i++) {

        if (Number(vview_top) >= Number(vview_totaltop[i])) {

            var objdiv = document.getElementById(vview_name[i].toString());
            objdiv.style.transform = "translate(0px,0px)";
            objdiv.style.filter = 'alpha(opacity=100)';
            objdiv.style.opacity = 1;
        }

    }

}




function check_top_mainmenu() {
    var obj_mainmenu_bottom = document.getElementById("d_mainmenu_bottom");
    var obj_image_logo = document.getElementById("Image_Logo");
    var obj_menu_back_main = document.getElementById("d_menu_back_main");

    var pos_y_now = document.documentElement.scrollTop || document.body.scrollTop;
    var clw = document.documentElement.clientWidth || document.body.clientWidth;

    var v_height_percent = pos_y_now;
    var v_logo_w = 170;
    var v_logo_h = 142;
    var v_logo_t = 0;
    var v_main_height = 180;    //220
    var v_main_colorbase = 0.4;

    if (Number(clw) < 1000) {
        v_logo_w = 140;
        v_logo_h = 120;
        v_main_height = 165;
    }
    if (Number(clw) < 850) {
        v_logo_w = 120;
        v_logo_h = 100;
        v_main_height = 150;
        v_main_colorbase = 0.4;
    }

    if (Number(v_height_percent) > 200) {
        v_height_percent = 200;
        obj_menu_back_main.style.borderBottom = "solid 1px #0078FF";
        obj_menu_back_main.style.boxShadow = "0px 5px 7px 0px rgba(0,0,0,0.5)";
    } else {
        obj_menu_back_main.style.borderBottom = "none";
        obj_menu_back_main.style.boxShadow = "none";
    }
    v_height_percent = Number(v_height_percent) / 20;

    obj_mainmenu_bottom.style.height = (80 - (4 * Number(v_height_percent))).toString() + "px";

    v_logo_w = ((Number(v_logo_w) * (100 - (Number(v_height_percent) * 3.7))) / 100);
    v_logo_h = ((Number(v_logo_h) * (100 - (Number(v_height_percent) * 3.7))) / 100);
    v_logo_t = 0 - (Number(v_height_percent) / 2);
    obj_image_logo.style.width = v_logo_w.toString() + "px";
    obj_image_logo.style.height = v_logo_h.toString() + "px";
    obj_image_logo.style.marginTop = v_logo_t.toString() + "px";

    obj_menu_back_main.style.height = (Number(v_main_height) - (6.5 * Number(v_height_percent))).toString() + "px";
    obj_menu_back_main.style.backgroundColor = "rgba(0,0,0," + (Number(v_main_colorbase) + (Number(v_height_percent) / 20)) + ")";
}




function objdiv_showhide(vobjname, vtypesrc) {
    var obj_div = document.getElementById(vobjname.toString());
    if (vtypesrc == "") {
        if (!(obj_div.getAttribute("vdis"))) {
            obj_div.setAttribute("vdis", "show");
        }
        vtype = obj_div.getAttribute("vdis");
    } else {
        vtype = vtypesrc;
    }

    if (vtype == 'hide') {
        if (vtypesrc == "") {
            obj_div.setAttribute("vdis", "show");
        }
        obj_div.style.filter = 'alpha(opacity=0)';
        obj_div.style.opacity = 0;
        setTimeout("objdiv_showhide_hide('" + vobjname.toString() + "')", 300);
    }
    if (vtype == 'show') {
        if (vtypesrc == "") {
            obj_div.setAttribute("vdis", "hide");
        }
        obj_div.style.filter = 'alpha(opacity=0)';
        obj_div.style.opacity = 0;
        obj_div.style.display = "block";
        setTimeout("objdiv_showhide_show('" + vobjname.toString() + "')", 5);
    }
}
function objdiv_showhide_hide(vobjname) {
    var obj_div = document.getElementById(vobjname.toString());
    obj_div.style.display = "none";
}
function objdiv_showhide_show(vobjname) {
    var obj_div = document.getElementById(vobjname.toString());
    obj_div.style.filter = 'alpha(opacity=100)';
    obj_div.style.opacity = 1;
}



function objdiv_up_down(vkey_name, vobj_name, vclass_name, vclass_name_o) {

    var obj_key = document.getElementById(vkey_name.toString());
    var obj_div = document.getElementById(vobj_name.toString());

    if (!(obj_key.getAttribute("vdis"))) {
        obj_key.setAttribute("vdis", "hide");
    }

    vtype = obj_key.getAttribute("vdis");

    if (vtype == 'hide') {
        obj_key.setAttribute("vdis", "show");
        obj_key.className = vclass_name_o.toString();
        obj_div.style.height = obj_div.scrollHeight.toString() + "px";
    }
    if (vtype == 'show') {
        obj_key.setAttribute("vdis", "hide");
        obj_key.className = vclass_name.toString();
        obj_div.style.height = "0px";
    }
}



var v_mobile_menu_cur_y = 0;
function check_mobile_menu() {

    var div_menu_app_main = document.getElementById("d_menu_app_main");
    var div_menu_app = document.getElementById("d_menu_app");
    
    var pos_y_now = document.documentElement.scrollTop || document.body.scrollTop;
    var cl_h = document.documentElement.clientHeight || document.body.clientHeight;
    var menu_h = div_menu_app.offsetHeight;
    var menu_t = div_menu_app_main.offsetTop;

    var vtype = 1;
    var vvalue = 0;

    if (Number(pos_y_now) == Number(v_mobile_menu_cur_y) || Number(cl_h) > Number(menu_h)) {
        return;
    }

    if (Number(pos_y_now) > Number(v_mobile_menu_cur_y)) {
        vtype = 1;
    } else {
        vtype = 2;
    }

    vvalue = Number(pos_y_now) - Number(v_mobile_menu_cur_y);

    switch (Number(vtype)) {
        case 1:
            menu_t = Number(menu_t) - Math.abs(Number(vvalue));
            if ((Number(menu_h) + Number(menu_t)) <= Number(cl_h)) {
                menu_t = 0 - (Number(menu_h) - Number(cl_h));
            }
            break;
        case 2:
            menu_t = Number(menu_t) + Math.abs(Number(vvalue));
            if (Number(menu_t) >= 0) {
                menu_t = 0;
            }
            break;
    }

    div_menu_app_main.style.top = menu_t.toString() + "px";

    v_mobile_menu_cur_y = pos_y_now;

}
