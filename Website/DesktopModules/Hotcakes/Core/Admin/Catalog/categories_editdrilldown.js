
function RemoveItem(lnk) {
    var id = $(lnk).attr('id');
    $.post('Categories_FacetDelete.aspx',
        { "id": id.replace('rem', '') },
        function () {
            lnk.parent().parent().parent().parent().parent().slideUp('slow', function () {
                lnk.parent().parent().parent().parent().parent().remove();
            });
        }
    );
}


$(document).ready(function () {

    $(".ui-sortable").sortable(
        {
            placeholder: 'ui-state-highlight',
            axis: 'y',
            containment: 'parent',
            opacity: '0.75',
            cursor: 'move',
            update: function (event, ui) {
                var sorted = $(this).sortable('toArray');
                sorted += '';
                $.post('Categories_FacetSort.aspx',
                    { "ids": sorted,
                      "bvin": $(this).attr('id')
                    });
            }
        });

    $('.ui-sortable').disableSelection();

    $('.trash').click(function () {
        if (window.confirm('Delete this Filter?')) {
            RemoveItem($(this));
        }
        return false;
    });

});

// Jquery Setup
$(document).ready(function () {

    $("#NameField").change(function () {

        rawName = $("#NameField").val();
        cleanName = $("#RewriteUrlField").val();

        $.post('../Controllers/Slugify.aspx',
                        { "input": rawName },
                        function (data) {
                            if (cleanName == "") {
                                $("#RewriteUrlField").val(data);
                            }
                        });
    });


});