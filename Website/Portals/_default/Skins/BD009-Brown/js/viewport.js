jQuery(document).ready(function($) {
								
// StickyNav
$(".StickyNav").sticky({topSpacing:0});


 

// Mobile Menu
$(".menu_box").click(function () {
            if (!$(this).hasClass("active")) {
                $(".MobileMenu").slideDown().addClass("menu_hidden");
                $(this).addClass("active");
            }
            else {
                $(".MobileMenu").slideUp("normal",function(){
                   $(this).removeAttr("style").removeClass("menu_hidden");
                });
                $(this).removeClass("active");
            }
        });

// Search
$(".icon-search").click(function () {
            if (!$(this).hasClass("active")) {
                $(".search_bg").slideDown().addClass("search_hide");
                $(this).addClass("active");
            }
            else {
                $(".search_bg").slideUp("normal",function(){
                   $(this).removeAttr("style").removeClass("search_hide");
                });
                $(this).removeClass("active");
            }
        });

var s = "search...";

    //START dnnsoftware.ir
if ($('body').hasClass('rtl') || $('html').attr("lang") == 'fa-IR') {
    s = "جستجو...";
}
    //END dnnsoftware.ir
	$("#dnn_dnnSEARCH_txtSearch").val(s).click(function(){
	var ss=$(this).val();if(ss==s)$(this).val("") })
	.blur(function(){
	var ss=$(this).val();if(ss=="")$(this).val(s) });

});


$('head').append('<meta name="viewport" content="width=device-width,minimum-scale=1.0, maximum-scale=2.0"/>');








