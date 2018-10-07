
function lang_click(vlang) {

    var objbutton = document.getElementById('BLangSel');
    var objhid = document.getElementById('HFLangSel');
    objhid.value = vlang.toString();
    objbutton.click();

}

function login_click() {

    var divlogin = document.getElementById('d_login_sub');

    switch (divlogin.getAttribute("vtype").toString()) {
        case "0":
            divlogin.style.display = "block";
            setTimeout(function () {
                divlogin.style.filter = 'alpha(opacity=100)';
                divlogin.style.opacity = '1';
                divlogin.style.top = (Number(divlogin.parentElement.parentElement.offsetHeight).toString() + "px");
            }, 20);
            divlogin.setAttribute("vtype", "1")
            break;
        case "1":
            divlogin.style.filter = 'alpha(opacity=0)';
            divlogin.style.opacity = '0';
            divlogin.style.top = (Number(divlogin.parentElement.parentElement.offsetHeight - 5).toString() + "px");
            divlogin.setAttribute("vtype", "0")
            setTimeout(function () {
                divlogin.style.display = "none";
            }, 300);
            break;
    }


}