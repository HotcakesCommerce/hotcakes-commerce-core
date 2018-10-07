var vtime;

function firsthideMenu() {
    for (var i = 1; i < 10; i++) {
        objmenu = document.getElementById("Menu" + i.toString() + "Sub");
        if (!(objmenu == null)) {
            objmenu.style.display = 'none';
        }
    }
}

function smkmenu(vmenu, v) {
    objmenu = document.getElementById("Menu" + vmenu.toString() + "Sub");

    if (!(objmenu == null)) {
        if (v == 1) {
            clearTimeout(vtime);
            if (objmenu.style.display == 'none') {
                objmenu.style.filter = 'alpha(opacity=0)';
                objmenu.style.opacity = 0;
                objmenu.style.display = 'block';
                smkmenushow(vmenu);
            }

            for (var i = 1; i < 10; i++) {
                objmenu = document.getElementById("Menu" + i.toString() + "Sub");
                if (!(objmenu == null || i == vmenu)) {
                    if (objmenu.style.display == 'block') {
                        objmenu.style.display = 'none';
                    }
                }
            }
            
        } else if (v == 2) {
            vtime = setTimeout('smkmenuhide(' + vmenu + ',100)', 500);
        }
    }
    return false;
}

function smkmenushow(vmenu) {

    var objmenu = document.getElementById("Menu" + vmenu.toString() + "Sub");

    objmenu.style.filter = 'alpha(opacity=100)';
    objmenu.style.opacity = 1;
    
}

function smkmenuhide(vmenu) {

    var objmenu = document.getElementById("Menu" + vmenu.toString() + "Sub");
    
    objmenu.style.filter = 'alpha(opacity=0)';
    objmenu.style.opacity = 0;

    setTimeout(function() {
        objmenu.style.display = 'none';
    },400);
}


function menuapp_click(vname, vm) {
    var divobjdetail = document.getElementById('menuapp_' + vname.toString());
    var divobjheader = document.getElementById('menuapp_' + vname.toString() + '_m');
    var vtype = divobjdetail.getAttribute('tp').toString();

    if (Number(vtype) == 1) {
        divobjdetail.setAttribute("tp", "2");
        if (Number(vm) == 1) {
            divobjdetail.style.height = 'auto';
        } else {
            divobjdetail.style.height = divobjdetail.scrollHeight.toString() + 'px';
            divobjheader.className = "KeyMenuAppSubItemUp";
        }
    } else {
        divobjdetail.setAttribute("tp", "1");
        divobjdetail.style.height = '0px';
        if (Number(vm) != 1) {
            divobjheader.className = "KeyMenuAppSubItem";
        }
    }
}





function menu_hotel_app_click() {

    var d_menu = document.getElementById("MBMenu1_Hotels_App");
    var d_menu_body = document.getElementById("MBMenu1_Hotels_App_Body");
    var vtype = 1;

    if (d_menu.hasAttribute("tp")) {
        vtype = d_menu.getAttribute("tp").toString();
    }

    if (Number(vtype) == 1) {

        d_menu.setAttribute("tp", "2");

        d_menu_body.style.height = d_menu_body.scrollHeight.toString() + "px";
        d_menu_body.style.filter = 'alpha(opacity=100)';
        d_menu_body.style.opacity = 1;

    } else {

        d_menu.setAttribute("tp", "1");

        d_menu_body.style.height = "0px";
        d_menu_body.style.filter = 'alpha(opacity=0)';
        d_menu_body.style.opacity = 0;

    }

}





function menu_hotel_res_click() {

    var pos_y_now = document.documentElement.scrollTop || document.body.scrollTop;
    var clh = document.documentElement.clientHeight || document.body.clientHeight;

    var d_menu = document.getElementById("d_hotelsmenu_key_res");
    var d_menu_body = document.getElementById("d_hotelsmenu_key_res_body");
    var d_menu_body_top = get_offset_top(d_menu_body);
    var vtop_now=0;
    var vtop_res = 0;
    var vtop_need = 0;
    var vtype = 1;

    if (d_menu.hasAttribute("tp")) {
        vtype = d_menu.getAttribute("tp").toString();
    }

    if (Number(vtype) == 1) {

        d_menu.setAttribute("tp", "2");

        d_menu_body.style.height = "320px";
        d_menu_body.style.filter = 'alpha(opacity=100)';
        d_menu_body.style.opacity = 1;

        vtop_now = (Number(clh) + Number(pos_y_now));
        vtop_res = (Number(d_menu_body_top) + 330);
        vtop_need = Number(pos_y_now) + (Number(vtop_res) - Number(vtop_now));
        if (vtop_now < vtop_res) {
            pageScrollToPos(vtop_need);
        }

    } else {

        d_menu.setAttribute("tp", "1");

        d_menu_body.style.height = "0px";
        d_menu_body.style.filter = 'alpha(opacity=0)';
        d_menu_body.style.opacity = 0;

    }

}

