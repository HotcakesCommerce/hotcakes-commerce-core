jQuery(function ($) {

    var API_GETREGIONS = hcc.getServiceUrl("estimateshipping/getregions");
    var API_VALIDATE = hcc.getServiceUrl("account/addressbook/validateaddress");

    var Address = {
        init: function ($form) {
            this.showDialog = true;
            this.$form = $form;
            this.$divMsg = $form.find("#hcAddressValidation");
            this.$formCountry = $form.find("#CountryBvin");
            this.$formAddress = $form.find("#Line1");
            this.$formAddress2 = $form.find("#Line2");
            this.$formCity = $form.find("#City");
            this.$formZip = $form.find("#PostalCode");
            this.$formState = $form.find("#RegionBvin");
            this.normalizedAddress = null;

            this.$submitButton = $(".hcHandleNormalization");
            this.$dialog = $form.find("#hcNormalizedAddressDlg");
            this.$dialogNormalizedAddr = this.$dialog.find(".hcNormalizedAddress");
            this.$dialogOriginalAddr = this.$dialog.find(".hcOriginalAddress");

            this.bindEvents();
            this.validateAddress();
            this.enableDialog(true);
        },
        bindEvents: function () {
            this.$formCountry.change(function (e) { Address.loadStates(e) });
            this.$form.find("input, select").change(function (e) { Address.addressChanged(e) });
            this.$form.find("#hcSaveNormalizedAction").click(function (e) { Address.saveNormalized(e) });
            this.$form.find("#hcSaveOriginalAction").click(function (e) { Address.saveOriginal(e) });
            this.$submitButton.click(function (e) { return Address.save(e) });
        },
        loadStates: function (e) {
            var self = this;
            var countryid = this.$formCountry.find(":selected").val();

            $.post(API_GETREGIONS + "/"+ countryid, { "regionid": '' }, null, "json")
                .done(function (data) { self.populateRegions(data.Regions); })
                .fail(function () { })
                .always(function () { });
        },
        populateRegions: function (rg) {
            this.$formState.html(rg);
            if (this.$formState.find("option").length == 1) {
                this.$formState.find("option").val("_");
            }
        },
        addressChanged: function (e) {
            this.validateAddress();
        },
        validateAddress: function (callback) {
            var self = this;
            var addr = this.getAddressFields();
            this.$divMsg.hide();

            if (addr.address != "" && addr.city != "" && addr.zip != "" && (addr.state != "" || this.$formState.find("option").length == 1)) {
                self.$divMsg.ajaxLoader('start');

                $.post(API_VALIDATE, addr, null, "json")
                    .done(function (data) { self.updateMessage(data); if (callback) callback(data); })
                    .fail(function () { })
                    .always(function () { self.$divMsg.ajaxLoader('stop'); });
            }
            else {
                if (callback) callback(null);
            };
        },
        getAddressFields: function () {
            return {
                country: this.$formCountry.find(':selected').val(),
                address: this.$formAddress.val(),
                address2: this.$formAddress2.val(),
                city: this.$formCity.val(),
                zip: this.$formZip.val(),
                state: this.$formState.find(':selected').val(),
            };
        },
        updateMessage: function (data) {
            if (data.Message != null && data.Message != "") {
                this.$divMsg.html(data.Message);
                this.$divMsg.show();
            }
        },
        saveNormalized: function (e) {
            this.$formAddress.val(this.normalizedAddress.Line1),
            this.$formAddress2.val(this.normalizedAddress.Line2),
            this.$formCity.val(this.normalizedAddress.City),
            this.$formZip.val(this.normalizedAddress.PostalCode),
            this.$formState.val(this.normalizedAddress.RegionBvin),

            this.$dialog.hcDialog("close");
            this.saveForm();
        },
        saveOriginal: function (e) {
            this.$dialog.hcDialog("close");
            this.saveForm();
        },
        save: function (e) {
            var self = this;
            if (this.showDialog) {
                e.stopPropagation();
                e.preventDefault();
                this.$form.ajaxLoader("start");
                this.validateAddress(function (res) {

                    self.$form.ajaxLoader("stop");

                    if (res != null && res.IsValid && res.NormalizedAddress != null) {
                        self.normalizedAddress = res.NormalizedAddress;
                        self.$dialogNormalizedAddr.html(res.NormalizedAddressHtml);
                        self.$dialogOriginalAddr.html(res.OriginalAddressHtml);
                        self.$dialog.hcDialog();
                    }
                    else {
                        self.saveForm();
                    }
                });

                return false;
            } else {
                return true;
            }
        },
        enableDialog: function (enable) {
            this.showDialog = enable;
            if (enable)
                this.$submitButton.data("nosubmit", true);
            else
                this.$submitButton.data("nosubmit", false);
        },
        saveForm: function () {
            this.enableDialog(false);
            this.$submitButton.click();
        }
    };

    Address.init($(".hcAddressEditor"));
});
