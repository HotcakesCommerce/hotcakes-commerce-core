
function Remove301(lnk) {    
    var id = lnk.attr('id');
    var idr = id.replace('remove', '');
    $.post('../content/CustomUrlRemove.aspx',
            { "id": idr },
            function () {
                lnk.parent().slideUp('slow', function () { lnk.parent().remove(); });
            }
            );             
}

$(document).ready(function () {

    $('.remove301').click(function () {
        Remove301($(this));
        return false;
    });

});