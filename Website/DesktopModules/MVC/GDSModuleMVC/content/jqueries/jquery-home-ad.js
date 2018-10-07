function home_ad_close() {
    var obj_main = document.getElementById("d_survey_home");

    obj_main.style.filter = "alpha(opacity=0)";
    obj_main.style.opacity = 0;

    setTimeout(function () {
        obj_main.style.display = "none";
    }, 500)

}