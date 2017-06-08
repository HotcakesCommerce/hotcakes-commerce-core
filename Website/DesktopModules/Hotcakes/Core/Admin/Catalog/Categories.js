
function RemoveItem(lnk) {
    var id = $(lnk).attr('id');
    $.post('Categories_Delete.aspx',
        { "id": id.replace('rem', '') },
        function () {
            lnk.parent().parent().parent().parent().parent().slideUp('slow', function () {
                lnk.parent().parent().parent().parent().parent().remove();
            });
        }
    );
}

var curCategory;

function callBackFunRemoveItem(event) {
    event.data.param1.dialog('close');
    RemoveItem(curCategory);
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
                $.post('Categories_Sort.aspx',
                    {
                        "ids": sorted,
                        "bvin": $(this).attr('id')
                    });
            }
        });

    $('.ui-sortable').disableSelection();

    $('.trash').click(function (e) {
        curCategory = $(this);
        return hcConfirm(e, 'Delete this Category?', callBackFunRemoveItem);
    });

});

