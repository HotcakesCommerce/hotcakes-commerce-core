app.controller('common_licensing', function ($scope, $attrs, $http, $location, CommonSvc, SweetAlert) {
    var common = CommonSvc.getData($scope);
    $scope.IsMultiple = true;
    $scope.Finish = function (FinishType) {
        if ($scope.ui.data.Editions.Value == "") {
            SweetAlert.swal("Please select an edition");
        }
        else {
            common.webApi.post('~licensing/setworkingedition', 'appname=' + common.ModuleFolder + '&edition=' + $scope.ui.data.Editions.Value).success(function (success) {
                if (success != null)
                    window.location.href = window.location.href.split("#")[0];
            });
        }
    };
    $scope.Activate = function () {
        if ($scope.ui.data.Editions.Value == "") {
            SweetAlert.swal("Please select an edition");
        }
        else {
            common.webApi.post('~licensing/setworkingedition', 'appname=' + common.ModuleFolder + '&edition=' + $scope.ui.data.Editions.Value).success(function (success) {
                if (success != null)
                    window.location.hash = '#/activation';
            });
        }
    };
    $scope.onInit = function () {
        if ($scope.ui.data.IsMultiple.Value == "false") {
            $scope.IsMultiple = false;
            if ($scope.ui.data.Editions.Value == '') {
                $scope.ui.data.Editions.Value = $scope.ui.data.Editions.Options[0];
            }
        }
        $scope.IsActivation = true;
        $scope.IsRequestActivation = false;
        $scope.IsApplyActivation = false;
        $scope.Activated = false;
        $scope.IsActivated = false;
        $scope.ActivationError = [];
        $scope.ActivationSuccess = [];
    };
    $scope.ShowDaysRemainings = function (row) {
        if (!row.IsEdition)
            return false;
        else if (row.Status === 'TRIAL')
            return true;
        else
            return false;
    };
    $scope.myFilter = function (item) {
        if ($scope.Activated)
            return true;
        else
            return item.Status === 'TRIAL' || item.Status === 'EXPIRED' || item.Status === "";
    };
    $scope.Click_Activate = function () {
        common.webApi.post('~activation/setworkingedition', '', $scope.ui.data.Products.Options).success(function (success) {
            if (success == "") {
                $scope.IsActivation = false;
                $scope.IsRequestActivation = true;
            }
            else {
                SweetAlert.swal(success);
            }
        });
    };
    $scope.Click_Cancel = function () {
        window.location = $scope.ui.data.HomeUrl.Value;
    };
    $scope.CheckAll = function () {
        $.each($scope.ui.data.Products.Options, function (key, pro) {
            if (pro.Status === 'TRIAL' || pro.Status === 'EXPIRED' || pro.Status === "")
                pro.IsChecked = $scope.IsChecked;
        });
    }
    $scope.Click_ApplyActivation = function () {
        common.webApi.post('~activation/setworkingedition', '', $scope.ui.data.Products.Options).success(function (success) {
            if (success == "") {
                $scope.IsActivation = false;
                $scope.IsRequestActivation = false;
                $scope.IsApplyActivation = true;
            }
            else {
                SweetAlert.swal(success);
            }
        });
    };
    $scope.RequestKey = function (sender) {
        if ($location.$$host != "localhost" || $location.$$host != "dnndev.me") {
            var IsValid = true;
            if (typeof mnValidationService !== 'undefined' && typeof mnValidationService.DoValidationAndSubmit === 'function')
                IsValid = mnValidationService.DoValidationAndSubmit(sender);
            if (IsValid) {
                SweetAlert.swal({
                    title: "Are you sure?",
                    text: "Licenses are non-transferrable. Activation Keys for selected products will be automatically requested and applied. You will receive an email for your records.",
                    type: "info",
                    showCancelButton: true,
                    closeOnConfirm: true,
                    closeOnCancel: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        common.webApi.post('~activation/activatemultiple', 'email=' + $scope.RequestEmail, $scope.ui.data.Products.Options).success(function (Data) {
                            $scope.ApplyError = [];
                            $scope.ApplySuccess = [];
                            $scope.IsActivated = true;
                            $.each(Data, function (key, value) {
                                if (value.Success == true)
                                    $scope.ApplySuccess.push(value);
                                else
                                    $scope.ApplyError.push(value);
                            });
                            if ($scope.ApplySuccess.length)
                                $scope.IsSucess = true;
                            if ($scope.ApplyError.length)
                                $scope.IsError = true;

                        });
                    }
                });
            }
        }
    };
    $scope.Click_Finish = function () {
        common.webApi.post('~activation/setworkingedition', '', $scope.ui.data.Products.Options).success(function (success) {
            if (success == "") {
                window.location = $scope.ui.data.HomeUrl.Value;
            }
            else {
                SweetAlert.swal(success);
            }
        });
    };
    $scope.Click_CancelActivation = function () {
        $scope.IsActivation = true;
        $scope.IsRequestActivation = false;
        $scope.IsApplyActivation = false;
        $scope.IsActivated = false;
    };
    $scope.ApplyKey = function (sender) {
        var IsValid = true;
        if (typeof mnValidationService !== 'undefined' && typeof mnValidationService.DoValidationAndSubmit === 'function')
            IsValid = mnValidationService.DoValidationAndSubmit(sender);

        if (IsValid) {
            common.webApi.post('~activation/applymultiple', '', $scope.ActivationKeys).success(function (Data) {
                $scope.ApplyError = [];
                $scope.ApplySuccess = [];
                $scope.IsActivated = true;
                $.each(Data, function (key, value) {
                    if (value.Success == true)
                        $scope.ApplySuccess.push(value);
                    else
                        $scope.ApplyError.push(value);
                });
                if ($scope.ApplySuccess.length)
                    $scope.IsSucess = true;
                if ($scope.ApplyError.length)
                    $scope.IsError = true;
            });
        }
    };
});