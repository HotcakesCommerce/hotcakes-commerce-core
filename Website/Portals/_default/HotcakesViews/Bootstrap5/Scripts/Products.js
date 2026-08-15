jQuery(function ($) {

    var currentmediumimage;

    function SubmitReviewForm() {
        var reviewform = $('#hcSubmitReviewForm');
        var destination = reviewform.attr('action');
        if (!destination)
            destination = reviewform.data('action');
        var postdata = reviewform.find("select, input, textarea").serialize();
        var outmessage = $('#hcSubmitReviewMessage');
        outmessage.html('');

        $.post(destination, postdata,
            function (data) {
                outmessage.html(data.message);
                outmessage.attr("class", data.ok ? "dnnFormMessage dnnFormSuccess" : "dnnFormMessage dnnFormWarning");
                outmessage.show();
                if (data.ok) {
                    reviewform.hide();
                }
            },
            "json");
    }
    function InitReviewForm() {
        $('#hcWriteReview #rating, #hcWriteReview #newreview').val('');
        $('#hcSubmitReviewMessage').hide();
        $('#hcSubmitReviewForm').show();
    }

    function UpdatePriceFormItem(price, $priceEl) {
        if (!price)
            $priceEl.hide();
        else {
            $priceEl.show();
            $priceEl.find("span").html(price.Text);
        }
    }

    function EvaluateSelections() {
        var $options = $('.hc-product-form').find('.hcIsOption, #productbvin');

        if ($options && $options.length > 0) {

            var $container = $('#hcProductDetails');
            var $valMessage = $('#hcValidationMessage');
            var $price = $('#hcPriceWrapper');
            var $actions = $('#hcProductActions');

            $actions.hide();
            $container.ajaxLoader("start");

            $.post(hcc.getServiceUrl("products/validate"),
                $options.serialize(),
                function (data) {
                    $valMessage.hide();

                    if (data.Message != null && data.Message.length > 0) {
                        $valMessage.find("div").html(data.Message);
                        $valMessage.show();
                    }

                    $('#hcProductImage').attr('src', data.MediumImageUrl);
                    $('#hcSku').html(data.Sku);
                    $('#hcStockDisplay').html(data.StockMessage);

                    if ($price) {
                        UpdatePriceFormItem(data.Prices.ListPrice, $price.find(".hc-listprice"));
                        UpdatePriceFormItem(data.Prices.SitePrice, $price.find(".hc-siteprice"));
                        UpdatePriceFormItem(data.Prices.YouSave, $price.find(".hc-yousave"));
                    }

                    if (!(data.IsValid === false)) {
                        $actions.show();
                    }
                }, 'json')
                .done(function (data) {
                    $container.ajaxLoader("stop");
                });
        }
    }
    function BindAdditionalImages() {

        var $addImages = $('#hcAdditionalImages a');
        var $img = $('#hcProductImage');
        $img.data('currentUrl', $img.attr('src'));

        $addImages.click(function () {
            var newurl = $(this).attr('href');
            $img.attr('src', newurl);
            $img.data('currentUrl', newurl);
            return false;
        });

        $addImages.mouseover(function () {
            var newurl = $(this).attr('href');
            $img.attr('src', newurl);
        });

        $addImages.mouseout(function () {
            var newurl = $(this).attr('href');
            $img.attr('src', $img.data('currentUrl'));
        });
    }

    function InitProductTabs() {
        var $tabs = $('#hcProductTabs');

        if (!$tabs.length) {
            return;
        }

        // Remove inline display styles left by the legacy hcTabs plugin.
        $tabs.find('.tab-content > .tab-pane').each(function () {
            this.style.removeProperty('display');
        });

        // Bootstrap 5 uses data-bs-toggle/data-bs-target to manage clicks.
        $tabs.find('[data-bs-toggle="tab"]').on('click', function (e) {
            e.preventDefault();

            var targetSelector = $(this).attr('data-bs-target');
            var target = document.querySelector(targetSelector);

            if (!target) {
                return;
            }

            $tabs.find('.nav-link').removeClass('active')
                .attr('aria-selected', 'false');

            $tabs.find('.tab-pane').removeClass('active show');

            $(this).addClass('active')
                .attr('aria-selected', 'true');

            $(target).addClass('active show');
        });
    }

    function Init() {
        $(".inventoryoutofstock").parent().addClass("inventoryoutofstock");

        BindAdditionalImages();

        // Review Form Submit
        $('#hcSubmitReviewBtn').click(function () {
            SubmitReviewForm();
            return false;
        });
        $('#hcWriteReviewbtn').click(function () {
            InitReviewForm();
        });

        $(".hcIsOption").change(function () {
            EvaluateSelections();
            return true;
        });

        EvaluateSelections();

        InitProductTabs();

        $('#giftcardpredefined').change(function () {
            var $input = $('#GiftCardAmount');
            var amount = $(this).val();

            if (amount == "") {
                $input.css("visibility", "");
            } else {
                $input.css("visibility", "hidden");
            }

            $input.val(amount);
        });
    }

    // Initialization ---------------

    Init();
});