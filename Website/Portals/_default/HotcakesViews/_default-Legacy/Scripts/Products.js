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
                .done(function () {
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

    function Init() {
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

        $('#hcProductTabs').dnnTabs({ selected: 0 });

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
