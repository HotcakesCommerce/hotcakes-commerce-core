jQuery(function ($) {
    // Common ----------------------
    var $pageminicarts = $('.hc-minicart');    

    function InitCommon() {

        $pageminicarts.each(function (key, item) {

            var $minicart = $(item);
            var $target = $minicart.children('.hc-tooltip');
            var $iconbox = $minicart.children(".hc-iconbox");
            var $cartActions = $minicart.children('.hcMiniCartTooltip').find('.dnnActions');
            $.data($minicart, 'itemsRendered', false);

            if ($cartActions.length) {
                $cartActions.hide();
            }

            if ($target.length) {
                $minicart.hover(function () {
                    $iconbox.css("background-color", "#FFF");
                    $target.slideDown().position(
                        'right'
                    );
                    LoadItems($minicart);
                }, function () {
                    $target.slideUp(function () {
                        $iconbox.css("background-color", "");
                    });
                });
            }
        });        
    }

    function LoadItems(minicart) {
        if (jQuery.data(minicart, "itemsRendered"))
            return;

        $.data(minicart, 'itemsRendered', true);
        $.post(hcc.getServiceUrl("Cart/MiniCartItems"), {}, function (data) {
            RenderMiniCartItems(data, minicart);
            if ($cartActions.length) {
                $cartActions.show();
            }
        });
    }

    function RenderMiniCartItems(cartViewModel, minicart) {

        var $target = minicart.children('.hc-tooltip');
        $grid = $target.find('.dnnGrid');

        if ($grid.length) {
            $ajaxLoader = $grid.find('.hcAjaxLoader');

            if ($ajaxLoader.length)
                $ajaxLoader.remove();

            var builtTable = '';
            $.each(cartViewModel.LineItems, function (key, item) {
                builtTable += '<tr><td>';

                if (item.ShowImage)
                    builtTable += '<a href="' + item.LinkUrl + '" title="' + item.Item.ProductName + '"><img src="' + item.ImageUrl + '" alt="' + item.Item.ProductName + '" /></a>';
                else
                    builtTable += '&nbsp;';

                builtTable += '</td>';

                builtTable += '<td><a href="' + item.LinkUrl + '" title="' + item.Item.ProductName + '">' + item.Item.ProductName + '</a></td>';

                builtTable += '<td>';
                if (item.HasDiscounts) {
                    builtTable += '<div class="hc-discount">$' + item.Item.LineTotalWithoutDiscounts.toFixed(2) + '</div>';
                }
                builtTable += '$' + item.Item.LineTotal.toFixed(2);
                builtTable += '</td></tr>';
            });

            builtTable += '<tr class="hc-subtotal"><td colspan="2">' + hcc.l10n.miniCart_SubTotal + '</td><td>$' + cartViewModel.CurrentOrder.TotalOrderBeforeDiscounts.toFixed(2) + '</td></tr>';

            $grid.append(builtTable);
        }
    }

    $(document).ready(function () {
        InitCommon();
    });
});