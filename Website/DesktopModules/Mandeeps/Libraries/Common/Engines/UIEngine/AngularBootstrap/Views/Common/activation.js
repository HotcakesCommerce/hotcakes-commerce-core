app.controller('common_activation', function ($scope, $attrs, $location, $http, CommonSvc, SweetAlert) {
    var common = CommonSvc.getData($scope);

    $scope.Status = "";
    $scope.AvailableEditions = [];
    $scope.Edition = "";

    $scope.IsRequestActivation = false;
    $scope.IsPurchaseKey = true;
    $scope.IsApplyActivation = false;

    $scope.RequestEmail = "";

    $scope.ActivationKey = "";

    $scope.CancelActivation = function () {
        window.location.hash = '#/licensing';
    };

    $scope.PurchaseActivationKey = function () {
        window.location = 'http://www.mandeeps.com/store.aspx' + "?addcart=" + $scope.ui.data.AppID.Value + "&Edition=" + $scope.ui.data.Editions.Value;
    };

    $scope.RequestActivationKey = function () {
        $scope.IsRequestActivation = true;
        $scope.IsPurchaseKey = false;
        $scope.IsApplyActivation = false;
        $scope.Status = "To request an activation key you must have purchased a license for this module. If you have not yet purchased a license then click the Cancel link otherwise <strong>enter the email address used to purchase the license below.</strong>";
    };

    $scope.CancelRequest = function () {
        $scope.IsRequestActivation = false;
        $scope.IsPurchaseKey = true;
        $scope.IsApplyActivation = false;
        $scope.RequestEmail = "";
        $scope.AvailableEditions = [];
        $scope.Edition = "";
        $scope.Status = "To request an activation key you must have purchased a license for this module. If you have not yet purchased a license then click the Cancel link otherwise <strong>enter the email address used to purchase the license below.</strong>";
    };

    $scope.ApplyActivationKey = function () {
        $scope.IsRequestActivation = false;
        $scope.IsPurchaseKey = false;
        $scope.IsApplyActivation = true;
        $scope.Status = "Enter the activation key and click submit.";
    };

    $scope.ApplyKey = function (sender) {
         var IsValid = true;

                if (typeof mnValidationService !== 'undefined' && typeof mnValidationService.DoValidationAndSubmit === 'function')
                    IsValid = mnValidationService.DoValidationAndSubmit(sender);

                if (IsValid) {
            if ($scope.ActivationKey.trim().replace(/-/g, '').length == 32) {
                common.webApi.post('~activation/activate', 'appname=' + common.ModuleFolder + '&key=' + $scope.ActivationKey).success(function (success) {
                    if (success.Status != null && success.Success == true)
                        window.location.href = window.location.href.split("#")[0];
                    else
                        $scope.Status = "Enter the activation key and click submit.<br />" + success.Status;
                });
            }
            else
                SweetAlert.swal("Please enter a valid Activation Key");
        }
    };

    $scope.CancelApplyKey = function () {
        $scope.IsRequestActivation = false;
        $scope.IsPurchaseKey = true;
        $scope.IsApplyActivation = false;
    };

    $scope.RequestKey = function (sender) {
        if ($location.$$host != "localhost" || $location.$$host != "dnndev.me") {
             var IsValid = true;

                if (typeof mnValidationService !== 'undefined' && typeof mnValidationService.DoValidationAndSubmit === 'function')
                    IsValid = mnValidationService.DoValidationAndSubmit(sender);

                if (IsValid) {
                SweetAlert.swal({
                    title: "Confirm",
                    text: $scope.ui.data.RequestKeyConfirmMessage.Value,
                    type: "info",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    showLoaderOnConfirm: true,
                }, function (isConfirm) {
                    if (isConfirm) {
                        if ($scope.AvailableEditions.length > 0 && $scope.Edition == "") {
                            SweetAlert.swal("Please select an edition");
                            return;
                        }
                        common.webApi.post('~activation/availableeditions', 'appname=' + common.ModuleFolder + '&edition=' + $scope.Edition + '&email=' + $scope.RequestEmail).success(function (success) {
                            if (success.Status != null)
                                $scope.Status = success.Status;
                            if (success.IsViewAvailableEditions != null && success.IsViewAvailableEditions == true && success.AvailableEditions != null) {
                                $scope.AvailableEditions = success.AvailableEditions;
                            }
                            if (success.IsApplyActivation != null && success.IsApplyActivation == true) {
                                $scope.IsApplyActivation = true;
                                $scope.IsRequestActivation = false;
                                $scope.IsPurchaseKey = false;
                                $scope.Status = "Enter the activation key and click submit.<br />" + success.Status;
                            }
                        });
                        swal.close();

                    }
                });
            }
        }
        else
            SweetAlert.swal("Localhost domain names cannot be activated. Please <a href=" + $scope.ui.data.ActivationHelpURL.Value + ">Contact Us</a> if you need further assistance. Thanks</span>");
    };
});