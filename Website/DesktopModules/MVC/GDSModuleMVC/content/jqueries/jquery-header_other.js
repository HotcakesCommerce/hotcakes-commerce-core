function setheadersize_other() {

    var div_pic = document.getElementById('ContentPlaceHolderHeader_divbackP');
    var clw = document.documentElement.clientWidth || document.body.clientWidth;

    if (div_pic == null) {
        div_pic = document.getElementById('ContentPlaceHolder1_divbackP');
    }

    if (div_pic == null) {
        return;
    }

    if (Number(clw) <= 300) {
        clw = 300;
    }
    var vpic_w = clw;
    var vpic_h = Number(clw) / 5.714;
    vpic_h = Math.round(vpic_h);

    div_pic.style.height = vpic_h.toString() + "px";
    
}