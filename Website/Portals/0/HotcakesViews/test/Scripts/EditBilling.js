jQuery(function ($) {
    var API_SAVEBILLINGINFO = hcc.getServiceUrl("editbilling/Save");
    var API_CLEANCC = hcc.getServiceUrl("editbilling/CleanCreditCard");

    // Edit Billing Dialog --------------------------

    var EditBilling = {
        init: function () {
            this.model = ko.mapping.fromJS(hcc.editBillingViewModel);
            this.$dialog = $("#hcBillingInfo");
            this.$form = $("#hcEditBilling");
            this.$saveBtn = $("#hcBillingInfoSave");
            this.$cancelBtn = $("#hcBillingInfoCancel");
            this.$message = this.$form.find(".hcValidationSummary");
            this.bindEvents();
        },
        bindEvents: function () {
            ko.applyBindings(this.model, this.$form[0]);

            this.$saveBtn.click(EditBilling.save);
            this.$cancelBtn.click(EditBilling.close);
            this.$form.find("#CardNumber").hcCardInput(".hc-card-icons", EditBilling.cleanCCNumber);
        },
        save: function (e) {
            var self = EditBilling;
            if (self.$saveBtn.data("nosubmit"))
                return;
            if (!hcc.formIsValid(self.$form))
                return;

            self.model.AddressModel.Countries([]);
            self.model.AddressModel.Regions([]);
            var data = ko.mapping.toJSON(self.model);

            self.$form.ajaxLoader('start');

            $.ajax(API_SAVEBILLINGINFO, {
                data: data,
                contentType: "application/json",
                type: "post"
            })
            .done(function (res) {

                if (res.Status == 'OK') {
                    EditBilling.close();
                    window.location.reload(true);
                }

                if (res.Message && res.Message != "") {
                    EditBilling.$message.hcFormMessage(res.Status, res.Message);
                } else {
                    EditBilling.$message.hide();
                }
            })
            .fail(function () { })
            .always(function () { self.$form.ajaxLoader('stop'); });
        },
        cleanCCNumber: function ($input) {

            $.post(API_CLEANCC, { number: $input.val() }, null, "json")
                .done(function (data) { $input.val(data); });
        },
        close: function (e) {
            EditBilling.$dialog.hcDialog("close");
        }
    };

    EditBilling.init();
});