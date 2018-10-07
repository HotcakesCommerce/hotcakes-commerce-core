//var angularFormsApp = angular.module('angularFormsApp', ['ui.router']);
var routerApp = angular.module('angularFormsApp', ['ui.router']);
routerApp.controller('GiftCard', function ($scope) {
    $scope.GiftButton = function () {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/GiftCard/CheckGiftCard",
            type: "GET",
            beforeSend: sf.setModuleHeaders,
            data: { PassKey: $scope.giftInput },
            success: function (data2) {
                alert(data2);
                $('#ResultAfterCreditEnquery').html(data2);
                $scope.$apply();
            }
            , statusCode: {
                404: function (data) {
                    alert(data);
                    $scope.$apply();
                }
            }
        });

    };
});
routerApp.controller('fiftyDiscount', function ($scope) {
    $scope.CheckRetiredText = "اعمال تخفیف 50 درصدی ویژه بازنشستگان";
    $scope.showSuccessAlert = false;
    $scope.showErrorAlert = false;
    $scope.asdasd = "در ارتباط با سیستم مرکزی مشکلی پیش آمده است. با پشتیبان فنی تماس بگیرید (Ip Is Invalid)";
    $scope.showSuccessAlert = true;
    $scope.ApplyTakhfif = function () {
        $scope.showSuccessAlert = false;
        $scope.showErrorAlert = false;
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/GDS/ApplyFiftyDiscount",
            type: "GET",
            beforeSend: sf.setModuleHeaders,
            data: { NationalCode: $scope.codemelli, DaftarNumber: $scope.daftarNumber },
            success: function (data2) {
                if (data2 == "1") {
                    $scope.showSuccessAlert = true;
                    $scope.successTextAlert = "اطلاعات ورودی با موفقیت ثبت شد و شما مشمول 50% تخفیف می شوید. لطفا هتل خود را جستجو نمایید.";
                    $scope.CheckRetiredText = "مشمول 50% تخفیف هستید";
                    $scope.isDisabled = true;
                    $scope.CheckText = "ادامه جهت جستجو و رزرو";
                }
                else if (data2 == "104") {
                    $scope.showErrorAlert = true;
                    $scope.successTextAlert = "اطلاعات یافت نشد . لطفا مجدد بررسی بفرمایید";

                }
                else if (data2 == "102") {
                    $scope.showErrorAlert = true;
                    $scope.successTextAlert = "در ارتباط با سیستم مرکزی مشکلی پیش آمده است. با پشتیبان فنی تماس بگیرید (Ip Is Invalid).";

                }
                else {
                    $scope.showErrorAlert = true;
                    $scope.successTextAlert = "خطا . کد خطا : " + data2;

                }

                $scope.$apply();
            }
            , statusCode: {
                500: function (data) {
                    alert("لطفا از صحیح بودن اطلاعات مطمن شوید");
                    $scope.$apply();
                }}
        });

    };
    $scope.CheckText = "بررسی اطلاعات";
    $scope.showSuccessAlert = false;
 
});
routerApp.config(function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/home');

    $stateProvider.state('home', {
        url: '/home',
        templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/Home.html',
        controller: 'moslem'
    })
        .state('Rooms', {
            url: '/Rooms',
            views: {
                '': { template: 'partial-about.html' },
                'columnOne@about': { template: 'Look I am a column!' },
                'columnTwo@about': {
                    template: 'table-data.html',
                    controller: 'scotchController'
                }
            }
        });


});
routerApp.controller('moslem', function ($scope) {
    $scope.isValidForDiscount = false;
    $scope.usediscount = false;
    $scope.haspartner = false;
    $scope.AmountBeforeDiscount = $('#amount').val();
    $scope.calculateDiscount = function () {
        $scope.AmountAfterDiscount = $scope.AmountBeforeDiscount;
        if ($scope.usediscount && $scope.haspartner && $scope.isValidForDiscount) {
            if ($scope.AmountBeforeDiscount > 6600000) {
                $scope.AmountAfterDiscount = $scope.AmountBeforeDiscount - 6600000;
            }
            else
                $scope.AmountAfterDiscount = 0;
        }
        else if ($scope.usediscount && !($scope.haspartner) && $scope.isValidForDiscount) {
            if ($scope.AmountBeforeDiscount > 3300000) {
                $scope.AmountAfterDiscount = $scope.AmountBeforeDiscount - 3300000;
            }
            else
                $scope.AmountAfterDiscount = 0;
        }
        $('#AmountAfterDiscount').val($scope.AmountAfterDiscount);
        // alert($('#AmountAfterDiscount').val());
        //  alert($scope.AmountAfterDiscount);
    }
    $scope.$watch('AmountAfterDiscount', function (newValue, oldValue) {
        if (newValue === "")
            $('AmountAfterDiscount').val(newValue);
    });
    $scope.CheckText = "بررسی";
    $scope.switchBool = function (value) {
        $scope[value] = !$scope[value];
    };
    $scope.ApplyTakhfif = function () {
        if ($('#ContentPlaceHolder1_TKharidar_NationalID').val() == '') {
            show_error_panel('d_panel_kharidarinfo', 'ContentPlaceHolder1_TKharidar_NationalID', 'کد ملی را را وارد نمایید', 1000);
            return false;
        }
        if ($('#daftar').val() == '') {
            show_error_panel('d_panel_kharidarinfo', 'daftar', 'شماره دفتر را وارد نمایید', 1000);
            return false;
        }
        $scope.CheckText = "لطفا صبر کنید ...";
        $scope.isDisabled = true;
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/GDS/RetiredIsValid",
            type: "GET",
            beforeSend: sf.setModuleHeaders,
            data: { NationalCode: $scope.NationalCode, DaftarNumber: $scope.daftarNumber },
            dataType: 'json',
            success: function (data) {
                $scope.calculateDiscount();
                $scope.CheckText = "بررسی";
                $scope.$apply();
                if (data == "102") {
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = "در ارتباط با سیستم مرکزی مشکلی پیش آمده است. با پشتیبان فنی تماس بگیرید (Ip Is Invalid)";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                else if (data == "0") {
                    $scope.showSuccessAlert = true;
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = "اطلاعات بازنشسته پیدا نشد";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                else if (data == "1") {

                    $.ajax({
                        url: "/DesktopModules/MVC/GDSModuleMVC/GDS/retiredhascredit",
                        type: "GET",
                        beforeSend: sf.setModuleHeaders,
                        data: { NationalCode: $scope.NationalCode, DaftarNumber: $scope.daftarNumber },
                        success: function (data2) {
                            $scope.showSuccessAlert = false;
                            $scope.isDisabled = true;
                            $scope.switchBool('true');
                            $scope.CheckText = "شما مجاز به استفاده تور بازنشستگی هستید.";
                            $scope.isValidForDiscount = true;
                            $scope.calculateDiscount();
                            $('#ResultAfterCreditEnquery').html(data2);
                            $scope.$apply();
                        }
                    });
                }
                else if (data == "103") {
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = " در ارتباط با سیستم مرکزی مشکلی پیش آمده است. با پشتیبان فنی تماس بگیرید (User Name/Password Invalid) ";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                else if (data == "104") {
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = "کد ملی یا شماره دفتر کل نامعتبر است.";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                return false;
            },
            statusCode: {
                500: function (data) {
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.isDisabled = false;
                    $scope.calculateDiscount();
                    $scope.CheckText = "بررسی";
                    $scope.successTextAlert = "خطای نا مشخصی پیش آمده است.";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
            }
        });
    };

});


