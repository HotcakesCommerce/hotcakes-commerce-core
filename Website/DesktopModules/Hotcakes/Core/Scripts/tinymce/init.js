
$(document).ready(function () {
    tinymce.init({
        selector: "textarea.tinymce",
        /*inline: true,*/
        plugins: "contextmenu,paste,fullscreen,table,link,code",
        toolbar: "undo redo styleselect bold italic alignleft aligncenter alignright bullist numlist outdent indent code",
        content_css: hcc.getResourceUrl("Scripts/tinymce/tinymce.css"),
        body_class: "tinymcebody",
        browser_spellcheck: true,
        contextmenu: true
        /*, skin: "oxide",
        height: 300,
        width: 300,
        ax_height: 500,
        max_width: 500,
        min_height: 100,
        min_width: 400,
        statusbar: false*/
    });
});