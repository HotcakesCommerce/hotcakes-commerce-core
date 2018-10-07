
jQuery(function ($) {

    var API_CANCELSUBSCRIPTION = hcc.getServiceUrl("orderhistory/CancelSubscription");

    var Subscription = {
        init: function () {
            this.$message = $(".hc-order-details > .hcValidationSummary");
            this.$grid = $("#hcSubscriptionGrid");
            this.$btnCancel = $(".hcSubCancel");
            this.bindEvents();
        },
        bindEvents: function () {
            this.$btnCancel.click(Subscription.cancelSubscription);
        },
        cancelSubscription: function (e) {
            Subscription.$grid.ajaxLoader('start');

            var $btnCancel = $(this);

            $.post(API_CANCELSUBSCRIPTION,
                {
                    orderId: hcc.getUrlVar("id"),
                    lineItemId: $btnCancel.data("itemid")
                }, null, "json")
                .done(function (res) {
                    if (res.Status == 'OK') {
                        $btnCancel.hide().parent().find(".hcSubCancelled").show();
                    } else {
                        Subscription.$message.hcFormMessage(res.Status, res.Message);
                    }
                })
                .fail(function () { })
                .always(function () { Subscription.$grid.ajaxLoader('stop'); });
        }
    };

    Subscription.init();
});