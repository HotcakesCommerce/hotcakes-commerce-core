
$(document).ready(function () {

    $('textarea.tinymce').tinymce({

        // Location of TinyMCE script
        script_url: hcc.getResourceUrl("Scripts/tinymce/tiny_mce.js"),

        // General options
        theme: "advanced",

        body_class: "content",

        plugins : "safari,style,contextmenu,paste,fullscreen,xhtmlxtras",
 
        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,forecolor,|,pastetext,pasteword,|,bullist,numlist,|,link,unlink,anchor,|,charmap,fullscreen,code",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",
        theme_advanced_toolbar_location : "top",
        theme_advanced_toolbar_align : "left",
        theme_advanced_statusbar_location : "bottom",
        theme_advanced_resizing : false,

        //width:800,
        //height: 400,

        extended_valid_elements: "iframe[src|width|height|name|align|frameborder|webkitAllowFullScreen|mozallowfullscreen|allowFullScreen]",
 
        // Example content CSS (should be your site CSS)
        content_css: hcc.getResourceUrl("Scripts/tinymce/tinymce.css"),
        body_class: "tinymcebody"

        // Drop lists for link/image/media/template dialogs
        //template_external_list_url : "lists/template_list.js",
        //external_link_list_url : "lists/link_list.js",
        //external_image_list_url : "lists/image_list.js",
        //media_external_list_url : "lists/media_list.js",
 
        // Replace values for the template plugin
        //template_replace_values : {
        //username : "Some User",
        //staffid : "991234"}

       });
});

