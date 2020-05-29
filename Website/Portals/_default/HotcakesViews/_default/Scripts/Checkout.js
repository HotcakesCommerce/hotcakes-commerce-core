jQuery(function ($) {

    // Common ----------------------

    function InitCommon() {

        // Handle required validators ----------
        $(".required").focusout(function () {
            if (!$(this).val()) {
                var msgId = "#" + $(this).attr("id") + "_error";
                $(msgId).show();
            }
        });

        // Handle Card Number plugin ----------
        $('#cccardnumber')
            .hcCardInput(".hc-card-icons",
                function ($input) {
                    $.post(hcc.getServiceUrl("checkout/CleanCreditCard"), { "CardNumber": $input.val() }, null, "json")
                        .done(function (data) {
                            $input.val(data.CardNumber);
                        });
                });

        $("#hcTakeOrder").click(function () {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    function ajaxErrorNotification() {
        $('#loginmessage').html(hcc.l10n.common_AjaxError).attr('class', 'col-xs-12 alert alert-danger').show();
    }

    // Login -----------------------

    function IsEmailKnown(forceSwitch, emailfieldid) {
        var emailfield = $(emailfieldid || '#customeremail').val().toLowerCase();
        $.post(hcc.getServiceUrl("checkout/IsEmailKnown"),
            {
                "email": emailfield
            },
            function (data) {
                if (data.success == "1") {
                    $('#hcLoginSection').show();
                    $('#loginmessage').html(hcc.l10n.checkout_PleaseLogin).attr('class', 'dnnFormMessage dnnFormSuccess').slideDown();

                    if (forceSwitch) {
                        $('#username').focus();
                        ClickLoginTab('#hcTabLogin');
                    }
                }
                else {
                    $('#loginmessage').attr('class', 'dnnFormMessage dnnFormError').slideUp();
                }
            },
            "json");
    }

    function LoginAjax() {
        $('#hcLoginSection').ajaxLoader("start");
        $('#loginmessage').hide();
        var username = $('#hcLoginSection #username').val();
        var passwordfield = $('#hcLoginSection #password').val();
        $.post(hcc.getServiceUrl("account/AjaxSignIn"),
            {
                "username": username,
                "password": passwordfield
            },
            function (data) {
                if (data.Success == "True" || data.Success == true) {
                    $('#loginmessage').html(hcc.l10n.checkout_LoggedIn).attr('class', 'dnnFormMessage dnnFormSuccess').show();
                    $('#hcLoginChoose').hide();
                    $('#hcTabLogin').hide();
                    UserLoggedIn();
                }
                else {
                    $('#loginmessage').html(hcc.l10n.checkout_LoginFailed).attr('class', 'dnnFormMessage dnnFormError').show();
                }
            },
            "json")
            .error(function () {
                ajaxErrorNotification();
            })
            .complete(function () {
                $('#hcLoginSection').ajaxLoader("stop");
            });
    }

    function UserLoggedIn() {
        $.ajax({
            type: 'POST',
            url: hcc.getServiceUrl("checkout/getcustomerdata"),
            dataType: 'json',
            success: function (data) {
                RewardPoints.displayRewardPoints(data.RewardPoints);
                Addresses.loadAddresses(data.Addresses, data.ShippingAddress, data.BillingAddress);
            },
            error: function (data) {
                ajaxErrorNotification();
            }
        });
    }

    function ChooseLoginTab(loginTabId) {
        $("#hcLoginChoose").parent().find(".hcTabPane").hide();
        $(loginTabId).show();
    }

    function ClickLoginTab(loginTabId) {
        $("#hcLoginChoose input[value='" + loginTabId + "']").click();
    }

    function CheckConfirmPassword() {
        if ($("#regconfirmpassword").val() != $("#regpassword").val()) {
            $("#regconfirmpassword_error").show();
        }
    }

    function InitLogin() {
        $('#loginmessage').hide();
        $('#customeremail').change(function () { IsEmailKnown(); });
        $('#regemail').change(function () { IsEmailKnown(false, '#regemail'); });
        $('#loginbutton').click(function () { LoginAjax(); return false; });
        $('#hcTabLogin').keydown(function (e) {
            if (e.which == 13) {
                LoginAjax();
                e.stopPropagation();
                e.preventDefault();
            }
        });
        $("#hcLoginChoose input").click(function () {
            $('#loginChoose0,#loginChoose1,#loginChoose2').prop('checked', false);
            $(this).prop('checked', true);
            ChooseLoginTab($(this).val());
        });
        $("#regconfirmpassword").focusout(function () { CheckConfirmPassword() });

        if (typeof (HCC) != "undefined" && typeof (HCC.CheckoutLogin) != "undefined") {
            if (HCC.CheckoutLogin.LoginTab) {
                ClickLoginTab(HCC.CheckoutLogin.LoginTab);
            } else {
                IsEmailKnown(true);
            }
        }
    }

    // Reward Points --------------------

    var RewardPoints = {
        init: function () {
            this.$radioButtons = $("input[name = 'userewardspoints']");
            this.$rpWrapper = $('#hccRewardPointsWrap');
            this.bindEvents();
        },
        bindEvents: function () {
            this.$radioButtons.change(function (e) { RewardPoints.changeRewardPoints(e); });
        },
        changeRewardPoints: function (e) {
            var self = RewardPoints;
            OrderSummary.showLoadingProgress();

            $.ajax({
                type: "POST",
                url: hcc.getServiceUrl("checkout/applyRewardPointsChange"),
                data: {
                    orderid: $('#orderbvin').val(),
                    userewardspoints: self.$radioButtons.filter(":checked").val()
                },
                dataType: "json",
                success: function (data) {
                    OrderSummary.displayTotals(data.totalsastable);
                    UpdatePaymentMethods(data.PaymentViewModel);
                    GiftCards.displayGiftCards(data.GiftCards);
                },
                error: function () { }
            });
        },
        displayRewardPoints: function (data) {
            var self = RewardPoints;

            if (data.ShowRewards) {
                self.$rpWrapper.show();
            } else {
                self.$rpWrapper.hide();
            }

            self.$rpWrapper.find('h3').html(data.LabelRewardPoints);
            self.$rpWrapper.find('label:first').text(data.RewardPointsAvailable);
            self.updatePointsAmount(data.LabelRewardsUse);
        },
        updatePointsAmount: function (points) {
            $('#userewardspointslabel1').text(points);
        }
    };

    // Shipping/Billing ------------

    var Addresses = {
        init: function () {
            this.isInitializing = true;

            this.showDialog = true;
            this.$billingWrapper = $('#hcBillingWrapper');
            this.$chkShowBilling = $('#chkbillsame');

            this.$shAvailableAddresses = $('#shippingAvailableAddresses');
            this.$shCountry = $('#shippingcountry');
            this.$shState = $('#shippingstate');
            this.$shFirstname = $('#shippingfirstname');
            this.$shLastname = $('#shippinglastname');
            this.$shAddress = $('#shippingaddress');
            this.$shAddress2 = $('#shippingaddress2');
            this.$shCity = $('#shippingcity');
            this.$shZip = $('#shippingzip');
            this.$shAll = $('#shippingcountry, #shippingstate, #shippingfirstname, #shippinglastname, #shippingaddress, #shippingaddress2, #shippingcity, #shippingzip');
            this.$deliveryWarning = $('#hcShippingNotValid');
            this.$shippingMessage = $('#hcShippingValidation');
            this.shippingNmAddr = null;

            this.$blAvailableAddresses = $('#billingAvailableAddresses');
            this.$blCountry = $('#billingcountry');
            this.$blState = $('#billingstate');
            this.$blFirstname = $('#billingfirstname');
            this.$blLastname = $('#billinglastname');
            this.$blAddress = $('#billingaddress');
            this.$blAddress2 = $('#billingaddress2');
            this.$blCity = $('#billingcity');
            this.$blZip = $('#billingzip');
            this.$blAll = $('#billingcountry, #billingstate, #billingfirstname, #billinglastname, #billingaddress, #billingaddress2, #billingcity, #billingzip');
            this.$billingMessage = $('#hcBillingValidation');
            this.billingNmAddr = null;

            this.$submitButton = $("#hcTakeOrder");
            this.$dialog = $("#hcNormalizedAddressDlg");
            this.$dialogShippingBlock = this.$dialog.find(".hcShipping");
            this.$dialogBillingBlock = this.$dialog.find(".hcBilling");
            this.$dialogShippingNormalized = this.$dialogShippingBlock.find(".hcNormalizedAddress");
            this.$dialogShippingOriginal = this.$dialogShippingBlock.find(".hcOriginalAddress");
            this.$dialogShippingRadio = this.$dialogShippingBlock.find("[name='shipping']");
            this.$dialogBillingNormalized = this.$dialogBillingBlock.find(".hcNormalizedAddress");
            this.$dialogBillingOriginal = this.$dialogBillingBlock.find(".hcOriginalAddress");
            this.$dialogBillingRadio = this.$dialogBillingBlock.find("[name='billing']");

            this.bindEvents();
            this.toggleBilling();
            this.shippingChanged();
            this.billingChanged();

            this.loadRegionsWithSelection($('#shippingstate'), $('#shippingcountry option:selected').val(), $('#shippingtempregion').val(), !this.isInitializing);
            this.loadRegionsWithSelection($('#billingstate'), $('#billingcountry option:selected').val(), $('#billingtempregion').val(), !this.isInitializing);

            this.forceAddressChange();

            this.isInitializing = false;
        },
        bindEvents: function () {
            var self = this;

            this.$chkShowBilling.change(function (e) { Addresses.toggleBilling(e); });
            this.$shAvailableAddresses.change(function (e) { Addresses.selectedAddressChanged(e); });

            this.$shCountry.change(function () {
                self.loadRegionsWithSelection(self.$shState, $('#shippingcountry :selected').val(), '');
            });
            this.$shState.change(function () {
                $('#shippingtempregion').val($(this).val());
            });
            this.$shAll.change(function (e) { Addresses.shippingChanged(e); });

            this.$blAvailableAddresses.change(function (e) { Addresses.selectedAddressChanged(e); });
            this.$blCountry.change(function () {
                self.loadRegionsWithSelection(self.$blState, $('#billingcountry :selected').val(), '');
            });
            this.$blState.change(function () {
                $('#billingtempregion').val($(this).val());
            });
            this.$blAll.change(function (e) { Addresses.billingChanged(e); });

            $("#hcSaveNormalizedAction").click(function (e) { Addresses.saveNormalized(e); });
            this.$submitButton.click(function (e) {
                $.xhrPool.abortAll();
                Addresses.save(e);
            });
        },
        toggleBilling: function (e) {
            if (this.$chkShowBilling.is(':checked')) {
                this.$billingWrapper.hide();
                this.$dialogBillingBlock.hide();
            }
            else {
                this.$billingWrapper.show();
                this.$dialogBillingBlock.show();
            }
            this.billingChanged();
        },
        getShippingData: function () {
            return {
                country: this.$shCountry.find(':selected').val(),
                state: this.$shState.find(':selected').val(),
                firstname: this.$shFirstname.val(),
                lastname: this.$shLastname.val(),
                address: this.$shAddress.val(),
                address2: this.$shAddress2.val(),
                city: this.$shCity.val(),
                zip: this.$shZip.val()
            };
        },
        isShippingValid: function () {
            var data = this.getShippingData();
            if (this.isEmpty(data.firstname)) { return false; }
            if (this.isEmpty(data.lastname)) { return false; }
            if (this.isEmpty(data.address)) { return false; }
            if (this.isEmpty(data.city)) { return false; }
            if (!data.zip || data.zip.length < 1) { return false; }
            return true;
        },
        isEmpty: function (input) {
            return !(typeof (input) == "undefined" || input.length > 0);
        },
        shippingChanged: function (e) {
            $('#hcShippingRates').html('');

            if (this.isShippingValid()) {
                RefreshShippingRates();
                this.$deliveryWarning.hide();
            }
            else {
                this.$deliveryWarning.show();
            }

            if (!this.isInitializing)
                this.forceAddressChange();
        },
        billingChanged: function (e) {
            if (!this.isInitializing)
                this.forceAddressChange();
        },
        handleAddressValidation: function (data) {
            var shRes = data.ShippingValidationResult;
            var blRes = data.BillingValidationResult;

            this.$shippingMessage.hide();
            if (this.$shAddress.val() != "" && this.$shCity.val() != "" && this.$shZip.val() != "" && this.$shState.val() != "") {
                if (shRes != null && shRes.Message != null && shRes.Message != "") {
                    this.$shippingMessage.html(shRes.Message);
                    this.$shippingMessage.show();
                }
            }

            this.$billingMessage.hide();
            if (this.$blAddress.val() != "" && this.$blCity.val() != "" && this.$blZip.val() != "" && this.$blState.val() != "") {
                if (blRes != null && blRes.Message != null && blRes.Message != "") {
                    this.$billingMessage.html(blRes.Message);
                    this.$billingMessage.show();
                }
            }
        },
        enableDialog: function (enable) {
            this.showDialog = enable;
            if (enable)
                this.$submitButton.attr("data-nosubmit", "true");
            else
                this.$submitButton.removeAttr("data-nosubmit");
        },
        saveNormalized: function (e) {

            if (this.shippingNmAddr != null && this.$dialogShippingRadio.filter(":checked").val() == "N") {
                this.$shAddress.val(this.shippingNmAddr.Line1);
                this.$shAddress2.val(this.shippingNmAddr.Line2);
                this.$shCity.val(this.shippingNmAddr.City);
                this.$shZip.val(this.shippingNmAddr.PostalCode);
                this.$shState.val(this.shippingNmAddr.RegionBvin);
            }

            if (this.billingNmAddr != null && this.$dialogBillingRadio.filter(":checked").val() == "N") {
                this.$blAddress.val(this.billingNmAddr.Line1);
                this.$blAddress2.val(this.billingNmAddr.Line2);
                this.$blCity.val(this.billingNmAddr.City);
                this.$blZip.val(this.billingNmAddr.PostalCode);
                this.$blState.val(this.billingNmAddr.RegionBvin);
            }

            this.$dialog.hcDialog("close");
            this.saveForm();
        },
        save: function (e) {
            var self = this;
            if (this.showDialog) {
                e.stopPropagation();
                e.preventDefault();
                $(".hc-checkout").ajaxLoader("start");

                this.applyAddressChange(function (res) {

                    $(".hc-checkout").ajaxLoader("stop");

                    var shRes = res.ShippingValidationResult;
                    var blRes = res.BillingValidationResult;
                    var showShippingNm = (shRes != null && shRes.NormalizedAddress != null);
                    var showBillingNm = (blRes != null && blRes.NormalizedAddress != null);

                    if (!showShippingNm && !showBillingNm) {
                        self.saveForm();
                    }
                    else {
                        if (showShippingNm) {
                            self.shippingNmAddr = shRes.NormalizedAddress;
                            self.$dialogShippingNormalized.html(shRes.NormalizedAddressHtml);
                            self.$dialogShippingOriginal.html(shRes.OriginalAddressHtml);
                            self.$dialogShippingBlock.show();
                        }
                        else {
                            self.$dialogShippingBlock.hide();
                        }

                        if (showBillingNm) {
                            self.billingNmAddr = blRes.NormalizedAddress;
                            self.$dialogBillingNormalized.html(blRes.NormalizedAddressHtml);
                            self.$dialogBillingOriginal.html(blRes.OriginalAddressHtml);
                            self.$dialogBillingBlock.show();
                        }
                        else {
                            self.$dialogBillingBlock.hide();
                        }

                        if (showShippingNm && showBillingNm) {
                            self.$dialog.hcDialog({ height: "500" });
                        } else {
                            self.$dialog.hcDialog({ height: "300" });
                        }
                    }
                });
                return false;
            }

            return true;
        },
        saveForm: function () {
            this.enableDialog(false);
            this.$submitButton.click();
        },
        forceAddressChange: function () {
            OrderSummary.showLoadingProgress();

            this.applyAddressChange(function (data) {
                OrderSummary.displayTotals(data.totalsastable);

                if (data.orderitems) OrderSummary.updateItems(data.orderitems);
                if (data.PaymentViewModel) UpdatePaymentMethods(data.PaymentViewModel);
            },
                this.isInitializing);
        },
        applyAddressChange: function (callback, isInitializing) {
            $.ajax({
                type: "POST",
                url: hcc.getServiceUrl("checkout/applyaddresschange"),
                data: {
                    shippingcountry: $('#shippingcountry :selected').val(),
                    shippingfirstname: $('#shippingfirstname').val(),
                    shippinglastname: $('#shippinglastname').val(),
                    shippingaddress: $('#shippingaddress').val(),
                    shippingaddress2: $('#shippingaddress2').val(),
                    shippingcity: $('#shippingcity').val(),
                    shippingzip: $('#shippingzip').val(),
                    shippingstate: $('#shippingtempregion').val(),
                    billingcountry: $('#billingcountry :selected').val(),
                    billingfirstname: $('#billingfirstname').val(),
                    billinglastname: $('#billinglastname').val(),
                    billingaddress: $('#billingaddress').val(),
                    billingaddress2: $('#billingaddress2').val(),
                    billingcity: $('#billingcity').val(),
                    billingzip: $('#billingzip').val(),
                    billingstate: $('#billingtempregion').val(),
                    orderid: $('#orderbvin').val(),
                    billshipsame: $('#chkbillsame').is(':checked'),
                },
                dataType: "json",
                success: function (data) {
                    callback(data);
                    Addresses.handleAddressValidation(data);
                    RewardPoints.updatePointsAmount(data.LabelRewardsUse);
                    GiftCards.displayGiftCards(data.GiftCards);
                },
                error: function () { }
            });
        },
        selectedAddressChanged: function (event) {
            var self = this;
            var addressList = $(event.target);
            var addressBvin = addressList.prop("value");
            var addressTable = addressList.closest(".hcAddressForm");

            $.ajax({
                type: 'POST',
                url: hcc.getServiceUrl("checkout/useraddress"),
                data: {
                    addressBvin: addressBvin
                },
                dataType: "json",
                success: function (data) {
                    self.populateAddressData(addressTable, data)
                },
                error: function (data) {
                    ajaxErrorNotification();
                }
            });
        },
        populateAddressData: function (adressWrapper, address) {
            var addressbvinField = adressWrapper.find("[id*='addressbvin']");

            if (address) {
                var countryField = adressWrapper.find("[id*='country']");
                var firstnameField = adressWrapper.find("[id*='firstname']");
                var lastnameField = adressWrapper.find("[id*='lastname']");
                var companyField = adressWrapper.find("[id*='company']");
                var addressField = adressWrapper.find("[id*='address'][type=text]");
                var address2Field = adressWrapper.find("[id*='address2'][type=text]");
                var cityField = adressWrapper.find("[id*='city']");
                var stateField = adressWrapper.find("[id*='state']");
                var tempRegionField = adressWrapper.find("[id*='tempregion']");
                var zipField = adressWrapper.find("[id*='zip']");
                var phoneField = adressWrapper.find("[id*='phone']");

                countryField.val(address.CountryBvin);
                firstnameField.val(address.FirstName);
                lastnameField.val(address.LastName);
                companyField.val(address.Company);
                addressField.val(address.Line1);
                address2Field.val(address.Line2);
                cityField.val(address.City);
                tempRegionField.val(address.RegionBvin);
                zipField.val(address.PostalCode);
                phoneField.val(address.Phone);
                addressbvinField.val(address.Bvin);

                this.loadRegionsWithSelection(stateField, address.CountryBvin, address.RegionBvin);
                if (countryField.attr("name").substring(0, 5) == "shipp") {
                    this.shippingChanged();
                }
                else {
                    this.billingChanged();
                }
            }
            else {
                addressbvinField.val('');
            }
        },
        loadAddresses: function (addresses, shippingAddress, billingAddress) {
            var self = this;
            if (addresses.length > 0) {
                $(".hcAvailableAddresses").show();

                $.each(addresses, function (index, value) {
                    var option = $("<option>", {
                        value: value.Bvin,
                        text: value.Line1
                    });
                    self.$shAvailableAddresses.append(option.clone());
                    self.$blAvailableAddresses.append(option.clone());
                });
            }

            this.populateAddressData($(".hc-shipping-section"), shippingAddress);
            this.populateAddressData($(".hc-billing-section"), billingAddress);

            this.$shAvailableAddresses.val(shippingAddress.Bvin);
            this.$blAvailableAddresses.val(billingAddress.Bvin);
        },
        loadRegionsWithSelection: function (regionlist, countryid, selectedregion, raiseChange) {
            $.post(hcc.getServiceUrl("estimateshipping/getregions/" + countryid),
                {
                    "regionid": selectedregion
                },
                function (data) {
                    regionlist.html(data.Regions);

                    //duplicate value to tempRegion
                    var tempRegion = regionlist.parent().find("[id*='tempregion']");
                    tempRegion.val(selectedregion);

                    if (raiseChange)
                        regionlist.change();
                },
                "json"
            );
        }
    };

    function RefreshShippingRates() {
        $('#hcShippingRates').html('');
        $('#hcDeliverySection').ajaxLoader('start');
        $('#hcShippingNotValid').hide();

        $.ajax({
            type: "POST",
            url: hcc.getServiceUrl("estimateshipping/getratesasradiobuttons"),
            data: {
                country: $('#shippingcountry :selected').val(),
                firstname: $('#shippingfirstname').val(),
                lastname: $('#shippinglastname').val(),
                address: $('#shippingaddress').val(),
                address2: $('#shippingaddress2').val(),
                city: $('#shippingcity').val(),
                zip: $('#shippingzip').val(),
                state: $('#shippingtempregion').val(),
                orderid: $('#orderbvin').val()
            },
            dataType: "json",
            success: function (data) {
                $('#hcDeliverySection').ajaxLoader('stop');
                $('#hcShippingRates').html(data.rates);
                $('#hcShippingRates').show();
                BindShippingRadioButtons();
                ApplyCurrentShippingRate();
            },
            error: function (data) {
                $('#hcDeliverySection').ajaxLoader('stop');
                $('#hcShippingNotValid').show();
            }
        });
    }

    function ApplyCurrentShippingRate() {
        var methodid = $("input[name='shippingrate']:checked").val();
        OrderSummary.showLoadingProgress();

        var orderid = $('#orderbvin').val();

        $.ajax({
            type: 'POST',
            url: hcc.getServiceUrl("checkout/applyshippingmethod"),
            data: {
                MethodId: methodid,
                OrderId: orderid
            },
            dataType: "json",
            success: function (data) {
                OrderSummary.displayTotals(data.totalsastable);
                OrderSummary.updateItems(data.orderitems);
                UpdatePaymentMethods(data.PaymentViewModel);
                GiftCards.displayGiftCards(data.GiftCards);
            },
            error: function (data) {
                OrderSummary.displayTotals("");
            }
        });
    }

    function BindShippingRadioButtons() {
        $("input[name = 'shippingrate']").change(function () { ApplyCurrentShippingRate(); });
    }

    function UpdatePaymentMethods(paymentViewModel) {
        if (paymentViewModel.NoPaymentNeeded) {
            $("#hcPayment").hide();
            $("#hcNoPayment").show();

            $("#hcNoPayment input[name=paymethod]").first().prop("checked", true);
        }
        else {
            $("#hcPayment").show();
            $("#hcNoPayment").hide();

            if ($("#hcNoPayment input[name=paymethod]").first().prop("checked")) {
                $("#hcPayment input[name=paymethod]").filter(function () {
                    return this.value == paymentViewModel.SelectedMethodId;
                }).prop("checked", true);
            }
        }
    }

    // Order Summary ------------------------

    var OrderSummary = {
        init: function () {
            this.$totals = $('#hcCheckoutTotal');
        },
        showLoadingProgress: function () {
            this.$totals.html(hcc.l10n.checkout_SummaryIsLoading);
            this.$totals.ajaxLoader('start');
        },
        displayTotals: function (totals) {
            this.$totals.ajaxLoader('stop');
            this.$totals.html(totals);
            UpdateTotalTable();
        },
        updateItems: function (orderItems) {
            $(".hcLineTotal").each(function (i, el) {
                var $el = $(el);
                var $totalBase = $el.find(".hcLineTotalBase");
                var $totalAdj = $el.find(".hcLineTotalAdjusted");
                var $lineTax = $el.find(".hcLineTax");


                var oItem = orderItems[i];
                if (oItem.HasAnyDiscounts)
                    $totalBase.show();
                else
                    $totalBase.hide();
                $totalBase.text(oItem.LineTotalWithoutDiscounts);
                $totalAdj.text(oItem.LineTotal);
                $lineTax.text(oItem.LineTaxPortion)
            });
        }
    }


    // Affiliates ------------------------

    function IsAffiliateValid(fieldId, messageId) {
        var affid = $('#' + fieldId).val();
        $.post(hcc.getServiceUrl("AffiliateRegistration/IsAffiliateValid"),
            {
                affiliateId: affid
            },
            function (data) {
                if (!data) {
                    $('#' + messageId).html(HCC.Affiliate.MessageAffiliateInvalid).slideDown();
                }
                else {
                    $('#' + messageId).slideUp();
                }
            },
            "json");
    }

    function InitAffiliate() {
        $('#affiliateid').change(function () { IsAffiliateValid('affiliateid', 'affiliatemessage'); });
    }

    // Gift Cards ----------------------

    var API_ADDGIFTCARD = hcc.getServiceUrl("checkout/AddNewGiftCard");
    var API_REMOVEGIFTCARD = hcc.getServiceUrl("checkout/RemoveGiftCard");

    var GiftCards = {
        init: function () {
            this.$gcPanel = $("#hcGiftCardSection");
            this.$addGiftCard = $("#hcGiftCardButton");
            this.$txtGiftCard = $("#hcGiftCardNumber");
            this.$divSummaryMsg = $("#hcGiftCardSummary");
            this.$tblGiftCards = $("#hcGiftCardList");
            this.$gcFormItem = $("#hcGiftCardsFormItem");
            this.$gcFormAction = $("#hcGiftCardsFormAction");

            this.bindEvents();
        },
        bindEvents: function () {
            var self = GiftCards;
            self.$addGiftCard.click(function (e) { self.addGiftCard(e); });
            self.$tblGiftCards.on("click", ".hc-delete", function (e) { self.removeGiftCard(e); });
        },
        addGiftCard: function (e) {
            var self = GiftCards;
            self.$gcPanel.ajaxLoader('start');
            OrderSummary.showLoadingProgress();

            $.post(API_ADDGIFTCARD, { card: self.$txtGiftCard.val() }, null, "json")
                .done(function (data) {
                    self.$txtGiftCard.val("");
                    self.displayGiftCards(data.GiftCards);
                    self.reloadPaymentAndTotals(data.TotalsAsTable, data.PaymentViewModel);
                })
                .fail(function () { })
                .always(function () { self.$gcPanel.ajaxLoader('stop'); });
        },
        removeGiftCard: function (e) {
            var self = GiftCards;
            var cardNum = $(e.currentTarget).closest("tr").find("td:first").text();
            self.$gcPanel.ajaxLoader('start');
            OrderSummary.showLoadingProgress();

            $.post(API_REMOVEGIFTCARD, { card: cardNum }, null, "json")
                .done(function (data) {
                    self.displayGiftCards(data.GiftCards);
                    self.reloadPaymentAndTotals(data.TotalsAsTable, data.PaymentViewModel);
                })
                .fail(function () { })
                .always(function () { self.$gcPanel.ajaxLoader('stop'); });
        },
        displayGiftCards: function (gcModel) {
            var self = GiftCards;
            var summary = gcModel.Summary;
            var cards = gcModel.Cards;

            if (gcModel.ShowGiftCards) {

                if (summary != "") {
                    self.$divSummaryMsg.html(summary);
                    self.$divSummaryMsg.show();
                } else {
                    self.$divSummaryMsg.hide();
                }
                if (cards.length > 0) this.$gcFormItem.show();
                else this.$gcFormItem.hide();

                self.$tblGiftCards.find("tr.hcGiftCardRow:visible").remove();
                var $template = self.$tblGiftCards.find(".hcGiftCardRow:hidden");

                for (var i = 0; i < cards.length; i++) {
                    var $row = $template.clone();
                    var $cells = $row.find("td");
                    var card = cards[i];
                    $cells.eq(0).text(card.CardNumber);
                    $cells.eq(1).text(card.Balance);
                    $cells.eq(2).text(card.Charge);

                    $row.show();
                    self.$tblGiftCards.append($row);
                }
            } else {
                if (summary != "") {
                    self.$divSummaryMsg.html(summary);
                    self.$divSummaryMsg.show();
                    self.$gcFormAction.hide();
                } else {
                    self.$gcPanel.hide();
                }
            }
        },
        reloadPaymentAndTotals: function (totals, payment) {
            OrderSummary.displayTotals(totals);
            UpdatePaymentMethods(payment);
        }
    };

    function UpdateTotalTable() {
        $("table.totaltable").attr("class", "table table-striped table-hover totaltable");
    }

    // Initialization --------------------------

    OrderSummary.init();
    Addresses.init();

    InitCommon();
    InitLogin();
    RewardPoints.init();
    InitAffiliate();

    GiftCards.init();
});
